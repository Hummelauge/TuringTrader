#if EXTENSION
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TuringTrader.SimulatorV2
{
    /// <summary>
    /// Container class to store potentially several validity periods of membership in an index
    /// Assets should be traded only during their validity periods
    /// </summary>
    public class ValidityPeriods
    {
        /// <summary>
        /// First date/time
        /// </summary>
        public DateTime FirstDateTime;
        /// <summary>
        /// Last date/time
        /// </summary>
        public DateTime LastDateTime;
    }

    public static partial class DataSource
    {
        /// <summary>
        /// Return dynamic universe. The constituents for these universes are
        /// time-variant, hence without survivorship bias.
        /// In the past, some companies were part of an index in several periods,
        /// that's why the ValidityPeriods list might contain several periods
        /// 
        /// The first row should contain the date formats used in the file.
        /// Rows starting with '#' are comments and ignored.
        /// Example of a universe file:
        /// 
        /// ticker,yyyy-MM-dd,yyyy-MM-dd
        /// #
        /// # Ticker, Date From, Date To
        /// #
        /// A,2000-06-05,
        /// AABA,1999-12-08,2017-06-19
        /// AAL,1996-01-02,1997-01-15
        /// 
        /// </summary>
        /// <param name="universe">universe name</param>
        /// <returns>universe constituents</returns>
        /// <exception cref="Exception"></exception>
        public static Dictionary<string, List<ValidityPeriods>> UniversesDynamic(string universe)
        {
            Dictionary<string, List<ValidityPeriods>> constituents = new();
            string universePath = Path.Combine(GlobalSettings.UniversesPath, universe);
            string dateFormatFrom = null;
            string dateFormatTo = null;
            bool firstLineProcessed = false;
            
            if (!File.Exists(universePath))
                throw new DirectoryNotFoundException($"Universe {0} does not exist: {universePath}");

            foreach (var line in File.ReadLines(universePath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue; // skip empty lines and comments

                var parts = line.Split(',').Where(part => !string.IsNullOrWhiteSpace(part)).ToArray();

                if (firstLineProcessed == false)
                {
                    if (parts.Length >= 3)
                    {
                        dateFormatFrom = parts[1].Trim();
                        dateFormatTo = parts[2].Trim();
                    }
                    firstLineProcessed = true;
                }
                else
                {
                    var asset = parts[0].Trim();
                    switch (parts.Length)
                    {
                        case 1:
                            // just the asset
                            if (!constituents.TryGetValue(asset, out var list))
                                constituents[asset] = list = new List<ValidityPeriods>();

                            list.Add(new ValidityPeriods
                            {
                                FirstDateTime = DateTime.MinValue,
                                LastDateTime = DateTime.MaxValue
                            });
                            break;
                        case 2:
                            // asset plus validity periods - but asset still valid
                            if (!constituents.ContainsKey(asset))
                                constituents[asset] = new List<ValidityPeriods>();

                            constituents[asset].Add(new ValidityPeriods()
                            {
                                FirstDateTime = DateTime.ParseExact(parts[1].Trim(), dateFormatFrom, CultureInfo.InvariantCulture),
                                LastDateTime = DateTime.MaxValue
                            });
                            break;
                        case 3:
                            // asset plus validity periods - asset not valid anymore
                            if (!constituents.ContainsKey(asset))
                                constituents[asset] = new List<ValidityPeriods>();

                            constituents[asset].Add(new ValidityPeriods()
                            {
                                FirstDateTime = DateTime.ParseExact(parts[1].Trim(), dateFormatFrom, CultureInfo.InvariantCulture),
                                LastDateTime = DateTime.ParseExact(parts[2].Trim(), dateFormatTo, CultureInfo.InvariantCulture)
                            });
                            break;
                    }
                }
            }

            return constituents;
        }
    }
}

//==============================================================================
// end of file
#endif