
using AcuPackageTools.CmdletBase;
using System;
using System.IO;
using System.Management.Automation;
using System.Net;
using AcuPackgeTools.CustomizationService;

namespace AcuPackageTools
{
    [Cmdlet(VerbsLifecycle.Invoke, "ServicePackageUnpublishAll")]
    public class Invoke_ServicePackageUnpublishAllCmdlet : ServiceCmdlet
    {
        protected override void PerformOperations(ServiceGateSoap client)
        {
            client.UnpublishAllPackages();
        }
    }
}