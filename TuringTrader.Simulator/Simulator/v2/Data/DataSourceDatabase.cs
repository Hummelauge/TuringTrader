//==============================================================================
// Project:     TuringTrader, simulator core v2
// Name:        DataSourceDatabase
// Description: Data source for Database Access.
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

#region libraries
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TuringTrader.Simulator;
using TuringTrader.SimulatorV2.Indicators;
using static System.Runtime.InteropServices.JavaScript.JSType;
#endregion

namespace TuringTrader.SimulatorV2
{
    #region Database loading helpers
    /// <summary>
    /// Centralized class for managing MariaDB connections with connection pooling
    /// </summary>
    public static class DbConnection
    {
        private static readonly string ConnectionString;

        /*
        Create code for the database:
        CREATE DATABASE `backtesting` /*!40100 COLLATE 'utf8mb4_uca1400_ai_ci' */

        /// <summary>
        /// Static constructor to initialize the connection string and optionally test the connection
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        static DbConnection()
        {
            ConnectionString = "Server=localhost;" +
                               "Port=3306;" +
                               "Database=backtesting;" +
                               "User ID=" + GlobalSettings.DBUser + ";" +
                               "Password=" + GlobalSettings.DBPassword + ";" +
                               "AllowLoadLocalInfile=true;" +
                               "Pooling=true;" +                    // Enable pooling (default is true)
                               "Minimum Pool Size=5;" +             // Minimum number of connections
                               "Maximum Pool Size=50;" +            // Maximum number of connections (adjust based on load)
                               "Connection Lifetime=300;" +         // Maximum lifetime of a connection in seconds (optional)
                               "Connection Idle Timeout=60;" +      // How long idle connections stay in the pool
                               "Connection Reset=false;" +          // Performance optimization
                               "Allow Zero Datetime=true;" +
                               "ConvertZeroDateTime=True;";


            // Optional: Test connection at startup
            try
            {
                using var testConn = new MySqlConnection(ConnectionString);
                testConn.Open();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error initializing MariaDB connection: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Returns a connection from the pool. Always use with 'using'!
        /// </summary>
        public static MySqlConnection GetConnection()
        {
            var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Establishes a connection for asynchronous calls
        /// </summary> 
        public static async Task<MySqlConnection> GetConnectionAsync()
        {
            var conn = new MySqlConnection(ConnectionString);
            await conn.OpenAsync();
            return conn;
        }
    }
    #endregion

    public static partial class DataSource
    {

        private static List<BarType<OHLCV>> DatabaseLoadData(Algorithm owner, Dictionary<DataSourceParam, string> info)
        {
            /*
            Create code for the database table:
            CREATE TABLE `tiingo_daily` (
        	`ticker` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_uca1400_ai_ci',
	        `date` DATE NOT NULL,
        	`open` DOUBLE UNSIGNED ZEROFILL NOT NULL DEFAULT '0000000000000000000000',
        	`high` DOUBLE UNSIGNED ZEROFILL NOT NULL DEFAULT '0000000000000000000000',
        	`low` DOUBLE UNSIGNED ZEROFILL NOT NULL DEFAULT '0000000000000000000000',
        	`close` DOUBLE UNSIGNED ZEROFILL NOT NULL DEFAULT '0000000000000000000000',
        	`volume` INT(10) UNSIGNED ZEROFILL NOT NULL DEFAULT '0000000000',
        	PRIMARY KEY(`ticker`, `date`) USING BTREE,
            INDEX `idx_ticker_date_desc` (`ticker`, `date` DESC) USING BTREE )
            COLLATE = 'utf8mb4_uca1400_ai_ci'
            ENGINE = InnoDB;
            */


            var bars = new List<BarType<OHLCV>>();
            var tradingDays = owner.TradingCalendar.TradingDays;
            var startDate = tradingDays.First();
            var endDate = tradingDays
                .Where(t => t <= DateTime.Now)
                .Last();

            using var sqlCon = DbConnection.GetConnection();

            var sql = """
                      SELECT  date, open, high, low, close, volume
                        FROM  tiingo_daily
                        WHERE ticker = @symbol
                          AND date >= @startDate
                          AND date <= @endDate
                        ORDER BY ticker, date
                      """;


            var sqlResult = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var cmd = new MySqlCommand(sql, sqlCon);
            cmd.Parameters.AddWithValue("@symbol", info[DataSourceParam.symbolDatabase]);
            cmd.Parameters.AddWithValue("@startDate", startDate);
            cmd.Parameters.AddWithValue("@endDate", endDate);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DateTime date = reader.GetDateTime("date");
                double open = reader.GetDouble("open");
                double high = reader.GetDouble("high");
                double low = reader.GetDouble("low");
                double close = reader.GetDouble("close");
                double volume = (double)reader.GetUInt64("volume");

                bars.Add(new BarType<OHLCV>(date, new OHLCV(open, high, low, close, volume)));
            }

            if (bars.Count == 0)
                Output.WriteWarning("Failed to load data for {0}", info[DataSourceParam.nickName]);
            else
                Output.WriteInfo("Loaded {0} data points for {1} from database", bars.Count, info[DataSourceParam.nickName]);

            return bars;

        }

        private static TimeSeriesAsset.MetaType DatabaseLoadMeta(Algorithm owner, Dictionary<DataSourceParam, string> info)
        {
            /*
            Create code for the database table tiingo_meta:
            CREATE TABLE `tiingo_meta` (
        	`ticker` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_uca1400_ai_ci',
        	`name` VARCHAR(200) NOT NULL COLLATE 'utf8mb4_uca1400_ai_ci',
        	`description` VARCHAR(16000) NOT NULL COLLATE 'utf8mb4_uca1400_ai_ci',
        	`startDate` DATETIME NULL DEFAULT NULL,
        	`endDate` DATETIME NULL DEFAULT NULL,
        	`exchangeCode` VARCHAR(30) NOT NULL COLLATE 'utf8mb4_uca1400_ai_ci',
        	`assetType` VARCHAR(5) NOT NULL COLLATE 'utf8mb4_uca1400_ai_ci',
        	`priceCurrency` VARCHAR(5) NOT NULL COLLATE 'utf8mb4_uca1400_ai_ci',
        	`lastMetaUpdate` DATETIME NOT NULL,
        	`lastDataUpdate` DATETIME NOT NULL,
        	PRIMARY KEY (`ticker`) USING BTREE )
            COLLATE='utf8mb4_uca1400_ai_ci'
            ENGINE=InnoDB ;
             */

            string name = string.Empty;

            using var sqlCon = DbConnection.GetConnection();

            var sql = """
                      SELECT  name
                        FROM  tiingo_meta
                        WHERE ticker = @symbol
                      """;

            var sqlResult = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var cmd = new MySqlCommand(sql, sqlCon);
            cmd.Parameters.AddWithValue("@symbol", info[DataSourceParam.symbolDatabase]);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                name = reader.GetString("name");
            }

            var meta = new TimeSeriesAsset.MetaType
            {
                Ticker = info[DataSourceParam.symbolDatabase],
                Description = name,
            };

            return meta;
        }

        private static Tuple<List<BarType<OHLCV>>, TimeSeriesAsset.MetaType> DatabaseGetAsset(Algorithm owner, Dictionary<DataSourceParam, string> info)
        {
            return Tuple.Create(
                DatabaseLoadData(owner, info),
                DatabaseLoadMeta(owner, info));
        }

    }
}

//==============================================================================
// end of file
