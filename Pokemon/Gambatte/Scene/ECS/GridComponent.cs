using System;
using System.Collections.Generic;
using System.Linq;


namespace Pokemon {

    public class GridComponent : SpriteComponent {

        public Bitmap Bitmap;

        private int XOffset = -1;
        private int YOffset = -1;
        private byte RSCX;
        private byte RSCY;
        private Bitmap Viewport;

        public byte LineBrightness = 0x19;
        public byte LineAlpha = 0xFF;

        public Dictionary<Action, byte[]> Colors;

        private bool CGBMode;

        private string _Path;
        public string Path {
            get { return _Path; }
            set {
                _Path = value.Trim();

                RSCX = Gb.CpuRead(0xFF43);
                RSCY = Gb.CpuRead(0xFF42);
                XOffset = RSCX;
                YOffset = RSCY;

                int xMin = 0;
                int yMin = 0;
                int xMax = 0;
                int yMax = 0;
                List<(int, int, byte[])> tiles = new List<(int, int, byte[])>();

                if(Path != "") {
                    string[] actions = Path.Split(" ");
                    int x = 0;
                    int y = 0;
                    for(int i = -1; i < actions.Length; i++) {
                        Action action = i < 0 ? Action.None : ActionFunctions.ToAction(actions[i]);
                        Action nextAction = i != actions.Length - 1 ? ActionFunctions.ToAction(actions[i + 1]) : Action.Up;

                        if(!ActionFunctions.IsDpad(action) && action != Action.None) {
                            continue;
                        }

                        if(action == Action.Up) {
                            y--;
                        } else if(action == Action.Down) {
                            y++;
                        } else if(action == Action.Left) {
                            x--;
                        } else if(action == Action.Right) {
                            x++;
                        }

                        byte[] color = Colors[nextAction];
                        tiles.Add((x, y, color));

                        xMin = Math.Min(x, xMin);
                        yMin = Math.Min(y, yMin);
                        xMax = Math.Max(x, xMax);
                        yMax = Math.Max(y, yMax);
                    }
                } else {
                    xMax = 32;
                    yMax = 32;
                }

                int xTileOffset = (int) MathF.Round(XOffset / 16.0f) + 4;
                int yTileOffset = (int) MathF.Round(YOffset / 16.0f) + 4;

                Bitmap = new Bitmap((xMax - xMin + xTileOffset + 6) * 16, (yMax - yMin + yTileOffset + 6) * 16);
                Bitmap.Clear(0x00);

                foreach((int, int, byte[]) tile in tiles) {
                    int xTile = tile.Item1 + xTileOffset - xMin;
                    int yTile = tile.Item2 + yTileOffset - yMin;
                    byte[] color = tile.Item3;
                    Bitmap.DrawRect(xTile * 16, yTile * 16, 16, 16, color[0], color[1], color[2], color[3]);
                }

                for(int i = 0; i < Bitmap.Width / 16; i++) {
                    Bitmap.DrawRect(i * 16, 0, 1, Bitmap.Height, LineBrightness, LineBrightness, LineBrightness, LineAlpha);
                }

                for(int i = 0; i < Bitmap.Height / 16; i++) {
                    Bitmap.DrawRect(0, i * 16, Bitmap.Width, 1, LineBrightness, LineBrightness, LineBrightness, LineAlpha);
                }

                XOffset += Math.Abs(xMin * 16);
                YOffset += Math.Abs(yMin * 16);
                Viewport = new Bitmap(160, 144);
                Texture = RenderContext.CreateTexture2D(Viewport.Width, Viewport.Height, null, PixelFormat.RGBA);
            }
        }

        public GridComponent() : base(null) {
            Colors = new Dictionary<Action, byte[]>() {
                { Action.Up, new byte[] { 0xFF, 0xFF, 0x00, 0x80 } },
                { Action.Down, new byte[] { 0xFF, 0xFF, 0x00, 0x80 } },
                { Action.Left, new byte[] { 0xFF, 0xFF, 0x00, 0x80 } },
                { Action.Right, new byte[] { 0xFF, 0xFF, 0x00, 0x80 } },
                { Action.A, new byte[] { 0x00, 0xFF, 0x00, 0x80 } },
                { Action.UpA, new byte[] { 0x00, 0xFF, 0x00, 0x80 } },
                { Action.DownA, new byte[] { 0x00, 0xFF, 0x00, 0x80 } },
                { Action.LeftA, new byte[] { 0x00, 0xFF, 0x00, 0x80 } },
                { Action.RightA, new byte[] { 0x00, 0xFF, 0x00, 0x80 } },
                { Action.StartB, new byte[] { 0x00, 0x00, 0xFF, 0x80 } },
                { Action.Select, new byte[] { 0xFF, 0x00, 0xFF, 0x80 } },
            };
        }

        public override void OnInit() {
            CGBMode = Entity.Scene.Gb is Gsc;
        }

        public override void OnEvent(Event e, EventDispatcher dispatcher) {
            base.OnEvent(e, dispatcher);
            dispatcher.Dispatch<UpdateEvent>(OnUpdate);
            dispatcher.Dispatch<RenderEvent>(OnRender);
        }

        private void OnUpdate(UpdateEvent e) {
            byte[] state = Gb.SaveState();
            byte newRSCX = Gb.CpuRead(0xFF43);
            byte newRSCY = Gb.CpuRead(0xFF42);
            int xChange = newRSCX - RSCX;
            int yChange = newRSCY - RSCY;
            if(xChange >= 128) xChange -= 256;
            if(yChange >= 128) yChange -= 256;
            if(xChange <= -128) xChange += 256;
            if(yChange <= -128) yChange += 256;
            int xAbs = Math.Abs(xChange);
            int yAbs = Math.Abs(yChange);
            if(xAbs <= 4 && yAbs <= 4) {
                XOffset += xChange;
                YOffset += yChange;
            }
            RSCX = newRSCX;
            RSCY = newRSCY;

            Bitmap.SubImage(Viewport, XOffset, YOffset);

            uint oamOffset = Gb.SaveStateLabels["hram"];
            uint vramOffset = Gb.SaveStateLabels["vram"];
            for(uint pointer = 0; pointer < 0xA0;) {
                OAMSprite oamSprite = new OAMSprite() {
                    Y = state[oamOffset + pointer++],
                    X = state[oamOffset + pointer++],
                    TileNumber = state[oamOffset + pointer++],
                    Attribute = state[oamOffset + pointer++],
                };

                if(oamSprite.Y >= 160) continue;

                uint spriteOffset = vramOffset + oamSprite.TileNumber * 16u;
                if(CGBMode) spriteOffset += oamSprite.VramBank * 0x2000u;

                byte[] spriteTile = state.Subarray(spriteOffset, 16);
                for(int y = 0; y < 8; y++) {
                    byte top = spriteTile[y * 2 + 0];
                    byte bot = spriteTile[y * 2 + 1];
                    for(int x = 0; x < 8; x++) {
                        byte pixel = (byte) (((top >> (7 - x)) & 1) + ((bot >> (7 - x)) & 1) * 2);
                        if(pixel > 0) {
                            int xPixel = oamSprite.X + (oamSprite.XFlip ? 7 - x : x) - 8;
                            int yPixel = oamSprite.Y + (oamSprite.YFlip ? 7 - y : y) - 16;
                            if(xPixel < 0 || yPixel < 0 || xPixel >= Viewport.Width || yPixel >= Viewport.Height) continue;
                            Viewport.SetPixel(xPixel, yPixel, 0x00, 0x00, 0x00, 0x00);
                        }
                    }
                }
            }

            byte winPos = state[Gb.SaveStateLabels["hram"] + 0x14A];
            uint windowTileMapOffset = 0;
            bool yFlip = false;
            if(winPos == 0x70) {
                windowTileMapOffset = vramOffset + 0x1C00; 
                yFlip = true;
            } else if(winPos != 0x00) windowTileMapOffset = vramOffset + 0x1800;

            if(windowTileMapOffset != 0) {
                for(uint vramPointer = 0; vramPointer < 0x800; vramPointer++) {
                    uint x = vramPointer % 32;
                    uint y = vramPointer / 32;
                    if(x >= 0 && x < 20 && y >= 0 && y < 18) {
                        if(yFlip) y = 17 - y;
                        byte tileIndex = state[windowTileMapOffset + vramPointer];
                        if(tileIndex >= 0x60 || tileIndex == 0x22) Viewport.DrawRect((int) x * 8, (int) y * 8, 8, 8, 0x00, 0x00, 0x00, 0x00);
                    }
                }
            }

            Texture.UpdateContents(Viewport.Data, PixelFormat.RGBA);
        }
    }
}
