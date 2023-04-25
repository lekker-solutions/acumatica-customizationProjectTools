#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2023 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2023/04/05
// ----------------------------------------------------------------------------------
#endregion

using System.Collections.Generic;

namespace AcuPackageTools.Models
{
    public class StartPublishRequest
    {
        public bool         isMergeWithExistingPackages       { get; set; }
        public bool         isOnlyValidation                  { get; set; }
        public bool         isOnlyDbUpdates                   { get; set; }
        public bool         isReplayPreviouslyExecutedScripts { get; set; }
        public List<string> projectNames                      { get; set; }
        public string       tenantMode                        { get; set; }
    }
}