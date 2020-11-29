using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {
    
    public class Entity : IDisposable {

        public Dictionary<Type, Component> Components;
        public float X;
        public float Y;
        public float RenderLayer;
        public float Width;
        public float Height;

        public Scene Scene;

        public Entity() : this(0, 0, 0, 0) {}
        public Entity(float x, float y, float width, float height) : this(x, y, 0, width, height) { }

        public Entity(float x, float y, float renderLayer, float width, float height) {
            (X, Y, RenderLayer, Width, Height) = (x, y, renderLayer, width, height);
            Components = new Dictionary<Type, Component>();
        }

        public virtual void OnInit() {
            foreach(Component component in Components.Values) {
                component.OnInit();
            }
        }

        public void Dispose() {
            foreach(Component component in Components.Values) {
                component.Dispose();
            }
        }

        public void OnEvent(Event e, EventDispatcher dispatcher) {
            foreach(Component component in Components.Values) {
                component.OnEvent(e, dispatcher);
            }
        }

        public Entity AddComponent<T>(T component) where T : Component {
            component.Entity = this;
            Components[typeof(T)] = component;
            return this;
        }

        public T GetComponent<T>() where T : Component {
            return (T) Components[typeof(T)];
        }
    }
}