using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace HolidayEngine.Drawing
{
    public class Tileset
    {
        public String Name;
        public Texture2D TextureMain;
        public List<TextureData> Tiles = new List<TextureData>();
        public int TileSize;

        public Tileset(String Name, Texture2D TextureMain, int TileSize)
        {
            this.TextureMain = TextureMain;
            this.TileSize = TileSize;
        }
    }
}
