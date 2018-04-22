using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Connect.Models.Core
{
    public class Settings
    {
        public int Id { get; set; }
        public string NoSessionsToday { get; set; }
        public string SessionCompleteMessage { get; set; }
    }
}