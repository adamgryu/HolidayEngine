using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HolidayEngine.Interface.ScreenElements
{
    public class ScreenText : ScreenElement
    {
        SpriteFont Font;
        List<String> LineText;

        public ScreenText(Screen screen, String text, SpriteFont font)
            : base (screen)
        {
            Font = font;
            LineText = SplitText(text);
            this.Size.Y = LineText.Count * Font.LineSpacing;
        }

        public void UpdateText(String Text)
        {
            LineText = SplitText(Text);
        }

        private List<String> SplitText(String Text)
        {
            String[] _array = Text.Split('#');
            List<String> textList = _array.ToList<String>();

            for(int i = 0; i < textList.Count; i++)
            {
                textList[i] = textList[i].Trim();

                String _newLine = "";
                String _oldLine = textList[i];
                int _count = 0;
                while (Font.MeasureString(_oldLine).X > this.Size.X)
                {
                    int _index = _oldLine.LastIndexOf(' ');
                    if (_index == -1)
                        break;

                    _newLine = _oldLine.Substring(_index) + _newLine;
                    _oldLine = _oldLine.Remove(_index);

                    _count++;
                    if (_count > 1000)
                        throw new Exception("Split 1000 lines of text. Error?");
                }

                if (_newLine != "")
                {
                    textList[i] = _oldLine;
                    textList.Insert(i + 1, _newLine);
                }
            }

            return textList;
        }

        public override void Draw(Engine engine, float alpha)
        {
            for(int i = 0; i < LineText.Count; i++)
            {
                engine.spriteBatch.DrawString(Font, LineText[i], screen.Position + this.Position + new Vector2(0, i * Font.LineSpacing), Color.White * alpha);
            }
        }
    }
}
