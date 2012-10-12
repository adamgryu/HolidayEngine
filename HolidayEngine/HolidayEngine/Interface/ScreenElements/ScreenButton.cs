using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HolidayEngine.Interface.ScreenElements
{
    public class ScreenButton : ScreenElement
    {
        public String Text;
        String Action;
        SpriteFont Font;
        Vector2 StringSize;

        Color ButtonColor = Color.Blue;

        public ScreenButton(Screen screen, String Action, SpriteFont Font)
            : base(screen)
        {
            this.Text = Action;
            this.Action = Action;
            this.Font = Font;
            StringSize = Font.MeasureString(Text);

            PositionButton();
        }

        public ScreenButton(Screen screen, String Action, String Text, SpriteFont Font)
            : base(screen)
        {
            this.Text = Text;
            this.Action = Action;
            this.Font = Font;
            StringSize = Font.MeasureString(Text);

            PositionButton();
        }

        private void PositionButton()
        {
            this.Position.X = (int)((screen.Size.X - StringSize.X) / 2 - Screen.boarderSize);
            this.Size.X = StringSize.X + (int)(Screen.boarderSize * 2);
            this.Size.Y = StringSize.Y + (int)(Screen.boarderSize * 2);
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
                    screen.PreformAction(engine, "#"+Action);
                }
            }
            else
                ButtonColor = Color.Blue;
        }

        public override void Draw(Engine engine, float alpha)
        {
            Texture2D _blank = engine.textureManager.Dic["blank"].TextureMain;
            engine.spriteBatch.Draw(_blank,
                new Rectangle((int)(Position.X + screen.Position.X), (int)(Position.Y + screen.Position.Y), (int)Size.X, (int)Size.Y),
                ButtonColor * alpha * 0.5f);
            engine.spriteBatch.DrawString(Font, Text, screen.Position + Position + Size / 2 - StringSize / 2, Color.White * alpha);
        }
    }
}
