//==============================================================================
// Project:     TuringTrader, simulator core
// Name:        StaticUniverses
// Description: Static universe definitions.
// History:     2022xii01, FUB, created
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
using System.Collections.Generic;
using System.Linq;

namespace TuringTrader.SimulatorV2
{
    public static partial class DataSource
    {
        #region static spx
        private static List<string> _staticSpx = new List<string>
        {
#if EXTENSION
            // as of 06/27/2025, see https://www.slickcharts.com/sp500

            "NVDA",   // 1 Nvidia
            "MSFT",   // 2 Microsoft
            "AAPL",   // 3 Apple Inc.
            "AMZN",   // 4 Amazon
            "META",   // 5 Meta Platforms
            "AVGO",   // 6 Broadcom
            "GOOG",   // 7 Alphabet Inc. (Class C)
            "GOOGL",   // 8 Alphabet Inc. (Class A)
            "TSLA",   // 9 Tesla, Inc.
            "BRK.B",   // 10 Berkshire Hathaway
            "JPM",   // 11 JPMorgan Chase
            "WMT",   // 12 Walmart
            "LLY",   // 13 Lilly (Eli)
            "V",   // 14 Visa Inc.
            "ORCL",   // 15 Oracle Corporation
            "NFLX",   // 16 Netflix
            "MA",   // 17 Mastercard
            "XOM",   // 18 ExxonMobil
            "COST",   // 19 Costco
            "PG",   // 20 Procter & Gamble
            "JNJ",   // 21 Johnson & Johnson
            "HD",   // 22 Home Depot (The)
            "BAC",   // 23 Bank of America
            "PLTR",   // 24 Palantir Technologies
            "ABBV",   // 25 AbbVie
            "KO",   // 26 Coca-Cola Company (The)
            "PM",   // 27 Philip Morris International
            "UNH",   // 28 UnitedHealth Group
            "CSCO",   // 29 Cisco
            "IBM",   // 30 IBM
            "GE",   // 31 GE Aerospace
            "TMUS",   // 32 T-Mobile US
            "WFC",   // 33 Wells Fargo
            "CRM",   // 34 Salesforce
            "CVX",   // 35 Chevron Corporation
            "AMD",   // 36 Advanced Micro Devices
            "ABT",   // 37 Abbott Laboratories
            "MS",   // 38 Morgan Stanley
            "LIN",   // 39 Linde plc
            "DIS",   // 40 Walt Disney Company (The)
            "AXP",   // 41 American Express
            "INTU",   // 42 Intuit
            "GS",   // 43 Goldman Sachs
            "NOW",   // 44 ServiceNow
            "MCD",   // 45 McDonald's
            "T",   // 46 AT&T
            "MRK",   // 47 Merck & Co.
            "UBER",   // 48 Uber
            "ISRG",   // 49 Intuitive Surgical
            "RTX",   // 50 RTX Corporation
            "TXN",   // 51 Texas Instruments
            "ACN",   // 52 Accenture
            "BKNG",   // 53 Booking Holdings
            "CAT",   // 54 Caterpillar Inc.
            "VZ",   // 55 Verizon
            "PEP",   // 56 PepsiCo
            "QCOM",   // 57 Qualcomm
            "ADBE",   // 58 Adobe Inc.
            "SCHW",   // 59 Charles Schwab Corporation
            "BLK",   // 60 BlackRock
            "C",   // 61 Citigroup
            "SPGI",   // 62 S&P Global
            "TMO",   // 63 Thermo Fisher Scientific
            "BSX",   // 64 Boston Scientific
            "BA",   // 65 Boeing
            "PGR",   // 66 Progressive Corporation
            "AMGN",   // 67 Amgen
            "SYK",   // 68 Stryker Corporation
            "AMAT",   // 69 Applied Materials
            "NEE",   // 70 NextEra Energy
            "HON",   // 71 Honeywell
            "DHR",   // 72 Danaher Corporation
            "MU",   // 73 Micron Technology
            "GEV",   // 74 GE Vernova
            "PFE",   // 75 Pfizer
            "DE",   // 76 Deere & Company
            "UNP",   // 77 Union Pacific Corporation
            "ETN",   // 78 Eaton Corporation
            "PANW",   // 79 Palo Alto Networks
            "TJX",   // 80 TJX Companies
            "COF",   // 81 Capital One
            "GILD",   // 82 Gilead Sciences
            "CMCSA",   // 83 Comcast
            "ANET",   // 84 Arista Networks
            "CRWD",   // 85 CrowdStrike
            "LRCX",   // 86 Lam Research
            "LOW",   // 87 Lowe's
            "ADP",   // 88 Automatic Data Processing
            "KLAC",   // 89 KLA Corporation
            "KKR",   // 90 KKR
            "ADI",   // 91 Analog Devices
            "APH",   // 92 Amphenol
            "VRTX",   // 93 Vertex Pharmaceuticals
            "COP",   // 94 ConocoPhillips
            "CB",   // 95 Chubb Limited
            "BX",   // 96 Blackstone Inc.
            "MDT",   // 97 Medtronic
            "LMT",   // 98 Lockheed Martin
            "MMC",   // 99 Marsh McLennan
            "ICE",   // 100 Intercontinental Exchange
            "SBUX",   // 101 Starbucks
            "DASH",   // 102 DoorDash
            "AMT",   // 103 American Tower
            "NKE",   // 104 Nike, Inc.
            "CEG",   // 105 Constellation Energy
            "WELL",   // 106 Welltower
            "SO",   // 107 Southern Company
            "MO",   // 108 Altria
            "INTC",   // 109 Intel
            "CME",   // 110 CME Group
            "PLD",   // 111 Prologis
            "TT",   // 112 Trane Technologies
            "BMY",   // 113 Bristol Myers Squibb
            "FI",   // 114 Fiserv
            "COIN",   // 115 Coinbase Global
            "WM",   // 116 Waste Management
            "HCA",   // 117 HCA Healthcare
            "MCK",   // 118 McKesson Corporation
            "DUK",   // 119 Duke Energy
            "CTAS",   // 120 Cintas
            "PH",   // 121 Parker Hannifin
            "CI",   // 122 Cigna
            "MDLZ",   // 123 Mondelez International
            "MCO",   // 124 Moody's Corporation
            "DELL",   // 125 Dell Technologies
            "UPS",   // 126 United Parcel Service
            "CVS",   // 127 CVS Health
            "SHW",   // 128 Sherwin-Williams
            "ELV",   // 129 Elevance Health
            "CDNS",   // 130 Cadence Design Systems
            "ABNB",   // 131 Airbnb
            "TDG",   // 132 TransDigm Group
            "AJG",   // 133 Arthur J. Gallagher & Co.
            "MMM",   // 134 3M
            "RCL",   // 135 Royal Caribbean Group
            "APO",   // 136 Apollo Global Management
            "FTNT",   // 137 Fortinet
            "GD",   // 138 General Dynamics
            "SNPS",   // 139 Synopsys
            "WMB",   // 140 Williams Companies
            "AON",   // 141 Aon
            "RSG",   // 142 Republic Services
            "ORLY",   // 143 O'Reilly Auto Parts
            "ECL",   // 144 Ecolab
            "EMR",   // 145 Emerson Electric
            "EQIX",   // 146 Equinix
            "MAR",   // 147 Marriott International
            "CMG",   // 148 Chipotle Mexican Grill
            "ITW",   // 149 Illinois Tool Works
            "PNC",   // 150 PNC Financial Services
            "PYPL",   // 151 PayPal
            "NOC",   // 152 Northrop Grumman
            "HWM",   // 153 Howmet Aerospace
            "USB",   // 154 U.S. Bancorp
            "CL",   // 155 Colgate-Palmolive
            "ZTS",   // 156 Zoetis
            "MSI",   // 157 Motorola Solutions
            "JCI",   // 158 Johnson Controls
            "EOG",   // 159 EOG Resources
            "ADSK",   // 160 Autodesk
            "BK",   // 161 BNY Mellon
            "VST",   // 162 Vistra Corp.
            "NEM",   // 163 Newmont
            "FCX",   // 164 Freeport-McMoRan
            "KMI",   // 165 Kinder Morgan
            "WDAY",   // 166 Workday, Inc.
            "APD",   // 167 Air Products
            "AXON",   // 168 Axon Enterprise
            "CARR",   // 169 Carrier Global
            "HLT",   // 170 Hilton Worldwide
            "CSX",   // 171 CSX Corporation
            "ROP",   // 172 Roper Technologies
            "MNST",   // 173 Monster Beverage
            "TRV",   // 174 Travelers Companies (The)
            "AZO",   // 175 AutoZone
            "NSC",   // 176 Norfolk Southern Railway
            "COR",   // 177 Cencora
            "DLR",   // 178 Digital Realty
            "REGN",   // 179 Regeneron Pharmaceuticals
            "PWR",   // 180 Quanta Services
            "AFL",   // 181 Aflac
            "TFC",   // 182 Truist Financial
            "NXPI",   // 183 NXP Semiconductors
            "CHTR",   // 184 Charter Communications
            "AEP",   // 185 American Electric Power
            "MET",   // 186 MetLife
            "FDX",   // 187 FedEx
            "SPG",   // 188 Simon Property Group
            "O",   // 189 Realty Income
            "MPC",   // 190 Marathon Petroleum
            "ALL",   // 191 Allstate
            "NDAQ",   // 192 Nasdaq, Inc.
            "OKE",   // 193 ONEOK
            "CTVA",   // 194 Corteva
            "PAYX",   // 195 Paychex
            "PSA",   // 196 Public Storage
            "TEL",   // 197 TE Connectivity
            "AMP",   // 198 Ameriprise Financial
            "PCAR",   // 199 Paccar
            "AIG",   // 200 American International Group
            "PSX",   // 201 Phillips 66
            "BDX",   // 202 Becton Dickinson
            "GWW",   // 203 W. W. Grainger
            "URI",   // 204 United Rentals
            "SRE",   // 205 Sempra
            "FAST",   // 206 Fastenal
            "GM",   // 207 General Motors
            "KR",   // 208 Kroger
            "D",   // 209 Dominion Energy
            "CPRT",   // 210 Copart
            "LHX",   // 211 L3Harris
            "SLB",   // 212 Schlumberger
            "EW",   // 213 Edwards Lifesciences
            "KDP",   // 214 Keurig Dr Pepper
            "CMI",   // 215 Cummins
            "TGT",   // 216 Target Corporation
            "GLW",   // 217 Corning Inc.
            "CCI",   // 218 Crown Castle
            "FICO",   // 219 Fair Isaac
            "MSCI",   // 220 MSCI
            "TTWO",   // 221 Take-Two Interactive
            "HES",   // 222 Hess Corporation
            "VLO",   // 223 Valero Energy
            "EXC",   // 224 Exelon
            "VRSK",   // 225 Verisk Analytics
            "OXY",   // 226 Occidental Petroleum
            "IDXX",   // 227 Idexx Laboratories
            "F",   // 228 Ford Motor Company
            "KMB",   // 229 Kimberly-Clark
            "FIS",   // 230 Fidelity National Information Services
            "ROST",   // 231 Ross Stores
            "AME",   // 232 Ametek
            "PEG",   // 233 Public Service Enterprise Group
            "CBRE",   // 234 CBRE Group
            "FANG",   // 235 Diamondback Energy
            "YUM",   // 236 Yum! Brands
            "KVUE",   // 237 Kenvue
            "CAH",   // 238 Cardinal Health
            "EA",   // 239 Electronic Arts
            "GRMN",   // 240 Garmin
            "DHI",   // 241 D. R. Horton
            "XEL",   // 242 Xcel Energy
            "OTIS",   // 243 Otis Worldwide
            "MCHP",   // 244 Microchip Technology
            "BKR",   // 245 Baker Hughes
            "CTSH",   // 246 Cognizant
            "TRGP",   // 247 Targa Resources
            "PRU",   // 248 Prudential Financial
            "RMD",   // 249 ResMed
            "ROK",   // 250 Rockwell Automation
            "ETR",   // 251 Entergy
            "SYY",   // 252 Sysco
            "BRO",   // 253 Brown & Brown
            "CCL",   // 254 Carnival
            "ED",   // 255 Consolidated Edison
            "HIG",   // 256 Hartford (The)
            "MPWR",   // 257 Monolithic Power Systems
            "WAB",   // 258 Wabtec
            "EQT",   // 259 EQT Corporation
            "CSGP",   // 260 CoStar Group
            "GEHC",   // 261 GE HealthCare
            "IR",   // 262 Ingersoll Rand
            "HSY",   // 263 Hershey Company (The)
            "VICI",   // 264 Vici Properties
            "LYV",   // 265 Live Nation Entertainment
            "ODFL",   // 266 Old Dominion
            "EBAY",   // 267 eBay
            "VMC",   // 268 Vulcan Materials Company
            "A",   // 269 Agilent Technologies
            "ACGL",   // 270 Arch Capital Group
            "DXCM",   // 271 Dexcom
            "WEC",   // 272 WEC Energy Group
            "MLM",   // 273 Martin Marietta Materials
            "DAL",   // 274 Delta Air Lines
            "NRG",   // 275 NRG Energy
            "EFX",   // 276 Equifax
            "XYL",   // 277 Xylem Inc.
            "IT",   // 278 Gartner
            "MTB",   // 279 M&T Bank
            "PCG",   // 280 PG&E Corporation
            "EXR",   // 281 Extra Space Storage
            "RJF",   // 282 Raymond James Financial
            "LVS",   // 283 Las Vegas Sands
            "KHC",   // 284 Kraft Heinz
            "ANSS",   // 285 Ansys
            "STX",   // 286 Seagate Technology
            "STT",   // 287 State Street Corporation
            "NUE",   // 288 Nucor
            "WTW",   // 289 Willis Towers Watson
            "IRM",   // 290 Iron Mountain
            "SMCI",   // 291 Supermicro
            "AVB",   // 292 AvalonBay Communities
            "LEN",   // 293 Lennar
            "DD",   // 294 DuPont
            "HUM",   // 295 Humana
            "EL",   // 296 Estée Lauder Companies (The)
            "STZ",   // 297 Constellation Brands
            "VTR",   // 298 Ventas
            "KEYS",   // 299 Keysight Technologies
            "EXE",   // 300 Expand Energy
            "LULU",   // 301 Lululemon Athletica
            "BR",   // 302 Broadridge Financial Solutions
            "WBD",   // 303 Warner Bros. Discovery
            "FITB",   // 304 Fifth Third Bancorp
            "GIS",   // 305 General Mills
            "TSCO",   // 306 Tractor Supply
            "WRB",   // 307 W. R. Berkley Corporation
            "IQV",   // 308 IQVIA
            "K",   // 309 Kellanova
            "DTE",   // 310 DTE Energy
            "AWK",   // 311 American Water Works
            "ROL",   // 312 Rollins, Inc.
            "CNC",   // 313 Centene Corporation
            "VRSN",   // 314 Verisign
            "AEE",   // 315 Ameren
            "ADM",   // 316 Archer Daniels Midland
            "PPG",   // 317 PPG Industries
            "EQR",   // 318 Equity Residential
            "UAL",   // 319 United Airlines Holdings
            "DRI",   // 320 Darden Restaurants
            "GDDY",   // 321 GoDaddy
            "VLTO",   // 322 Veralto
            "DOV",   // 323 Dover Corporation
            "SYF",   // 324 Synchrony Financial
            "DG",   // 325 Dollar General
            "TYL",   // 326 Tyler Technologies
            "PPL",   // 327 PPL Corporation
            "MTD",   // 328 Mettler Toledo
            "SBAC",   // 329 SBA Communications
            "TPL",   // 330 Texas Pacific Land Corporation
            "IP",   // 331 International Paper
            "ATO",   // 332 Atmos Energy
            "HPE",   // 333 Hewlett Packard Enterprise
            "HBAN",   // 334 Huntington Bancshares
            "FTV",   // 335 Fortive
            "NTRS",   // 336 Northern Trust
            "CNP",   // 337 CenterPoint Energy
            "TDY",   // 338 Teledyne Technologies
            "CBOE",   // 339 Cboe Global Markets
            "STE",   // 340 Steris
            "CHD",   // 341 Church & Dwight
            "CDW",   // 342 CDW
            "HPQ",   // 343 HP Inc.
            "ES",   // 344 Eversource Energy
            "JBL",   // 345 Jabil
            "FE",   // 346 FirstEnergy
            "CPAY",   // 347 Corpay
            "CINF",   // 348 Cincinnati Financial
            "SW",   // 349 Smurfit WestRock
            "ON",   // 350 ON Semiconductor
            "WDC",   // 351 Western Digital
            "PODD",   // 352 Insulet Corporation
            "HUBB",   // 353 Hubbell Incorporated
            "LH",   // 354 LabCorp
            "EXPE",   // 355 Expedia Group
            "AMCR",   // 356 Amcor
            "NVR",   // 357 NVR, Inc.
            "CMS",   // 358 CMS Energy
            "TROW",   // 359 T. Rowe Price
            "WAT",   // 360 Waters Corporation
            "RF",   // 361 Regions Financial Corporation
            "NTAP",   // 362 NetApp
            "PHM",   // 363 PulteGroup
            "ULTA",   // 364 Ulta Beauty
            "MKC",   // 365 McCormick & Company
            "DLTR",   // 366 Dollar Tree
            "DVN",   // 367 Devon Energy
            "PTC",   // 368 PTC Inc.
            "INVH",   // 369 Invitation Homes
            "LDOS",   // 370 Leidos
            "LII",   // 371 Lennox International
            "WSM",   // 372 Williams-Sonoma
            "DGX",   // 373 Quest Diagnostics
            "CTRA",   // 374 Coterra
            "TSN",   // 375 Tyson Foods
            "EIX",   // 376 Edison International
            "STLD",   // 377 Steel Dynamics
            "DOW",   // 378 Dow Inc.
            "CFG",   // 379 Citizens Financial Group
            "GPN",   // 380 Global Payments
            "WY",   // 381 Weyerhaeuser
            "IFF",   // 382 International Flavors & Fragrances
            "L",   // 383 Loews Corporation
            "BIIB",   // 384 Biogen
            "KEY",   // 385 KeyCorp
            "LYB",   // 386 LyondellBasell
            "NI",   // 387 NiSource
            "ESS",   // 388 Essex Property Trust
            "ZBH",   // 389 Zimmer Biomet
            "TPR",   // 390 Tapestry, Inc.
            "GEN",   // 391 Gen Digital
            "LUV",   // 392 Southwest Airlines
            "TRMB",   // 393 Trimble Inc.
            "HAL",   // 394 Halliburton
            "ERIE",   // 395 Erie Indemnity
            "PFG",   // 396 Principal Financial Group
            "MAA",   // 397 Mid-America Apartment Communities
            "PNR",   // 398 Pentair
            "PKG",   // 399 Packaging Corporation of America
            "FSLR",   // 400 First Solar
            "RL",   // 401 Ralph Lauren Corporation
            "HRL",   // 402 Hormel Foods
            "FFIV",   // 403 F5, Inc.
            "FDS",   // 404 FactSet
            "GPC",   // 405 Genuine Parts Company
            "SNA",   // 406 Snap-on
            "WST",   // 407 West Pharmaceutical Services
            "BALL",   // 408 Ball Corporation
            "MOH",   // 409 Molina Healthcare
            "ZBRA",   // 410 Zebra Technologies
            "EVRG",   // 411 Evergy
            "DPZ",   // 412 Domino's
            "J",   // 413 Jacobs Solutions
            "EXPD",   // 414 Expeditors International
            "DECK",   // 415 Deckers Brands
            "BAX",   // 416 Baxter International
            "LNT",   // 417 Alliant Energy
            "APTV",   // 418 Aptiv
            "TER",   // 419 Teradyne
            "CF",   // 420 CF Industries
            "CLX",   // 421 Clorox
            "HOLX",   // 422 Hologic
            "TKO",   // 423 TKO Group Holdings
            "TXT",   // 424 Textron
            "BBY",   // 425 Best Buy
            "EG",   // 426 Everest Group
            "KIM",   // 427 Kimco Realty
            "JBHT",   // 428 J.B. Hunt
            "OMC",   // 429 Omnicom Group
            "COO",   // 430 Cooper Companies (The)
            "INCY",   // 431 Incyte
            "AVY",   // 432 Avery Dennison
            "ALGN",   // 433 Align Technology
            "UDR",   // 434 UDR, Inc.
            "MAS",   // 435 Masco
            "IEX",   // 436 IDEX Corporation
            "JKHY",   // 437 Jack Henry & Associates
            "SOLV",   // 438 Solventum
            "BLDR",   // 439 Builders FirstSource
            "REG",   // 440 Regency Centers
            "ARE",   // 441 Alexandria Real Estate Equities
            "BF.B",   // 442 Brown–Forman
            "FOXA",   // 443 Fox Corporation (Class A)
            "PAYC",   // 444 Paycom
            "BEN",   // 445 Franklin Resources
            "JNPR",   // 446 Juniper Networks
            "ALLE",   // 447 Allegion
            "CPT",   // 448 Camden Property Trust
            "NDSN",   // 449 Nordson Corporation
            "DOC",   // 450 Healthpeak Properties
            "FOX",   // 451 Fox Corporation (Class B)
            "RVTY",   // 452 Revvity
            "UHS",   // 453 Universal Health Services
            "AKAM",   // 454 Akamai Technologies
            "SWKS",   // 455 Skyworks Solutions
            "MOS",   // 456 Mosaic Company (The)
            "CHRW",   // 457 C.H. Robinson
            "POOL",   // 458 Pool Corporation
            "BG",   // 459 Bunge Global
            "HST",   // 460 Host Hotels & Resorts
            "MRNA",   // 461 Moderna
            "BXP",   // 462 BXP, Inc.
            "VTRS",   // 463 Viatris
            "DVA",   // 464 DaVita
            "PNW",   // 465 Pinnacle West
            "SWK",   // 466 Stanley Black & Decker
            "SJM",   // 467 J.M. Smucker Company (The)
            "HAS",   // 468 Hasbro
            "GL",   // 469 Globe Life
            "KMX",   // 470 CarMax
            "AIZ",   // 471 Assurant
            "EPAM",   // 472 EPAM Systems
            "WBA",   // 473 Walgreens Boots Alliance
            "CAG",   // 474 Conagra Brands
            "WYNN",   // 475 Wynn Resorts
            "LKQ",   // 476 LKQ Corporation
            "NWS",   // 477 News Corp (Class B)
            "TAP",   // 478 Molson Coors Beverage Company
            "HII",   // 479 Huntington Ingalls Industries
            "CPB",   // 480 Campbell Soup Company
            "MGM",   // 481 MGM Resorts
            "AOS",   // 482 A. O. Smith
            "HSIC",   // 483 Henry Schein
            "IPG",   // 484 Interpublic Group of Companies (The)
            "DAY",   // 485 Dayforce
            "EMN",   // 486 Eastman Chemical Company
            "NCLH",   // 487 Norwegian Cruise Line Holdings
            "GNRC",   // 488 Generac
            "PARA",   // 489 Paramount Global
            "NWSA",   // 490 News Corp (Class A)
            "TECH",   // 491 Bio-Techne
            "MKTX",   // 492 MarketAxess
            "FRT",   // 493 Federal Realty Investment Trust
            "AES",   // 494 AES Corporation
            "ALB",   // 495 Albemarle Corporation
            "LW",   // 496 Lamb Weston
            "MTCH",   // 497 Match Group
            "CRL",   // 498 Charles River Laboratories
            "IVZ",   // 499 Invesco
            "APA",   // 500 APA Corporation
            "MHK",   // 501 Mohawk Industries
            "CZR",   // 502 Caesars Entertainment
            "ENPH",   // 503 Enphase Energy
#else
            // as of 12/01/2022, see https://www.slickcharts.com/sp500

            "AAPL",  // 1   Apple Inc. AAPL
            "MSFT",  // 2   Microsoft Corporation MSFT
            "AMZN",  // 3   Amazon.com Inc. AMZN
            "GOOGL", // 4   Alphabet Inc. Class A   GOOGL
            "BRK.B", // 5   Berkshire Hathaway Inc. Class B BRK.B
            "GOOG",  // 6   Alphabet Inc. Class C   GOOG
            "TSLA",  // 7   Tesla Inc   TSLA
            "UNH",   // 8   UnitedHealth Group Incorporated UNH
            "JNJ",   // 9   Johnson & Johnson   JNJ
            "XOM",   // 10  Exxon Mobil Corporation XOM
            "NVDA",  // 11  NVIDIA Corporation  NVDA
            "JPM",   // 12  JPMorgan Chase & Co.    JPM
            "PG",    // 13  Procter & Gamble Company    PG
            "V",     // 14  Visa Inc. Class A   V
            "HD",    // 15  Home Depot Inc. HD
            "CVX",   // 16  Chevron Corporation CVX
            "MA",    // 17  Mastercard Incorporated Class A MA
            "LLY",   // 18  Eli Lilly and Company   LLY
            "ABBV",  // 19  AbbVie Inc. ABBV
            "PFE",   // 20  Pfizer Inc. PFE
            "MRK",   // 21  Merck & Co. Inc.    MRK
            "META",  // 22  Meta Platforms Inc. Class A META
            "BAC",   // 23  Bank of America Corp    BAC
            "PEP",   // 24  PepsiCo Inc.    PEP
            "KO",    // 25  Coca-Cola Company   KO
            "COST",  // 26  Costco Wholesale Corporation    COST
            "AVGO",  // 27  Broadcom Inc.   AVGO
            "TMO",   // 28  Thermo Fisher Scientific Inc.   TMO
            "WMT",   // 29  Walmart Inc.    WMT
            "CSCO",  // 30  Cisco Systems Inc.  CSCO
            "MCD",   // 31  McDonald's Corporation	MCD
            "ACN",   // 32  Accenture Plc Class A   ACN
            "ABT",   // 33  Abbott Laboratories ABT
            "WFC",   // 34  Wells Fargo & Company   WFC
            "DHR",   // 35  Danaher Corporation DHR
            "DIS",   // 36  Walt Disney Company DIS
            "BMY",   // 37  Bristol-Myers Squibb Company    BMY
            "LIN",   // 38  Linde plc   LIN
            "NEE",   // 39  NextEra Energy Inc. NEE
            "TXN",   // 40  Texas Instruments Incorporated  TXN
            "VZ",    // 41  Verizon Communications Inc. VZ
            "ADBE",  // 42  Adobe Incorporated  ADBE
            "CMCSA", // 43  Comcast Corporation Class A CMCSA
            "CRM",   // 44  Salesforce Inc. CRM
            "COP",   // 45  ConocoPhillips  COP
            "PM",    // 46  Philip Morris International Inc.    PM
            "AMGN",  // 47  Amgen Inc.  AMGN
            "HON",   // 48  Honeywell International Inc.    HON
            "RTX",   // 49  Raytheon Technologies Corporation   RTX
            "QCOM",  // 50  QUALCOMM Incorporated   QCOM
            "UPS",   // 51  United Parcel Service Inc. Class B  UPS
            "NKE",   // 52  NIKE Inc. Class B   NKE
            "T",     // 53  AT&T Inc.   T
            "NFLX",  // 54  Netflix Inc.    NFLX
            "LOW",   // 55  Lowe's Companies Inc.	LOW
            "UNP",   // 56  Union Pacific Corporation   UNP
            "IBM",   // 57  International Business Machines Corporation IBM
            "CVS",   // 58  CVS Health Corporation  CVS
            "GS",    // 59  Goldman Sachs Group Inc.    GS
            "ELV",   // 60  Elevance Health Inc.    ELV
            "ORCL",  // 61  Oracle Corporation  ORCL
            "SCHW",  // 62  Charles Schwab Corp SCHW
            "AMD",   // 63  Advanced Micro Devices Inc. AMD
            "CAT",   // 64  Caterpillar Inc.    CAT
            "MS",    // 65  Morgan Stanley  MS
            "INTC",  // 66  Intel Corporation   INTC
            "DE",    // 67  Deere & Company DE
            "SPGI",  // 68  S&P Global Inc. SPGI
            "SBUX",  // 69  Starbucks Corporation   SBUX
            "INTU",  // 70  Intuit Inc. INTU
            "LMT",   // 71  Lockheed Martin Corporation LMT
            "GILD",  // 72  Gilead Sciences Inc.    GILD
            "ADP",   // 73  Automatic Data Processing Inc.  ADP
            "PLD",   // 74  Prologis Inc.   PLD
            "BLK",   // 75  BlackRock Inc.  BLK
            "MDT",   // 76  Medtronic Plc   MDT
            "AMT",   // 77  American Tower Corporation  AMT
            "CI",    // 78  Cigna Corporation   CI
            "BA",    // 79  Boeing Company  BA
            "ISRG",  // 80  Intuitive Surgical Inc. ISRG
            "AMAT",  // 81  Applied Materials Inc.  AMAT
            "AXP",   // 82  American Express Company    AXP
            "GE",    // 83  General Electric Company    GE
            "TJX",   // 84  TJX Companies Inc   TJX
            "C",     // 85  Citigroup Inc.  C
            "MDLZ",  // 86  Mondelez International Inc. Class A MDLZ
            "CB",    // 87  Chubb Limited   CB
            "TMUS",  // 88  T-Mobile US Inc.    TMUS
            "PYPL",  // 89  PayPal Holdings Inc.    PYPL
            "ADI",   // 90  Analog Devices Inc. ADI
            "MMC",   // 91  Marsh & McLennan Companies Inc. MMC
            "NOW",   // 92  ServiceNow Inc. NOW
            "MO",    // 93  Altria Group Inc    MO
            "EOG",   // 94  EOG Resources Inc.  EOG
            "BKNG",  // 95  Booking Holdings Inc.   BKNG
            "VRTX",  // 96  Vertex Pharmaceuticals Incorporated VRTX
            "REGN",  // 97  Regeneron Pharmaceuticals Inc.  REGN
            "SYK",   // 98  Stryker Corporation SYK
            "NOC",   // 99  Northrop Grumman Corp.  NOC
            "TGT",   // 100 Target Corporation  TGT
            "PGR",   // 101 Progressive Corporation PGR
            "DUK",   // 102 Duke Energy Corporation DUK
            "SLB",   // 103 Schlumberger NV SLB
            "ZTS",   // 104 Zoetis Inc. Class A ZTS
            "SO",    // 105 Southern Company    SO
            "BDX",   // 106 Becton Dickinson and Company    BDX
            "CSX",   // 107 CSX Corporation CSX
            "MMM",   // 108 3M Company  MMM
            "HUM",   // 109 Humana Inc. HUM
            "PNC",   // 110 PNC Financial Services Group Inc.   PNC
            "ADP",   // 111 Air Products and Chemicals Inc. APD
            "FISV",  // 112 Fiserv Inc. FISV
            "ETN",   // 113 Eaton Corp. Plc ETN
            "AON",   // 114 Aon Plc Class A AON
            "CL",    // 115 Colgate-Palmolive Company   CL
            "LRCX",  // 116 Lam Research Corporation    LRCX
            "BSX",   // 117 Boston Scientific Corporation   BSX
            "ITW",   // 118 Illinois Tool Works Inc.    ITW
            "MU",    // 119 Micron Technology Inc.  MU
            "WM",    // 120 Waste Management Inc.   WM
            "CME",   // 121 CME Group Inc. Class A  CME
            "EQIX",  // 122 Equinix Inc.    EQIX
            "TFC",   // 123 Truist Financial Corporation    TFC
            "USB",   // 124 U.S. Bancorp    USB
            "CCI",   // 125 Crown Castle Inc.   CCI
            "MPC",   // 126 Marathon Petroleum Corporation  MPC
            "ICE",   // 127 Intercontinental Exchange Inc.  ICE
            "NSC",   // 128 Norfolk Southern Corporation    NSC
            "MRNA",  // 129 Moderna Inc.    MRNA
            "GM",    // 130 General Motors Company  GM
            "SHW",   // 131 Sherwin-Williams Company    SHW
            "DG",    // 132 Dollar General Corporation  DG
            "FCX",   // 133 Freeport-McMoRan Inc.   FCX
            "GD",    // 134 General Dynamics Corporation    GD
            "EMR",   // 135 Emerson Electric Co.    EMR
            "PXD",   // 136 Pioneer Natural Resources Company   PXD
            "KLAC",  // 137 KLA Corporation KLAC
            "ORLY",  // 138 O'Reilly Automotive Inc.	ORLY
            "F",     // 139 Ford Motor Company  F
            "MCK",   // 140 McKesson Corporation    MCK
            "ADM",   // 141 Archer-Daniels-Midland Company  ADM
            "EL",    // 142 Estee Lauder Companies Inc. Class A EL
            "ATVI",  // 143 Activision Blizzard Inc.    ATVI
            "VLO",   // 144 Valero Energy Corporation   VLO
            "SRE",   // 145 Sempra Energy   SRE
            "PSX",   // 146 Phillips 66 PSX
            "SNPS",  // 147 Synopsys Inc.   SNPS
            "OXY",   // 148 Occidental Petroleum Corporation    OXY
            "HCA",   // 149 HCA Healthcare Inc  HCA
            "MET",   // 150 MetLife Inc.    MET
            "D",     // 151 Dominion Energy Inc D
            "GIS",   // 152 General Mills Inc.  GIS
            "AZO",   // 153 AutoZone Inc.   AZO
            "CNC",   // 154 Centene Corporation CNC
            "AEP",   // 155 American Electric Power Company Inc.    AEP
            "CTVA",  // 156 Corteva Inc CTVA
            "AIG",   // 157 American International Group Inc.   AIG
            "EW",    // 158 Edwards Lifesciences Corporation    EW
            "APH",   // 159 Amphenol Corporation Class A    APH
            "CDNS",  // 160 Cadence Design Systems Inc. CDNS
            "PSA",   // 161 Public Storage  PSA
            "MCO",   // 162 Moody's Corporation	MCO
            "ROP",   // 163 Roper Technologies Inc. ROP
            "A",     // 164 Agilent Technologies Inc.   A
            "NXPI",  // 165 NXP Semiconductors NV   NXPI
            "KMB",   // 166 Kimberly-Clark Corporation  KMB
            "JCI",   // 167 Johnson Controls International plc  JCI
            "DXCM",  // 168 DexCom Inc. DXCM
            "MSI",   // 169 Motorola Solutions Inc. MSI
            "MAR",   // 170 Marriott International Inc. Class A MAR
            "CMG",   // 171 Chipotle Mexican Grill Inc. CMG
            "TRV",   // 172 Travelers Companies Inc.    TRV
            "DVN",   // 173 Devon Energy Corporation    DVN
            "BIIB",  // 174 Biogen Inc. BIIB
            "FIS",   // 175 Fidelity National Information Services Inc. FIS
            "SYY",   // 176 Sysco Corporation   SYY
            "ADSK",  // 177 Autodesk Inc.   ADSK
            "MCHP",  // 178 Microchip Technology Incorporated   MCHP
            "ENPH",  // 179 Enphase Energy Inc. ENPH
            "LHX",   // 180 L3Harris Technologies Inc   LHX
            "CHTR",  // 181 Charter Communications Inc. Class A CHTR
            "FDX",   // 182 FedEx Corporation   FDX
            "WMB",   // 183 Williams Companies Inc. WMB
            "AJG",   // 184 Arthur J. Gallagher & Co.   AJG
            "TT",    // 185 Trane Technologies plc  TT
            "AFL",   // 186 Aflac Incorporated  AFL
            "ROST",  // 187 Ross Stores Inc.    ROST
            "EXC",   // 188 Exelon Corporation  EXC
            "MSCI",  // 189 MSCI Inc. Class A   MSCI
            "STZ",   // 190 Constellation Brands Inc. Class A   STZ
            "IQV",   // 191 IQVIA Holdings Inc  IQV
            "TEL",   // 192 TE Connectivity Ltd.    TEL
            "PRU",   // 193 Prudential Financial Inc.   PRU
            "HES",   // 194 Hess Corporation    HES
            "CTAS",  // 195 Cintas Corporation  CTAS
            "COF",   // 196 Capital One Financial Corp  COF
            "MNST",  // 197 Monster Beverage Corporation    MNST
            "PAYX",  // 198 Paychex Inc.    PAYX
            "NUE",   // 199 Nucor Corporation   NUE
            "HLT",   // 200 Hilton Worldwide Holdings Inc   HLT
            "O",     // 201 Realty Income Corporation   O
            "SPG",   // 202 Simon Property Group Inc.   SPG
            "PH",    // 203 Parker-Hannifin Corporation PH
            "XEL",   // 204 Xcel Energy Inc.    XEL
            "KMI",   // 205 Kinder Morgan Inc Class P   KMI
            "NEM",   // 206 Newmont Corporation NEM
            "CARR",  // 207 Carrier Global Corp.    CARR
            "ECL",   // 208 Ecolab Inc. ECL
            "DOW",   // 209 Dow Inc.    DOW
            "YUM",   // 210 Yum! Brands Inc.    YUM
            "PCAR",  // 211 PACCAR Inc  PCAR
            "ALL",   // 212 Allstate Corporation    ALL
            "AMP",   // 213 Ameriprise Financial Inc.   AMP
            "IDXX",  // 214 IDEXX Laboratories Inc. IDXX
            "CMI",   // 215 Cummins Inc.    CMI
            "DD",    // 216 DuPont de Nemours Inc.  DD
            "FTNT",  // 217 Fortinet Inc.   FTNT
            "EA",    // 218 Electronic Arts Inc.    EA
            "HSY",   // 219 Hershey Company HSY
            "ED",    // 220 Consolidated Edison Inc.    ED
            "ANET",  // 221 Arista Networks Inc.    ANET
            "HAL",   // 222 Halliburton Company HAL
            "ILMN",  // 223 Illumina Inc.   ILMN
            "BK",    // 224 Bank of New York Mellon Corp    BK
            "RMD",   // 225 ResMed Inc. RMD
            "MTD",   // 226 Mettler-Toledo International Inc.   MTD
            "WELL",  // 227 Welltower Inc   WELL
            "KDP",   // 228 Keurig Dr Pepper Inc.   KDP
            "VICI",  // 229 VICI Properties Inc VICI
            "OTIS",  // 230 Otis Worldwide Corporation  OTIS
            "AME",   // 231 AMETEK Inc. AME
            "ON",    // 232 ON Semiconductor Corporation    ON
            "KEYS",  // 233 Keysight Technologies Inc   KEYS
            "TDG",   // 234 TransDigm Group Incorporated    TDG
            "DLR",   // 235 Digital Realty Trust Inc.   DLR
            "SBAC",  // 236 SBA Communications Corp. Class A    SBAC
            "ALB",   // 237 Albemarle Corporation   ALB
            "CTSH",  // 238 Cognizant Technology Solutions Corporation Class A  CTSH
            "KR",    // 239 Kroger Co.  KR
            "CSGP",  // 240 CoStar Group Inc.   CSGP
            "PPG",   // 241 PPG Industries Inc. PPG
            "DLTR",  // 242 Dollar Tree Inc.    DLTR
            "KHC",   // 243 Kraft Heinz Company KHC
            "CEG",   // 244 Constellation Energy Corporation    CEG
            "WEC",   // 245 WEC Energy Group Inc    WEC
            "ROK",   // 246 Rockwell Automation Inc.    ROK
            "PEG",   // 247 Public Service Enterprise Group Inc PEG
            "MTB",   // 248 M&T Bank Corporation    MTB
            "DFS",   // 249 Discover Financial Services DFS
            "OKE",   // 250 ONEOK Inc.  OKE
            "WBA",   // 251 Walgreens Boots Alliance Inc.   WBA
            "BKR",   // 252 Baker Hughes Company Class A    BKR
            "FAST",  // 253 Fastenal Company    FAST
            "STT",   // 254 State Street Corporation    STT
            "VRSK",  // 255 Verisk Analytics Inc    VRSK
            "GPN",   // 256 Global Payments Inc.    GPN
            "ES",    // 257 Eversource Energy   ES
            "RSG",   // 258 Republic Services Inc.  RSG
            "APTV",  // 259 Aptiv PLC   APTV
            "BAX",   // 260 Baxter International Inc.   BAX
            "CPRT",  // 261 Copart Inc. CPRT
            "TROW",  // 262 T. Rowe Price Group TROW
            "ODFL",  // 263 Old Dominion Freight Line Inc.  ODFL
            "IT",    // 264 Gartner Inc.    IT
            "HPQ",   // 265 HP Inc. HPQ
            "GWW",   // 266 W.W. Grainger Inc.  GWW
            "AWK",   // 267 American Water Works Company Inc.   AWK
            "DHI",   // 268 D.R. Horton Inc.    DHI
            "WTW",   // 269 Willis Towers Watson Public Limited Company WTW
            "IFF",   // 270 International Flavors & Fragrances Inc. IFF
            "ABC",   // 271 AmerisourceBergen Corporation   ABC
            "FANG",  // 272 Diamondback Energy Inc. FANG
            "GPC",   // 273 Genuine Parts Company   GPC
            "GLW",   // 274 Corning Inc GLW
            "CBRE",  // 275 CBRE Group Inc. Class A CBRE
            "CDW",   // 276 CDW Corp.   CDW
            "TSCO",  // 277 Tractor Supply Company  TSCO
            "EIX",   // 278 Edison International    EIX
            "WBD",   // 279 Warner Bros. Discovery Inc. Series A    WBD
            "EBAY",  // 280 eBay Inc.   EBAY
            "PCG",   // 281 PG&E Corporation    PCG
            "ZBH",   // 282 Zimmer Biomet Holdings Inc. ZBH
            "URI",   // 283 United Rentals Inc. URI
            "HIG",   // 284 Hartford Financial Services Group Inc.  HIG
            "FITB",  // 285 Fifth Third Bancorp FITB
            "WY",    // 286 Weyerhaeuser Company    WY
            "ULTA",  // 287 Ulta Beauty Inc.    ULTA
            "AVG",   // 288 AvalonBay Communities Inc.  AVB
            "VMC",   // 289 Vulcan Materials Company    VMC
            "FTV",   // 290 Fortive Corp.   FTV 0.069955       67.73    0.18    (0.26%)
            "EFX",   // 291 Equifax Inc.    EFX
            "ETR",   // 292 Entergy Corporation ETR
            "LUV",   // 293 Southwest Airlines Co.  LUV
            "FRC",   // 294 First Republic Bank FRC
            "NDAQ",  // 295 Nasdaq Inc. NDAQ
            "ARE",   // 296 Alexandria Real Estate Equities Inc.    ARE
            "AEE",   // 297 Ameren Corporation  AEE
            "MLM",   // 298 Martin Marietta Materials Inc.  MLM
            "RJF",   // 299 Raymond James Financial Inc.    RJF
            "DAL",   // 300 Delta Air Lines Inc.    DAL
            "LEN",   // 301 Lennar Corporation Class A  LEN
            "FE",    // 302 FirstEnergy Corp.   FE
            "DTE",   // 303 DTE Energy Company  DTE
            "HBAN",  // 304 Huntington Bancshares Incorporated  HBAN
            "CTRA",  // 305 Coterra Energy Inc. CTRA
            "ANSS",  // 306 ANSYS Inc.  ANSS
            "EQR",   // 307 Equity Residential  EQR
            "RF",    // 308 Regions Financial Corporation   RF
            "CAH",   // 309 Cardinal Health Inc.    CAH
            "ACGL",  // 310 Arch Capital Group Ltd. ACGL
            "LH",    // 311 Laboratory Corporation of America Holdings  LH
            "HPE",   // 312 Hewlett Packard Enterprise Co.  HPE
            "PPL",   // 313 PPL Corporation PPL
            "IR",    // 314 Ingersoll Rand Inc. IR
            "LYB",   // 315 LyondellBasell Industries NV    LYB
            "CF",    // 316 CF Industries Holdings Inc. CF
            "EXR",   // 317 Extra Space Storage Inc.    EXR
            "PWR",   // 318 Quanta Services Inc.    PWR
            "EPAM",  // 319 EPAM Systems Inc.   EPAM
            "MKC",   // 320 McCormick & Company Incorporated    MKC
            "CFG",   // 321 Citizens Financial Group Inc.   CFG
            "PFG",   // 322 Principal Financial Group Inc.  PFG
            "MRO",   // 323 Marathon Oil Corporation    MRO
            "WAT",   // 324 Waters Corporation  WAT
            "DOV",   // 325 Dover Corporation   DOV
            "XYL",   // 326 Xylem Inc.  XYL
            "CHD",   // 327 Church & Dwight Co. Inc.    CHD
            "MOH",   // 328 Molina Healthcare Inc.  MOH
            "TDY",   // 329 Teledyne Technologies Incorporated  TDY
            "CNP",   // 330 CenterPoint Energy Inc. CNP
            "TSN",   // 331 Tyson Foods Inc. Class A    TSN
            "NTRS",  // 332 Northern Trust Corporation  NTRS
            "AES",   // 333 AES Corporation AES
            "HOLX",  // 334 Hologic Inc.    HOLX
            "EXPD",  // 335 Expeditors International of Washington Inc. EXPD
            "INVH",  // 336 Invitation Homes Inc.   INVH
            "MAA",   // 337 Mid-America Apartment Communities Inc.  MAA
            "VRSN",  // 338 VeriSign Inc.   VRSN
            "AMCR",  // 339 Amcor PLC   AMCR
            "STE",   // 340 STERIS Plc  STE
            "VTR",   // 341 Ventas Inc. VTR
            "WAB",   // 342 Westinghouse Air Brake Technologies Corporation WAB
            "K",     // 343 Kellogg Company K
            "SYF",   // 344 Synchrony Financial SYF
            "CLX",   // 345 Clorox Company  CLX
            "CAG",   // 346 Conagra Brands Inc. CAG
            "DRI",   // 347 Darden Restaurants Inc. DRI
            "IEX",   // 348 IDEX Corporation    IEX
            "MOS",   // 349 Mosaic Company  MOS
            "DGX",   // 350 Quest Diagnostics Incorporated  DGX
            "CINF",  // 351 Cincinnati Financial Corporation    CINF
            "BALL",  // 352 Ball Corporation    BALL
            "CMS",   // 353 CMS Energy Corporation  CMS
            "PKI",   // 354 PerkinElmer Inc.    PKI
            "KEY",   // 355 KeyCorp KEY
            "FDS",   // 356 FactSet Research Systems Inc.   FDS
            "BBY",   // 357 Best Buy Co. Inc.   BBY
            "WST",   // 358 West Pharmaceutical Services Inc.   WST
            "BR",    // 359 Broadridge Financial Solutions Inc. BR
            "ABMD",  // 360 ABIOMED Inc.    ABMD
            "MPWR",  // 361 Monolithic Power Systems Inc.   MPWR
            "TRGP",  // 362 Targa Resources Corp.   TRGP
            "ATO",   // 363 Atmos Energy Corporation    ATO
            "ETSY",  // 364 Etsy Inc.   ETSY
            "TTWO",  // 365 Take-Two Interactive Software Inc.  TTWO
            "SJM",   // 366 J.M. Smucker Company    SJM
            "SEDG",  // 367 SolarEdge Technologies Inc. SEDG
            "FMC",   // 368 FMC Corporation FMC
            "OMC",   // 369 Omnicom Group Inc   OMC
            "J",     // 370 Jacobs Solutions Inc.   J
            "PAYC",  // 371 Paycom Software Inc.    PAYC
            "EXPE",  // 372 Expedia Group Inc.  EXPE
            "AVY",   // 373 Avery Dennison Corporation  AVY
            "IRM",   // 374 Iron Mountain Inc.  IRM
            "WRB",   // 375 W. R. Berkley Corporation   WRB
            "EQT",   // 376 EQT Corporation EQT
            "COO",   // 377 Cooper Companies Inc.   COO
            "LVS",   // 378 Las Vegas Sands Corp.   LVS
            "SWKS",  // 379 Skyworks Solutions Inc. SWKS
            "JBHT",  // 380 J.B. Hunt Transport Services Inc.   JBHT
            "APA",   // 381 APA Corp.   APA
            "TXT",   // 382 Textron Inc.    TXT
            "AKAM",  // 383 Akamai Technologies Inc.    AKAM
            "NTAP",  // 384 NetApp Inc. NTAP
            "LDOS",  // 385 Leidos Holdings Inc.    LDOS
            "TRMB",  // 386 Trimble Inc.    TRMB
            "INCY",  // 387 Incyte Corporation  INCY
            "FLT",   // 388 FLEETCOR Technologies Inc.  FLT
            "TER",   // 389 Teradyne Inc.   TER
            "GRMN",  // 390 Garmin Ltd. GRMN
            "NVR",   // 391 NVR Inc.    NVR
            "ALGN",  // 392 Align Technology Inc.   ALGN
            "MTCH",  // 393 Match Group Inc.    MTCH
            "ESS",   // 394 Essex Property Trust Inc.   ESS
            "LKQ",   // 395 LKQ Corporation LKQ
            "UAL",   // 396 United Airlines Holdings Inc.   UAL
            "KIM",   // 397 Kimco Realty Corporation    KIM
            "PEAK",  // 398 Healthpeak Properties Inc.  PEAK
            "ZBRA",  // 399 Zebra Technologies Corporation Class A  ZBRA
            "LNT",   // 400 Alliant Energy Corp LNT
            "DPZ",   // 401 Domino's Pizza Inc.	DPZ
            "HWM",   // 402 Howmet Aerospace Inc.   HWM
            "TYL",   // 403 Tyler Technologies Inc. TYL
            "JKHY",  // 404 Jack Henry & Associates Inc.    JKHY
            "BRO",   // 405 Brown & Brown Inc.  BRO
            "HRL",   // 406 Hormel Foods Corporation    HRL
            "SIVB",  // 407 SVB Financial Group SIVB
            "EVRG",  // 408 Evergy Inc. EVRG
            "IP",    // 409 International Paper Company IP
            "CBOE",  // 410 Cboe Global Markets Inc CBOE
            "IPG",   // 411 Interpublic Group of Companies Inc. IPG
            "PTC",   // 412 PTC Inc.    PTC
            "RE",    // 413 Everest Re Group Ltd.   RE
            "HST",   // 414 Host Hotels & Resorts Inc.  HST
            "GEN",   // 415 Gen Digital Inc.    GEN
            "BF.B",  // 416 Brown-Forman Corporation Class B    BF.B
            "VTRS",  // 417 Viatris Inc.    VTRS
            "RCL",   // 418 Royal Caribbean Group   RCL
            "TECH",  // 419 Bio-Techne Corporation  TECH
            "POOL",  // 420 Pool Corporation    POOL
            "SNA",   // 421 Snap-on Incorporated    SNA
            "PKG",   // 422 Packaging Corporation of America    PKG
            "CPT",   // 423 Camden Property Trust   CPT
            "NDSN",  // 424 Nordson Corporation NDSN
            "LW",    // 425 Lamb Weston Holdings Inc.   LW
            "CHRW",  // 426 C.H. Robinson Worldwide Inc.    CHRW
            "UDR",   // 427 UDR Inc.    UDR
            "SWK",   // 428 Stanley Black & Decker Inc. SWK
            "MGM",   // 429 MGM Resorts International   MGM
            "MAS",   // 430 Masco Corporation   MAS
            "WDC",   // 431 Western Digital Corporation WDC
            "CRL",   // 432 Charles River Laboratories International Inc.   CRL
            "L",     // 433 Loews Corporation   L
            "NI",    // 434 NiSource Inc    NI
            "HSIC",  // 435 Henry Schein Inc.   HSIC
            "KMX",   // 436 CarMax Inc. KMX
            "CPB",   // 437 Campbell Soup Company   CPB
            "GL",    // 438 Globe Life Inc. GL
            "TFX",   // 439 Teleflex Incorporated   TFX
            "CZR",   // 440 Caesars Entertainment Inc   CZR
            "JNPR",  // 441 Juniper Networks Inc.   JNPR
            "CE",    // 442 Celanese Corporation    CE
            "EMN",   // 443 Eastman Chemical Company    EMN
            "VFC",   // 444 V.F. Corporation    VFC
            "CDAY",  // 445 Ceridian HCM Holding Inc.   CDAY
            "PHM",   // 446 PulteGroup Inc. PHM
            "TAP",   // 447 Molson Coors Beverage Company Class B   TAP
            "LYV",   // 448 Live Nation Entertainment Inc.  LYV
            "STX",   // 449 Seagate Technology Holdings PLC STX
            "QRVO",  // 450 Qorvo Inc.  QRVO
            "BXP",   // 451 Boston Properties Inc.  BXP
            "PARA",  // 452 Paramount Global Class B    PARA
            "BWA",   // 453 BorgWarner Inc. BWA
            "ALLE",  // 454 Allegion Public Limited Company ALLE
            "MKTX",  // 455 MarketAxess Holdings Inc.   MKTX
            "REG",   // 456 Regency Centers Corporation REG
            "NRG",   // 457 NRG Energy Inc. NRG
            "FOXA",  // 458 Fox Corporation Class A FOXA
            "BBWI",  // 459 Bath & Body Works Inc.  BBWI
            "CCL",   // 460 Carnival Corporation    CCL
            "WRK",   // 461 WestRock Company    WRK
            "TPR",   // 462 Tapestry Inc.   TPR
            "CMA",   // 463 Comerica Incorporated   CMA
            "AAL",   // 464 American Airlines Group Inc.    AAL
            "HII",   // 465 Huntington Ingalls Industries Inc.  HII
            "FFIV",  // 466 F5 Inc. FFIV
            "AAP",   // 467 Advance Auto Parts Inc. AAP
            "ROL",   // 468 Rollins Inc.    ROL
            "CTLT",  // 469 Catalent Inc    CTLT
            "BIO",   // 470 Bio-Rad Laboratories Inc. Class A   BIO
            "PNW",   // 471 Pinnacle West Capital Corporation   PNW
            "WYNN",  // 472 Wynn Resorts Limited    WYNN
            "UHS",   // 473 Universal Health Services Inc. Class B  UHS
            "SBNY",  // 474 Signature Bank  SBNY
            "IVZ",   // 475 Invesco Ltd.    IVZ
            "RHI",   // 476 Robert Half International Inc.  RHI
            "FBHS",  // 477 Fortune Brands Home & Security Inc. FBHS
            "HAS",   // 478 Hasbro Inc. HAS
            "WHR",   // 479 Whirlpool Corporation   WHR
            "SEE",   // 480 Sealed Air Corporation  SEE
            "ZION",  // 481 Zions Bancorporation N.A.   ZION
            "AOS",   // 482 A. O. Smith Corporation AOS
            "FRT",   // 483 Federal Realty Investment Trust FRT
            "PNR",   // 484 Pentair plc PNR
            "NWSA",  // 485 News Corporation Class A    NWSA
            "BEN",   // 486 Franklin Resources Inc. BEN
            "AIZ",   // 487 Assurant Inc.   AIZ
            "DXC",   // 488 DXC Technology Co.  DXC
            "NCLH",  // 489 Norwegian Cruise Line Holdings Ltd. NCLH
            "GNRC",  // 490 Generac Holdings Inc.   GNRC
            "OGN",   // 491 Organon & Co.   OGN
            "XRAY",  // 492 DENTSPLY SIRONA Inc.    XRAY
            "LNC",   // 493 Lincoln National Corp   LNC
            "ALK",   // 494 Alaska Air Group Inc.   ALK
            "MHK",   // 495 Mohawk Industries Inc.  MHK
            "LUMN",  // 496 Lumen Technologies Inc. LUMN
            "NWL",   // 497 Newell Brands Inc   NWL
            "RL",    // 498 Ralph Lauren Corporation Class A    RL
            "FOX",   // 499 Fox Corporation Class B FOX
            "DVA",   // 500 DaVita Inc. DVA
            "DISH",  // 501 DISH Network Corporation Class A    DISH
            "VNO",   // 502 Vornado Realty Trust    VNO
            "NWS",   // 503 News Corporation Class B    NWS
#endif
        };
#endregion
        #region static oex
        // for now, we just take the top-100 of spx
        #endregion
        #region static ndx
        private static List<string> _staticNdx = new List<string>
        {
            // see https://www.slickcharts.com/nasdaq100
#if EXTENSION
            "NVDA",   // 1 Nvidia
            "MSFT",   // 2 Microsoft
            "AAPL",   // 3 Apple Inc.
            "AMZN",   // 4 Amazon
            "META",   // 5 Meta Platforms
            "AVGO",   // 6 Broadcom Inc.
            "GOOG",   // 7 Alphabet Inc. (Class C)
            "GOOGL",   // 8 Alphabet Inc. (Class A)
            "TSLA",   // 9 Tesla, Inc.
            "NFLX",   // 10 Netflix
            "COST",   // 11 Costco
            "PLTR",   // 12 Palantir Technologies
            "ASML",   // 13 ASML Holding
            "CSCO",   // 14 Cisco
            "TMUS",   // 15 T-Mobile US
            "AMD",   // 16 Advanced Micro Devices Inc.
            "LIN",   // 17 Linde plc
            "AZN",   // 18 AstraZeneca
            "INTU",   // 19 Intuit
            "ISRG",   // 20 Intuitive Surgical
            "TXN",   // 21 Texas Instruments
            "BKNG",   // 22 Booking Holdings
            "PEP",   // 23 PepsiCo
            "QCOM",   // 24 Qualcomm
            "ARM",   // 25 Arm Holdings
            "ADBE",   // 26 Adobe Inc.
            "AMGN",   // 27 Amgen
            "PDD",   // 28 PDD Holdings
            "AMAT",   // 29 Applied Materials
            "SHOP",   // 30 Shopify
            "HON",   // 31 Honeywell
            "MU",   // 32 Micron Technology
            "PANW",   // 33 Palo Alto Networks
            "GILD",   // 34 Gilead Sciences
            "CMCSA",   // 35 Comcast
            "MELI",   // 36 MercadoLibre
            "CRWD",   // 37 CrowdStrike
            "LRCX",   // 38 Lam Research
            "ADP",   // 39 ADP
            "KLAC",   // 40 KLA Corporation
            "APP",   // 41 Applovin Corp
            "ADI",   // 42 Analog Devices
            "VRTX",   // 43 Vertex Pharmaceuticals
            "MSTR",   // 44 MicroStrategy Inc.
            "SBUX",   // 45 Starbucks
            "DASH",   // 46 DoorDash
            "CEG",   // 47 Constellation Energy
            "INTC",   // 48 Intel
            "CTAS",   // 49 Cintas
            "MDLZ",   // 50 Mondelez International
            "CDNS",   // 51 Cadence Design Systems
            "ABNB",   // 52 Airbnb
            "FTNT",   // 53 Fortinet
            "SNPS",   // 54 Synopsys
            "ORLY",   // 55 O'Reilly Automotive
            "MAR",   // 56 Marriott International
            "PYPL",   // 57 PayPal
            "MRVL",   // 58 Marvell Technology
            "ADSK",   // 59 Autodesk
            "WDAY",   // 60 Workday, Inc.
            "AXON",   // 61 Axon Enterprise Inc.
            "CSX",   // 62 CSX Corporation
            "ROP",   // 63 Roper Technologies
            "MNST",   // 64 Monster Beverage
            "REGN",   // 65 Regeneron Pharmaceuticals
            "NXPI",   // 66 NXP Semiconductors
            "CHTR",   // 67 Charter Communications
            "AEP",   // 68 American Electric Power
            "TEAM",   // 69 Atlassian
            "PAYX",   // 70 Paychex
            "PCAR",   // 71 Paccar
            "ZS",   // 72 Zscaler
            "FAST",   // 73 Fastenal
            "CPRT",   // 74 Copart
            "DDOG",   // 75 Datadog
            "KDP",   // 76 Keurig Dr Pepper
            "TTWO",   // 77 Take-Two Interactive
            "EXC",   // 78 Exelon
            "VRSK",   // 79 Verisk
            "IDXX",   // 80 Idexx Laboratories
            "CCEP",   // 81 Coca-Cola Europacific Partners
            "ROST",   // 82 Ross Stores
            "FANG",   // 83 Diamondback Energy
            "EA",   // 84 Electronic Arts
            "XEL",   // 85 Xcel Energy
            "MCHP",   // 86 Microchip Technology
            "BKR",   // 87 Baker Hughes
            "CTSH",   // 88 Cognizant
            "TTD",   // 89 Trade Desk (The)
            "CSGP",   // 90 CoStar Group
            "GEHC",   // 91 GE HealthCare
            "ODFL",   // 92 Old Dominion Freight Line
            "DXCM",   // 93 DexCom
            "KHC",   // 94 Kraft Heinz
            "ANSS",   // 95 Ansys
            "LULU",   // 96 Lululemon Athletica
            "WBD",   // 97 Warner Bros. Discovery
            "CDW",   // 98 CDW Corporation
            "ON",   // 99 Onsemi
            "GFS",   // 100 GlobalFoundries
            "BIIB",   // 101 Biogen
#else
            "AAPL",  // 1   Apple Inc   AAPL
            "MSFT",  // 2   Microsoft Corp  MSFT
            "AMZN",  // 3   Amazon.com Inc  AMZN
            "GOOG",  // 4   Alphabet Inc    GOOG
            "GOOGL", // 5   Alphabet Inc    GOOGL
            "TSLA",  // 6   Tesla Inc   TSLA
            "NVDA",  // 7   NVIDIA Corp NVDA
            "PEP",   // 8   PepsiCo Inc PEP
            "COST",  // 9   Costco Wholesale Corp   COST
            "META",  // 10  Meta Platforms Inc  META
            "AVGO",  // 11  Broadcom Inc    AVGO
            "CSCO",  // 12  Cisco Systems Inc   CSCO
            "TMUS",  // 13  T-Mobile US Inc TMUS
            "TXN",   // 14  Texas Instruments Inc   TXN
            "CMCSA", // 15  Comcast Corp    CMCSA
            "ADBE",  // 16  Adobe Inc   ADBE
            "AMGN",  // 17  Amgen Inc   AMGN
            "HON",   // 18  Honeywell International Inc HON
            "QCOM",  // 19  QUALCOMM Inc    QCOM
            "NFLX",  // 20  Netflix Inc NFLX
            "INTC",  // 21  Intel Corp  INTC
            "AMD",   // 22  Advanced Micro Devices Inc  AMD
            "SBUX",  // 23  Starbucks Corp  SBUX
            "GILD",  // 24  Gilead Sciences Inc GILD
            "INTU",  // 25  Intuit Inc  INTU
            "ADP",   // 26  Automatic Data Processing Inc   ADP
            "ISRG",  // 27  Intuitive Surgical Inc  ISRG
            "MDLZ",  // 28  Mondelez International Inc  MDLZ
            "PYPL",  // 29  PayPal Holdings Inc PYPL
            "AMAT",  // 30  Applied Materials Inc   AMAT
            "ADI",   // 31  Analog Devices Inc  ADI
            "VRTX",  // 32  Vertex Pharmaceuticals Inc  VRTX
            "BKNG",  // 33  Booking Holdings Inc    BKNG
            "REGN",  // 34  Regeneron Pharmaceuticals Inc   REGN
            "CSX",   // 35  CSX Corp    CSX
            "MRNA",  // 36  Moderna Inc MRNA
            "FISV",  // 37  Fiserv Inc  FISV
            "CHTR",  // 38  Charter Communications Inc  CHTR
            "MU",    // 39  Micron Technology Inc   MU
            "LRCX",  // 40  Lam Research Corp   LRCX
            "ATVI",  // 41  Activision Blizzard Inc ATVI
            "KDP",   // 42  Keurig Dr Pepper Inc    KDP
            "ORLY",  // 43  O'Reilly Automotive Inc	ORLY
            "KLAC",  // 44  KLA Corp    KLAC
            "MNST",  // 45  Monster Beverage Corp   MNST
            "MAR",   // 46  Marriott International Inc/MD   MAR
            "PANW",  // 47  Palo Alto Networks Inc  PANW
            "ASML",  // 48  ASML Holding NV ASML
            "SNPS",  // 49  Synopsys Inc    SNPS
            "AEP",   // 50  American Electric Power Co Inc  AEP
            "KHC",   // 51  Kraft Heinz Co/The  KHC
            "CTAS",  // 52  Cintas Corp CTAS
            "CDNS",  // 53  Cadence Design Systems Inc  CDNS
            "MELI",  // 54  MercadoLibre Inc    MELI
            "LULU",  // 55  Lululemon Athletica Inc LULU
            "DXCM",  // 56  Dexcom Inc  DXCM
            "NXPI",  // 57  NXP Semiconductors NV   NXPI
            "PAYX",  // 58  Paychex Inc PAYX
            "BIIB",  // 59  Biogen Inc  BIIB
            "ADSK",  // 60  Autodesk Inc    ADSK
            "ENPH",  // 61  Enphase Energy Inc  ENPH
            "MCHP",  // 62  Microchip Technology Inc    MCHP
            "ROST",  // 63  Ross Stores Inc ROST
            "FTNT",  // 64  Fortinet Inc    FTNT
            "EXC",   // 65  Exelon Corp EXC
            "AZN",   // 66  AstraZeneca PLC ADR AZN
            "ABNB",  // 67  Airbnb Inc  ABNB
            "XEL",   // 68  Xcel Energy Inc XEL
            "PDD",   // 69  Pinduoduo Inc ADR   PDD
            "MRVL",  // 70  Marvell Technology Inc  MRVL
            "PCAR",  // 71  PACCAR Inc  PCAR
            "WBA",   // 72  Walgreens Boots Alliance Inc    WBA
            "EA",    // 73  Electronic Arts Inc EA
            "IDXX",  // 74  IDEXX Laboratories Inc  IDXX
            "ILMN",  // 75  Illumina Inc    ILMN
            "DLTR",  // 76  Dollar Tree Inc DLTR
            "ODFL",  // 77  Old Dominion Freight Line Inc   ODFL
            "CTSH",  // 78  Cognizant Technology Solutions Corp CTSH
            "CEG",   // 79  Constellation Energy Corp   CEG
            "CPRT",  // 80  Copart Inc  CPRT
            "CRWD",  // 81  Crowdstrike Holdings Inc    CRWD
            "FAST",  // 82  Fastenal Co FAST
            "WDAY",  // 83  Workday Inc WDAY
            "VRSK",  // 84  Verisk Analytics Inc    VRSK
            "JD",    // 85  JD.com Inc ADR  JD
            "SIRI",  // 86  Sirius XM Holdings Inc  SIRI
            "EBAY",  // 87  eBay Inc    EBAY
            "SGEN",  // 88  Seagen Inc  SGEN
            "DDOG",  // 89  Datadog Inc DDOG
            "ANSS",  // 90  ANSYS Inc   ANSS
            "VRSN",  // 91  VeriSign Inc    VRSN
            "ZS",    // 92  Zscaler Inc ZS
            "BIDU",  // 93  Baidu Inc ADR   BIDU
            "ZM",    // 94  Zoom Video Communications Inc   ZM
            "TEAM",  // 95  Atlassian Corp  TEAM
            "LCID",  // 96  Lucid Group Inc LCID
            "ALGN",  // 97  Align Technology Inc    ALGN
            "SWKS",  // 98  Skyworks Solutions Inc  SWKS
            "MTCH",  // 99  Match Group Inc MTCH
            "SPLK",  // 100 Splunk Inc  SPLK
            "NTES",  // 101 NetEase Inc ADR NTES
            "DOCU",  // 102 DocuSign Inc    DOCU
#endif
        };
#endregion
        #region static dji
        private static List<string> _staticDji = new List<string>
        {
            // as of 12/01/2022, see https://www.slickcharts.com/dowjones
            "UNH",   // 1   UnitedHealth Group Incorporated UNH
            "GS",    // 2   Goldman Sachs Group Inc.    GS
            "HD",    // 3   Home Depot Inc. HD
            "AMGN",  // 4   Amgen Inc.  AMGN
            "MCD",   // 5   McDonald's Corporation	MCD
            "MSFT",  // 6   Microsoft Corporation   MSFT
            "CAT",   // 7   Caterpillar Inc.    CAT
            "HON",   // 8   Honeywell International Inc.    HON
            "V",     // 9   Visa Inc. Class A   V
            "TRV",   // 10  Travelers Companies Inc.    TRV
            "CVX",   // 11  Chevron Corporation CVX
            "BA",    // 12  Boeing Company  BA
            "JNJ",   // 13  Johnson & Johnson   JNJ
            "CRM",   // 14  Salesforce Inc. CRM
            "AXP",   // 15  American Express Company    AXP
            "WMT",   // 16  Walmart Inc.    WMT
            "PG",    // 17  Procter & Gamble Company    PG
            "IBM",   // 18  International Business Machines Corporation IBM
            "AAPL",  // 19  Apple Inc.  AAPL
            "JPM",   // 20  JPMorgan Chase & Co.    JPM
            "MMM",   // 21  3M Company  MMM
            "MRK",   // 22  Merck & Co. Inc.    MRK
            "NKE",   // 23  NIKE Inc. Class B   NKE
            "DIS",   // 24  Walt Disney Company DIS
            "KO",    // 25  Coca-Cola Company   KO
            "DOW",   // 26  Dow Inc.    DOW
            "CSCO",  // 27  Cisco Systems Inc.  CSCO
            "WBA",   // 28  Walgreens Boots Alliance Inc.   WBA
            "VZ",    // 29  Verizon Communications Inc. VZ
            "INTC",  // 30  Intel Corporation   INTC
        };
        #endregion

#if EXTENSION
        #region static futures ETF
        private static List<string> _staticFuturesETF = new List<string>
        {
            "6A=F",     // 1  Australian dollar
            "6B=F",     // 2  British pound
            "ZC=F",     // 3  Corn
            "CC=F",     // 4  Cocoa
            "6C=F",     // 5  Canadian dollar
            "CL=F",     // 6  Crude oil
            "CT=F",     // 7  Cotton
            "6E=F",     // 8  Euro
            "GF=F",     // 10 Feeder cattle
            "GC=F",     // 11 Gold
            "HG=F",     // 12 Copper
            "HO=F",     // 13 Heating oil
            "RB=F",     // 14 Unleaded gas
            "6J=F",     // 15 Japanese yen
            "KC=F",     // 16 Coffee
            "LE=F",     // 17 Live cattle
            "HE=F",     // 18 Hogs
            "6M=F",     // 19 Mexican peso
            "NG=F",     // 20 Natural gas
            "ZL=F",     // 21 Soybeans
            "SB=F",     // 22 Sugar
            "6S=F",     // 23 Swiss franc
            "SI=F",     // 24 Silver
            "ZF=F",     // 25 Treasury notes
            "ZB=F",     // 26 Treasury bonds
            "ZW=F",     // 27 Wheat
            "BTC=F",    // 28 Bitcoin
        };
        #endregion
        #region static country ETF
        private static List<string> _staticCountryETF = new List<string>
        {
            "ENZL",    // 1  New Zealand
            "INDA",    // 2  India
            "VTI",     // 3  United States
            "EWA",     // 4  Australia
            "EWT",     // 5  Taiwan
            "EDEN",    // 6  Denmark
            "EWL",     // 7  Switzerland
            "THD",     // 8  Thailand
            "EWQ",     // 10 France
            "EWC",     // 11 Canada
            "EWJ",     // 12 Japan
            "EWN",     // 13 Netherlands
            "EWK",     // 14 Belgium
            "EWM",     // 15 Malaysia
            "EIRL",    // 16 Ireland
            "EWU",     // 17 United Kingdom
            "VNM",     // 18 Vietnam
            "EPU",     // 19 Peru
            "EWG",     // 20 Germany
            "EWD",     // 21 Sweden
            "EWH",     // 22 Hong Kong
            "EZA",     // 23 South Africa
            "EWS",     // 24 Singapore
            "EIDO",    // 25 Indonesia
            "EWY",     // 26 South Korea
            "ARGT",    // 27 Argentina
            "EIS",     // 28 Israel
            "ECH",     // 29 Chile
            "EWI",     // 30 Italy
            "EPHE",    // 31 Philippines
            "EPOL",    // 32 Poland
            "NORW",    // 33 Norway
            "EWW",     // 34 Mexico
            "QAT",     // 35 Qatar
            "EWP",     // 36 Spain
            "FXI",     // 37 China
            "EWO",     // 38 Austria
            "TUR",     // 39 Turkey
            "EWZ",     // 40 Brazil
            "GREK",    // 41 Greece
            "GXG",     // 42 Colombia
        };
        #endregion
        #region static industry ETF
        private static List<string> _staticIndustryETF = new List<string>
        {
            "IYR",     // 1  iShares U.S. Real Estate ETF
            "IYT",     // 2  iShares Transportation Average ETF
            "SMH",     // 3  VanEck Semiconductor ETF
            "XHB",     // 4  SPDR S&P Homebuilders ETF
            "XLB",     // 5  The Materials Select Sector SPDR Fund
            "XLE",     // 6  The Energy Select Sector SPDR Fund
            "XLF",     // 7  The Financial Select Sector SPDR Fund
            "XLI",     // 8  The Industrial Select Sector SPDR Fund
            "XLK",     // 10 The Technology Select Sector SPDR Fund
            "XLP",     // 11 The Consumer Staples Select Sector SPDR Fund
            "XLU",     // 12 The Utilities Select Sector SPDR Fund
            "XLV",     // 13 The Health Care Select Sector SPDR Fund
            "XLY",     // 14 The Consumer Discretionary Select Sector SPDR Fund
            "XRT",     // 15 SPDR S&P Retail ETF
        };
        #endregion
#endif
        /// <summary>
        /// Return static universe. The constituents for these universes are
        /// time-invariant, hence suffering from survivorship bias.
        /// </summary>
        /// <param name="algo">parent algorithm</param>
        /// <param name="universe">universe name</param>
        /// <param name="datafeed">datafeed name</param>
        /// <returns>universe constituents</returns>
        /// <exception cref="Exception"></exception>
        public static HashSet<string> StaticGetUniverse(Algorithm algo, string universe, string datafeed)
        {
            var constituents = universe.ToLower() switch
            {
                "$spx" => _staticSpx,
                "$oex" => _staticSpx.Take(100),
                "$ndx" => _staticNdx,
                "$dji" => _staticDji,
#if EXTENSION
                "$fut" => _staticFuturesETF,
                "$cnt" => _staticCountryETF,
                "$ind" => _staticIndustryETF,
#endif
                _ => throw new Exception(string.Format("Universe {0}:{1} not supported", datafeed, universe)),
            };
#if EXTENSION
            return constituents.ToHashSet();
#else
            var datafeedL = datafeed.ToLower();
            return constituents
                .Select(name => datafeed + ':' + name)
                .ToHashSet();
#endif
        }
    }
}

//==============================================================================
// end of file
