using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using HolidayEngine.Drawing;

namespace HolidayEngine.Interface.ScreenElements
{
    public class ScreenPictureButton : ScreenElement
    {
        public TextureData Tex;
        String Action;

        Color ButtonColor = Color.Blue;
        public Color DefaultColor = Color.Blue;

        public ScreenPictureButton(Screen screen, String Action, TextureData Tex)
            : base(screen)
        {
            this.Action = Action;
            this.Tex = Tex;

            PositionButton();
        }

        public ScreenPictureButton(Screen screen, String Action, TextureData Tex, Vector2 Pos, Vector2 Size)
            : base(screen)
        {
            this.Action = Action;
            this.Tex = Tex;

            this.Position = Pos;
            this.Size = Size;
        }

        private void PositionButton()
        {
            this.Position.X = (int)((screen.Size.X - Tex.TextureMain.Width) / 2 - Screen.boarderSize);
            this.Size.X = Tex.TextureMain.Width + (int)(Screen.boarderSize * 2);
            this.Size.Y = Tex.TextureMain.Height + (int)(Screen.boarderSize * 2);
        }

        public override void Update(Engine engine)
        {
            if (engine.inputManager.mouse.X > Position.X + screen.Position.X && engine.inputManager.mouse.X < Position.X + screen.Position.X + Size.X
                && engine.inputManager.mouse.Y > Position.Y + screen.Position.Y && engine.inputManager.mouse.Y < Position.Y + screen.Position.Y + Size.Y)
            {
                ButtonColor = Color.Red;
                if (engine.inputManager.MouseLeftButtonTapped)
                {
                    ButtonColor = Color.Blue;
                    screen.PreformAction(engine, Action);
                }
                if (engine.inputManager.MouseRightButtonTapped)
                {
                    ButtonColor = Color.Green;
                    screen.PreformAction(engine, "#" + Action);
                }
            }
            else
                ButtonColor = DefaultColor;
        }

        public override void Draw(Engine engine, float alpha)
        {
            Texture2D _blank = engine.textureManager.Dic["blank"].TextureMain;
            engine.spriteBatch.Draw(_blank,
                new Rectangle((int)(Position.X + screen.Position.X), (int)(Position.Y + screen.Position.Y), (int)Size.X, (int)Size.Y),
                ButtonColor * alpha * 0.5f);
            engine.spriteBatch.Draw(Tex.TextureMain, screen.Position + Position + Size / 2 - new Vector2(Tex.TextureMain.Width, Tex.TextureMain.Height) / 2, Color.White);
        }
    }
}
