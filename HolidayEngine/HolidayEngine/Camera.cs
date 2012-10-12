using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HolidayEngine.Drawing;
using HolidayEngine.Level;
using Microsoft.Xna.Framework.Input;
using HolidayEngine.Level.Objects.PlayerAI;
using HolidayEngine.Level.Objects;

namespace HolidayEngine
{
    public enum CameraSetting
    {
        Fixed,
        Free,
        FacingPlayer
    }

    public class Camera
    {
        public Vector3 GotoPosition = Vector3.Zero;
        public Vector3 Position = Vector3.Zero;
        public Vector3 GotoFocus = Vector3.UnitY;
        public Vector3 Focus = Vector3.UnitY;

        public float xyDirection = 0;
        public float zDirection = 0;

        public Player PlayerFocus;

        public float GotoPSpeed = 8;
        public float GotoFSpeed = 8;

        public CameraSetting cameraSetting = CameraSetting.FacingPlayer;


        public Camera()
        {

        }


        public virtual void Update(Engine engine)
        {
            switch (cameraSetting)
            {
                case CameraSetting.FacingPlayer:
                    if (PlayerFocus != null)
                    {
                        Vector3 AddToFocus = Vector3.UnitZ * 4 * Block.Size - Vector3.UnitY * 8 * Block.Size;

                        // This whole block tries to move the camera if blocks are obscuring the player.
                        if (!Cube.IsMoveBlockSafeIgnoreEdges(engine, PlayerFocus.GridPosition - Vector3.UnitY))
                            if (!Cube.IsMoveBlockSafeIgnoreEdges(engine, PlayerFocus.GridPosition - Vector3.UnitY + Vector3.UnitZ))
                            {
                                ShiftFocusToSide(engine, ref AddToFocus);
                                if (Cube.IsMoveBlockSafeIgnoreEdges(engine, PlayerFocus.GridPosition + Vector3.UnitZ))
                                {
                                    AddToFocus.Z += 4 * Block.Size;
                                }
                            }
                            else
                            {
                                if (!Cube.IsMoveBlockSafeIgnoreEdges(engine, PlayerFocus.GridPosition + Vector3.UnitZ))
                                {
                                    ShiftFocusToSide(engine, ref AddToFocus);
                                }
                                else
                                {
                                    AddToFocus.Z += 4 * Block.Size;
                                }
                            }
                        else
                        {
                            if (!Cube.IsMoveBlockSafeIgnoreEdges(engine, PlayerFocus.GridPosition + Vector3.UnitZ))
                            {
                                AddToFocus.Z -= 7 * Block.Size;
                            }
                        }

                        this.GotoFocus = PlayerFocus.DrawPosition + new Vector3(Block.Size, Block.Size, Block.Size) / 2;
                        this.GotoPosition = GotoFocus + AddToFocus;

                        if (GotoPosition.Z < Block.Size * 1.5f)
                            GotoPosition.Z = Block.Size * 1.5f;

                    }
                    break;

                case CameraSetting.Free:
                    engine.IsMouseVisible = true;
                    if (engine.inputManager.IsKeyDown(Keys.LeftControl))
                    {
                        engine.IsMouseVisible = false;
                        this.xyDirection -= (engine.inputManager.mouse.X - engine.windowSize.X / 2) / 512f;
                        this.zDirection -= (engine.inputManager.mouse.Y - engine.windowSize.Y / 2) / 512f;

                        Mouse.SetPosition((int)(engine.windowSize.X / 2), (int)(engine.windowSize.Y / 2));
                        xyDirection = MathHelper.WrapAngle(xyDirection);
                        if (zDirection > MathHelper.PiOver2)
                            zDirection = MathHelper.PiOver2;
                        if (zDirection < -MathHelper.PiOver2)
                            zDirection = -MathHelper.PiOver2;
                    }

                    Vector3 _lookingVector = (4 * (float)Math.Cos(xyDirection) * Vector3.UnitX
                        + 4 * (float)Math.Sin(xyDirection) * Vector3.UnitY) * (float)Math.Cos(zDirection)
                        + 4 * (float)Math.Sin(zDirection) * Vector3.UnitZ;

                    if (engine.inputManager.IsKeyDown(Keys.LeftControl))
                    {
                        if (engine.inputManager.IsKeyDown(InputManager.KeyForward))
                            this.GotoPosition += _lookingVector;
                        if (engine.inputManager.IsKeyDown(InputManager.KeyBackward))
                            this.GotoPosition -= _lookingVector;
                        if (engine.inputManager.IsKeyDown(InputManager.KeyLeft))
                            this.GotoPosition -= Vector3.Normalize(Vector3.Cross(_lookingVector, Vector3.UnitZ)) * 4;
                        if (engine.inputManager.IsKeyDown(InputManager.KeyRight))
                            this.GotoPosition += Vector3.Normalize(Vector3.Cross(_lookingVector, Vector3.UnitZ)) * 4;
                    }

                    if (engine.inputManager.mouse.RightButton == ButtonState.Pressed)
                    {
                        Vector2 _vec = (engine.inputManager.MousePrevPosition - engine.inputManager.MousePosition);
                        _vec.X *= -1;
                        float _angle = (float)Math.Atan2(_vec.Y, _vec.X) + MathHelper.PiOver2;
                        Vector2 _cam = (float)Math.Cos(xyDirection + _angle) * Vector2.UnitX + (float)Math.Sin(xyDirection + _angle) * Vector2.UnitY;
                        GotoPosition += new Vector3(_cam, 0) * _vec.Length() / 2;
                    }

                    this.GotoFocus = GotoPosition
                        + _lookingVector;
                    break;

                case CameraSetting.Fixed:
                    break;
            }

            this.Position = Position + (GotoPosition - Position) / GotoPSpeed;
            this.Focus = Focus + (GotoFocus - Focus) / GotoFSpeed;
            engine.primManager.SetView(Position, Focus);
        }


        private void ShiftFocusToSide(Engine engine, ref Vector3 ShiftVector)
        {
            ShiftVector.Y += Block.Size * 2;
            if (Position.X > PlayerFocus.DrawPosition.X + Block.Size / 2)
            {
                if (Cube.IsMoveBlockSafeIgnoreEdges(engine, PlayerFocus.GridPosition + Vector3.UnitX))
                    ShiftVector.X += Block.Size * 5;
                else if (Cube.IsMoveBlockSafeIgnoreEdges(engine, PlayerFocus.GridPosition - Vector3.UnitX))
                    ShiftVector.X -= Block.Size * 5;
            }
            else
            {
                if (Cube.IsMoveBlockSafeIgnoreEdges(engine, PlayerFocus.GridPosition - Vector3.UnitX))
                    ShiftVector.X -= Block.Size * 5;
                else if (Cube.IsMoveBlockSafeIgnoreEdges(engine, PlayerFocus.GridPosition + Vector3.UnitX))
                    ShiftVector.X += Block.Size * 5;
            }
        }


        public void Set(Engine engine, CameraSetting setting)
        {
            switch (setting)
            {
                case CameraSetting.FacingPlayer:
                    GotoFSpeed = 32;
                    GotoPSpeed = 64;
                    cameraSetting = setting;
                    FindFocusPlayer(engine);
                    break;
                default:
                    GotoFSpeed = 8;
                    GotoPSpeed = 8;
                    cameraSetting = setting;
                    break;
            }
        }


        public void FindFocusPlayer(Engine engine)
        {
            Cube _cube = engine.cubeManager.AllCubes.Find(
                delegate(Cube cb)
                {
                    return cb is Player;
                }
            );

            if (_cube != null)
                PlayerFocus = (Player)_cube;
        }


        public void JumpToGoto(Engine engine, bool UseDirection)
        {
            if (UseDirection)
            {
                Vector3 _lookingVector = (4 * (float)Math.Cos(xyDirection) * Vector3.UnitX
                + 4 * (float)Math.Sin(xyDirection) * Vector3.UnitY) * (float)Math.Cos(zDirection)
                + 4 * (float)Math.Sin(zDirection) * Vector3.UnitZ;

                this.GotoFocus = GotoPosition
                    + _lookingVector;
            }
            this.Focus = GotoFocus;
            this.Position = GotoPosition;
            engine.primManager.SetView(Position, Focus);
        }
    }
}
