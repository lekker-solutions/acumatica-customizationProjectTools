using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AcuPackageTools.Models;

public record PublishEndResponse(bool IsCompleted, bool IsFailed, List<Log> Log)
{
    [JsonPropertyName("isCompleted")]
    public bool IsCompleted { get; } = IsCompleted;

    [JsonPropertyName("isFailed")]
    public bool IsFailed { get; } = IsFailed;

    [JsonPropertyName("log")] 
    public List<Log> Log { get; } = Log;
}
