using AcmPackageTools.Service;
using AcuPackageTools.CmdletBase;
using System;
using System.IO;
using System.Management.Automation;
using System.Net;

namespace AcuPackageTools
{
    [Cmdlet(VerbsLifecycle.Invoke, "ServicePackageUnpublishAll")]
    public class Invoke_ServicePackageUnpublishAllCmdlet : ServiceCmdlet
    {
        internal override void PerformOperations(ServiceGateSoap client)
        {
            client.UnpublishAllPackages(new UnpublishAllPackagesRequest());
        }
    }
}