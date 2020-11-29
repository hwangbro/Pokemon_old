using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public class Scene : IDisposable {

        public IntPtr WindowHandle;
        public GameBoy Gb;

        public Vec4 ClearColor;
        private List<Entity> Entities;

        public int WindowWidth {
            get { return Window.GetWidth(WindowHandle); }
        }

        public int WindowHeight {
            get { return Window.GetHeight(WindowHandle); }
        }

        public Scene(int width, int height, GameBoy gb) : this(Window.Create("", 50, 50, width, height), gb) {
        }

        public Scene(IntPtr window, GameBoy gb) {
            Entities = new List<Entity>();
            WindowHandle = window;
            Gb = gb;
            Gb.Scene = this;
            ClearColor = new Vec4(0, 0, 0, 1);

            RenderContext.Create(window, false);
            RenderContext.DrawParams.ShouldWriteDepth = true;
            RenderContext.DrawParams.FaceCulling = FaceCulling.Back;
            RenderContext.DrawParams.SourceBlend = BlendFunc.SrcAlpha;
            RenderContext.DrawParams.DestBlend = BlendFunc.OneMinusSrcAlpha;

            RenderPass sceneDrawPass = RenderContext.RegisterRenderPass("sceneDraw", target => {
                RenderContext.DrawParams.DepthFunc = DrawFunc.Less;
                RenderContext.Clear(target, true, true, 0.0f, 0.0f, 0.0f, 0.0f);
                OnEvent(new BeginSceneEvent());
                OnEvent(new RenderEvent(target));
            });

            RenderPass displayPass = RenderContext.RegisterRenderPass("display", target => {
                RenderContext.DrawParams.DepthFunc = DrawFunc.Always;
                RenderContext.DrawFullscreenQuad(target, RenderContext.Shader, sceneDrawPass.Texture);
                OnEvent(new EndSceneEvent());
            });
        }

        public void Dispose() {
            foreach(Entity entity in Entities) {
                entity.Dispose();
            }
            Gb.Scene = null;
        }

        public void Render() {
            Window.ProcessMessages();
            OnEvent(new UpdateEvent());
            RenderContext.Render();
            Window.Present(WindowHandle);
        }

        public void OnEvent(Event e) {
            foreach(Entity entity in Entities) {
                entity.OnEvent(e, new EventDispatcher(e));
            }
        }

        public void Add(Entity e) {
            Entities.Add(e);
            e.Scene = this;
            e.OnInit();
        }

        public void Remove(Entity e) {
            Entities.Remove(e);
            e.Scene = null;
        }
    }
}
