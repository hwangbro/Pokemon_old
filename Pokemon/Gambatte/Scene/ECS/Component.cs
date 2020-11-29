using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public abstract class Component : IDisposable {

        public Entity Entity;

        public GameBoy Gb {
            get { return Entity.Scene.Gb; }
        }

        public virtual void OnInit() { }
        public virtual void Dispose() { }
        public abstract void OnEvent(Event e, EventDispatcher dispatcher);
    }
}
