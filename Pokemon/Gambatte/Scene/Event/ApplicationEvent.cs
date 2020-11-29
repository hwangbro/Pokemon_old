using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public class UpdateEvent : Event {

        public override EventType GetEventType() {
            return EventType.Update;
        }

        public override uint GetCategoryFlags() {
            return (uint) EventCategory.Application;
        }
    }

    public class BeginSceneEvent : Event {

        public override EventType GetEventType() {
            return EventType.BeginScene;
        }

        public override uint GetCategoryFlags() {
            return (uint) EventCategory.Application;
        }
    }

    public class RenderEvent : Event {

        public uint Target;

        public RenderEvent(uint target) => (Target) = (target);

        public override EventType GetEventType() {
            return EventType.Render;
        }

        public override uint GetCategoryFlags() {
            return (uint) EventCategory.Application;
        }
    }

    public class EndSceneEvent : Event {

        public override EventType GetEventType() {
            return EventType.EndScene;
        }

        public override uint GetCategoryFlags() {
            return (uint) EventCategory.Application;
        }
    }
}
