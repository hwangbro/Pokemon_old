using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Pokemon {

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Address {

        public byte Bank;
        public ushort Addr;

        public Address(byte bank, ushort addr) => (Bank, Addr) = (bank, addr);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RomAddress {

        public uint Value;

        public byte Bank {
            get { return (byte) (Value / 0x4000U); }
        }

        public ushort Address {
            get { return (ushort) (Value % 0x4000U + 0x4000U); }
        }

        public RomAddress(uint val) => Value = val;
        public RomAddress(byte bank, ushort addr) => Value = (bank == 0 ? addr : (bank * 0x4000U) + (addr - 0x4000U));

        public static implicit operator RomAddress(uint v) => new RomAddress(v);
        public static implicit operator uint(RomAddress a) => a.Value;
        public static implicit operator int(RomAddress a) => (int) a.Value;
        public static implicit operator short(RomAddress a) => (short) a.Value;
        public static implicit operator ushort(RomAddress a) => (ushort) a.Value;

        public static RomAddress operator +(RomAddress addr1, RomAddress addr2) { return new RomAddress(addr1.Value + addr2.Value); }
        public static RomAddress operator -(RomAddress addr1, RomAddress addr2) { return new RomAddress(addr1.Value - addr2.Value); }
        public static RomAddress operator *(RomAddress addr1, RomAddress addr2) { return new RomAddress(addr1.Value * addr2.Value); }
        public static RomAddress operator /(RomAddress addr1, RomAddress addr2) { return new RomAddress(addr1.Value / addr2.Value); }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CpuAddress {

        public uint Value;

        public byte Bank {
            get { return (byte) ((Value >> 16) & 0xFF); }
        }

        public ushort Address {
            get { return (ushort) (Value & 0xFFFF); }
        }

        public CpuAddress(uint val) => Value = val;
        public CpuAddress(byte bank, ushort addr) => Value = (uint) (bank == 0 ? addr : (bank << 16) | addr);

        public static implicit operator CpuAddress(uint v) => new CpuAddress(v);
        public static implicit operator uint(CpuAddress a) => a.Value;
        public static implicit operator int(CpuAddress a) => (int) a.Value;
        public static implicit operator short(CpuAddress a) => (short) a.Value;
        public static implicit operator ushort(CpuAddress a) => (ushort) a.Value;

        public static CpuAddress operator +(CpuAddress addr1, CpuAddress addr2) { return new CpuAddress(addr1.Value + addr2.Value); }
        public static CpuAddress operator -(CpuAddress addr1, CpuAddress addr2) { return new CpuAddress(addr1.Value - addr2.Value); }
        public static CpuAddress operator *(CpuAddress addr1, CpuAddress addr2) { return new CpuAddress(addr1.Value * addr2.Value); }
        public static CpuAddress operator /(CpuAddress addr1, CpuAddress addr2) { return new CpuAddress(addr1.Value / addr2.Value); }
    }

    public class SymEntry {

        private Address Address;

        public byte Bank {
            get { return Address.Bank; }
        }

        public ushort Addr {
            get { return Address.Addr; }
        }

        public SymEntry(Address address) => Address = address;

        public RomAddress Rom() {
            return this;
        }

        public CpuAddress Cpu() {
            return this;
        }

        public static implicit operator RomAddress(SymEntry s) {
            if(s.Address.Bank == 0) return s.Address.Addr;
            else return (s.Address.Bank * 0x4000U) + (s.Address.Addr - 0x4000U);
        }

        public static implicit operator CpuAddress(SymEntry s) {
            if(s.Address.Bank == 0) return s.Address.Addr;
            else return (uint) (s.Address.Bank << 16 | s.Address.Addr);
        }

        public static bool operator ==(CpuAddress address, SymEntry entry) { return entry.Cpu().Value == address.Value; }
        public static bool operator !=(CpuAddress address, SymEntry entry) { return entry.Cpu().Value != address.Value; }
        public static bool operator ==(RomAddress address, SymEntry entry) { return entry.Rom().Value == address.Value; }
        public static bool operator !=(RomAddress address, SymEntry entry) { return entry.Rom().Value != address.Value; }
        public static bool operator ==(SymEntry entry, CpuAddress address) { return entry.Cpu().Value == address.Value; }
        public static bool operator !=(SymEntry entry, CpuAddress address) { return entry.Cpu().Value != address.Value; }
        public static bool operator ==(SymEntry entry, RomAddress address) { return entry.Rom().Value == address.Value; }
        public static bool operator !=(SymEntry entry, RomAddress address) { return entry.Rom().Value != address.Value; }
    }

    public class Sym : Dictionary<string, SymEntry> {

        public Sym(string filePath) {
            string[] lines = File.ReadAllLines(filePath);

            for(int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                line = line.Trim();

                if(line.StartsWith(";") || line == "") {
                    continue;
                }

                byte bank = byte.Parse(line.Substring(0, 2), NumberStyles.HexNumber);
                ushort addr = ushort.Parse(line.Substring(3, 4), NumberStyles.HexNumber);
                string name = line.Substring(8);
                this[name] = new SymEntry(new Address(bank, addr));
            }
        }

        public string GetAddressName(RomAddress address) {
            foreach(KeyValuePair<string, SymEntry> entry in this) {
                RomAddress romaddress = entry.Value;
                if(romaddress.Value == address.Value) {
                    return entry.Key;
                }
            }

            return null;
        }

        public string GetAddressName(CpuAddress address) {
            foreach(KeyValuePair<string, SymEntry> entry in this) {
                CpuAddress cpuaddress = entry.Value;
                if(cpuaddress.Value == address.Value) {
                    return entry.Key;
                }
            }

            return null;
        }
    }
}
