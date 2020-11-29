using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public enum EventType {
        WindowClose, WindowResize, WindowFocus, WindowLostFocus, WindowMoved,
        Update, BeginScene, Render, EndScene,
        KeyPressed, KeyReleased,
        MouseButtonPressed, MouseButtonReleased, MouseMoved, MouseScrolled,
    }

    public enum EventCategory : uint {
        Application = 1 << 0,
        Input       = 1 << 1,
        Keyboard    = 1 << 2,
        Mouse       = 1 << 3,
        MouseButton = 1 << 4,
    }
    
    public abstract class Event {

        public abstract EventType GetEventType();
        public abstract uint GetCategoryFlags();

        public bool IsInCategory(EventCategory category) {
            return (GetCategoryFlags() & (uint) category) != 0;
        }

        public override string ToString() {
            return GetType().Name;
        }
    }

    public delegate void EventCallbackFn<T>(T e) where T : Event;

    public class EventDispatcher {

        private Event Event;

        public EventDispatcher(Event e) => Event = e;

        public bool Dispatch<T>(EventCallbackFn<T> func) where T : Event {
            if(Event.GetType() == typeof(T)) {
                func((T) Event);
                return true;
            }
            return false;
        }
    }
}
