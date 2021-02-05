# AzureFunctions-DiagnosticSource

Azure Functions does not support DiagnosticSource and DiagnosticListener callbacks when using System.Diagnostics.DiagnosticSource package > 4.7.0

The solution contains an Azure Function which makes an HTTP call and listens to DiagnosticSuurce events logged by HttpClient. It returns a list of event names logged by HttpClient when the outgoing GetStringAsync(string requestUri) call is made.

The `main` branch of this repo uses System.Diagnostics.DiagnosticSource(=4.7.0) and it works fine. The function returns a list of events logged by the HttpClient.
The `DiagnosticSource_5.0.1` branch of this repo uses System.Diagnostics.DiagnosticSource(=latest stable 5.0.1) and the function just returns an empty list as the DiagnosticListener callback is never called.

