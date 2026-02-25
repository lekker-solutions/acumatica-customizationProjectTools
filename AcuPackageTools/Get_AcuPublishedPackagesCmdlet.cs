using System.Management.Automation;
using System.Text.Json;
using AcuPackageTools.CmdletBase;
using AcuPackageTools.Models;

namespace AcuPackageTools
{
    [Cmdlet(VerbsCommon.Get, "AcuPublishedPackages")]
    public class Get_AcuPublishedPackagesCmdlet : ApiCmdlet
    {
        public const string GetPublishedEndpoint = "/CustomizationApi/getPublished";

        protected override void PerformApiOperations()
        {
            using var response = SendRequest(GetPublishedEndpoint);
            var responseObject = response.Deserialize<GetPublishedResponse>();

            foreach (var log in responseObject.Log)
            {
                WriteVerbose(log.Message);
            }

            WriteObject(responseObject);
        }
    }
}
