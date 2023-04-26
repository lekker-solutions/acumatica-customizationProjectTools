using AcuPackageTools.CmdletBase;
using System;
using System.IO;
using System.Management.Automation;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using AcuPackageTools.Models;

namespace AcuPackageTools
{
    [Cmdlet(VerbsLifecycle.Invoke, "ApiPackageUpload")]
    public class Invoke_ApiPackageUploadCmdlet : ApiCmdlet
    {
        public const string ImportEndpoint = "/CustomizationApi/import";

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

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("d")]
        public string PackageDescr { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("lvl")]
        public int? Level { get; set; }

        protected override void PerformApiOperations()
        {
            if (!File.Exists(PackagePath))
            {
                throw new InvalidOperationException("File does not exist at path: " + PackagePath);
            }

            var binData = File.OpenRead(PackagePath);
            var binDataMemoryStream = new MemoryStream();
            binData.CopyTo(binDataMemoryStream);
            binData.Dispose();
            var request = 
                new ImportPackageRequest(Level,
                    ReplacePackage,
                    PackageName, PackageDescr,
                Convert.ToBase64String(binDataMemoryStream.ToArray()));

            using var response = SendRequest(ImportEndpoint, request);
            var responseObject = response.Deserialize<ApiResponseRoot>();

            foreach (var log in responseObject.Log)    
            {
                WriteVerbose(log.Message);
            }
        }
    }
}