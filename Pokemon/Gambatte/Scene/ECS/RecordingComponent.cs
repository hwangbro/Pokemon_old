using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Pokemon {

    internal class FFMPEGStream {

        private Process Process;
        private Stream Stream;

        public FFMPEGStream(string args) {
            Process = FFMPEG.StartPipe(args);
            Stream = Process.StandardInput.BaseStream;
        }

        public void Close() {
            Stream.Flush();
            Stream.Close();
            Process.WaitForExit();
        }

        public void Write(byte[] data) {
            Stream.Write(data);
        }
    }

    public class RecordingComponent : Component {

        private FFMPEGStream VideoStream;
        private FFMPEGStream AudioStream;
        public double RecordingNow;
        private string VideoTarget;
        private string AudioTarget;
        private string OutputTarget;

        private bool Recording;

        private byte[] OffscreenBuffer;
        private PixelBuffer PixelBuffer;

        public RecordingComponent(string movieName) {
            string folder = "movies/" + movieName;
            Directory.CreateDirectory(folder);
            VideoTarget = folder + "/video";
            AudioTarget = folder + "/audio";
            OutputTarget = folder + "/output";
        }

        public override void OnInit() {
            int width = Window.GetWidth(Entity.Scene.WindowHandle);
            int height = Window.GetHeight(Entity.Scene.WindowHandle);
            OffscreenBuffer = new byte[width * height * 3];
            PixelBuffer = RenderContext.CreatePixelBuffer(OffscreenBuffer.Length);
            VideoStream = new FFMPEGStream("-y -f rawvideo -s " + width + "x" + height + " -pix_fmt rgb24 -r 60 -i - -crf 0 -filter_complex vflip " + VideoTarget + ".mp4");
            AudioStream = new FFMPEGStream("-y -f s16le -ar 2097152 -ac 2 -i - -af volume=0.1 " + AudioTarget + ".mp3");
            Start();
        }

        public override void Dispose() {
            VideoStream.Close();
            AudioStream.Close();
            FFMPEG.RunCommand("-y -i " + VideoTarget + ".mp4 -i " + AudioTarget + ".mp3 -c:v copy -c:a copy -shortest " + OutputTarget + ".mp4");
        }

        public void Start() {
            Recording = true;
        }

        public void Stop() {
            Recording = false;
        }

        public override void OnEvent(Event e, EventDispatcher dispatcher) {
            dispatcher.Dispatch<EndSceneEvent>(OnEndScene);
        }

        private void OnEndScene(EndSceneEvent e) {
            if(Recording) {
                AudioStream.Write(Gb.AudioBuffer.Subarray(0, (int) Gb.Samples * 4));
                PixelBuffer.ReadTextureContents(RenderContext.RenderPasses["sceneDraw"].Texture, PixelFormat.RGB, OffscreenBuffer);
                while(Gb.TotalCyclesRan > RecordingNow) {
                    VideoStream.Write(OffscreenBuffer);
                    RecordingNow += Math.Pow(2, 21) / 60.0;
                }
            }
        }
    }
}
