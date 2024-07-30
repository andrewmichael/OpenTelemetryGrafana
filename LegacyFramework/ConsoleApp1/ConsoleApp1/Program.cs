using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;

using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var traceProvider = Sdk.CreateTracerProviderBuilder()
                .AddSource("foo")
                .AddConsoleExporter()
                .AddOtlpExporter(x => x.Endpoint = new Uri("http://****:4317"))
                .AddHttpClientInstrumentation()
                .ConfigureResource(r => r.AddService("example"))
                .Build();

            ActivitySource activitySource = new ActivitySource("foo");

            using (var activity = activitySource.StartActivity("process", ActivityKind.Internal))
            {

                activity?.SetTag("foo", 1);
                activity?.SetTag("bar", "Hello, World!");
                activity?.SetTag("baz", new int[] { 1, 2, 3 });
                activity?.SetStatus(ActivityStatusCode.Ok);

                using (var httpClient = new HttpClient())
                {
                    httpClient.GetAsync("http://****:5100/weatherforecast?latitude=41&longitude=41").GetAwaiter().GetResult();
                }
            }

            traceProvider.Dispose();
        }
    }
}
