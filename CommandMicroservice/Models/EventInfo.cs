using System.Collections;

namespace CommandMicroservice.Models
{
    public class EventInfo
    {
        public int Id { get; set; }
        public string EventType { get; set; }
        public string LocationId { get; set; }
        public double Value { get; set; }
        public DateTime EventDate { get; set; }

        public EventInfo()
        {
            EventType = string.Empty;
            LocationId = string.Empty;
            Value = 0.0;
            EventDate = DateTime.Now;
        }
    }
}
