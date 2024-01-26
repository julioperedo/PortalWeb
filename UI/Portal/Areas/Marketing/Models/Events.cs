using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Marketing.Models {
    public class EventsPage {
        public int Next { get; set; }
        public List<Event> Items { get; set; }
    }
    public class Event {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public bool Detail { get; set; }
    }
}
