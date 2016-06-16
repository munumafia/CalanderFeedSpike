using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CalendarFeedSpike.Models
{
    public class Event
    {
        public string Description { get; set; }

        public DateTime DTEnd { get; set; }

        public string Location { get; set; }

        public DateTime DTStart { get; set; }

        public string Summary { get; set; }
    }
}