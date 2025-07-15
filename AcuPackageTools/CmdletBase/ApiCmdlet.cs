using System;
using System.Management.Automation;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AcuPackageTools.CmdletBase
{
    public abstract class ApiCmdlet : PSCmdlet
    {
        protected HttpClient Client;
        private   bool       _disposed;
        private   bool       _loggedIn;

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string Url { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("u")]
        public string Username { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("p")]
        public string Password { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("t")]
        public string Tenant { get; set; }

        protected override void BeginProcessing()
        {
            var handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer()
            };
            Client = new HttpClient(handler, true);
        }

        protected override void ProcessRecord()
        {
            var loggedIn = false;
            try
            {
                Login();

                PerformApiOperations();
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "", ErrorCategory.ConnectionError, default));
            }
            finally
            {
                Logout();
            }
        }

        protected abstract void PerformApiOperations();

        protected override void EndProcessing()
        {
            DisposeClient();
        }

        private void Login()
        {
            var loginRequest = new AcuPackageTools.Models.LoginRequest(Username, Password, Tenant);
            SendRequest("/entity/auth/login", loginRequest);
            _loggedIn = true;
        }

        protected JsonDocument SendRequest(string resource, object body = null)
        {
            var uriBuilder = new UriBuilder(Url);
            uriBuilder.Path += resource;
            var                 url = uriBuilder.ToString();
            HttpResponseMessage response;
            if (body is null)
            {
                response = Client.PostAsync(url, new StringContent(string.Empty, Encoding.UTF8, "application/json"))
                                 .GetAwaiter().GetResult();
                WriteVerbose("Posting Empty Body to " + url);
            }
            else
            {
                string requestContent = JsonSerializer.Serialize(body, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                    WriteIndented          = true
                });
                response = Client.PostAsync(url, new StringContent(requestContent, Encoding.UTF8, "application/json"))
                                 .GetAwaiter().GetResult();
                WriteVerbose($"Posting content to " + url);
                WriteVerbose(requestContent);
            }

            string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            JsonDocument jDoc = string.IsNullOrWhiteSpace(responseContent)
                ? default
                : JsonDocument.Parse(responseContent);
            if (!response.IsSuccessStatusCode
             && !string.IsNullOrWhiteSpace(responseContent))
                throw new HttpListenerException((int)response.StatusCode,
                    $"There was a failure when calling {url}: "
                  + Environment.NewLine
                  + JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true }));

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpListenerException((int)response.StatusCode,
                    $"There was a failure when calling {url}");
            }

            WriteVerbose(JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true }));

            return jDoc;
        }

        private void Logout()
        {
            if (!_loggedIn) return;
            try
            {
                SendRequest("/entity/auth/logout");
            }
            catch (Exception e)
            {
                WriteWarning($"Error during logout: {e.Message}");
            }
            finally
            {
                _loggedIn = false;
            }
        }

        private void DisposeClient()
        {
            if (_disposed) return;
            Client?.Dispose();
            _disposed = true;
        }
    }
}