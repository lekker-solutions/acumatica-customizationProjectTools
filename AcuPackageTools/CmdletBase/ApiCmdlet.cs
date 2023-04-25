using AcmPackageTools.Service;
using System;
using System.Management.Automation;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AcuPackageTools.CmdletBase
{
    public abstract class ApiCmdlet : PSCmdlet
    {
        protected HttpClient Client;
        private bool _disposed;
        protected Uri BaseUrl => new Uri(Url);

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("url")]
        public string Url { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("u")]
        public string Username { get; set; }

        [Parameter(
            Mandatory = false,
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
            Client = new HttpClient(handler,true);
        }

        protected override void ProcessRecord()
        {
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

        public abstract void PerformApiOperations();

        protected override void StopProcessing()
        {
            DisposeClient();
        }

        protected void Login()
        {
            var loginRequest = new AcuPackageTools.Models.LoginRequest(Username, Password, Tenant);

            Client.PostAsync(new Uri(BaseUrl, "entuty/auth/login"), new StringContent(JsonSerializer.Serialize(loginRequest, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            })));
        }

        protected void Logout()
        {
            Client.PostAsync(new Uri(BaseUrl, "entity/auth/logout"),new StringContent(string.Empty));
        }

        protected void DisposeClient()
        {
            if (_disposed) return;
            Client.Dispose();
            _disposed = true;
        }
    }
}