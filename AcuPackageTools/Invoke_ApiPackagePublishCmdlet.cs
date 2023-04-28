using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Text.Json;
using System.Threading;
using AcuPackageTools.CmdletBase;
using AcuPackageTools.Models;
using AcuPackageTools.Models.Enum;

namespace AcuPackageTools
{
    [Cmdlet(VerbsLifecycle.Invoke, "ApiPackagePublish")]
    public class Invoke_ApiPackagePublishCmdlet : ApiCmdlet
    {
        [Alias("pn")]
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string[] ProjectNames { get; set; }

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
        public SwitchParameter MergeWithExisting { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public SwitchParameter OnlyValidate { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public SwitchParameter DbUpdateOnly { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public SwitchParameter ExecuteAllScripts { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("tln")]
        public string[] TenantLoginNames { get; set; }


        public const string PublishBeginEndpoint = "/CustomizationApi/publishBegin";
        public const string PublishEndEndpoint = "/CustomizationApi/publishEnd";

        protected override void PerformApiOperations()
        {
            using var startResponse =
                SendRequest(PublishBeginEndpoint,
                    new PublishBeginRequest(
                        MergeWithExisting,
                        OnlyValidate,
                        DbUpdateOnly,
                        ExecuteAllScripts,
                        ProjectNames,
                        TenantMode,
                        TenantLoginNames));

            PublishEndResponse responseData;
            HashSet<DateTime> existingTimeStamps = new();
            bool isCompleted = false;
            bool isFailed = false;
            do
            {
                var endResponse = SendRequest(PublishEndEndpoint);
                responseData =
                    startResponse.Deserialize<PublishEndResponse>();

                foreach (var log in responseData.Log)
                {
                    if (existingTimeStamps.Contains(log.Timestamp)) continue;
                    switch (log.LogType)
                    {
                        case LogType.Information:
                        case LogType.Trace:
                            WriteInformation(new InformationRecord(log.Message, "Customization API"));
                            break;
                        case LogType.Warning:
                            WriteWarning(log.Message);
                            break;
                        case LogType.Error:
                            throw new HttpListenerException(500, log.Message);
                    }

                    existingTimeStamps.Add(log.Timestamp);
                }
                Thread.Sleep(1000);

                // For some reason this will NOT DESERALIZE :((((((
                JsonElement value;
                endResponse.RootElement.TryGetProperty("isCompleted", out value);
                isCompleted = value.GetBoolean();
                endResponse.RootElement.TryGetProperty("isFailed", out value);
                isFailed = value.GetBoolean();
            } while (!isCompleted && !isFailed);
        }
    }
}