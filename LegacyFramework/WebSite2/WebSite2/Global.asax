<%@ Application Language="C#" %>
<%@ Import Namespace="WebSite2" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="OpenTelemetry" %>
<%@ Import Namespace="OpenTelemetry.Instrumentation" %>
<%@ Import Namespace="OpenTelemetry.Instrumentation.Http" %>
<%@ Import Namespace="OpenTelemetry.Instrumentation.Runtime" %>
<%@ Import Namespace="OpenTelemetry.Trace" %>
<%@ Import Namespace="OpenTelemetry.Resources" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.Net.Http" %>

<script runat="server">

    private TracerProvider tracerProvider;

    void Application_Start(object sender, EventArgs e)
    {
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);

        tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddAspNetInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter(x => x.Endpoint = new Uri("http://****:4317"))
            .AddSource("web-app")
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: "my-service-name", serviceVersion: "1.0.0"))

            .Build();

            
        ActivitySource activitySource = new ActivitySource("web-app");

        using (var activity = activitySource.StartActivity("start", ActivityKind.Internal))
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
    }

    void Application_End()
    {
        tracerProvider?.Dispose();
    }

</script>
