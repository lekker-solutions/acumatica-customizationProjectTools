using System;
using System.Management.Automation;
using System.Net.Http;
using System.Text;
using AcuPackageTools.Connection;

namespace AcuPackageTools
{
    [Cmdlet(VerbsCommunications.Disconnect, "AcuInstance")]
    public class Disconnect_AcuInstanceCmdlet : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            if (!AcuConnectionManager.IsConnected)
            {
                WriteWarning("Not connected to any Acumatica instance.");
                return;
            }

            var url = AcuConnectionManager.Url;

            try
            {
                // Try to logout gracefully
                var client = AcuConnectionManager.Client;
                if (client != null)
                {
                    var uriBuilder = new UriBuilder(url);
                    uriBuilder.Path += "/entity/auth/logout";
                    var logoutUrl = uriBuilder.ToString();

                    WriteVerbose($"Logging out from {url}...");

                    try
                    {
                        client.PostAsync(logoutUrl,
                            new StringContent(string.Empty, Encoding.UTF8, "application/json"))
                            .GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        WriteVerbose($"Logout request failed (connection may already be closed): {e.Message}");
                    }
                }
            }
            finally
            {
                AcuConnectionManager.ClearConnection();
                WriteVerbose($"Disconnected from {url}");

                WriteObject(new {
                    Connected = false,
                    Url = url,
                    Message = "Successfully disconnected"
                });
            }
        }
    }
}
