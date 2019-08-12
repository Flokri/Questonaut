using System;
using System.Collections.Generic;
using FFImageLoading.DataResolvers;
using Newtonsoft.Json;

namespace Questonaut.Model
{
    public class Contents
    {
        public string en { get; set; }
    }

    public class Headings
    {
        public string en { get; set; }
    }

    public class HttpPost
    {
        [JsonIgnore]
        public static string url = "https://onesignal.com";

        public string app_id { get; set; }
        public Contents contents { get; set; }
        public Headings headings { get; set; }
        public string ios_badgeType { get; set; }
        public string ios_badgeCount { get; set; }
        public IList<string> include_player_ids { get; set; }
    }
}
