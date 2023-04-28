using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AcuPackageTools.Models;

public record PublishEndResponse(bool IsCompleted, bool IsFailed, List<Log> Log) : ApiResponseRoot(Log)
{
    [JsonPropertyName("isCompleted")]
    public bool IsCompleted { get; } = IsCompleted;

    [JsonPropertyName("isFailed")]
    public bool IsFailed { get; } = IsFailed;
}
