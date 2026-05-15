//==============================================================================
// Project: TuringTrader v2
// Name: ResilientHttpClientFactory
// Description: Central factory for creating resilient typed HttpClients
//==============================================================================

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Timeout;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace TuringTrader.SimulatorV2
{
    /// <summary>
    /// Central factory for creating resilient typed HttpClients with lazy initialization.
    /// </summary>
    public static class ResilientHttpClientFactory
    {
        // Lazy initialization - created only when first accessed
        private static readonly Lazy<ServiceProvider> _tiingoProvider =
            new Lazy<ServiceProvider>(() => CreateClient<TiingoService>(
                baseAddress: "https://api.tiingo.com",
                clientTimeout: TimeSpan.FromSeconds(30),
                pipelineName: "TiingoPipeline",
                rateLimitTokens: 20,
                tokensPerPeriod: 10,
                maxRetries: 4,
                retryDelay: TimeSpan.FromSeconds(1),
                circuitBreakDuration: TimeSpan.FromSeconds(20),
                authenticationScheme: "Token",
                apiToken: Simulator.GlobalSettings.TiingoApiKey
                ),
                LazyThreadSafetyMode.ExecutionAndPublication);

        private static readonly Lazy<ServiceProvider> _yahooProvider =
            new Lazy<ServiceProvider>(() => CreateClient<YahooService>(
                baseAddress: "https://query1.finance.yahoo.com",
                clientTimeout: TimeSpan.FromSeconds(25),
                pipelineName: "YahooPipeline",
                rateLimitTokens: 8,
                tokensPerPeriod: 3,
                maxRetries: 3,
                retryDelay: TimeSpan.FromSeconds(1.5),
                circuitBreakDuration: TimeSpan.FromSeconds(30)),
                LazyThreadSafetyMode.ExecutionAndPublication);

        private static readonly Lazy<ServiceProvider> _stooqProvider =
            new Lazy<ServiceProvider>(() => CreateClient<StooqService>(
                baseAddress: "https://stooq.com",
                clientTimeout: TimeSpan.FromSeconds(20),
                pipelineName: "StooqPipeline",
                rateLimitTokens: 30,    
                tokensPerPeriod: 15,
                maxRetries: 3,
                retryDelay: TimeSpan.FromSeconds(1),
                circuitBreakDuration: TimeSpan.FromSeconds(25)),
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Generic method to create a typed resilient HttpClient.
        /// </summary>
        private static ServiceProvider CreateClient<TService>(
            string baseAddress,
            TimeSpan clientTimeout,
            string pipelineName,
            int rateLimitTokens,
            int tokensPerPeriod,
            int maxRetries,
            TimeSpan retryDelay,
            TimeSpan circuitBreakDuration,
            string authenticationScheme = null,
            string apiToken = null)
            where TService : class
        {
            var services = new ServiceCollection();

            services.AddHttpClient<TService>(client =>
            {
                client.BaseAddress = new Uri(baseAddress);
                client.Timeout = clientTimeout;

                // === Default headers for all  clients ===
                if (!client.DefaultRequestHeaders.Contains("User-Agent"))
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "TuringTrader-Simulator/2.0");
                    client.DefaultRequestHeaders.Add("Cookie", "session=your_session_value_here");
                }

                // === Optional: Add authentication header if scheme and token are provided ===
                if (!string.IsNullOrEmpty(authenticationScheme) && !string.IsNullOrEmpty(apiToken))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue(authenticationScheme, apiToken);
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer(),
                AutomaticDecompression = System.Net.DecompressionMethods.GZip
                                       | System.Net.DecompressionMethods.Deflate,
                AllowAutoRedirect = true
            })
            .AddResilienceHandler(pipelineName, builder =>
            {
                ConfigureResiliencePipeline(builder, rateLimitTokens, tokensPerPeriod,
                                           maxRetries, retryDelay, circuitBreakDuration);
            });

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Generic resilience pipeline configuration used by all data sources.
        /// </summary>
        private static void ConfigureResiliencePipeline(
            ResiliencePipelineBuilder<HttpResponseMessage> builder,
            int rateLimitTokens,
            int tokensPerPeriod,
            int maxRetries,
            TimeSpan retryDelay,
            TimeSpan circuitBreakDuration)
        {
            var tokenBucketLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
            {
                TokenLimit = rateLimitTokens,
                TokensPerPeriod = tokensPerPeriod,
                ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                QueueLimit = 30,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                AutoReplenishment = true
            });

            builder.AddRateLimiter(new HttpRateLimiterStrategyOptions
            {
                RateLimiter = _ => tokenBucketLimiter.AcquireAsync(1, CancellationToken.None)
            });

            builder.AddTimeout(new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromSeconds(90) });

            builder.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = maxRetries,
                Delay = retryDelay,
                MaxDelay = TimeSpan.FromSeconds(15),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutRejectedException>()
                    .HandleResult(r => (int)r.StatusCode >= 500
                                    || r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            });

            builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                SamplingDuration = TimeSpan.FromSeconds(30),
                FailureRatio = 0.45,
                MinimumThroughput = 8,
                BreakDuration = circuitBreakDuration,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutRejectedException>()
                    .HandleResult(r => (int)r.StatusCode >= 500)
            });

            builder.AddTimeout(new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromSeconds(25) });
        }

        #region Public Service Accessors

        /// <summary>
        /// Gets the configured TiingoService (lazy initialized)
        /// </summary>
        public static TiingoService Tiingo => _tiingoProvider.Value.GetRequiredService<TiingoService>();

        /// <summary>
        /// Gets the configured YahooService (lazy initialized)
        /// </summary>
        public static YahooService Yahoo => _yahooProvider.Value.GetRequiredService<YahooService>();

        /// <summary>
        /// Gets the configured StooqService (lazy initialized)
        /// </summary>
        public static StooqService Stooq => _stooqProvider.Value.GetRequiredService<StooqService>();
        #endregion
    }

    public class HttpAccessService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructor - HttpClient is already configured by the factory
        /// </summary>
        /// <param name="httpClient">Pre-configured resilient HttpClient</param>
        public HttpAccessService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Retrieves data as JSON string
        /// </summary>
        public async Task<string> GetAsJsonAsync(string url, string symbol, string typeOfData, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetStringAsync(url, cancellationToken) ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch {typeOfData} for {symbol}", ex);
            }
        }

        /// <summary>
        /// Retrieves data as a readable string (UTF-8) for both HTML pages and JSON
        /// </summary>
        public async Task<string> GetAsStringAsync(string url, string symbol, string typeOfData, CancellationToken cancellationToken = default)
        {
            try
            {
                // IMPORTANT: Use GetAsync + ReadAsStringAsync instead of GetStringAsync
                using var response = await _httpClient.GetAsync(url, cancellationToken);

                response.EnsureSuccessStatusCode();   // throws on 4xx/5xx

                // Reads the content as a string with the correct encoding
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch {typeOfData} for {symbol}", ex);
            }
        }

    }

    /// <summary>
    /// Typed HttpClient service for Tiingo API with built-in resilience pipeline.
    /// </summary>
    public class TiingoService : HttpAccessService
    {
        public TiingoService(HttpClient httpClient) : base(httpClient) { }
    }

    /// <summary>
    /// Typed HttpClient service for Yahoo API with built-in resilience pipeline.
    /// </summary>
    public class YahooService : HttpAccessService
    {
        public YahooService(HttpClient httpClient) : base(httpClient) { }
    }

    /// <summary>
    /// Typed HttpClient service for Stooq API with built-in resilience pipeline.
    /// </summary>
    public class StooqService : HttpAccessService
    {
        public StooqService(HttpClient httpClient) : base(httpClient) { }
    }
}
//==============================================================================
// end of file