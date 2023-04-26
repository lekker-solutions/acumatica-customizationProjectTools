using AcmPackageTools.Service;
using AcuPackageTools.CmdletBase;
using System;
using System.IO;
using System.Management.Automation;

namespace AcuPackageTools
{
    [Cmdlet(VerbsLifecycle.Invoke, "ServicePackageUpload")]
    public class Invoke_ServicePackageUploadCmdlet : ServiceCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("pn")]
        public string PackageName { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("pp")]
        public string PackagePath { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("r")]
        public SwitchParameter ReplacePackage { get; set; }

        protected override void ProcessRecord()
        {
            var packagePath = ValidatePackagePath();

            try
            {
                var client = GetClient();
                client.LoginAsync(new LoginRequest(Username, Password));
                client.UploadPackageAsync(new UploadPackageRequest(PackageName, File.ReadAllBytes(packagePath), ReplacePackage));
                client.LogoutAsync(new LogoutRequest());
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, string.Empty, ErrorCategory.OperationStopped, default));
            }
        }

        private string ValidatePackagePath()
        {
            var packagePath = !Path.IsPathRooted(PackagePath)
                ? Path.Combine(PackagePath, this.MyInvocation.PSScriptRoot)
                : PackagePath;

            if (!File.Exists(packagePath))
                throw new InvalidOperationException("File does not exist at " + packagePath);
            return packagePath;
        }

        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }
}