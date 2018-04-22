using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Connect.Models.Core
{
    public class Session
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime PostingLimitTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}