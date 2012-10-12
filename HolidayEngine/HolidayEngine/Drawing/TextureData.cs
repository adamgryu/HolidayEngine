using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HolidayEngine.Drawing
{
    public class TextureData
    {
        private const float EdgeBuffer = 0.0000f;

        public String Name;
        public Texture2D TextureMain;
        public Vector2 TopLeftPercent;
        public Vector2 BottomRightPercent;

        public TextureData(String Name, Texture2D TextureMain, Vector2 TopLeftPercent, Vector2 SizePercent)
        {
            this.Name = Name;
            this.TextureMain = TextureMain;
            this.TopLeftPercent = TopLeftPercent + new Vector2(EdgeBuffer, EdgeBuffer);
            this.BottomRightPercent = TopLeftPercent + SizePercent - new Vector2(EdgeBuffer, EdgeBuffer);
        }

        public TextureData(String Name, Texture2D TextureMain)
        {
            this.Name = Name;
            this.TextureMain = TextureMain;
            this.TopLeftPercent = Vector2.Zero;
            this.BottomRightPercent = new Vector2(1f, 1f);
        }
    }
}
