#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2023 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2023/04/05
// ----------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;

namespace AcuPackageTools.Models
{
    public class CustomizationResponse
    {
        public bool      isCompleted { get; set; }
        public bool      isFailed    { get; set; }
        public List<Log> log         { get; set; }
    }

    public class Log
    {
        public DateTime timestamp { get; set; }
        public string   logType   { get; set; }
        public string   message   { get; set; }
    }

}