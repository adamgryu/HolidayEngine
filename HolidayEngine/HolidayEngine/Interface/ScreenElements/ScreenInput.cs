using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HolidayEngine.Interface.ScreenElements
{
    public class ScreenInput : ScreenElement
    {
        SpriteFont Font;
        public String InputString = "";
        short Blink = 0;
        short Back = 0;
        bool Enabled = true;
        String Command = "Input";

        public ScreenInput(Screen screen, SpriteFont Font)
            : base(screen)
        {
            this.Font = Font;
            this.Size.Y = Font.LineSpacing;
        }

        public ScreenInput(Screen screen, SpriteFont Font, String Command)
            : base(screen)
        {
            this.Font = Font;
            this.Size.Y = Font.LineSpacing;
            this.Command = Command;
        }

        public override void Update(Engine engine)
        {
            if (engine.inputManager.MouseLeftButtonTapped)
            {
                if (engine.inputManager.mouse.X > Position.X + screen.Position.X && engine.inputManager.mouse.X < Position.X + screen.Position.X + Size.X
                    && engine.inputManager.mouse.Y > Position.Y + screen.Position.Y && engine.inputManager.mouse.Y < Position.Y + screen.Position.Y + Size.Y)
                {
                    Enabled = true;
                }
                else
                    Enabled = false;
            }

            Blink++;
            if (Blink > 10)
                Blink = -10;

            if (Enabled)
            {
                // This code was borrowed from Caleb.
                KeyboardState _ks = engine.inputManager.keyboard;
                foreach (Keys key in _ks.GetPressedKeys())
                {
                    if (!engine.inputManager.keyboardPrev.IsKeyDown(key))
                    {
                        string _str = key.ToString();
                        if (_str.Length == 1)
                        {
                            if (_ks.IsKeyDown(Keys.LeftShift) || _ks.IsKeyDown(Keys.RightShift))
                                InputString += key.ToString();
                            else
                                InputString += key.ToString().ToLower();
                        }
                        else if (_str.Length == 2)
                        {
                            switch (_str)
                            {
                                case "D9":
                                    if (_ks.IsKeyDown(Keys.LeftShift) || _ks.IsKeyDown(Keys.RightShift))
                                        InputString += "(";
                                    else
                                        InputString += _str.Remove(0, 1);
                                    break;
                                case "D0":
                                    if (_ks.IsKeyDown(Keys.LeftShift) || _ks.IsKeyDown(Keys.RightShift))
                                        InputString += ")";
                                    else
                                        InputString += _str.Remove(0, 1);
                                    break;
                                default:
                                    InputString += _str.Remove(0, 1);
                                    break;
                            }
                        }
                        else if (_str == "Enter")
                        {
                            screen.PreformAction(engine, Command);
                        }
                        else
                        {
                            switch (_str)
                            {
                                case "Space":
                                    InputString += " ";
                                    break;
                                case "Back":
                                    try { InputString = InputString.Remove(InputString.Length - 1); }
                                    catch { }
                                    break;
                                case "OemPeriod":
                                    InputString += ".";
                                    break;
                                case "OemQuestion":
                                    InputString += "/";
                                    break;
                                case "OemPipe":
                                    InputString += "\\";
                                    break;
                                case "OemSemicolon":
                                    InputString += ";";
                                    break;
                                case "OemPlus":
                                    InputString += "=";
                                    break;
                                case "OemMinus":
                                    if (_ks.IsKeyDown(Keys.LeftShift) || _ks.IsKeyDown(Keys.RightShift))
                                        InputString += "_";
                                    else
                                        InputString += "-";
                                    break;
                            }
                        }
                    }
                }

                if (_ks.IsKeyDown(Keys.Back))
                {
                    Back++;
                    if (Back == 30)
                        InputString = "";
                }
                else
                {
                    Back = 0;
                }
            }
        }

        public override void Draw(Engine engine, float alpha)
        {
            Texture2D _blank = engine.textureManager.Dic["blank"].TextureMain;

            engine.spriteBatch.Draw(_blank,
                new Rectangle((int)(Position.X + screen.Position.X), (int)(Position.Y + screen.Position.Y), (int)Size.X, (int)Size.Y),
                Color.White * alpha);
            String _str = InputString;
            if (Blink > 0 && Enabled)
                _str += "#";
            engine.spriteBatch.DrawString(Font,_str , screen.Position + this.Position + Vector2.UnitX * 4, Color.Black * alpha);
        }
    }
}
