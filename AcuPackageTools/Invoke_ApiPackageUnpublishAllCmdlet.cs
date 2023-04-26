using System;
using System.Management.Automation;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using AcuPackageTools.CmdletBase;
using AcuPackageTools.Models;

namespace AcuPackageTools
{
    [Cmdlet(VerbsLifecycle.Invoke, "ApiPackageUnpublishAll")]
    public class Invoke_ApiPackageUnpublishAllCmdlet : ApiCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("tm")]
        public TenantMode TenantMode { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("tln")]
        public string[] TenantLoginNames { get; set; }

        public const string UnpublishAllEndpoint = "/CustomizationApi/unpublishAll";
        protected override void PerformApiOperations()
        {
            using var response = SendRequest( UnpublishAllEndpoint, new UnpublishAllRequest(TenantMode, TenantLoginNames));
        }
    }
}