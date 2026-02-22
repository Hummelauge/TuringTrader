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

        private static List<BarType<OHLCV>> FakeBTOptionLoadData(Algorithm algo, Dictionary<DataSourceParam, string> info)
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
        private static TimeSeriesAsset.MetaType FakeBTOptionLoadMeta(Algorithm algo, Dictionary<DataSourceParam, string> info)
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

        private static Tuple<List<BarType<OHLCV>>, TimeSeriesAsset.MetaType> FakeBTOptionGetAsset(Algorithm owner, Dictionary<DataSourceParam, string> info)
        {
            return Tuple.Create(
                FakeOptionLoadData(owner, info),
                FakeOptionLoadMeta(owner, info));
        }
    }



    public class AmericanOptionBinomialTree
    {
        public enum OptionType { Call, Put }
        public enum OptionStyle { European, American }

        // --- Input parameters -------------------------------------------------
        private double S0;          // Spot price at t=0
        private double K;           // Strike price
        private double T;           // Time to expiry (years)
        private double r;           // Risk-free rate (continuous)
        private double sigma;       // Annual volatility
        private double q;           // Continuous dividend yield
        private int N;              // Number of time steps
        private OptionType type;
        private OptionStyle style;

        // Discrete dividends: list of (time-until-dividend, amount)
        private List<(double time, double amount)> discreteDividends;

        // --- Pre-computed factors --------------------------------------------
        private double dt;          // Length of one time step
        private double u;           // Up-factor
        private double d;           // Down-factor
        private double p;           // Risk-neutral probability of up-move
        private double discount;    // Discount factor per step  e^(-r*dt)

        // --------------------------------------------------------------------
        public AmericanOptionBinomialTree(
            double spot, double strike, double timeToExpiry, double rate,
            double volatility, double dividendYield, int steps,
            OptionType optionType, OptionStyle optionStyle = OptionStyle.American,
            List<(double time, double amount)> discreteDividends = null)
        {
            S0 = spot;
            K = strike;
            T = timeToExpiry;
            r = rate;
            sigma = volatility;
            q = dividendYield;
            N = steps;
            type = optionType;
            style = optionStyle;
            this.discreteDividends = discreteDividends ?? new List<(double, double)>();

            // Cox-Ross-Rubinstein (CRR) parametrization
            dt = T / N;
            u = Math.Exp(sigma * Math.Sqrt(dt));
            d = 1.0 / u;
            discount = Math.Exp(-r * dt);

            // Risk-neutral drift factor
            double a = Math.Exp((r - q) * dt);
            p = (a - d) / (u - d);
        }

        /// <summary>
        /// Calculates the option price using a binomial tree.
        /// </summary>
        public double Price()
        {
            // ----------------------------------------------------------------
            // 1) Build the stock-price lattice (forward pass)
            // ----------------------------------------------------------------
            double[][] stock = new double[N + 1][];   // stock[i][j] = price at node (i,j)
            for (int i = 0; i <= N; i++) stock[i] = new double[i + 1];

            for (int i = 0; i <= N; i++)
            {
                double timeFromNow = T - i * dt;      // time remaining to expiry
                for (int j = 0; j <= i; j++)
                {
                    // Base price without dividends
                    double price = S0 * Math.Pow(u, j) * Math.Pow(d, i - j);

                    // Subtract any **discrete** dividend that falls exactly on this node
                    foreach (var div in discreteDividends)
                    {
                        if (Math.Abs(timeFromNow - div.time) < dt / 2.0)
                        {
                            price = Math.Max(price - div.amount, 0.0);
                        }
                    }
                    stock[i][j] = price;
                }
            }

            // ----------------------------------------------------------------
            // 2) Option values at maturity
            // ----------------------------------------------------------------
            double[][] option = new double[N + 1][];
            for (int i = 0; i <= N; i++) option[i] = new double[i + 1];

            for (int j = 0; j <= N; j++)
            {
                double payoff = type == OptionType.Call
                    ? Math.Max(stock[N][j] - K, 0.0)
                    : Math.Max(K - stock[N][j], 0.0);
                option[N][j] = payoff;
            }

            // ----------------------------------------------------------------
            // 3) Backward induction – early exercise for American style
            // ----------------------------------------------------------------
            for (int i = N - 1; i >= 0; i--)
            {
                for (int j = 0; j <= i; j++)
                {
                    // Continuation value (discounted expected value)
                    double continuation = discount *
                        (p * option[i + 1][j + 1] + (1 - p) * option[i + 1][j]);

                    // Intrinsic value if exercised now
                    double exercise = type == OptionType.Call
                        ? Math.Max(stock[i][j] - K, 0.0)
                        : Math.Max(K - stock[i][j], 0.0);

                    // American = max(continuation, exercise); European = continuation
                    option[i][j] = style == OptionStyle.American
                        ? Math.Max(continuation, exercise)
                        : continuation;
                }
            }

            return option[0][0];   // price at root node
        }

        // ====================================================================
        // BACK-TESTING SECTION
        // ====================================================================
        public class BacktestResult
        {
            public DateTime EntryDate { get; set; }
            public double Spot { get; set; }
            public double ModelPrice { get; set; }
            public double MarketPrice { get; set; }   // optional, NaN if unknown
            public double ImpliedVol { get; set; }    // optional
        }

        /// <summary>
        /// Runs a back-test over a series of historical spot prices.
        /// </summary>
        public List<BacktestResult> Backtest(
            List<(DateTime date, double spot)> historicalData,
            double strike,
            double yearsToExpiry,
            double rate,
            double dividendYield,
            Func<DateTime, double> getMarketOptionPrice = null)
        {
            var results = new List<BacktestResult>();

            // Use at least one step per trading day for realistic resolution
            int minSteps = (int)(yearsToExpiry * 252);
            int steps = Math.Max(N, minSteps);

            foreach (var data in historicalData)
            {
                var tree = new AmericanOptionBinomialTree(
                    data.spot, strike, yearsToExpiry, rate, sigma,
                    dividendYield, steps, type, style, discreteDividends);

                double model = tree.Price();
                double market = getMarketOptionPrice?.Invoke(data.date) ?? double.NaN;

                results.Add(new BacktestResult
                {
                    EntryDate = data.date,
                    Spot = data.spot,
                    ModelPrice = model,
                    MarketPrice = market
                });
            }
            return results;
        }
    }
}
#endif
//==============================================================================
// end of file