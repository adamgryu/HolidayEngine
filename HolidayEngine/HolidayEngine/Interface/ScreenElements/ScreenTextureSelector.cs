using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HolidayEngine.Level;
using HolidayEngine.Drawing;
using Microsoft.Xna.Framework.Graphics;

namespace HolidayEngine.Interface.ScreenElements
{
    public class ScreenTextureSelector : ScreenElement
    {
        /// <summary>
        /// The size of the tiles when previewed in this selector.
        /// </summary>
        const float TilePreviewSize = 32;


        /// <summary>
        /// An input field to be used.
        /// </summary>
        ScreenInput InputField;

        /// <summary>
        /// Stores the previous input to detect change.
        /// </summary>
        String PreviousInput = "";


        /// <summary>
        /// A list of all tiles to display.
        /// </summary>
        public List<TextureData> FilteredList;

        /// <summary>
        /// A list of all tiles that are on the current page.
        /// A subset of FilteredList.
        /// </summary>
        public List<TextureData> PageList;

        /// <summary>
        /// The current page.
        /// </summary>
        public short Page = 0;


        /// <summary>
        /// The current texture the mouse is hovering over.
        /// </summary>
        public TextureData MouseTexture = null;

        /// <summary>
        /// The maximum number of rows to be used in the preview box.
        /// </summary>
        public int PrevMaximumRows;

        /// <summary>
        /// The maximum number of columns to be used in the preview box.
        /// </summary>
        public int PrevMaximumColumns;




        /// <summary>
        /// The constructor for this selector.
        /// </summary>
        public ScreenTextureSelector(Engine engine, Screen screen)
            : base(screen)
        {
            // Adds the page forward and back buttons to the screen.
            int _buttonHeight = screen.GetTotalElementHeight();
            ScreenButton _but = new ScreenButton(screen, "Back", "<", engine.FontMain);
            _but.Position.X = Screen.boarderSize;
            _but.Size.X = 32;
            _but.Position.Y = _buttonHeight;
            screen.ElementList.Add(_but);
            _but = new ScreenButton(screen, "Forward", ">", engine.FontMain);
            _but.Position.X = Screen.boarderSize + 32 + Screen.boarderSize;
            _but.Size.X = 32;
            _but.Position.Y = _buttonHeight;
            screen.ElementList.Add(_but);

            // Creates a new input field and adds it to the screen.
            this.InputField = new ScreenInput(screen, engine.FontMain);
            screen.AddElement(InputField);

            // Resizes and places the bread and butter of this element.
            this.Position.Y = screen.GetTotalElementHeight();
            this.Size.Y = 128;

            // Determines the max rows and columns that will fit.
            this.PrevMaximumColumns = (int)Math.Floor(Size.X / TilePreviewSize);
            this.PrevMaximumRows = (int)Math.Floor(Size.Y / TilePreviewSize);

            // Creates new lists to display.
            FilterTiles(engine);
        }




        /// <summary>
        /// Updates the element. This should not happen if the window is minimized.
        /// </summary>
        public override void Update(Engine engine)
        {
            MouseTexture = null;

            int yy = 0;
            int xx = 0;
            foreach (TextureData t in PageList)
            {
                // Changes rows and such.
                if (yy == PrevMaximumRows)
                {
                    yy = 0;
                    xx++;
                    if (xx == PrevMaximumColumns)
                        break;
                }

                // If mouse is within region.
                if (engine.inputManager.mouse.X > Position.X + screen.Position.X + xx * TilePreviewSize && engine.inputManager.mouse.X < Position.X + screen.Position.X + TilePreviewSize + xx * TilePreviewSize
                    && engine.inputManager.mouse.Y > Position.Y + screen.Position.Y + yy * TilePreviewSize && engine.inputManager.mouse.Y < Position.Y + screen.Position.Y + (yy + 1) * TilePreviewSize)
                {
                    // Sets the mouse texture.
                    MouseTexture = t;

                    // Returns the texture clicked.
                    if (engine.inputManager.MouseLeftButtonTapped)
                    {
                        screen.PreformAction(engine, "Click Texture");
                    }
                    // Returns the texture clicked.
                    if (engine.inputManager.MouseRightButtonTapped)
                    {
                        screen.PreformAction(engine, "Cover Texture");
                    }
                }
                yy++;
            }

            base.Update(engine);

            // Updates the filtered list if necessary.
            if (PreviousInput != InputField.InputString)
                FilterTiles(engine);
            PreviousInput = InputField.InputString;
        }




        /// <summary>
        /// Filters and updates the page and tile list.
        /// </summary>
        public void FilterTiles(Engine engine)
        {
            List<TextureData> _newList = new List<TextureData>();
            if (InputField.InputString == "")
            {
                foreach (TextureData t in engine.room.BlockSet.TilesetMain.Tiles)
                {
                    if (t != null)
                        _newList.Add(t);
                }
            }
            else
            {
                foreach (TextureData t in engine.room.BlockSet.TilesetMain.Tiles)
                {
                    if (t != null)
                        if (t.Name.Contains(InputField.InputString))
                            _newList.Add(t);
                }
            }
            FilteredList = _newList;
            Page = 0;
            UpdatePage();
        }




        /// <summary>
        /// Updates the current page.
        /// </summary>
        public void UpdatePage()
        {
            int _count = Math.Min(FilteredList.Count - Page * PrevMaximumRows * PrevMaximumColumns, PrevMaximumRows * PrevMaximumColumns);
            PageList = FilteredList.GetRange(Page * PrevMaximumRows * PrevMaximumColumns, _count);
        }




        /// <summary>
        /// The drawing code for this element.
        /// </summary>
        public override void Draw(Engine engine, float alpha)
        {
            Texture2D _blank = engine.textureManager.Dic["blank"].TextureMain;

            // Draws the solid background for the 'preview' blocks.
            engine.spriteBatch.Draw(
                _blank,
                new Rectangle(
                    (int)(Position.X + screen.Position.X),
                    (int)(Position.Y + screen.Position.Y),
                    (int)(this.Size.X),
                    (int)(this.Size.Y)),
                Color.Red * alpha * 0.5f);

            // Draws yellow under blocks the mouse is selecting.
            int yy = 0;
            int xx = 0;
            foreach (TextureData t in PageList)
            {
                // Changes rows and such.
                if (yy == PrevMaximumRows)
                {
                    yy = 0;
                    xx++;
                    if (xx == PrevMaximumColumns)
                        break;
                }

                // Draws the tile.
                engine.spriteBatch.Draw(t.TextureMain,
                    new Rectangle(
                        (int)(screen.Position.X + Position.X + xx * TilePreviewSize),
                        (int)(screen.Position.Y + Position.Y + yy * TilePreviewSize),
                        (int)TilePreviewSize, (int)TilePreviewSize),
                    new Rectangle(
                        (int)(t.TopLeftPercent.X * t.TextureMain.Width),
                        (int)(t.TopLeftPercent.Y * t.TextureMain.Height),
                        engine.room.BlockSet.TilesetMain.TileSize,
                        engine.room.BlockSet.TilesetMain.TileSize),
                        Color.White * alpha);
                yy++;
            }

            if (MouseTexture != null)
            {
                Vector2 _strLength = engine.FontMain.MeasureString(MouseTexture.Name);
                engine.spriteBatch.Draw(engine.textureManager.Dic["blank"].TextureMain,
                    new Rectangle(engine.inputManager.mouse.X, engine.inputManager.mouse.Y - 32, (int)_strLength.X, (int)_strLength.Y),
                    Color.Black);
                engine.spriteBatch.DrawString(engine.FontMain, MouseTexture.Name, engine.inputManager.MousePosition - Vector2.UnitY * 32, Color.White);
            }

            base.Draw(engine, alpha);
        }
    }
}
