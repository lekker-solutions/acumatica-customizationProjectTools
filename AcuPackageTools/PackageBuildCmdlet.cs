using System.Management.Automation;

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
            PackageBuilder.BuildCustomizationPackage(CustomizationPath, PackageFileName, Description, Level, "20.202");
            WriteVerbose($"Package {PackageFileName} Completed Build");
            WriteVerbose(string.Empty);
        }
    }
}
