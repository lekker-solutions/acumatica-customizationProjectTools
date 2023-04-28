using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AcuPackageTools.Models.Enum;

namespace AcuPackageTools.Models;

public record ApiResponseRoot(List<Log> Log)
{
    [JsonPropertyName("log")] public List<Log> Log { get; } = Log;
}

public record Log(
    DateTime Timestamp,
    LogType  LogType,
    string   Message
)
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; } = Timestamp;

    [JsonPropertyName("logType")] public LogType LogType { get; } = LogType;

    [JsonPropertyName("message")]
    public string Message { get; } = Message;
}