using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reactive;
using System.Diagnostics;
using System.Net.Http;

namespace AzureFunctions_DiagnosticSource
{
    public static class HttpHandlerDiagnosticListenerFunction
    {
        [FunctionName("HttpHandlerDiagnosticListener")]
        public static async Task<IActionResult> Run(
[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            /*using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetSampler(new AlwaysOnSampler())
            .AddSource("MyCompany.MyProduct.MyLibrary")
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .Build();
            */

            // string name = req.Query["name"];

            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // dynamic data = JsonConvert.DeserializeObject(requestBody);

            string eventNames = string.Empty;

            Action<KeyValuePair<string, object>> callback = (KeyValuePair<string, object> evnt) =>
            {
                eventNames += evnt.Key + "\n";
                Console.WriteLine($"{evnt.Key} {evnt.Value}");
                Console.WriteLine($"Activity.Current.Id: {Activity.Current?.Id}");
                Console.WriteLine($"Activity.Current.ParentId: {Activity.Current?.ParentId}");
                Console.WriteLine($"Activity.Current.OperationName: {Activity.Current?.OperationName}");
                Console.WriteLine("=========================================================================");
            };

            // Turn it into an observer (using System.Reactive.Core's AnonymousObserver)
            IObserver<KeyValuePair<string, object>> observer = new AnonymousObserver<KeyValuePair<string, object>>(callback);

            // Create a predicate (asks only for one kind of event)
            Func<string, object, object, bool> predicate = (string eventName, object request, object arg3) =>
            {
                return true;
            };

            DiagnosticListener.AllListeners.Subscribe(delegate (DiagnosticListener listener)
            {
                // We get a callback of every Diagnostics Listener that is active in the system (past present or future)
                if (listener.Name == "HttpHandlerDiagnosticListener")
                {
                    Console.WriteLine("Subscribed to HttpHandlerDiagnosticListener");
                    listener.Subscribe(observer, predicate);
                }
            });

            var httpClient = new HttpClient();
            var res = await httpClient.GetStringAsync("http://google.com");

            return new OkObjectResult(eventNames);
        }
    }
}
