using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HolidayEngine.Level;
using Microsoft.Xna.Framework;
using HolidayEngine.Drawing;
using HolidayEngine.Interface.ScreenElements;
using Microsoft.Xna.Framework.Graphics;

namespace HolidayEngine.Interface
{
    public class BlockDesigner : Screen
    {
        Block CurrentBlock;
        Tileset tileset;
        BlockPlacer parentScreen;

        Vector2 PrevBox = new Vector2(Screen.boarderSize * 2 + 32 * 3, Screen.boarderSize + Screen.headerSize);
        float PrevBoxSize = 128;
        float BoxSize = 38;

        float xyGotoAngle = 0;
        float zGotoAngle = 0;
        float xyAngle = 0;
        float zAngle = 0;

        int SelectedSide = 0;
        ScreenTextureSelector TextureSelector;

        bool EditMode;

        ScreenPictureButton BBack;
        ScreenPictureButton BTop;
        ScreenPictureButton BLeft;
        ScreenPictureButton BRight;
        ScreenPictureButton BFront;
        ScreenPictureButton BBottom;

        public BlockDesigner(Engine engine, Tileset tileset, BlockPlacer parentScreen)
            : base("Block Designer")
        {
            this.parentScreen = parentScreen;
            this.tileset = tileset;
            this.EditMode = false;
            this.CurrentBlock = new Block("", 0, tileset, true, true);

            SetUpScreen(engine);
        }

        public BlockDesigner(Engine engine, Tileset tileset, BlockPlacer parentScreen, Block block)
            : base("Block Designer")
        {
            this.parentScreen = parentScreen;
            this.tileset = tileset;
            this.EditMode = true;
            this.CurrentBlock = block;

            SetUpScreen(engine);

            ScreenButton _but = new ScreenButton(this, "Delete", "Del", engine.FontMain);
            _but.Position.X = Screen.boarderSize * 6 + 2 * 32 + 48 + 64 + 48;
            _but.Position.Y = PrevBox.Y + PrevBoxSize + boarderSize;;
            _but.Size.X = 32;
            this.AddElement(_but);
        }

        private void SetUpScreen(Engine engine)
        {
            this.Size.X = 320;
            this.AddCloseButton(engine);
            this.DemandPriority = true;

            Vector2 _tl = new Vector2(Screen.boarderSize, Screen.boarderSize + Screen.headerSize);
            Vector2 _sz = new Vector2(32, 32);
            Vector2 _left = new Vector2(32, 0);
            Vector2 _down = new Vector2(0, 32);
            BBack = new ScreenPictureButton(this, "Set Back", engine.textureManager.Dic["backarrow"], _tl, _sz);
            BTop = new ScreenPictureButton(this, "Set Top", engine.textureManager.Dic["uparrow"], _tl + _left, _sz);
            BLeft = new ScreenPictureButton(this, "Set Left", engine.textureManager.Dic["leftarrow"], _tl + _down, _sz);
            BRight = new ScreenPictureButton(this, "Set Right", engine.textureManager.Dic["rightarrow"], _tl + _down + _left * 2, _sz);
            BFront = new ScreenPictureButton(this, "Set Front", engine.textureManager.Dic["frontarrow"], _tl + _down + _left, _sz);
            BBottom = new ScreenPictureButton(this, "Set Bottom", engine.textureManager.Dic["downarrow"], _tl + _down * 2 + _left, _sz);
            this.AddElement(BBack);
            this.AddElement(BTop);
            this.AddElement(BLeft);
            this.AddElement(BRight);
            this.AddElement(BFront);
            this.AddElement(BBottom);

            UpdateButtonColor();

            TextureSelector = new ScreenTextureSelector(engine, this);
            AddElement(TextureSelector);

            float _butHeight = PrevBox.Y + PrevBoxSize + boarderSize;
            ScreenButton _but = new ScreenButton(this, "Name", engine.FontMain);
            _but.Position.X = Screen.boarderSize * 3 + 2 * 32;
            _but.Position.Y = _butHeight;
            _but.Size.X = 48;
            this.AddElement(_but);
            _but = new ScreenButton(this, "Settings", engine.FontMain);
            _but.Position.X = Screen.boarderSize * 4 + 2 * 32 + 48;
            _but.Position.Y = _butHeight;
            _but.Size.X = 64;
            this.AddElement(_but);
            _but = new ScreenButton(this, "Save", engine.FontMain);
            _but.Position.X = Screen.boarderSize * 5 + 2 * 32 + 48 + 64;
            _but.Position.Y = _butHeight;
            _but.Size.X = 48;
            this.AddElement(_but);

            UpdateHeader();
        }

        public override void Update(Engine engine, bool Selected)
        {
            xyAngle = xyAngle + (xyGotoAngle - xyAngle) / 16;
            zAngle = zAngle + (zGotoAngle - zAngle) / 16;
            base.Update(engine, Selected);
        }

        public override int GetTotalElementHeight()
        {
            int _ret = base.GetTotalElementHeight();
            if (PrevBox.Y + PrevBoxSize + boarderSize > _ret)
                _ret = (int)(PrevBox.Y + PrevBoxSize + boarderSize);
            return _ret;
        }

        private void UpdateButtonColor()
        {
            List<Color> colorDic = new List<Color>() {Color.Blue, Color.Green, Color.Orange, Color.Violet};
            BFront.DefaultColor = colorDic[CurrentBlock.SideProperty[0]];
            BRight.DefaultColor = colorDic[CurrentBlock.SideProperty[1]];
            BBack.DefaultColor = colorDic[CurrentBlock.SideProperty[2]];
            BLeft.DefaultColor = colorDic[CurrentBlock.SideProperty[3]];
            BTop.DefaultColor = colorDic[CurrentBlock.SideProperty[5]];
            BBottom.DefaultColor = colorDic[CurrentBlock.SideProperty[4]];
        }

        public override void PreformAction(Engine engine, string ActionName, params String[] Arguments)
        {
            switch (ActionName)
            {
                case "Set Back":
                    xyGotoAngle = MathHelper.Pi;
                    zGotoAngle = 0;
                    SelectedSide = 2;
                    break;
                case "Set Front":
                    xyGotoAngle = 0;
                    zGotoAngle = 0;
                    SelectedSide = 0;
                    break;
                case "Set Left":
                    xyGotoAngle = MathHelper.PiOver2;
                    zGotoAngle = 0;
                    SelectedSide = 3;
                    break;
                case "Set Right":
                    xyGotoAngle = -MathHelper.PiOver2;
                    zGotoAngle = 0;
                    SelectedSide = 1;
                    break;
                case "Set Top":
                    xyGotoAngle = 0;
                    zGotoAngle = MathHelper.PiOver2;
                    SelectedSide = 5;
                    break;
                case "Set Bottom":
                    xyGotoAngle = 0;
                    zGotoAngle = -MathHelper.PiOver2;
                    SelectedSide = 4;
                    break;
                case "#Set Back":
                    CurrentBlock.SideProperty[2] += 1;
                    if (CurrentBlock.SideProperty[2] == Block.MaxSideProperty)
                        CurrentBlock.SideProperty[2] = 0;
                    UpdateButtonColor();
                    break;
                case "#Set Front":
                    CurrentBlock.SideProperty[ 0] += 1;
                    if (CurrentBlock.SideProperty[ 0] == Block.MaxSideProperty)
                        CurrentBlock.SideProperty[ 0] = 0;
                    UpdateButtonColor();
                    break;
                case "#Set Left":
                    CurrentBlock.SideProperty[3] += 1;
                    if (CurrentBlock.SideProperty[3] == Block.MaxSideProperty)
                        CurrentBlock.SideProperty[3] = 0;
                    UpdateButtonColor();
                    break;
                case "#Set Right":
                    CurrentBlock.SideProperty[1] += 1;
                    if (CurrentBlock.SideProperty[1] == Block.MaxSideProperty)
                        CurrentBlock.SideProperty[1] = 0;
                    UpdateButtonColor();
                    break;
                case "#Set Top":
                    CurrentBlock.SideProperty[5] += 1;
                    if (CurrentBlock.SideProperty[5] == Block.MaxSideProperty)
                        CurrentBlock.SideProperty[5] = 0;
                    UpdateButtonColor();
                    break;
                case "#Set Bottom":
                    CurrentBlock.SideProperty[4] += 1;
                    if (CurrentBlock.SideProperty[4] == Block.MaxSideProperty)
                        CurrentBlock.SideProperty[4] = 0;
                    UpdateButtonColor();
                    break;
                case "Back":
                    if (TextureSelector.Page > 0)
                    {
                        TextureSelector.Page--;
                        TextureSelector.UpdatePage();
                        UpdateHeader();
                    }
                    break;
                case "Forward":
                    if (TextureSelector.FilteredList.Count > (TextureSelector.PrevMaximumColumns * TextureSelector.PrevMaximumRows) * (TextureSelector.Page + 1))
                    {
                        TextureSelector.Page++;
                        TextureSelector.UpdatePage();
                        UpdateHeader();
                    }
                    break;
                case "Click Texture":
                    CurrentBlock.Tex[SelectedSide] = tileset.Tiles.FindIndex(TextureSelector.MouseTexture.Equals);
                    break;
                case "Cover Texture":
                    for (int i = 0; i < 6; i++)
                        CurrentBlock.Tex[i] = tileset.Tiles.FindIndex(TextureSelector.MouseTexture.Equals);
                    break;
                case "Input":
                    if (TextureSelector.FilteredList.Count > 0)
                        CurrentBlock.Tex[SelectedSide] = tileset.Tiles.FindIndex(TextureSelector.FilteredList[0].Equals);
                    break;
                case "Name":
                    Screen _scr = new InputPrompt(engine, this, "Name Chooser", "Type in a name for this block to be used in searches.", CurrentBlock.Name, false);
                    _scr.Center(engine);
                    engine.screenManager.AddScreen(_scr);
                    break;
                case "Name Chooser":
                    CurrentBlock.Name = Arguments[0];
                    UpdateHeader();
                    break;
                case "Give Block Name":
                    CurrentBlock.Name = Arguments[0];
                    PreformAction(engine, "Save");
                    break;
                case "Save":
                    if (CurrentBlock.Name == "")
                    {
                        Screen _scre = new InputPrompt(engine, this, "Give Block Name", "Give this block a name before you save it.", CurrentBlock.Name, false);
                        _scre.Center(engine);
                        engine.screenManager.AddScreen(_scre);
                    }
                    else
                    {
                        if (!EditMode)
                        {
                            int _firstNullSpot = engine.room.BlockSet.Blocks.FindIndex(
                                delegate(Block block)
                                {
                                    return block == null;
                                });
                            if (_firstNullSpot != -1)
                                engine.room.BlockSet.Blocks[_firstNullSpot] = CurrentBlock;
                            else
                                engine.room.BlockSet.Blocks.Add(CurrentBlock);
                        }
                        parentScreen.FilterBlocks(engine);
                        engine.room.UpdateRoomVertices();
                        PreformAction(engine, "Close");
                    }
                    break;
                case "Settings":
                    engine.screenManager.AddScreen(new BlockSettings(engine, CurrentBlock));
                    break;
                case "Delete":
                    if (engine.room.BlockSet.Blocks.FindIndex(CurrentBlock.Equals) > 2)
                        engine.screenManager.AddScreen(new ConfirmPrompt(engine, this, "Confirm Delete", "Are you sure you want to delete this block?"));
                    else
                        engine.screenManager.AddScreen(new Message(engine, "This is a special block, you cannot delete it.",true));
                    break;
                case "Confirm Delete Yes":
                    int ii = engine.room.BlockSet.Blocks.FindIndex(CurrentBlock.Equals);
                    engine.room.ReplaceBlocks(ii, 1);
                    engine.room.BlockSet.Blocks[ii] = null;
                    parentScreen.SelectedBlock = 1;
                    parentScreen.FilterBlocks(engine);
                    PreformAction(engine, "Close");
                    engine.room.UpdateRoomVertices();
                    break;
            }
            base.PreformAction(engine, ActionName);
        }

        public void UpdateHeader()
        {
            this.Name = CurrentBlock.Name + " - Page " + (TextureSelector.Page + 1).ToString();
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
                engine.primManager.worldMatrixOrtho = Matrix.CreateRotationZ(xyAngle) * Matrix.CreateRotationX(zAngle + MathHelper.PiOver2)
                    * Matrix.CreateTranslation(Position.X + PrevBox.X + PrevBoxSize / 2, Position.Y + PrevBox.Y + PrevBoxSize / 2, 0);
                engine.primManager.SetOrthographic();

                CurrentBlock.Draw(engine.primManager, new Vector3(-BoxSize, -BoxSize, -BoxSize), BoxSize * 2);

                // Return the primitive manager to the normal state.
                engine.primManager.SetPerspective();
                engine.primManager.SetDepthBuffer(true);
            }
        }

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
                        (int)(Position.X + PrevBox.X),
                        (int)(Position.Y + PrevBox.Y),
                        (int)(PrevBoxSize),
                        (int)(PrevBoxSize)),
                    Color.Red * alpha * 0.5f);
            }
        }
    }
}
