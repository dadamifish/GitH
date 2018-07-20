using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.EntityFramework
{
    public class DbOptions
    {
        public const string SectionKey = "DbOptions";

        public DbOptions()
        {

        }

        public Dictionary<string, string> LocalDbConnStrings { get; set; }

        public string ParkDbConnString { get; set; }
    }
}
