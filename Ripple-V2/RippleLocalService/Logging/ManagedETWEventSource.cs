using System;
using System.Diagnostics.Tracing;

namespace MicrosoftIT.ManagedLogging
{
    [EventSource(Name = "MicrosoftIT-Logging-ProviderMicrosoftIT", Guid = "{90030054-AE81-4C8F-A8F4-966A50DBADBF}")]
    internal class ManagedETWEventSource : EventSource
    {
        public class Keywords
        {
            public const EventKeywords MANAGED = (EventKeywords)0x00000001;
        }

        public class Tasks
        {
            public const EventTask tracemessage = (EventTask)10;
        }

        [Event(100, Level = EventLevel.Critical, Keywords = Keywords.MANAGED, Task = Tasks.tracemessage)]
        public void _MANAGED_1(string message) { if (IsEnabled()) WriteEvent(100, message); }

        [Event(101, Level = EventLevel.Error, Keywords = Keywords.MANAGED, Task = Tasks.tracemessage)]
        public void _MANAGED_2(string message) { if (IsEnabled()) WriteEvent(101, message); }

        [Event(102, Level = EventLevel.Warning, Keywords = Keywords.MANAGED, Task = Tasks.tracemessage)]
        public void _MANAGED_3(string message) { if (IsEnabled()) WriteEvent(102, message); }

        [Event(103, Level = EventLevel.Informational, Keywords = Keywords.MANAGED, Task = Tasks.tracemessage)]
        public void _MANAGED_4(string message) { if (IsEnabled()) WriteEvent(103, message); }

        [Event(104, Level = EventLevel.Verbose, Keywords = Keywords.MANAGED, Task = Tasks.tracemessage)]
        public void _MANAGED_5(string message) { if (IsEnabled()) WriteEvent(104, message); }

        public static ManagedETWEventSource Logger = new ManagedETWEventSource();
    }
}
