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

            var underlyingAsset = algo.Asset(info[DataSourceParam.optionUnderlying]); // DataSource.LoadAsset(algo, info[DataSourceParam.optionUnderlying]);
            var underlyingVolatility = underlyingAsset.Close.Volatility(10);

            bool isCall = info[DataSourceParam.optionRight] == "call" ? true : false;

            DateTime expiry = DateTime.ParseExact(info[DataSourceParam.optionExpiration], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            DateTime firstDate = expiry - TimeSpan.FromDays(420);

            double strike = double.Parse(info[DataSourceParam.optionStrike], CultureInfo.CreateSpecificCulture("en-US"));// CultureInfo.InvariantCulture);

            double open = 0d;
            double high = 0d;
            double low = 0d;
            double close = 0d;

            var volatilities = new Dictionary<double, TimeSeriesAsset>
                { { 30 / 365.25, algo.Asset( "$VIX" ) }, };

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

                    //volatility = underlyingVolatility[idx] / 100.0;

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