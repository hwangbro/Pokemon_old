using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Pokemon {

    public static class FFMPEG {

        public static Process StartPipe(string args) {
            Process p = new Process();
            p.StartInfo.FileName = "C:\\ffmpeg\\bin\\ffmpeg.exe";
            p.StartInfo.Arguments = args;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            //p.ErrorDataReceived += Process_DataReceived;
            p.Start();
            p.BeginErrorReadLine();
            return p;
        }

        private static void Process_DataReceived(object sender, DataReceivedEventArgs e) {
            Console.WriteLine(e.Data);
        }

        public static void RunCommand(string args) {
            Process p = StartPipe(args);
            p.WaitForExit();
        }

        public static void Stack(params (string, int, int)[] videos) {
            string inputs = "";
            string videofilter = "";
            string layout = "";
            string audiofilter = "";
            for(int i = 0; i < videos.Length; i++) {
                inputs += "-i " + videos[i].Item1 + " ";
                videofilter += "[" + i + ":v]";
                audiofilter += "[" + i + ":a]";
                layout += videos[i].Item2 + "_" + videos[i].Item3;
                if(i != videos.Length - 1) {
                    layout += "|";
                }
            }
            videofilter += "xstack=inputs=" + videos.Length + ":layout=" + layout;
            audiofilter += "amerge=inputs=" + videos.Length;

            RunCommand("-y " + inputs + "-filter_complex \"" + videofilter + ";" + audiofilter + "\" -ac 2 output.mp4");
        }
    }
}
