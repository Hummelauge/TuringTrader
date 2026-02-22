//==============================================================================
// Project:     TuringTrader, simulator core v2
// Name:        Account_Default
// Description: Default account class.
// History:     2022x25, FUB, created
//------------------------------------------------------------------------------
// Copyright:   (c) 2011-2023, Bertram Enterprises LLC dba TuringTrader.
//              https://www.turingtrader.org
// License:     This file is part of TuringTrader, an open-source backtesting
//              engine/ trading simulator.
//              TuringTrader is free software: you can redistribute it and/or 
//              modify it under the terms of the GNU Affero General Public 
//              License as published by the Free Software Foundation, either 
//              version 3 of the License, or (at your option) any later version.
//              TuringTrader is distributed in the hope that it will be useful,
//              but WITHOUT ANY WARRANTY; without even the implied warranty of
//              MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//              GNU Affero General Public License for more details.
//              You should have received a copy of the GNU Affero General Public
//              License along with TuringTrader. If not, see 
//              https://www.gnu.org/licenses/agpl-3.0.
//==============================================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using static TuringTrader.Simulator.LogAnalysis;

namespace TuringTrader.SimulatorV2
{
    /// <summary>
    /// The Account class maintains the status of the trading account.
    /// </summary>
    public class Account_Default : IAccount
    {
        #region internal stuff
        private readonly Algorithm _algorithm;
        private List<IAccount.OrderTicket> _orderQueue = new List<IAccount.OrderTicket>();
        private List<IAccount.OrderReceipt> _tradeLog = new List<IAccount.OrderReceipt>();
        private Dictionary<string, double> _positions = new Dictionary<string, double>();
        private const double INITIAL_CAPITAL = 1000.00;
        private double _cash = INITIAL_CAPITAL;
        private double _navNextOpen = 0.0;
        private DateTime _firstDate = default(DateTime);
        private DateTime _lastDate = default(DateTime);
        private double _navMax = 0.0;
        private double _mdd = 0.0;
        // TODO: revisit default friction. 0.1% might be a better choice?
        private const double DEFAULT_FRICTION = 0.0005; // $100.00 x 0.05% = $0.05
        private double _friction = DEFAULT_FRICTION;

#if EXTENSION
        private ITradingCalendar _calendar = new TradingCalendar_US();

        private Dictionary<string, List<TTExtEntryPosition>> _openEntries = new();
        private List<TTExtClosedPosition> _closedPositions = new();
        private ConcurrentDictionary<string, Statistics> _statistics = new ConcurrentDictionary<string, Statistics>(new[] { new KeyValuePair<string, Statistics>("ALL_ASSETS", new Statistics()) });
        private int _positionCounter = 0;
        private int _loopCounter = 0;
#endif

        private enum NavType
        {
            openThisBar,
            closeThisBar,
            openNextBar,
        }

        private double CalcNetAssetValue(NavType navType = NavType.closeThisBar)
        {
            // FIXME: need to verify that the assets trade on current date.
            // Otherwise, we should probably remove them and issue a warning.

            return _positions
                .Sum(kv => kv.Value
                    * navType switch
                    {
                        NavType.openThisBar => _algorithm.Asset(kv.Key).Open[0],
                        NavType.closeThisBar => _algorithm.Asset(kv.Key).Close[0],
                        NavType.openNextBar => _algorithm.Asset(kv.Key).Open[-1],
                        _ => throw new ArgumentOutOfRangeException(nameof(navType), $"Unexpected NAV type value: {navType}")
                    })
                + _cash;
        }
        #endregion

        /// <summary>
        /// Create new account.
        /// </summary>
        /// <param name="algorithm">parent algorithm, to get access to assets and pricing</param>
        public Account_Default(Algorithm algorithm)
        {
            _algorithm = algorithm;
        }

        /// <summary>
        /// Submit and queue order.
        /// </summary>
        /// <param name="Name">asset name</param>
        /// <param name="weight">asset target allocation</param>
        /// <param name="orderType">order type</param>
        /// <param name="orderPrice">trigger price for stop and limit orders</param>
#if EXTENSION
        /// <param name="comment">Additional information</param>
        public void SubmitOrder(string Name, double weight, OrderType orderType, double orderPrice = 0.0, string comment = "")
        {
            _orderQueue.Add(
                new IAccount.OrderTicket(
                    _algorithm.SimDate,
                    Name, weight, orderType, orderPrice, comment));
        }
#else
        public void SubmitOrder(string Name, double weight, OrderType orderType, double orderPrice = 0.0)
        {
            _orderQueue.Add(
                new IAccount.OrderTicket(
                    _algorithm.SimDate,
                    Name, weight, orderType, orderPrice));
        }
#endif

        /// <summary>
        /// Process bar. This method will loop through the queued
        /// orders, execute them as required, and return a bar
        /// representing the strategy's NAV.
        /// </summary>
        /// <returns></returns>
        public OHLCV ProcessBar()
        {
            if (_firstDate == default)
                _firstDate = _algorithm.SimDate;
            if (_lastDate < _algorithm.SimDate)
                _lastDate = _algorithm.SimDate;

            var navOpen = 0.0;
            var navClose = 0.0;

#if EXTENSION
            foreach (var symbol in OpenEntries)
                foreach (var pos in symbol.Value)
                    pos.BarsHeld += 1;
#endif

            foreach (var orderTypeFilter in new List<OrderType> {
                    // this list is ordered chronologically
                    OrderType.closeThisBar,
                    OrderType.openNextBar,
                    // we expect sells to happen before buys,
                    // and stop orders before limit orders
                    OrderType.sellStopNextBar,
                    OrderType.sellLimitNextBar,
                    OrderType.buyStopNextBar,
                    OrderType.buyLimitNextBar})
            {
                // execute pending next-day market-on-open orders on close of last bar
                var orderType = orderTypeFilter switch
                {
                    OrderType.openNextBar => _algorithm.IsLastBar ? OrderType.closeThisBar : orderTypeFilter,
                    _ => orderTypeFilter,
                };

                var navType = orderType switch
                {
                    OrderType.closeThisBar => NavType.closeThisBar,
                    // we are using NAV at next day's open for all 
                    // order types filled tomorrow. this is incorrect
                    // for stopand limit orders, but we don't see any
                    // better alternative as the various asset's high
                    // and low prices are not aligned in time.
                    _ => NavType.openNextBar,
                };

                var execDate = orderType switch
                {
                    OrderType.closeThisBar => _algorithm.SimDate,
                    _ => _algorithm.NextSimDate,
                };

                //----- process orders
                foreach (var order in _orderQueue.Where(o => o.OrderType == orderTypeFilter))
                {
                    var orderAsset = _algorithm.Asset(order.Name);

                    // FIXME: this code has not been thoroughly tested with short positions
                    // it is likely there are issues around the handling of isBuy and price2
                    var price = orderType switch
                    {
                        //--- market orders
                        OrderType.closeThisBar => orderAsset.Close[0],
                        OrderType.openNextBar => orderAsset.Open[-1],
                        //--- stop orders
                        OrderType.sellStopNextBar => Math.Min(orderAsset.Open[-1], order.OrderPrice),
                        OrderType.buyStopNextBar => Math.Max(orderAsset.Open[-1], order.OrderPrice),
                        //--- limit orders
                        OrderType.buyLimitNextBar => Math.Min(orderAsset.Open[-1], order.OrderPrice),
                        OrderType.sellLimitNextBar => Math.Max(orderAsset.Open[-1], order.OrderPrice),
                        //--- default, should never happen
                        _ => throw new NotImplementedException(),
                    };

                    // we need to calculate the nav every time we get
                    // here due to trading friction.
                    var nav = CalcNetAssetValue(navType);

                    var currentShares = _positions.ContainsKey(order.Name) ? _positions[order.Name] : 0;
                    var currentAlloc = currentShares * price / nav;
                    var targetAlloc = Math.Abs(order.TargetAllocation) >= MinPosition ? order.TargetAllocation : 0.0;

#if false
                    // TODO: determine if skipping small orders is helpful
                    //       it is unclear how much this really increases execution speed.
                    //       At the same time, it seems that for an equal-weighted index,
                    //       this optimization might result in about 0.2% deviation in CAGR
                    if (Math.Abs(targetAlloc - currentAlloc) < MinPosition && targetAlloc != 0.0)
                        continue;
#endif

                    if (currentAlloc == 0.0 && targetAlloc == 0.0)
                        continue;

                    var isOrderFilling = orderType switch
                    {
                        //--- market orders
                        OrderType.closeThisBar => true,
                        OrderType.openNextBar => true,
                        //--- stop orders
                        OrderType.sellStopNextBar => orderAsset.Low[-1] <= order.OrderPrice && currentAlloc > targetAlloc,
                        OrderType.buyStopNextBar => orderAsset.High[-1] >= order.OrderPrice && currentAlloc < targetAlloc,
                        //--- limit orders
                        OrderType.buyLimitNextBar => orderAsset.Low[-1] <= order.OrderPrice && currentAlloc < targetAlloc,
                        OrderType.sellLimitNextBar => orderAsset.High[-1] >= order.OrderPrice && currentAlloc > targetAlloc,
                        //--- default, should never happen
                        _ => throw new NotImplementedException(),
                    };

                    if (!isOrderFilling)
                        continue;

                    {
                        // TODO: move this block to a virtual function
                        //       which we can overload to implement
                        //       alternative fill models.
                        // parameters
                        //   - order ticket
                        //   - nominal fill price
                        //   - current shares
                        //   - current allocation
                        //   - target allocation
                        // required operation
                        //   - adjust _Positions
                        //   - adjust _Cash
                        //   - add to _TradeLog

                        var isBuy = currentAlloc < targetAlloc;

                        var price2 = isBuy
                            ? price * (1.0 + Friction) // when buying, we reduce the # of shares to cover for commissions
                            : price;
                        var deltaShares = nav * (targetAlloc - currentAlloc) / price2;

#if EXTENSION
                        if (MinimumCash != null
                            && isBuy == true
                            && (Cash - targetAlloc + currentAlloc) < MinimumCash)
                            continue;

                        AddClosedPosition(new IAccount.OrderReceipt(
                                order,
                                execDate,
                                targetAlloc - currentAlloc,
                                price,
                                deltaShares * price,
                                Math.Abs(deltaShares) * price * Friction));
#endif

                        if (targetAlloc != 0.0)
                        {
                            var targetShares = currentShares + deltaShares;
                            _positions[order.Name] = targetShares;
                        }
                        else
                        {
                            _positions.Remove(order.Name);
                        }

                        var orderAmount = deltaShares * price;
                        var frictionAmount = Math.Abs(deltaShares) * price * Friction;
                        _cash -= orderAmount;
                        _cash -= frictionAmount;

                        if (!_algorithm.IsOptimizing)
                            _tradeLog.Add(new IAccount.OrderReceipt(
                                order,
                                execDate,
                                targetAlloc - currentAlloc,
                                price,
                                orderAmount,
                                frictionAmount));
                    }
                }

                //----- save NAV
                switch (orderTypeFilter)
                {
                    case OrderType.closeThisBar:
                        navClose = CalcNetAssetValue(NavType.closeThisBar);
                        break;
                    case OrderType.openNextBar:
                        navOpen = _algorithm.IsFirstBar ? INITIAL_CAPITAL : _navNextOpen;
                        _navNextOpen = CalcNetAssetValue(NavType.openNextBar);
                        break;
                }
            }

            _orderQueue.Clear();

            //----- calculate NAV at open and close
            var navHigh = Math.Max(navOpen, navClose);
            var navLow = Math.Min(navOpen, navClose);
            _navMax = Math.Max(_navMax, navClose);
            _mdd = Math.Max(_mdd, 1.0 - navClose / _navMax);

            return new OHLCV(navOpen, navHigh, navLow, navClose, 0);
        }

        /// <summary>
        /// Return net asset value in currency, starting with $1,000
        /// at the beginning of the simulation. Note that currency
        /// has no relevance throughout the v2 engine. We use this
        /// value to make the NAV more tangible during analysis and
        /// debugging.
        /// </summary>
        public double NetAssetValue { get => CalcNetAssetValue(); }

        /// <summary>
        /// Calculate annualized return over the full simulation range.
        /// </summary>
        public double AnnualizedReturn { get => Math.Pow(CalcNetAssetValue() / INITIAL_CAPITAL, 365.25 / (_lastDate - _firstDate).TotalDays) - 1.0; }

        /// <summary>
        /// Return maximum drawdown over the full simulation range.
        /// </summary>
        public double MaxDrawdown { get => _mdd; }

        /// <summary>
        /// Return dictionary with currently open positions.
        /// The key is the nickname used to load the asset. The value
        /// is a floating point number, representing the allocation to
        /// the asset as a fraction of the accounts NAV.
        /// </summary>
        public Dictionary<string, double> Positions
        {
            get
            {
                var nav = CalcNetAssetValue();
                var result = new Dictionary<string, double>();
                foreach (var kv in _positions)
                {
                    result[kv.Key] = kv.Value * _algorithm.Asset(kv.Key).Close[0] / nav;
                }
                return result;
            }
        }

        /// <summary>
        /// Return of cash available, as fraction of NAV.
        /// </summary>
        public double Cash { get => _cash / CalcNetAssetValue(); }

        /// <summary>
        /// Deposit cash to account.
        /// </summary>
        /// <param name="cashPcnt">cash deposited as fraction of NAV</param>
        public void Deposit(double cashPcnt)
        {
            var currency = cashPcnt * CalcNetAssetValue();
            _cash += currency;
        }

        /// <summary>
        /// Retrieve trade log. Note that this log only contains trades executed
        /// and not orders that were not executed.
        /// </summary>
        public List<IAccount.OrderReceipt> TradeLog { get { return new List<IAccount.OrderReceipt>(_tradeLog); } }

        /// <summary>
        /// Friction to model commissions, fees, and slippage
        /// expressed as a fraction of the traded value. A value of 0.01 is 
        /// equivalent to 1% of friction. Setting Friction to a negative 
        /// value will reset it to its default setting.
        /// </summary>
        public double Friction { get => _friction; set { _friction = value >= 0.0 ? value : DEFAULT_FRICTION; } }

        /// <summary>
        /// Minimum position size, as fraction of total account value. A
        /// value of 0.01 is equivalent to a minimum position of 1% of the
        /// account's liquidation value.
        /// </summary>
        public double MinPosition { get; set; } = 0.001; // 0.1%  minimum position

#if EXTENSION

        /// <summary>
        /// Minimum Cash
        /// </summary> 
        public double? MinimumCash { get; set; }

        /// <summary>
        /// Closed Positions
        /// </summary> 
        public List<TTExtClosedPosition> ClosedPositions { get => _closedPositions; }

        /// <summary>
        /// Statistics for all closed positions
        /// </summary> 
        public ConcurrentDictionary<string, Statistics> Statistics { get => _statistics; }

        /// <summary>
        /// Initial Capital
        /// </summary>
        public double IniCapital = INITIAL_CAPITAL;

        /// <summary>
        /// Currently open Positions
        /// </summary>
        public Dictionary<string, List<TTExtEntryPosition>> OpenEntries
        {
            get
            {
                foreach (var symbol in _openEntries)
                    foreach (var positionEntry in symbol.Value)
                        positionEntry.OpenProfitPerc = (double)(_algorithm.Asset(positionEntry.Symbol).Close[0] - positionEntry.EntryOrder.FillPrice) / positionEntry.EntryOrder.FillPrice * 100;
                return _openEntries;
            }
        }

        private void AddClosedPosition(IAccount.OrderReceipt order, bool lastInFirstOut = true)
        {
            _loopCounter++;

            if (!_openEntries.ContainsKey(order.OrderTicket.Name))
                _openEntries[order.OrderTicket.Name] = new List<TTExtEntryPosition>();

            var openEntryQuantity = _openEntries[order.OrderTicket.Name].Sum(i => i.OpenQuantity);
            var openCoverQuantity = (decimal)order.OrderAmount / (decimal)order.FillPrice;

            while (openCoverQuantity != 0)
            {
                //--- add new positionEntry
                if (order.OrderAmount > 0 && openEntryQuantity >= 0
                  || order.OrderAmount < 0 && openEntryQuantity <= 0)
                {
                    string underlyingSymbol = order.OrderTicket.Name;
                    if (order.OrderTicket.Name.StartsWith("fake_option:"))
                        underlyingSymbol = order.OrderTicket.Name.Substring(12).Split(new[] { "||" }, StringSplitOptions.None)[0];

                    _openEntries[order.OrderTicket.Name].Add(new TTExtEntryPosition
                    {
                        Symbol = order.OrderTicket.Name,
                        UnderlyingSymbol = underlyingSymbol,
                        OpenQuantity = (decimal)order.OrderAmount / (decimal)order.FillPrice,
                        EntryOrder = order,
                        LogIndex = _loopCounter,
                        SubmitDateOpen = order.OrderTicket.SubmitDate,
                        CommentOpen = order.OrderTicket.Comment,
                        BarsHeld = order.OrderTicket.OrderType == OrderType.closeThisBar ? 1 : 0,
                        Statistics = _statistics["ALL_ASSETS"].AddAllAssetStatistics(_statistics.GetOrAdd(order.OrderTicket.Name, _ => new Statistics()).GetAssetStatistics()),
                    });

                    openCoverQuantity = 0;
                }

                //--- (at least partially) close positionEntry/ create new position
                if (order.OrderAmount < 0 && openEntryQuantity > 0
                || order.OrderAmount > 0 && openEntryQuantity < 0)
                {
                    if (!_openEntries.ContainsKey(order.OrderTicket.Name)
                    || _openEntries[order.OrderTicket.Name].Count() == 0)
                    {
                        throw new Exception(
                            string.Format("LogAnalysis.GroupOrders: no matching positionEntry found for Symbol {0}",
                                order.OrderTicket.Name));
                    }

                    TTExtEntryPosition entryOrder = lastInFirstOut
                        ? _openEntries[order.OrderTicket.Name].Last()   // LIFO
                        : _openEntries[order.OrderTicket.Name].First(); // FIFO


                    // set start and end date to calculate number of bars held
                    _calendar.StartDate = entryOrder.EntryOrder.ExecDate;
                    _calendar.EndDate = order.ExecDate;


                    // create a new position
                    decimal positionQuantity = openCoverQuantity < 0
                        ? -Math.Min(Math.Abs(openCoverQuantity), entryOrder.OpenQuantity) // close long
                        : Math.Min(openCoverQuantity, Math.Abs(entryOrder.OpenQuantity)); // close short

                    _positionCounter += 1;

                    decimal _netProfit = -positionQuantity * ((decimal)order.FillPrice - (decimal)entryOrder.EntryOrder.FillPrice) - (decimal)order.FrictionAmount - (decimal)entryOrder.EntryOrder.FrictionAmount;

                    var pos = new TTExtClosedPosition
                    {
                        Symbol = order.OrderTicket.Name,
                        UnderlyingSymbol = entryOrder.UnderlyingSymbol,
                        LogIndex = entryOrder.LogIndex,
                        Quantity = -positionQuantity,
                        NetProfit = _netProfit,
                        NetProfitPerc = _netProfit / Math.Abs((decimal)entryOrder.EntryOrder.OrderAmount) * 100,
                        BarsHeld = order.OrderTicket.OrderType == OrderType.closeThisBar ? entryOrder.BarsHeld - 1 : entryOrder.BarsHeld,
                        Commissions = (decimal)order.FrictionAmount + (decimal)entryOrder.EntryOrder.FrictionAmount,
                        SubmitDateOpen = entryOrder.SubmitDateOpen,
                        ExecutionDateOpen = entryOrder.EntryOrder.ExecDate,
                        SubmitDateClose = order.OrderTicket.SubmitDate,
                        ExecutionDateClose = order.ExecDate,
                        PositionIndex = _positionCounter,
                        CommentOpen = entryOrder.CommentOpen,
                        CommentClose = order.OrderTicket.Comment,
                        Statistics = entryOrder.Statistics,
                    };

                    _closedPositions.Add(pos);

                    _statistics["ALL_ASSETS"].AddPosition(pos.NetProfitPerc);
                    _statistics[order.OrderTicket.Name].AddPosition(pos.NetProfitPerc);

                    openCoverQuantity -= positionQuantity;
                    if ((float)Math.Abs(openCoverQuantity) < 0.00001)
                        openCoverQuantity = 0;

                    openEntryQuantity += positionQuantity;
                    if ((float)Math.Abs(openEntryQuantity) < 0.00001)
                        openEntryQuantity = 0;

                    // adjust or remove positionEntry
                    entryOrder.OpenQuantity += positionQuantity;
                    if ((float)Math.Abs(entryOrder.OpenQuantity) < 0.00001)
                        entryOrder.OpenQuantity = 0;
                    if (entryOrder.OpenQuantity == 0)
                        _openEntries[order.OrderTicket.Name].Remove(entryOrder);
                }
            }
        }
#endif
    }

#if EXTENSION 

    #region public class TTExtEntryPosition
    /// <summary>
    /// Container to hold order positionEntry information
    /// </summary>
    public class TTExtEntryPosition
    {
        /// <summary>
        /// Symbol
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// underlying Symbol
        /// </summary>
        public string UnderlyingSymbol { get; set; }
        /// <summary>
        /// position quantity
        /// </summary>
        public decimal OpenQuantity { get; set; }
        /// <summary>
        /// order log positionEntry 
        /// </summary>
        public IAccount.OrderReceipt EntryOrder { get; set; }
        /// <summary>
        /// index in order log
        /// </summary>
        public int LogIndex { get; set; }
        /// <summary>
        /// Date/Time when order was submitted
        /// </summary>        
        public DateTime SubmitDateOpen { get; set; }
        /// <summary>
        /// Open profit percentage
        /// </summary>        
        public double OpenProfitPerc { get; set; }
        /// <summary>
        /// Number of bars the position was held
        /// </summary>        
        public int BarsHeld { get; set; }
        /// <summary>
        /// Comment for position entry
        /// </summary>        
        public string CommentOpen { get; set; }
        /// <summary>
        /// Statistics when position was opened
        /// </summary>        
        public TTExtStatistic Statistics { get; set; }
    }
    #endregion

    #region public class TTExtClosedPosition
    public class TTExtClosedPosition
    {
        public string Symbol { get; set; }
        public string UnderlyingSymbol { get; set; }
        public int LogIndex { get; set; }
        public decimal Quantity { get; set; }
        public decimal NetProfit { get; set; }
        public decimal NetProfitPerc { get; set; }
        public int BarsHeld { get; set; }
        public decimal Commissions { get; set; }
        public DateTime SubmitDateOpen { get; set; }
        public DateTime ExecutionDateOpen { get; set; }
        public DateTime SubmitDateClose { get; set; }
        public DateTime ExecutionDateClose { get; set; }
        public int PositionIndex { get; set; }
        public string CommentOpen { get; set; }
        public string CommentClose { get; set; }
        public TTExtStatistic Statistics { get; set; }
    }
    #endregion

    #region public class TTExtStatistic
    public class TTExtStatistic
    {
        public decimal AllSymbolsLastPositionNetchange { get; set; }
        public decimal AllSymbolsLast2PositionsNetchange { get; set; }
        public decimal AllSymbolsLast4PositionsNetchange { get; set; }
        public decimal AllSymbolsLast8PositionsNetchange { get; set; }
        public decimal AllSymbolsLast16PositionsNetchange { get; set; }
        public decimal AllSymbolsLast32PositionsNetchange { get; set; }
        public decimal AllSymbolsLast64PositionsNetchange { get; set; }
        public decimal AllSymbolsLast128PositionsNetchange { get; set; }
        public decimal AllSymbolsLast256PositionsNetchange { get; set; }
        public decimal SymbolLastPositionNetchange { get; set; }
        public decimal SymbolLast2PositionsNetchange { get; set; }
        public decimal SymbolLast4PositionsNetchange { get; set; }
        public decimal SymbolLast8PositionsNetchange { get; set; }
        public decimal SymbolLast16PositionsNetchange { get; set; }
        public decimal SymbolLast32PositionsNetchange { get; set; }
        public decimal SymbolLast64PositionsNetchange { get; set; }
        public decimal SymbolLast128PositionsNetchange { get; set; }
        public decimal SymbolLast256PositionsNetchange { get; set; }
    }
    #endregion

    public class ProfitSlidingWindow
    {
        private readonly int _maxSize;
        private readonly Queue<decimal> _q = new();
        private decimal _sum;
        public decimal Sum => _q.Count == _maxSize ? _sum : 0;

        public ProfitSlidingWindow(int maxSize) => _maxSize = maxSize;

        public void Add(decimal value)
        {
            _q.Enqueue(value);
            _sum += value;
            if (_q.Count > _maxSize)
                _sum -= _q.Dequeue();
        }
    }

    /// <summary>
    /// Provides statistics for asset positions, including sliding window net changes.
    /// </summary>
    public class Statistics
    {
        /// <summary>
        /// Sliding window for last position net change.
        /// </summary>
        public ProfitSlidingWindow LastPositionNetchange = new ProfitSlidingWindow(1);
        /// <summary>
        /// Sliding window for last 2 positions net change.
        /// </summary>
        public ProfitSlidingWindow Last2PositionsNetchange = new ProfitSlidingWindow(2);
        /// <summary>
        /// Sliding window for last 4 positions net change.
        /// </summary>
        public ProfitSlidingWindow Last4PositionsNetchange = new ProfitSlidingWindow(4);
        /// <summary>
        /// Sliding window for last 8 positions net change.
        /// </summary>
        public ProfitSlidingWindow Last8PositionsNetchange = new ProfitSlidingWindow(8);
        /// <summary>
        /// Sliding window for last 16 positions net change.
        /// </summary>
        public ProfitSlidingWindow Last16PositionsNetchange = new ProfitSlidingWindow(16);
        /// <summary>
        /// Sliding window for last 32 positions net change.
        /// </summary>
        public ProfitSlidingWindow Last32PositionsNetchange = new ProfitSlidingWindow(32);
        /// <summary>
        /// Sliding window for last 64 positions net change.
        /// </summary>
        public ProfitSlidingWindow Last64PositionsNetchange = new ProfitSlidingWindow(64);
        /// <summary>
        /// Sliding window for last 128 positions net change.
        /// </summary>
        public ProfitSlidingWindow Last128PositionsNetchange = new ProfitSlidingWindow(128);
        /// <summary>
        /// Sliding window for last 256 positions net change.
        /// </summary>
        public ProfitSlidingWindow Last256PositionsNetchange = new ProfitSlidingWindow(256);

        /// <summary>
        /// Adds a position's net change percentage to all sliding windows.
        /// </summary>
        /// <param name="netChange">Net change percentage to add.</param>
        public void AddPosition(decimal netChange)
        {
            LastPositionNetchange.Add(netChange);
            Last2PositionsNetchange.Add(netChange);
            Last4PositionsNetchange.Add(netChange);
            Last8PositionsNetchange.Add(netChange);
            Last16PositionsNetchange.Add(netChange);
            Last32PositionsNetchange.Add(netChange);
            Last64PositionsNetchange.Add(netChange);
            Last128PositionsNetchange.Add(netChange);
            Last256PositionsNetchange.Add(netChange);
        }

        /// <summary>
        /// Gets asset statistics for the current sliding windows.
        /// </summary>
        /// <returns>Asset statistics.</returns>
        public TTExtStatistic GetAssetStatistics()
        {
            return new TTExtStatistic
            {
                SymbolLastPositionNetchange = LastPositionNetchange.Sum,
                SymbolLast2PositionsNetchange = Last2PositionsNetchange.Sum,
                SymbolLast4PositionsNetchange = Last4PositionsNetchange.Sum,
                SymbolLast8PositionsNetchange = Last8PositionsNetchange.Sum,
                SymbolLast16PositionsNetchange = Last16PositionsNetchange.Sum,
                SymbolLast32PositionsNetchange = Last32PositionsNetchange.Sum,
                SymbolLast64PositionsNetchange = Last64PositionsNetchange.Sum,
                SymbolLast128PositionsNetchange = Last128PositionsNetchange.Sum,
                SymbolLast256PositionsNetchange = Last256PositionsNetchange.Sum
            };
        }

        /// <summary>
        /// Adds all asset statistics to the provided asset statistics object.
        /// </summary>
        /// <param name="assetStatistics">Asset statistics to update.</param>
        /// <returns>Updated asset statistics.</returns>
        public TTExtStatistic AddAllAssetStatistics(TTExtStatistic assetStatistics)
        {
            assetStatistics.AllSymbolsLastPositionNetchange = LastPositionNetchange.Sum;
            assetStatistics.AllSymbolsLast2PositionsNetchange = Last2PositionsNetchange.Sum;
            assetStatistics.AllSymbolsLast4PositionsNetchange = Last4PositionsNetchange.Sum;
            assetStatistics.AllSymbolsLast8PositionsNetchange = Last8PositionsNetchange.Sum;
            assetStatistics.AllSymbolsLast16PositionsNetchange = Last16PositionsNetchange.Sum;
            assetStatistics.AllSymbolsLast32PositionsNetchange = Last32PositionsNetchange.Sum;
            assetStatistics.AllSymbolsLast64PositionsNetchange = Last64PositionsNetchange.Sum;
            assetStatistics.AllSymbolsLast128PositionsNetchange = Last128PositionsNetchange.Sum;
            assetStatistics.AllSymbolsLast256PositionsNetchange = Last256PositionsNetchange.Sum;
            return assetStatistics;
        }
    }
#endif
}

//==============================================================================
// end of file
