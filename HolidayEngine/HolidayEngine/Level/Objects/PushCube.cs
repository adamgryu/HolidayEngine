using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HolidayEngine.Drawing;

namespace HolidayEngine.Level.Objects
{
    public class PushCube : WorldCube
    {
        TextureData Tex;

        public PushCube(Vector3 Position, TextureData Tex)
            : base(Position)
        {
            this.UsesIce = true;
            this.Tex = Tex;
        }

        public override void Update(Engine engine)
        {
            if (AnimationStep == 0)
                if (Cube.IsMoveSafe(engine, GridPosition - Vector3.UnitZ))
                    if (Move(engine, GridPosition - Vector3.UnitZ))
                        Falling();

            base.Update(engine);
        }

        public override void Collide(Engine engine, WorldCube otherCube, Vector3 direction)
        {
            if (direction != Vector3.UnitZ)
                Move(engine, GridPosition + direction);
        }

        public override void Draw(Engine engine)
        {
            PrimHelper.DrawCube(engine.primManager, DrawPosition, DrawPosition + new Vector3(Block.Size, Block.Size, Block.Size), Tex);
        }
    }
}
