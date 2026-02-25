using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net.Http;
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

            HashSet<DateTime> existingTimeStamps = new();
            var progressRecord = new ProgressRecord(1, "Publishing Packages", "Starting publication...");
            int elapsedSeconds = 0;

            PublishEndResponse responseData;
            do
            {
                using var endResponse = SendRequest(PublishEndEndpoint);
                responseData = JsonSerializer.Deserialize<PublishEndResponse>(
                    endResponse.RootElement.GetRawText(), SerializerOptions);

                foreach (var log in responseData.Log)
                {
                    if (existingTimeStamps.Contains(log.Timestamp)) continue;
                    switch (log.LogType)
                    {
                        case "information":
                            WriteInformation(new InformationRecord(log.Message, "Customization API"));
                            break;
                        case "warning":
                            WriteWarning(log.Message);
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

                if (!responseData.IsCompleted && !responseData.IsFailed)
                    Thread.Sleep(1000);

                if (responseData.IsFailed)
                    WriteError(
                        new ErrorRecord(
                            new HttpRequestException("Customization publish failed. Check the log output for details."),
                            "AcuPublishFailed",
                            ErrorCategory.NotSpecified,
                            ProjectNames));
            } while (!responseData.IsCompleted && !responseData.IsFailed);

            progressRecord.RecordType = ProgressRecordType.Completed;
            WriteProgress(progressRecord);
        }
    }
}
