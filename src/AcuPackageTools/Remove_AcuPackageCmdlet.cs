using System.Management.Automation;
using System.Text.Json;
using AcuPackageTools.CmdletBase;
using AcuPackageTools.Models;

namespace AcuPackageTools
{
    [Cmdlet(VerbsCommon.Remove, "AcuPackage", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class Remove_AcuPackageCmdlet : ApiCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("pn")]
        [ValidateNotNullOrEmpty]
        public string ProjectName { get; set; }

        public const string DeleteEndpoint = "/CustomizationApi/delete";

        protected override void PerformApiOperations()
        {
            if (!ShouldProcess(ProjectName, "Delete customization project"))
            {
                return;
            }

            using var response = SendRequest(DeleteEndpoint, new DeletePackageRequest(ProjectName));
            var responseObject = response.Deserialize<ApiResponseRoot>();

            foreach (var log in responseObject.Log)
            {
                WriteVerbose(log.Message);
            }
        }
    }
}
