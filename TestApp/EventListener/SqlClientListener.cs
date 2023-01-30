using System;
using System.Diagnostics.Tracing;

namespace TestApp.EventListeners
{
    public class SqlClientListener : EventListener
    {
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            // Only enable events from SqlClientEventSource.
            if (eventSource.Name.Equals("Microsoft.Data.SqlClient.EventSource"))
            {
                // Use EventKeyWord 2 to capture basic application flow events. See the above table
                // for all available keywords.
                EnableEvents(eventSource, EventLevel.Informational, (EventKeywords)2);
            }
        }

        // This callback runs whenever an event is written by SqlClientEventSource. Event data is
        // accessed through the EventWrittenEventArgs parameter.
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            // Print event data.
            Console.WriteLine(eventData.Payload[0]);
        }
    }
}