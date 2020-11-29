using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Pokemon {

    public class Yellow : Rby {

        public Yellow(bool speedup = false, string saveName = "roms/pokeyellow.sav", string rom = "roms/pokeyellow.gbc") : base(rom, saveName, speedup ? SpeedupFlags.NoSound | SpeedupFlags.NoVideo : SpeedupFlags.None) { }

        public static readonly Dictionary<YoloballTypes, Joypad[]> Yoloballs = new Dictionary<YoloballTypes, Joypad[]>() {
            {YoloballTypes.Normal, new Joypad[] {Joypad.Down, Joypad.A, Joypad.A | Joypad.Right}},
            {YoloballTypes.Select, new Joypad[] {Joypad.Down, Joypad.A, Joypad.Select, Joypad.A}},
            {YoloballTypes.SelectSlot2, new Joypad[] {Joypad.Down, Joypad.A, Joypad.Down | Joypad.Select, Joypad.A}},
        };

        public override void Inject(Joypad joypad) {
            CpuWrite(0xFFF5, (byte) joypad);
        }

        public bool Yoloball(YoloballTypes type = YoloballTypes.Normal, int waitFrames = 0) {
            AdvanceWithJoypadToAddress(Joypad.B, Sym["ManualTextScroll"]);
            for(int i = 0; i < waitFrames; i++) {
                AdvanceToAddress(Sym["_Joypad"]);
                AdvanceFrame();
            }
            Inject(Joypad.A);
            AdvanceFrame(Joypad.A);
            AdvanceToAddress(Sym["PlayCry"], Sym["PlayPikachuSoundClip"]);
            Press(Yoloballs[type]);
            return AdvanceWithJoypadToAddress(Joypad.A, Sym["ItemUseBall.captured"], Sym["ItemUseBall.failedToCapture"]) == Sym["ItemUseBall.captured"];
        }

        public void fastestIntro() {
            gfSkip();
            intro0();
        }

        public void gfSkip() {
            base.Press(Joypad.Start);
        }

        public void gfWait() {
            base.AdvanceToAddress("PlayShootingStar.endDelay");
        }

        public void intro0() {
            base.Press(Joypad.A);
        }

        public void intro1() {
            base.AdvanceToAddress("YellowIntroScene2");
            base.Press(Joypad.A);
        }

        public void intro5() {
            base.AdvanceToAddress("YellowIntroScene10");
            base.Press(Joypad.A);
        }

        public void introwait() {
            base.AdvanceToAddress("DisplayTitleScreen");
        }
    }
}
