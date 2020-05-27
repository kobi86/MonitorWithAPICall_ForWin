using System;
using System.Collections.Generic;
using System.Text;

namespace CallAPIService
{
    public class AppSettings
    {
        public string Folder1 { get; set; }
        public string Folder2 { get; set; }
        public string Folder3 { get; set; }
        public string Filesloglocation { get; set; }
        public string AccessToken { get; set; }
        public string BuildID { get; set; }
        public int ApiCallTime { get; set; }
        public string OrgName { get; set; }
        public string ProjectName { get; set; }

    }

    public class Monitor_Folder1
    {
        public string Folder1 { get; set; }
    }

    public class Monitor_Folder2
    {
        public string Folder2 { get; set; }
    }

    public class Monitor_Folder3
    {
        public string Folder3 { get; set; }
    }
    public class LogFile
    {
        public string Filesloglocation { get; set; }
    }

    public class Token
    {
        public string AccessToken { get; set; }
    }

    public class BuildDefinition
    {
        public string BuildID { get; set; }
    }

    public class WaitTimeForAPI
    {
        public int ApiCallTime { get; set; }
    }

    public class VSTSOrgName
    {
        public int OrgName { get; set; }
    }

    public class VSTSprojectName
    {
        public int ProjectName { get; set; }
    }

}
