using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public class TimerComponent : TextComponent {

        public static Vec4 LiveSplitRunningColor = new Vec4(92.0f / 255.0f, 208.0f / 255.0f, 123.0f / 255.0f, 1.0f);
        public static Vec4 LiveSplitFinishedColor = new Vec4(76.0f / 255.0f, 170.0f / 255.0f, 228.0f / 255.0f, 1.0f);

        public bool Running;
        public string Format;
        public Vec4 RunningColor;
        public Vec4 StoppedColor;
        private double StartCC;

        public TimerComponent(string format) : this(format, LiveSplitRunningColor, LiveSplitFinishedColor) { }
        public TimerComponent(string format, Vec4 runningColor, Vec4 stoppedColor) : base("", 0, 0, 0, 0) {
            (Running, Format, RunningColor, StoppedColor) = (false, format, runningColor, stoppedColor);
        }

        public void Start() {
            Running = true;
            R = RunningColor.R;
            G = RunningColor.G;
            B = RunningColor.B;
            A = RunningColor.A;
            StartCC = Gb.SceneNow;
        }

        public void Stop() {
            Running = false;
            R = StoppedColor.R;
            G = StoppedColor.G;
            B = StoppedColor.B;
            A = StoppedColor.A;
        }

        public override void OnEvent(Event e, EventDispatcher dispatcher) {
            base.OnEvent(e, dispatcher);
            dispatcher.Dispatch<UpdateEvent>(OnUpdate);
        }

        private void OnUpdate(UpdateEvent e) {
            if(Running) {
                double time = (Gb.SceneNow - StartCC) / 2097152.0;
                TimeSpan duration = TimeSpan.FromSeconds(time);
                Text = string.Format("{0:" + Format + "}", duration);
            }
        }
    }
}