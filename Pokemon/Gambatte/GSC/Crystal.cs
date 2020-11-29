using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public class Crystal : Gsc {

        public Crystal(bool speedup = false, string saveName = "roms/pokecrystal.sav", string rom = "roms/pokecrystal.gbc") : base(rom, saveName, speedup ? SpeedupFlags.NoSound | SpeedupFlags.NoVideo : SpeedupFlags.None) { }

        public override void Inject(Joypad joypad) {
            CpuWrite(0xFFA7, (byte) joypad);
            CpuWrite(0xFFA8, (byte) joypad);
        }

        public override void InjectMenu(Joypad joypad) {
            CpuWrite(0xFFA4, (byte) joypad);
        }
    }
}
