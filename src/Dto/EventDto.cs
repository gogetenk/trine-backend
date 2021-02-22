using System;

namespace Dto
{
    public class EventDto
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }

        public string UserId { get; set; }
        // Content data (everything is nullable)
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string MainContent { get; set; }
        public string SubContent { get; set; }
        public string Icon1 { get; set; }
        public string Icon2 { get; set; }

        // Context data 
        public string ContextId { get; set; } // The item to navigate to onclick (can be a mission if it's an invitation)
        public object Details { get; set; } // Can be anything depending on the context. Ex: number of worked days for the activity report notification
        public EventTypeEnum EventType { get; set; } // How to show the event (if shown)
        public ContextEnum ContextType { get; set; } // On which dashboard to show the event
        public string TargetId { get; set; } // Can be userId if personal, organizationId if in orga dashboard, or missionId
        public bool IsEnabled { get; set; } // Is the event shown on the dashboard ?

        public enum EventTypeEnum
        {
            LOG, // Not shown on the dashboard
            INFO, // Shown as a swipeable card on the dashboard
            ALERT, // Shown as a swipeable alert card on the dashboard
            ACTION, // Shown as a card with an action button, that will go off only when the particular action is performed
            FATAL // Shown as a red card with an action button, like an action event but dangerous
        }

        public enum ContextEnum
        {
            PERSONAL,
            ORGANIZATION,
            ACTIVITY,
            MISSION,
            USER
        }
    }
}
