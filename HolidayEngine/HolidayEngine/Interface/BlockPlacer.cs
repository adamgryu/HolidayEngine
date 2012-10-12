using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HolidayEngine.Drawing;
using HolidayEngine.Level;
using Microsoft.Xna.Framework.Graphics;
using HolidayEngine.Interface.ScreenElements;
using Microsoft.Xna.Framework.Input;

namespace HolidayEngine.Interface
{
    public class BlockPlacer : Screen
    {
        /// <summary>
        /// The parent level editor to use.
        /// </summary>
        private LevelEditor parentEditor;


        /// <summary>
        /// The top left corner of the preview box.
        /// </summary>
        private Vector2 PrevPosition;

        /// <summary>
        /// The size of the preview box.
        /// </summary>
        private const float PrevBoxWidth = 72;

        /// <summary>
        /// The length of one box in the preview.
        /// </summary>
        private const float PrevBoxHeight = 72;

        /// <summary>
        /// The size of the orthographic cube being drawn.
        /// </summary>
        private const float BoxSize = 20;

        /// <summary>
        /// The maximum number of blocks to show at once.
        /// </summary>
        private int PrevMaximumColumns = 3;

        /// <summary>
        /// The maximum number of blocks to show at once.
        /// </summary>
        private int PrevMaximumRows = 6;


        /// <summary>
        /// A premade matrix that rotates a world by a fixed amount.
        /// </summary>
        private Matrix rotationX = Matrix.CreateRotationX(MathHelper.PiOver2 + 0.4f);

        /// <summary>
        /// A precalculated ratio of how the length of an object is changed when
        /// rotated by the matrix rotationX.
        /// </summary>
        private float orthoRatio = (float)Math.Cos(0.4f);

        /// <summary>
        /// The current angle at which the blocks are spinning.
        /// </summary>
        private float angle = 0;


        /// <summary>
        /// The block the mouse is currently over.
        /// </summary>
        public Block MouseBlock = null;

        /// <summary>
        /// The position of the selected block type in the local blockset.
        /// </summary>
        public int SelectedBlock = 0;

        /// <summary>
        /// The list of blocks to show.
        /// </summary>
        private List<Block> FilteredList;

        /// <summary>
        /// A subsection of the filtered list with blocks of current page.
        /// </summary>
        private List<Block> PageList;

        /// <summary>
        /// The input field for this window.
        /// </summary>
        private ScreenInput InputField;

        /// <summary>
        /// Tracks the previous frame input for comparison.
        /// </summary>
        private String PreviousInput = "";

        /// <summary>
        /// Which page of blocks to be shown.
        /// </summary>
        private short Page = 0;




        /// <summary>
        /// Constructs a block selector window.
        /// </summary>
        public BlockPlacer(Engine engine, LevelEditor parentEditor)
            : base("Block Selector")
        {
            this.parentEditor = parentEditor;

            ScreenButton _but = new ScreenButton(this, "Back", "<", engine.FontMain);
            _but.Position.X = boarderSize;
            _but.Size.X = 32;
            ElementList.Add(_but);
            _but = new ScreenButton(this, "Forward", ">", engine.FontMain);
            _but.Position.X = boarderSize + 32 + boarderSize;
            _but.Size.X = 32;
            _but.Position.Y = headerSize + boarderSize;
            ElementList.Add(_but);
            _but = new ScreenButton(this, "New Block", "New", engine.FontMain);
            _but.Position.X = boarderSize + 32 + boarderSize + 32 + boarderSize;
            _but.Size.X = 48;
            _but.Position.Y = headerSize + boarderSize;
            ElementList.Add(_but);
            _but = new ScreenButton(this, "Edit Block", "Edit", engine.FontMain);
            _but.Position.X = boarderSize + 32 + boarderSize + 32 + boarderSize + 48 + boarderSize;
            _but.Size.X = 48;
            _but.Position.Y = headerSize + boarderSize;
            ElementList.Add(_but);
            _but = new ScreenButton(this, "Save/Load", "Sv/Ld", engine.FontMain);
            _but.Position.X = boarderSize + 32 + boarderSize + 32 + boarderSize + 48 + boarderSize + 48 + boarderSize;
            _but.Size.X = 48;
            _but.Position.Y = headerSize + boarderSize;
            ElementList.Add(_but);

            InputField = new ScreenInput(this, engine.FontMain);
            AddElement(InputField);

            PrevPosition = new Vector2((Size.X - PrevBoxWidth * PrevMaximumColumns) / 2, GetTotalElementHeight());
            this.Size.Y = PrevPosition.Y + Screen.boarderSize + PrevMaximumRows * PrevBoxHeight;

            FilterBlocks(engine);

            this.Position = new Vector2(engine.windowSize.X - this.Size.X, 0);
        }


        /// <summary>
        /// Updates the window and controls the block selector.
        /// </summary>
        public override void Update(Engine engine, bool Selected)
        {
            // If box is to be placed.
            if (Selected)
            {
                if (engine.screenManager.NoFocus)
                {
                    if ((engine.inputManager.mouse.LeftButton == ButtonState.Pressed && !parentEditor.StackBlocks) || (engine.inputManager.MouseLeftButtonTapped && parentEditor.StackBlocks))
                    {
                        if (engine.room.GetGridBlockSafe(parentEditor.Cursor3D) != null)
                        {
                            engine.room.GridArray[(int)parentEditor.Cursor3D.X, (int)parentEditor.Cursor3D.Y, (int)parentEditor.Cursor3D.Z] = SelectedBlock;
                            engine.room.UpdateRoomVertices();
                            engine.primManager.myEffect.UpdateShadowMap(engine);
                        }
                    }
                }
            }

            base.Update(engine, Selected);

            if (!Minimized && Selected)
            {
                MouseBlock = null;

                // Draws yellow under blocks the mouse is selecting.
                int i = 0;
                int xx = 0;
                foreach (Block block in PageList)
                {
                    // Changes rows and such.
                    if (i == PrevMaximumRows)
                    {
                        i = 0;
                        xx++;
                        if (xx == PrevMaximumColumns)
                            break;
                    }

                    // If mouse is within region.
                    if (engine.inputManager.mouse.X > Position.X + PrevPosition.X + xx * PrevBoxWidth && engine.inputManager.mouse.X < Position.X + PrevPosition.X + PrevBoxWidth + xx * PrevBoxWidth
                        && engine.inputManager.mouse.Y > Position.Y + PrevPosition.Y + i * PrevBoxHeight && engine.inputManager.mouse.Y < Position.Y + PrevPosition.Y + (i + 1) * PrevBoxHeight)
                    {
                        MouseBlock = block;
                        if (engine.inputManager.MouseLeftButtonTapped)
                        {
                            SelectedBlock = engine.room.BlockSet.Blocks.FindIndex(block.Equals);
                            if (SelectedBlock == -1)
                                throw new Exception("Block selected does not match any block in the current room blockset.");
                        }
                    }
                    i++;
                }

                angle += 0.01f;

                if (PreviousInput != InputField.InputString)
                    FilterBlocks(engine);
                PreviousInput = InputField.InputString;
            }
        }


        public override void PreformAction(Engine engine, string ActionName, params String[] Arguments)
        {
            switch (ActionName)
            {
                case "Back":
                    if (Page > 0)
                    {
                        Page--;
                        UpdatePage();
                    }
                    break;
                case "Forward":
                    if (FilteredList.Count > (PrevMaximumColumns * PrevMaximumRows) * (Page + 1))
                    {
                        Page++;
                        UpdatePage();
                    }
                    break;
                case "New Block":
                    engine.screenManager.AddScreen(new BlockDesigner(engine, engine.room.BlockSet.TilesetMain, this));
                    break;
                case "Edit Block":
                    engine.screenManager.AddScreen(new BlockDesigner(engine, engine.room.BlockSet.TilesetMain, this, engine.room.BlockSet.Blocks[SelectedBlock]));
                    break;
                case "Save/Load":
                    engine.screenManager.AddScreen(new SaveLoadBlockset(engine, this));
                    break;
                case "Input":
                    if (FilteredList.Count > 0)
                        SelectedBlock = engine.room.BlockSet.Blocks.FindIndex(FilteredList[0].Equals);
                    break;
            }

            base.PreformAction(engine, ActionName);
        }


        /// <summary>
        /// Filters the blocks according to the string in the InputField.
        /// </summary>
        public void FilterBlocks(Engine engine)
        {
            List<Block> _newList = new List<Block>();
            if (InputField.InputString == "")
            {
                foreach (Block b in engine.room.BlockSet.Blocks)
                {
                    if (b != null)
                        _newList.Add(b);
                }
            }
            else
            {
                foreach (Block b in engine.room.BlockSet.Blocks)
                {
                    if (b != null)
                        if (b.Name.Contains(InputField.InputString))
                            _newList.Add(b);
                }
            }
            FilteredList = _newList;
            Page = 0;
            UpdatePage();
        }

        public void UpdatePage()
        {
            Name = "Block Selector - Page " + (Page + 1).ToString();
            int _count = Math.Min(FilteredList.Count - Page * PrevMaximumRows * PrevMaximumColumns, PrevMaximumRows * PrevMaximumColumns);
            PageList = FilteredList.GetRange(Page * PrevMaximumRows * PrevMaximumColumns, _count);
        }


        public override void Draw3D(Engine engine)
        {
            if (engine.screenManager.IsSelectedScreen(this))
            {
                engine.primManager.myEffect.Alpha = 0.5f;
                engine.room.BlockSet.Blocks[SelectedBlock].Draw(engine.primManager, parentEditor.Cursor3D * Block.Size, Block.Size);
                engine.primManager.myEffect.Alpha = 1f;
            }
        }


        /// <summary>
        /// Draws the 3D orthographic elements of the menu.
        /// </summary>
        public override void Draw3DOrtho(Engine engine)
        {
            if (!Minimized)
            {
                // Prepares the primitive manager.
                engine.primManager.SetDepthBuffer(false);

                // Rotates and translates the world.
                engine.primManager.worldMatrixOrtho = Matrix.CreateRotationZ(angle) * rotationX
                    * Matrix.CreateTranslation(Position.X + PrevPosition.X + PrevBoxWidth / 2, Position.Y + PrevPosition.Y + PrevBoxHeight / 2 + 4, 0);
                engine.primManager.SetOrthographic();

                // Draws the cubes.
                int i = 0;
                int xx = 0;
                foreach (Block block in PageList)
                {
                    // TODO: make this Block.DrawCube
                    if (i == PrevMaximumRows)
                    {
                        i = 0;
                        xx++;
                        engine.primManager.worldMatrixOrtho = Matrix.CreateRotationZ(angle) * rotationX
                            * Matrix.CreateTranslation(Position.X + PrevPosition.X + PrevBoxWidth * xx + PrevBoxWidth / 2, Position.Y + PrevPosition.Y + PrevBoxHeight / 2 + 4, 0);
                        engine.primManager.SetOrthographic();
                        if (xx == PrevMaximumColumns)
                            break;
                    }
                    block.Draw(engine.primManager, new Vector3(-BoxSize, -BoxSize, -BoxSize - i * (PrevBoxHeight / orthoRatio)), BoxSize * 2);
                    i++;
                }

                // Return the primitive manager to the normal state.
                engine.primManager.SetPerspective();
                engine.primManager.SetDepthBuffer(true);
            }
        }


        /// <summary>
        /// Draws the 2D portion of the 
        /// </summary>
        /// <param name="alpha"></param>
        public override void Draw2D(Engine engine, float alpha)
        {
            // Draws the basic elements of the window.
            base.Draw2D(engine, alpha);

            // Gets the blank texture.
            Texture2D _blank = engine.textureManager.Dic["blank"].TextureMain;

            if (!Minimized)
            {
                // Draws the solid background for the 'preview' blocks.
                engine.spriteBatch.Draw(
                    _blank,
                    new Rectangle(
                        (int)(Position.X + PrevPosition.X),
                        (int)(Position.Y + PrevPosition.Y),
                        (int)(PrevBoxWidth * PrevMaximumColumns),
                        (int)(PrevMaximumRows * PrevBoxHeight)),
                    Color.Red * alpha * 0.5f);

                // Draws yellow under blocks the mouse is selecting.
                int i = 0;
                int xx = 0;
                foreach (Block block in PageList)
                {
                    // Changes rows and such.
                    if (i == PrevMaximumRows)
                    {
                        i = 0;
                        xx++;
                        if (xx == PrevMaximumColumns)
                            break;
                    }

                    // If mouse is within region.
                    if (block == MouseBlock)
                    {
                        // Draw the rectangle.
                        engine.spriteBatch.Draw(
                            _blank,
                            new Rectangle(
                                (int)(Position.X + PrevPosition.X + xx * PrevBoxWidth),
                                (int)(Position.Y + PrevPosition.Y + i * PrevBoxHeight),
                                (int)(PrevBoxWidth),
                                (int)(PrevBoxHeight)),
                            Color.Yellow * alpha);

                        Vector2 _length = engine.FontTiny.MeasureString(block.Name);

                        // Draws the rectangle for the text background.
                        engine.spriteBatch.Draw(
                            _blank,
                            new Rectangle(
                                (int)(Position.X + PrevPosition.X + xx * PrevBoxWidth),
                                (int)(Position.Y + PrevPosition.Y + i * PrevBoxHeight),
                                (int)(_length.X),
                                (int)(_length.Y)),
                            Color.Yellow * alpha);

                        // Draw the name.
                        engine.spriteBatch.DrawString(engine.FontTiny, block.Name, new Vector2(Position.X + PrevPosition.X + xx * PrevBoxWidth, Position.Y + PrevPosition.Y + i * PrevBoxHeight), Color.Black * alpha);
                    }

                    if (block == engine.room.BlockSet.Blocks[SelectedBlock])
                    {
                        // Draw the rectangle.
                        engine.spriteBatch.Draw(
                            _blank,
                            new Rectangle(
                                (int)(Position.X + PrevPosition.X + xx * PrevBoxWidth),
                                (int)(Position.Y + PrevPosition.Y + i * PrevBoxHeight),
                                (int)(PrevBoxWidth),
                                (int)(PrevBoxHeight)),
                            Color.Cyan * alpha);

                        Vector2 _length = engine.FontTiny.MeasureString(block.Name);

                        // Draws the rectangle for the text background.
                        engine.spriteBatch.Draw(
                            _blank,
                            new Rectangle(
                                (int)(Position.X + PrevPosition.X + xx * PrevBoxWidth),
                                (int)(Position.Y + PrevPosition.Y + i * PrevBoxHeight),
                                (int)(_length.X),
                                (int)(_length.Y)),
                            Color.Cyan * alpha);

                        // Draw the name.
                        engine.spriteBatch.DrawString(engine.FontTiny, block.Name, new Vector2(Position.X + PrevPosition.X + xx * PrevBoxWidth, Position.Y + PrevPosition.Y + i * PrevBoxHeight), Color.Black * alpha);
                    }
                    i++;
                }
            }
        }
    }
}
