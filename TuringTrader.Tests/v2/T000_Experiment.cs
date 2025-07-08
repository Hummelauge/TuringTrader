using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace TuringTrader.SimulatorV2.Tests
{
    [TestClass]
    public class T000_Experiment
    {
        private class DoNothing : Algorithm { }

        [TestMethod]
        public void Test_Experiment()
        {
            var tickers = new List<string> { "SPY", "UGL", "DGP", "VIXY", "VXX" };
            foreach (var nickname in tickers)
            {
                var algo = new DoNothing();
                algo.StartDate = DateTime.Now - TimeSpan.FromDays(5);
                algo.EndDate = DateTime.Now;
                var quote = algo.Asset(nickname).Close[DateTime.Now];
                Assert.AreNotEqual(0.0, quote, 1e-5);
            }
        }
    }
}
