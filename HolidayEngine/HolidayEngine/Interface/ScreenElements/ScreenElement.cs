using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HolidayEngine.Interface.ScreenElements
{
    public class ScreenElement
    {
        public Vector2 Position;
        public Vector2 Size;
        public Screen screen;

        public ScreenElement(Screen screen)
        {
            this.screen = screen;
            this.Position = new Vector2(Screen.boarderSize, screen.GetTotalElementHeight());
            this.Size = new Vector2(screen.Size.X - 2 * Screen.boarderSize, 128);
        }

        public virtual void Update(Engine engine)
        {

        }

        public virtual void Draw(Engine engine, float alpha)
        {

        }
    }
}
