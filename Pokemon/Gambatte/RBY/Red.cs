using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public class Red : RedBlue {

        public Red(bool speedup = false, string saveName = "roms/pokered.sav", string rom = "roms/pokered.gbc") : base(rom, saveName, speedup) {
        }
    }
}
