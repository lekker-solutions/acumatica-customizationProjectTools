using System;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace AcuPackageTools
{
    [Cmdlet(VerbsLifecycle.Invoke, "PackageBuild")]
    public class PackageBuildCmdlet : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("cp")]
        public string CustomizationPath { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("pfn")]
        public string PackageFileName { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("pv")]
        public string ProductVersion { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("d")]
        public string Description { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("l")]
        public int Level { get; set; }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            WriteVerbose(string.Empty);
            WriteVerbose($"Building Package {PackageFileName}");

            if (!CheckProductVersion()) return;

            PackageBuilder.BuildCustomizationPackage(CustomizationPath, PackageFileName, Description, Level, ProductVersion);
            WriteVerbose($"Package {PackageFileName} Completed Build");
            WriteVerbose(string.Empty);
        }

        private bool CheckProductVersion()
        {
            var productVersionRegex = Regex.Match(ProductVersion, @"^\d+\.\d+$");
            if (!productVersionRegex.Success)
            {
                WriteError(new ErrorRecord(
                    new Exception($"Product Version {ProductVersion} does not match expected pattern ##.###"), "",
                    ErrorCategory.InvalidArgument, ProductVersion));
                return false;
            }

            return true;
        }
    }
}
