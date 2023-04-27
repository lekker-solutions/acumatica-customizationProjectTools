
using System.Management.Automation;
using AcuPackageTools.CmdletBase;
using AcuPackgeTools.CustomizationService;

namespace AcuPackageTools
{
    [Cmdlet(VerbsLifecycle.Invoke, "ServicePackagePublish")]
    public class Invoke_ServicePackagePublishCmdlet : ServiceCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("pn")]
        public string[] PackageNames { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("m")]
        public SwitchParameter MergeWithExisting { get; set; }

        protected override void PerformOperations(ServiceGateSoap client)
        {
            client.PublishPackages(PackageNames, MergeWithExisting);
        }
    }
}