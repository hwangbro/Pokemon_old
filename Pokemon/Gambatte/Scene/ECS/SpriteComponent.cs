using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {
    
    public class SpriteComponent : Component {

        public Texture2D Texture;
        public float R;
        public float G;
        public float B;
        public float A;

        public SpriteComponent(Texture2D texture, float r = 1.0f, float g = 1.0f, float b = 1.0f, float a = 1.0f) => (Texture, R, G, B, A) = (texture, r, g, b, a);

        public override void Dispose() {
        }

        public override void OnEvent(Event e, EventDispatcher dispatcher) {
            dispatcher.Dispatch<RenderEvent>(OnRender);
        }

        public virtual void OnRender(RenderEvent e) {
            RenderContext.DrawQuad(e.Target, Texture, Entity.X, Entity.Y, Entity.RenderLayer, Entity.Width, Entity.Height);
        }
    }
}
