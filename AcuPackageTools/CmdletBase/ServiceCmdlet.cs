using System;
using System.Management.Automation;
using System.ServiceModel;
using AcmPackageTools.Service;

namespace AcuPackageTools.CmdletBase
{
    public class ServiceCmdlet : PSCmdlet
    {
        private BasicHttpBinding _binding;

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

        internal ServiceGateSoap GetClient()
        {
            EndpointAddress address = new EndpointAddress(Url + "/api/ServiceGate.asmx");
            return new ServiceGateSoapClient(_binding, address);
        }
    }
}