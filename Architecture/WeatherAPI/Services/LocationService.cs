using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using Serilog.Configuration;
using WeatherAPI.Models;
using WeatherAPI.Services.Interfaces;

namespace WeatherAPI.Services
{
    public class LocationService : ILocationService
    {
        private HttpClient _httpClient;
        private JsonSerializerOptions _options;
        private readonly ILogger<LocationService> _logger;
        private readonly ActivitySource _activitySource;

        public LocationService(HttpClient httpClient, ILogger<LocationService> logger, ActivitySource activitySource)
        {
            _httpClient = httpClient;
            _logger = logger;
            _activitySource = activitySource;
            
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<Location> GetLocation(double latitude, double longitude)
        {
            //Tracer.CurrentSpan.Context.TraceId;
            using (var activity = _activitySource.StartActivity("process"))
            {
                activity.SetTag("key", "value");
                activity.AddEvent(new ActivityEvent("event"));
                
                _logger.LogInformation("something");
                
                // https://localhost:5501/Location?latitude=51.260197&longitude=4.402771
                return JsonSerializer.Deserialize<Location>(
                    await _httpClient.GetStringAsync(
                        $"http://location.api:5500/Location?latitude={latitude.ToString(CultureInfo.InvariantCulture)}&longitude={longitude.ToString(CultureInfo.InvariantCulture)}"),
                    _options);
            }
        }
    }
}

