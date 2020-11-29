using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public class VideoBufferComponent : SpriteComponent {

        public VideoBufferComponent() : base(RenderContext.CreateTexture2D(160, 144, null)) {
        }

        public override void OnEvent(Event e, EventDispatcher dispatcher) {
            base.OnEvent(e, dispatcher);
            dispatcher.Dispatch<UpdateEvent>(OnUpdate);
        }

        private void OnUpdate(UpdateEvent e) {
            Texture.UpdateContents(Gb.VideoBuffer, PixelFormat.BGRA);
        }
    }
}