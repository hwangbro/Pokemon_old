using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public class TextComponent : Component {

        public string Text;
        public float R;
        public float G;
        public float B;
        public float A;

        public TextComponent(string text, float r = 1.0f, float g = 1.0f, float b = 1.0f, float a = 1.0f) => (Text, R, G, B, A) = (text, r, g, b, a);

        public override void Dispose() {
        }

        public override void OnEvent(Event e, EventDispatcher dispatcher) {
            dispatcher.Dispatch<RenderEvent>(OnRender);
        }

        private void OnRender(RenderEvent e) {
            RenderContext.DrawString(e.Target, RenderContext.Font, Text, Entity.X, Entity.Y, Entity.RenderLayer, Entity.Width, Entity.Height, R, G, B, A);
        }
    }
}
