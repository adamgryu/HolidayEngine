using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HolidayEngine.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HolidayEngine.Level
{
    public class Block
    {
        /// <summary>
        /// Gives the side that is run into when an object moving a certain
        /// vector direction bumbs into a block.
        /// </summary>
        public static Dictionary<Vector3, int> VectorToSide = new Dictionary<Vector3, int>()
        {
            {Vector3.UnitY, 0},
            {-Vector3.UnitX, 1},
            {-Vector3.UnitY, 2},
            {Vector3.UnitX, 3},
            {Vector3.UnitZ, 4},
            {-Vector3.UnitZ, 5}
        };

        public static bool VectorInDic(Vector3 vector)
        {
            return VectorToSide.Keys.Contains<Vector3>(vector);
        }

        /// <summary>
        /// The size of a square block on the grid.
        /// </summary>
        public const float Size = 16f;

        /// <summary>
        /// The intger representing the number of side states that exist.
        /// </summary>
        public const int MaxSideProperty = 3;

        /// <summary>
        /// The name of the block; only be used in searches.
        /// </summary>
        public String Name;

        /// <summary>
        /// The tileset this block uses. 
        /// </summary>
        public Tileset TilesetMain;

        /// <summary>
        /// The texture information for this block.
        /// 0: Front.
        /// 1: Right.
        /// 2: Back.
        /// 3: Left.
        /// 4: Bottom.
        /// 5: Top.
        /// </summary>
        public int[] Tex;

        /// <summary>
        /// Ladder information for this block.
        /// Uses the same array layout as Tex.
        /// Property 0: Nothing.
        /// Property 1: Ladder.
        /// Property 2: Ice.
        /// </summary>
        public short[] SideProperty;

        /// <summary>
        /// If other blocks should cull the side that touches this block.
        /// </summary>
        public bool Culling;

        /// <summary>
        /// If the block is solid.
        /// </summary>
        public bool Solid;

        /// <summary>
        /// Constructs a new block.
        /// </summary>
        public Block(String Name, int Tex, Tileset TilesetMain, bool Culling, bool Solid)
        {
            this.Name = Name;
            this.TilesetMain = TilesetMain;
            this.Tex = new int[6] { Tex, Tex, Tex, Tex, Tex, Tex };
            this.SideProperty = new short[6];
            this.Culling = Culling;
            this.Solid = Solid;
        }

        public Block(String Name, int[] Tex, short[] SideProperty, Tileset TilesetMain, bool Culling, bool Solid)
        {
            this.Name = Name;
            this.TilesetMain = TilesetMain;
            this.Tex = Tex;
            this.SideProperty = SideProperty;
            this.Culling = Culling;
            this.Solid = Solid;
        }

        /// <summary>
        /// Generates vertex and index information for this block.
        /// </summary>
        public VertexIndexData GenerateVertices(Room CurrentRoom, Vector3 Position)
        {
            Vector3 MasterBottomLeftFront = new Vector3(Size, Size, Size) * Position;
            Vector3 MasterTopRightBack = MasterBottomLeftFront + new Vector3(Size, Size, Size);

            Vector3 topLeftFront = new Vector3(MasterBottomLeftFront.X, MasterBottomLeftFront.Y, MasterTopRightBack.Z);
            Vector3 bottomLeftFront = MasterBottomLeftFront;
            Vector3 topRightFront = new Vector3(MasterTopRightBack.X, MasterBottomLeftFront.Y, MasterTopRightBack.Z);
            Vector3 bottomRightFront = new Vector3(MasterTopRightBack.X, MasterBottomLeftFront.Y, MasterBottomLeftFront.Z);
            Vector3 topLeftBack = new Vector3(MasterBottomLeftFront.X, MasterTopRightBack.Y, MasterTopRightBack.Z);
            Vector3 bottomLeftBack = new Vector3(MasterBottomLeftFront.X, MasterTopRightBack.Y, MasterBottomLeftFront.Z);
            Vector3 topRightBack = MasterTopRightBack;
            Vector3 bottomRightBack = new Vector3(MasterTopRightBack.X, MasterTopRightBack.Y, MasterBottomLeftFront.Z);

            Vector3 frontNormal = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 backNormal = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 topNormal = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 bottomNormal = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 leftNormal = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f);

            VertexIndexData _returnData = new VertexIndexData();

            if (Position.Y > 0)
                if (CurrentRoom.GetGridBlock((int)Position.X, (int)Position.Y - 1, (int)Position.Z).Culling == false)
                {
                    if (Tex[0] > 0)
                        _returnData.AddData(PrimHelper.GenerateWallVertices(bottomLeftFront, topRightFront, frontNormal, TilesetMain.Tiles[Tex[0]]));
                }

            if (Position.Y < CurrentRoom.Depth - 1)
                if (CurrentRoom.GetGridBlock((int)Position.X, (int)Position.Y + 1, (int)Position.Z).Culling == false)
                {
                    if (Tex[1] > 0)
                        _returnData.AddData(PrimHelper.GenerateWallVertices(bottomRightBack, topLeftBack, backNormal, TilesetMain.Tiles[Tex[2]]));
                }

            if (Position.X > 0)
                if (CurrentRoom.GetGridBlock((int)Position.X - 1, (int)Position.Y, (int)Position.Z).Culling == false)
                {
                    if (Tex[2] > 0)
                        _returnData.AddData(PrimHelper.GenerateWallVertices(bottomLeftBack, topLeftFront, leftNormal, TilesetMain.Tiles[Tex[3]]));
                }

            if (Position.X < CurrentRoom.Width - 1)
                if (CurrentRoom.GetGridBlock((int)Position.X + 1, (int)Position.Y, (int)Position.Z).Culling == false)
                {
                    if (Tex[3] > 0)
                        _returnData.AddData(PrimHelper.GenerateWallVertices(bottomRightFront, topRightBack, rightNormal, TilesetMain.Tiles[Tex[1]]));
                }

            if (Position.Z > 0)
                if (CurrentRoom.GetGridBlock((int)Position.X, (int)Position.Y, (int)Position.Z - 1).Culling == false)
                {
                    if (Tex[4] > 0)
                        _returnData.AddData(PrimHelper.GenerateFloorVertices(bottomLeftBack, bottomRightFront, bottomNormal, TilesetMain.Tiles[Tex[4]]));
                }

            if (Position.Z < CurrentRoom.Height - 1)
                if (CurrentRoom.GetGridBlock((int)Position.X, (int)Position.Y, (int)Position.Z + 1).Culling == false)
                {
                    if (Tex[5] > 0)
                        _returnData.AddData(PrimHelper.GenerateFloorVertices(topLeftFront, topRightBack, topNormal, TilesetMain.Tiles[Tex[5]]));
                }

            return _returnData;
        }

        public void Draw(PrimManager PrimManager, Vector3 BottomLeft, float Size)
        {
            Vector3 MasterBottomLeftFront = BottomLeft;
            Vector3 MasterTopRightBack = MasterBottomLeftFront + new Vector3(Size, Size, Size);

            Vector3 topLeftFront = new Vector3(MasterBottomLeftFront.X, MasterBottomLeftFront.Y, MasterTopRightBack.Z);
            Vector3 bottomLeftFront = MasterBottomLeftFront;
            Vector3 topRightFront = new Vector3(MasterTopRightBack.X, MasterBottomLeftFront.Y, MasterTopRightBack.Z);
            Vector3 bottomRightFront = new Vector3(MasterTopRightBack.X, MasterBottomLeftFront.Y, MasterBottomLeftFront.Z);
            Vector3 topLeftBack = new Vector3(MasterBottomLeftFront.X, MasterTopRightBack.Y, MasterTopRightBack.Z);
            Vector3 bottomLeftBack = new Vector3(MasterBottomLeftFront.X, MasterTopRightBack.Y, MasterBottomLeftFront.Z);
            Vector3 topRightBack = MasterTopRightBack;
            Vector3 bottomRightBack = new Vector3(MasterTopRightBack.X, MasterTopRightBack.Y, MasterBottomLeftFront.Z);

            Vector3 frontNormal = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 backNormal = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 topNormal = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 bottomNormal = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 leftNormal = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f);

            VertexIndexData _returnData = new VertexIndexData();

            _returnData.AddData(PrimHelper.GenerateWallVertices(bottomLeftFront, topRightFront, frontNormal, TilesetMain.Tiles[Tex[0]]));

            _returnData.AddData(PrimHelper.GenerateWallVertices(bottomRightBack, topLeftBack, backNormal, TilesetMain.Tiles[Tex[2]]));
            _returnData.AddData(PrimHelper.GenerateWallVertices(bottomLeftBack, topLeftFront, leftNormal, TilesetMain.Tiles[Tex[3]]));

            _returnData.AddData(PrimHelper.GenerateWallVertices(bottomRightFront, topRightBack, rightNormal, TilesetMain.Tiles[Tex[1]]));

            _returnData.AddData(PrimHelper.GenerateFloorVertices(bottomLeftBack, bottomRightFront, bottomNormal, TilesetMain.Tiles[Tex[4]]));
            _returnData.AddData(PrimHelper.GenerateFloorVertices(topLeftFront, topRightBack, topNormal, TilesetMain.Tiles[Tex[5]]));

            PrimManager.DrawVertices(_returnData, TilesetMain.TextureMain);
        }
    }
}
