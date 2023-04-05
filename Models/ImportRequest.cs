#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2023 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2023/04/05
// ----------------------------------------------------------------------------------
#endregion

namespace Velixo.Common.CustomizationPackageTools.Models
{
    public class ImportRequest
    {
        public int    projectLevel         { get; set; }
        public bool   isReplaceIfExists    { get; set; }
        public string projectName          { get; set; }
        public string projectDescription   { get; set; }
        public string projectContentBase64 { get; set; }
    }
}