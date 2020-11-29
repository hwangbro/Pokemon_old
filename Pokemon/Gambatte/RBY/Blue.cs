using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public class Blue : RedBlue {

        public Blue(bool speedup = false, string saveName = "roms/pokeblue.sav", string rom = "roms/pokeblue.gbc") : base(rom, saveName, speedup) {
        }
    }
}
