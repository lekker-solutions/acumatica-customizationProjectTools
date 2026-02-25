using System;
using System.Management.Automation;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AcuPackageTools.Connection;

namespace AcuPackageTools.CmdletBase
{
    public abstract class ApiCmdlet : PSCmdlet, IDisposable
    {
        protected HttpClient Client;
        private bool _disposed;
        private bool _loggedIn;
        private bool _useSharedConnection;
        private string _effectiveUrl;

        internal static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Url { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Credential]
        public PSCredential Credential { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("t")]
        public string Tenant { get; set; }

        protected override void BeginProcessing()
        {
            // Determine which mode to use
            if (Credential != null && !string.IsNullOrEmpty(Url))
            {
                // One-off mode: credentials provided explicitly
                _useSharedConnection = false;
                _effectiveUrl = Url;
                Client = AcuConnectionManager.CreateNewClient();
                WriteVerbose("Using one-off connection mode");
            }
            else if (AcuConnectionManager.IsConnected)
            {
                // Shared connection mode
                _useSharedConnection = true;
                _effectiveUrl = AcuConnectionManager.Url;
                Client = AcuConnectionManager.Client;
                WriteVerbose($"Using shared connection to {_effectiveUrl}");
            }
            else
            {
                // No connection available
                throw new InvalidOperationException(
                    "Not connected to an Acumatica instance. " +
                    "Use Connect-AcuInstance first, or provide -Url and -Credential parameters.");
            }
        }

        protected override void ProcessRecord()
        {
            try
            {
                if (!_useSharedConnection)
                {
                    Login();
                }
                PerformApiOperations();
            }
            catch (Exception e)
            {
                var category = e is HttpRequestException
                    ? ErrorCategory.ConnectionError
                    : ErrorCategory.NotSpecified;
                WriteError(new ErrorRecord(e, "AcuApiRequestFailed", category, _effectiveUrl));
            }
            finally
            {
                if (!_useSharedConnection)
                {
                    Logout();
                }
            }
        }

        protected abstract void PerformApiOperations();

        protected override void EndProcessing()
        {
            if (!_useSharedConnection)
            {
                Dispose();
            }
        }

        private void Login()
        {
            var networkCredential = Credential.GetNetworkCredential();
            var loginRequest = new AcuPackageTools.Models.LoginRequest(
                networkCredential.UserName,
                networkCredential.Password,
                Tenant);
            SendRequest("/entity/auth/login", loginRequest);
            _loggedIn = true;
        }

        protected string EffectiveUrl => _effectiveUrl;

        protected JsonDocument SendRequest(string resource, object body = null)
        {
            var uriBuilder = new UriBuilder(_effectiveUrl);
            uriBuilder.Path += resource;
            var url = uriBuilder.ToString();
            HttpResponseMessage response;
            if (body is null)
            {
                response = Client.PostAsync(url, new StringContent(string.Empty, Encoding.UTF8, "application/json"))
                                 .GetAwaiter().GetResult();
                WriteVerbose("Posting Empty Body to " + url);
            }
            else
            {
                string requestContent = JsonSerializer.Serialize(body, SerializerOptions);
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
                throw new HttpRequestException(
                    $"There was a failure when calling {url} (HTTP {(int)response.StatusCode}): "
                  + Environment.NewLine
                  + JsonSerializer.Serialize(jDoc, SerializerOptions));

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"There was a failure when calling {url} (HTTP {(int)response.StatusCode})");
            }

            WriteVerbose(JsonSerializer.Serialize(jDoc, SerializerOptions));

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing && !_useSharedConnection)
            {
                Client?.Dispose();
            }
            _disposed = true;
        }

        protected override void StopProcessing()
        {
            if (!_useSharedConnection)
            {
                Dispose();
            }
            base.StopProcessing();
        }
    }
}
