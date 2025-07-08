//==============================================================================
// Project:     TuringTrader, simulator core
// Name:        DataSourceCsv
// Description: Virtual data source to use data from algorithms.
// History:     2022xi25, FUB, created
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

#if EXTENSION

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TuringTrader.SimulatorV2.Indicators;
using TuringTrader.Support;

namespace TuringTrader.SimulatorV2
{
    public static partial class DataSource
    {
        #region internal helpers
        #endregion

        private static List<BarType<OHLCV>> FakeOptionLoadData(Algorithm algo, Dictionary<DataSourceParam, string> info)
        {
            var bars = new List<BarType<OHLCV>>();

            var underlyingAsset = DataSource.LoadAsset(algo, info[DataSourceParam.optionUnderlying]);

            bool isCall = info[DataSourceParam.optionRight] == "call" ? true : false;

            DateTime expiry = DateTime.ParseExact(info[DataSourceParam.optionExpiration], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            DateTime firstDate = expiry - TimeSpan.FromDays(420);

            double strike = double.Parse(info[DataSourceParam.optionStrike], CultureInfo.CreateSpecificCulture("en-US"));// CultureInfo.InvariantCulture);

            double open = 0d;
            double high = 0d;
            double low = 0d;
            double close = 0d;

#if MORE_VIX_PERIODS
            var volatilities = new Dictionary<double, TimeSeriesAsset>
                { { 9 / 365.25, DataSource.LoadAsset(algo, "$VIX9D") },
                  { 30 / 365.25, DataSource.LoadAsset(algo, "$VIX") },
                  { 91 / 365.25, DataSource.LoadAsset(algo, "$VIX3M") },
                  { 182 / 365.25, DataSource.LoadAsset(algo, "$VIX6M") },
                  { 365 / 365.25, DataSource.LoadAsset(algo, "$VIX1Y") }, };
#else
            var volatilities = new Dictionary<double, TimeSeriesAsset>
                { { 30 / 365.25, DataSource.LoadAsset(algo, "$VIX") }, };
#endif

            for (int idx = 0; idx < underlyingAsset.Data.Count; idx++)
            {
                var underlyingBar = underlyingAsset.Data[idx];

                if (underlyingBar.Date >= firstDate && underlyingBar.Date < expiry)
                {
                    double T = (expiry - underlyingBar.Date).TotalDays / 365.25;

                    var volatility = 0.0;
                    if (T < volatilities.Keys.Min())
                    {
                        volatility = volatilities[volatilities.Keys.Min()].Close[idx] / 100.0;
                    }
                    else if (T > volatilities.Keys.Max())
                    {
                        volatility = volatilities[volatilities.Keys.Max()].Close[idx] / 100.0;
                    }
                    else
                    {
                        var vLow = volatilities
                            .Where(v => v.Key <= T)
                            .OrderByDescending(v => v.Key)
                            .FirstOrDefault();
                        var vHigh = volatilities
                            .Where(v => v.Key >= T)
                            .OrderBy(v => v.Key)
                            .FirstOrDefault();
                        var p = (T - vLow.Key) / (vHigh.Key - vLow.Key);
                        volatility = 0.01 * (vLow.Value.Close[idx]
                            + p * (vHigh.Value.Close[idx] - vLow.Value.Close[idx]));
                    }

                    double z = (strike - underlyingBar.Value.Open) / (Math.Sqrt(T) * underlyingBar.Value.Open * volatility);
                    double vol = volatility * (1.0 + 0.30 * Math.Abs(z));
                    open = OptionSupport.GBlackScholes(isCall, underlyingBar.Value.Open, strike, T, 0.0, // risk-free rate
                                                                                                     0.0, // cost-of-carry rate
                                                                                                     vol);

                    z = (strike - underlyingBar.Value.High) / (Math.Sqrt(T) * underlyingBar.Value.High * volatility);
                    vol = volatility * (1.0 + 0.30 * Math.Abs(z));
                    high = OptionSupport.GBlackScholes(isCall, underlyingBar.Value.High, strike, T, 0.0, // risk-free rate
                                                                                                    0.0, // cost-of-carry rate
                                                                                                    vol);

                    z = (strike - underlyingBar.Value.Low) / (Math.Sqrt(T) * underlyingBar.Value.Low * volatility);
                    vol = volatility * (1.0 + 0.30 * Math.Abs(z));
                    low = OptionSupport.GBlackScholes(isCall, underlyingBar.Value.Low, strike, T, 0.0, // risk-free rate
                                                                                                  0.0, // cost-of-carry rate
                                                                                                  vol);

                    z = (strike - underlyingBar.Value.Close) / (Math.Sqrt(T) * underlyingBar.Value.Close * volatility);
                    vol = volatility * (1.0 + 0.30 * Math.Abs(z));
                    close = OptionSupport.GBlackScholes(isCall, underlyingBar.Value.Close, strike, T, 0.0, // risk-free rate
                                                                                                      0.0, // cost-of-carry rate
                                                                                                      vol);

                    BarType<OHLCV> bar = new BarType<OHLCV>(underlyingBar.Date, new OHLCV(open, high, low, close, 0));

                    bars.Add(bar);
                }
            }

            return bars;
        }
        private static TimeSeriesAsset.MetaType FakeOptionLoadMeta(Algorithm algo, Dictionary<DataSourceParam, string> info)
        {
            return new TimeSeriesAsset.MetaType
            {
                Ticker = info[DataSourceParam.nickName2],
                Description = info.ContainsKey(DataSourceParam.name)
                    ? info[DataSourceParam.name]
                    : info[DataSourceParam.nickName2],
                UnderlyingTicker = info[DataSourceParam.optionUnderlying],
            };
        }

        private static Tuple<List<BarType<OHLCV>>, TimeSeriesAsset.MetaType> FakeOptionGetAsset(Algorithm owner, Dictionary<DataSourceParam, string> info)
        {
            return Tuple.Create(
                FakeOptionLoadData(owner, info),
                FakeOptionLoadMeta(owner, info));
        }
    }
}
#endif
//==============================================================================
// end of file