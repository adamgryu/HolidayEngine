using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HolidayEngine.Drawing;
using HolidayEngine.Level;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HolidayEngine.Interface.ScreenElements;

namespace HolidayEngine.Interface
{

    public class LevelEditor : Screen
    {
        // 3D Cursor control settings.
        public Vector3 Cursor3D = Vector3.Zero;
        private int ZLevel = 1;
        public bool StackBlocks = true;
        public bool RestrictZ = true;

        BlockPlacer blockSelector;

        public ScreenPictureButton StackButton;
        ScreenPictureButton CameraButton;
        ScreenPictureButton ShowNullButton;

        bool DrawNull = false;

        public LevelEditor(Engine engine)
            : base("Editor")
        {
            this.Size = new Vector2(Screen.boarderSize * 4 + 32);
            CameraButton = new ScreenPictureButton(this, "Toggle Camera", engine.textureManager.Dic["camera_button"]);
            this.AddElement(CameraButton);
            StackButton = new ScreenPictureButton(this, "Toggle Block Stacking", engine.textureManager.Dic["stack_on_button"]);
            this.AddElement(StackButton);
            this.AddElement(new ScreenPictureButton(this, "Save Layout", engine.textureManager.Dic["save_button"]));
            this.AddElement(new ScreenPictureButton(this, "Load Layout", engine.textureManager.Dic["open_button"]));
            this.AddElement(new ScreenPictureButton(this, "Open Directory", engine.textureManager.Dic["dir_button"]));
            this.AddElement(new ScreenPictureButton(this, "Room Settings", engine.textureManager.Dic["room_settings"]));
            ShowNullButton = new ScreenPictureButton(this, "Toggle Show Null", engine.textureManager.Dic["show_null"]);
            this.AddElement(ShowNullButton);
            this.AddElement(new ScreenPictureButton(this, "Light Settings", engine.textureManager.Dic["light_button"]));
            this.Position = Vector2.Zero;
            blockSelector = new BlockPlacer(engine, this);
            engine.screenManager.AddScreen(blockSelector);

            engine.camera.Set(engine, CameraSetting.Free);
            this.PreformAction(engine, "Reset Camera");
        }

        public override void Update(Engine engine, bool Selected)
        {
            if (RestrictZ)
                ZLevel = Math.Min(Math.Max(ZLevel, 0), engine.room.Height - 1);

            // Controls the Z-Level of the 3D cursor.
            if (engine.inputManager.MouseScrollUp || engine.inputManager.KeyboardTapped(Keys.R))
            {
                if (ZLevel < engine.room.Height - 1 || !RestrictZ)
                {
                    ZLevel += 1;
                    if (engine.camera.cameraSetting == CameraSetting.Free)
                    {
                        engine.camera.GotoPosition.Z += Block.Size;
                        engine.camera.JumpToGoto(engine, true);
                    }
                }
            }

            if (engine.inputManager.MouseScrollDown || engine.inputManager.KeyboardTapped(Keys.F))
            {
                if (ZLevel > 0 || !RestrictZ)
                {
                    ZLevel -= 1;
                    if (engine.camera.cameraSetting == CameraSetting.Free)
                    {
                        engine.camera.GotoPosition.Z -= Block.Size;
                        engine.camera.JumpToGoto(engine, true);
                    }
                }
            }

            // Get mouse to grid coordinates.
            Vector3 _spot = ScreenToPlane(engine, engine.inputManager.MousePosition, ZLevel * Block.Size);
            Cursor3D = new Vector3((float)Math.Floor(_spot.X / Block.Size),
                (float)Math.Floor(_spot.Y / Block.Size), ZLevel);

            // Stacks block if necessary.
            if (StackBlocks && engine.room.GetGridBlockSafe((int)Cursor3D.X, (int)Cursor3D.Y, (int)Cursor3D.Z) != null)
            {
                int _top = engine.room.Height - 1;

                while (engine.room.GridArray[(int)Cursor3D.X, (int)Cursor3D.Y, _top] == 0 && _top > 0)
                {
                    _top -= 1;
                }

                if (blockSelector != null)
                    if (blockSelector.SelectedBlock == 0)
                        if (_top > 0 && _top < engine.room.Height - 1)
                            if (engine.room.GridArray[(int)Cursor3D.X, (int)Cursor3D.Y, _top + 1] == 0)
                                _top--;

                Cursor3D.Z = _top + 1;
            }

            base.Update(engine, Selected);
        }

        public override void PreformAction(Engine engine, string ActionName, params String[] Arguments)
        {
            switch (ActionName)
            {
                case "Reset Camera":
                    engine.camera.GotoPosition = Vector3.UnitX * (engine.room.Width / 2) * Block.Size
                        + Vector3.UnitZ * ((engine.room.Height * 3 / 4) + ZLevel) * Block.Size
                        - Vector3.UnitY * Block.Size * engine.room.Height;
                    engine.camera.xyDirection = MathHelper.PiOver2;
                    engine.camera.zDirection = -0.2f;
                    break;
                case "Toggle Camera":
                    if (engine.camera.cameraSetting == CameraSetting.FacingPlayer)
                    {
                        engine.camera.Set(engine, CameraSetting.Free);
                        CameraButton.Tex = engine.textureManager.Dic["camera_button"];
                        PreformAction(engine, "Reset Camera");
                    }
                    else
                    {
                        engine.camera.Set(engine, CameraSetting.FacingPlayer);
                        CameraButton.Tex = engine.textureManager.Dic["camera_button_follow"];
                    }
                    break;
                case "Toggle Block Stacking":
                    StackBlocks = !StackBlocks;
                    if (StackBlocks)
                        StackButton.Tex = engine.textureManager.Dic["stack_on_button"];
                    else
                        StackButton.Tex = engine.textureManager.Dic["stack_off_button"];
                    break;
                case "Save Layout":
                    engine.screenManager.AddScreen(new InputPrompt(engine, this, "Save Filename", "Type in the name of the layout to save it.", "", true));
                    break;
                case "Load Layout":
                    engine.screenManager.AddScreen(new InputPrompt(engine, this, "Load Filename", "Type in the name of the layout to load it.", "", true));
                    engine.room.UpdateRoomVertices();
                    break;
                case "Save Filename":
                    engine.room.SaveLayout(engine, Arguments[0]);
                    break;
                case "Load Filename":
                    if (!engine.room.LoadLayout(engine, Arguments[0]))
                        engine.screenManager.AddScreen(new Message(engine, "The file was not found.", true));
                    engine.room.UpdateRoomVertices();
                    break;
                case "Open Directory":
                    System.Diagnostics.Process.Start("explorer.exe", "Content");
                    break;
                case "Room Settings":
                    engine.screenManager.AddScreen(new RoomSettings(engine));
                    break;
                case "Light Settings":
                    engine.screenManager.AddScreen(new LightSettings(engine, this));
                    break;
                case "Toggle Show Null":
                    DrawNull = !DrawNull;
                    if (DrawNull)
                        ShowNullButton.Tex = engine.textureManager.Dic["show_null_on"];
                    else
                        ShowNullButton.Tex = engine.textureManager.Dic["show_null"];
                    break;
            }

            base.PreformAction(engine, ActionName);
        }

        public override void Draw3D(Engine engine)
        {
            // Draws the grid at the ZLevel indicated.
            engine.primManager.DrawLines(PrimHelper.GenerateGridVertices(
                new Vector3(0, 0, ZLevel * Block.Size + 0.5f), Vector3.UnitX * Block.Size, Vector3.UnitY * Block.Size, engine.room.Width, engine.room.Depth, Color.Blue));

            // Draws the verticle line indicators.
            Vector3 _pos = Cursor3D * Block.Size;
            engine.primManager.DrawLines(PrimHelper.GenerateLineVertices(
                new Vector3(_pos.X, _pos.Y, 0), new Vector3(_pos.X, _pos.Y, engine.room.Height * Block.Size), Color.White));
            engine.primManager.DrawLines(PrimHelper.GenerateLineVertices(
                new Vector3(_pos.X + Block.Size, _pos.Y, 0), new Vector3(_pos.X + Block.Size, _pos.Y, engine.room.Height * Block.Size), Color.White));
            engine.primManager.DrawLines(PrimHelper.GenerateLineVertices(
                new Vector3(_pos.X, _pos.Y + Block.Size, 0), new Vector3(_pos.X, _pos.Y + Block.Size, engine.room.Height * Block.Size), Color.White));
            engine.primManager.DrawLines(PrimHelper.GenerateLineVertices(
                new Vector3(_pos.X + Block.Size, _pos.Y + Block.Size, 0), new Vector3(_pos.X + Block.Size, _pos.Y + Block.Size, engine.room.Height * Block.Size), Color.White));

            engine.primManager.SetDepthBuffer(false);
            engine.primManager.DrawLines(PrimHelper.GenerateWireCubeVertices(Cursor3D * Block.Size, (Cursor3D + new Vector3(1, 1, 1)) * Block.Size, Color.Red));
            engine.primManager.SetDepthBuffer(true);

            // Draws null blocks if necessary.
            if (DrawNull)
            {
                // Draws the null solids.
                engine.primManager.myEffect.Alpha = 0.5f;

                // Draws invisible null blocks.
                VertexIndexData _data = new VertexIndexData();
                TextureData _tex = engine.room.BlockSet.TilesetMain.Tiles[0];
                for (int w = 0; w < engine.room.Width; w++)
                {
                    for (int d = 0; d < engine.room.Depth; d++)
                    {
                        for (int h = 0; h < engine.room.Height; h++)
                        {
                            if (engine.room.GridArray[w, d, h] == 1)
                            {
                                _data.AddData(PrimHelper.GenerateCubeVertices(new Vector3(w, d, h) * Block.Size, new Vector3(w + 1, d + 1, h + 1) * Block.Size, _tex));
                            }
                        }
                    }
                }
                if (_data.VertexList.Count != 0)
                    engine.primManager.DrawVertices(_data, _tex.TextureMain);

                // Resets alpha.
                engine.primManager.myEffect.Alpha = 1f;
            }
        }

        public static Vector3 ScreenToPlane(Engine engine, Vector2 screenPos, float zPlaneHeight)
        {
            // NOTE: this code was borrowed from online forums.
            // Create 2 positions in screenspace using the cursor position.
            Vector3 nearSource = new Vector3(screenPos, 0f);
            Vector3 farSource = new Vector3(screenPos, 1f);

            // Find the two screen space positions in world space.
            Vector3 nearPoint = engine.graphics.GraphicsDevice.Viewport.Unproject(nearSource,
                                engine.primManager.projectionMatrix,
                                engine.primManager.viewMatrix,
                                engine.primManager.worldMatrix);

            Vector3 farPoint = engine.graphics.GraphicsDevice.Viewport.Unproject(farSource,
                                engine.primManager.projectionMatrix,
                                engine.primManager.viewMatrix,
                                engine.primManager.worldMatrix);

            // Compute normalized direction vector from nearPoint to farPoint.
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // Create a ray using nearPoint as the source.
            Ray r = new Ray(nearPoint, direction);

            // Calculate the ray-plane intersection point.
            Vector3 n = new Vector3(0f, 0f, 1f);
            Plane p = new Plane(n, -zPlaneHeight);

            // Calculate distance of intersection point from r.origin.
            float denominator = Vector3.Dot(p.Normal, r.Direction);
            float numerator = Vector3.Dot(p.Normal, r.Position) + p.D;
            float t = -(numerator / denominator);

            // Calculate the picked position on the y = 0 plane.
            Vector3 pickedPosition = nearPoint + direction * t;

            return pickedPosition;
        }

    }
}
