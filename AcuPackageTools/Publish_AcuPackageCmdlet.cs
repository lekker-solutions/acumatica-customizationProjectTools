using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Text.Json;
using System.Threading;
using AcuPackageTools.CmdletBase;
using AcuPackageTools.Models;

namespace AcuPackageTools
{
    [Cmdlet(VerbsData.Publish, "AcuPackage", SupportsShouldProcess = true)]
    public class Publish_AcuPackageCmdlet : ApiCmdlet
    {
        [Alias("pn")]
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
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
            var projectList = string.Join(", ", ProjectNames);
            if (!ShouldProcess(projectList, "Publish customization packages"))
            {
                return;
            }

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
            var progressRecord = new ProgressRecord(1, "Publishing Packages", "Starting publication...");
            int elapsedSeconds = 0;

            do
            {
                using var endResponse = SendRequest(PublishEndEndpoint);
                responseData = endResponse.Deserialize<PublishEndResponse>();

                foreach (var log in responseData.Log)
                {
                    if (existingTimeStamps.Contains(log.Timestamp)) continue;
                    switch (log.LogType)
                    {
                        case "information":
                            WriteInformation(new InformationRecord(log.Message, "Customization API"));
                            break;
                        case "error":
                            WriteWarning(log.Message);
                            break;
                    }

                    existingTimeStamps.Add(log.Timestamp);
                }

                elapsedSeconds++;
                progressRecord.StatusDescription = $"Waiting for completion... ({elapsedSeconds}s)";
                WriteProgress(progressRecord);

                Thread.Sleep(1000);

                JsonElement value;
                endResponse.RootElement.TryGetProperty("isCompleted", out value);
                isCompleted = value.GetBoolean();
                endResponse.RootElement.TryGetProperty("isFailed", out value);
                isFailed = value.GetBoolean();

                if (isFailed)
                    WriteError(
                        new ErrorRecord(
                            new HttpListenerException(500, "Package publication failed"),
                            "AcuPublishFailed",
                            ErrorCategory.ReadError,
                            ProjectNames));
            } while (!isCompleted && !isFailed);

            progressRecord.RecordType = ProgressRecordType.Completed;
            WriteProgress(progressRecord);
        }
    }
}
