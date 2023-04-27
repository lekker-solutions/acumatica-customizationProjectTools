using System;
using System.Management.Automation;
using System.Net;
using System.ServiceModel;
using AcmPackageTools.Service;

namespace AcuPackageTools.CmdletBase
{
    public abstract class ServiceCmdlet : PSCmdlet
    {
        private BasicHttpBinding _binding;
        private bool _loggedIn;

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
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

        internal abstract void PerformOperations(ServiceGateSoap client);

        protected override void BeginProcessing()
        {
            _binding = new BasicHttpBinding
            {
                AllowCookies = true,
                Security =
                {
                    Mode = BasicHttpSecurityMode.Transport
                },
                OpenTimeout = new TimeSpan(0, 10, 0),
                SendTimeout = new TimeSpan(0, 10, 0),
                ReceiveTimeout = new TimeSpan(0, 10, 0)
            };
        }

        protected override void ProcessRecord()
        {
            ServiceGateSoap client = null;
            try
            {
                client = GetClient();
                Login(client);

                PerformOperations(client);
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, string.Empty, ErrorCategory.OperationStopped, default));
            }
            finally
            {
                Logout(client);
            }
        }

        private void Login(ServiceGateSoap client)
        {
            var response = client.Login(new LoginRequest(Username, Password));
            switch (response.LoginResult.Code)
            {
                case ErrorCode.OK:
                    break;
                case ErrorCode.INVALID_CREDENTIALS:
                    throw new HttpListenerException(403, response.LoginResult.Message);
                case ErrorCode.INTERNAL_ERROR:
                    throw new HttpListenerException(500, response.LoginResult.Message);
                case ErrorCode.INVALID_API_VERSION:
                    throw new HttpListenerException(401, response.LoginResult.Message);
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _loggedIn = true;
        }

        private void Logout(ServiceGateSoap client)
        {
            if (!_loggedIn) return;
            client?.Logout(new LogoutRequest());
            _loggedIn = false;
        }

        private ServiceGateSoap GetClient()
        {
            EndpointAddress address = new(Url + "/api/ServiceGate.asmx");
            return new ServiceGateSoapClient(_binding, address);
        }
    }
}