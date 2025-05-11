using System;
using System.Collections.Generic;

namespace Ludo.Core.Analytics
{
    public class AnalyticsEvent
    {
        public string EventName { get; }
        public Dictionary<string, object> Parameters { get; }
        public DateTime Timestamp { get; }

        public AnalyticsEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            EventName = eventName;
            Parameters = parameters ?? new Dictionary<string, object>();
            Timestamp = DateTime.UtcNow;
        }
    }
}