using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HolidayEngine.Drawing;
using HolidayEngine.Level;
using System.IO;
using HolidayEngine.Level.Objects;

namespace HolidayEngine.Level
{
    public class Room
    {
        /// <summary>
        /// The 3D array of integers that represent the type of block at that spot.
        /// </summary>
        public int[, ,] GridArray;

        /// <summary>
        /// The width (X length) of the room in blocks.
        /// </summary>
        public int Width;

        /// <summary>
        /// The depth (Y length) of the room in blocks.
        /// </summary>
        public int Depth;

        /// <summary>
        /// The height (Z length) of the room in blocks.
        /// </summary>
        public int Height;


        /// <summary>
        /// The background texture to use.
        /// </summary>
        public TextureData Background;


        /// <summary>
        /// The set of blocks used for this room to be refered to in the GridArray.
        /// </summary>
        public Blockset BlockSet;

        /// <summary>
        /// Data holding all static vertices for this room.
        /// </summary>
        private VertexIndexData VertexData = new VertexIndexData();
        // TODO: Make this data buffered.



        /// <summary>
        /// Constructs a room.
        /// </summary>
        /// <param name="blockSet">The blockset to be used for this room.</param>
        public Room(Engine engine, String blockSetName, int Width, int Depth, int Height)
        {
            // Builds the basic room.
            GridArray = new int[Width, Depth, Height];
            this.Width = Width;
            this.Depth = Depth;
            this.Height = Height;

            this.Background = engine.textureManager.Dic["background_generic"];

            // This is the default room style.
            FillBorders(2);

            // Checks and finds the correct blockset.
            LoadBlockSet(engine, blockSetName);

            // Creates the initial mesh.
            UpdateRoomVertices();
        }


        /// <summary>
        /// Creates a room using a layout.
        /// </summary>
        public Room(Engine engine, String blockSetName, String filename)
        {
            this.Background = engine.textureManager.Dic["background_generic"];

            // Checks and finds the correct blockset.
            LoadBlockSet(engine, blockSetName);

            this.LoadLayout(engine, filename);
            // Creates the initial mesh.
            UpdateRoomVertices();
        }


        /// <summary>
        /// Loads or finds the reference to the blockset if already loaded.
        /// </summary>
        private void LoadBlockSet(Engine engine, String name)
        {
            if (engine.blockSetList.Keys.Contains(name))
            {
                BlockSet = engine.blockSetList[name];
            }
            else
            {
                BlockSet = Blockset.LoadBlockSet(engine, name);
            }
        }


        /// <summary>
        /// Recreates a new grid array for the map at a new size.
        /// </summary>
        public void RebuildArray(Engine engine, Vector3 NewSize, Vector3 Shift)
        {
            foreach (Cube cube in engine.cubeManager.CubeList)
            {
                cube.AllPosition = cube.GridPosition + Shift;
            }

            int[, ,] _newArray = new int[(int)NewSize.X, (int)NewSize.Y, (int)NewSize.Z];

            int XM = Math.Min((int)NewSize.X, (int)Shift.X + Width);
            int YM = Math.Min((int)NewSize.Y, (int)Shift.Y + Depth);
            int ZM = Math.Min((int)NewSize.Z, (int)Shift.Z + Height);

            int XS = Math.Max((int)Shift.X, 0);
            int YS = Math.Max((int)Shift.Y, 0);
            int ZS = Math.Max((int)Shift.Z, 0);

            int xS = (int)Shift.X;
            int yS = (int)Shift.Y;
            int zS = (int)Shift.Z;

            for (int xx = XS; xx < XM; xx++)
            {
                for (int yy = YS; yy < YM; yy++)
                {
                    for (int zz = ZS; zz < ZM; zz++)
                    {
                        _newArray[xx, yy, zz] = GridArray[xx - xS, yy - yS, zz - zS];
                    }
                }
            }

            GridArray = _newArray;
            Width = (int)NewSize.X;
            Depth = (int)NewSize.Y;
            Height = (int)NewSize.Z;
        }


        /// <summary>
        /// Resets any blocks larger than the current array to the default block.
        /// </summary>
        public void ResetInvalidBlocks()
        {
            for (int w = 0; w < Width; w++)
            {
                for (int d = 0; d < Depth; d++)
                {
                    for (int h = 0; h < Height; h++)
                    {
                        if (GridArray[w, d, h] >= BlockSet.Blocks.Count)
                            GridArray[w, d, h] = 1;
                    }
                }
            }
        }


        /// <summary>
        /// Replaces all of a certain block with another block.
        /// </summary>
        public void ReplaceBlocks(int FindBlock, int NewBlock)
        {
            for (int w = 0; w < Width; w++)
            {
                for (int d = 0; d < Depth; d++)
                {
                    for (int h = 0; h < Height; h++)
                    {
                        if (GridArray[w, d, h] == FindBlock)
                            GridArray[w, d, h] = NewBlock;
                    }
                }
            }
        }


        /// <summary>
        /// Clears the current vertices and generates a new representation
        /// of the room in vertices.
        /// </summary>
        public void UpdateRoomVertices()
        {
            VertexData = new VertexIndexData();
            for (int d = Depth - 1; d >= 0; d--)
            {
                for (int h = 0; h < Height; h++)
                {
                    for (int w = 0; w < Width; w++)
                    {
                        int _num = GridArray[w, d, h];
                        if (_num > 1)
                            VertexData.AddData(BlockSet.Blocks[GridArray[w, d, h]].GenerateVertices(this, new Vector3(w, d, h)));
                    }
                }
            }
        }


        /// <summary>
        /// Gets the block instance representing the block at X, Y, Z.
        /// </summary>
        public Block GetGridBlock(int x, int y, int z)
        {
            return BlockSet.Blocks[GridArray[x, y, z]];
        }

        public Block GetGridBlock(Vector3 pos)
        {
            return BlockSet.Blocks[GridArray[(int)pos.X, (int)pos.Y, (int)pos.Z]];
        }


        /// <summary>
        /// Gets the block instance safely for X, Y, Z.
        /// Returns null if it is not safe.
        /// </summary>
        public Block GetGridBlockSafe(int x, int y, int z)
        {
            if (x >= 0 && x < Width)
                if (y >= 0 && y < Depth)
                    if (z >= 0 && z < Height)
                        return BlockSet.Blocks[GridArray[x, y, z]];
            return null;
        }

        public Block GetGridBlockSafe(Vector3 pos)
        {
            if ((int)pos.X >= 0 && (int)pos.X < Width)
                if ((int)pos.Y >= 0 && (int)pos.Y < Depth)
                    if ((int)pos.Z >= 0 && (int)pos.Z < Height)
                        return BlockSet.Blocks[GridArray[(int)pos.X, (int)pos.Y, (int)pos.Z]];
            return null;
        }


        /// <summary>
        /// Draws all vertices for this room.
        /// </summary>
        public void DrawRoom(Engine engine)
        {
            engine.primManager.DrawVertices(VertexData, BlockSet.TilesetMain.TextureMain);
        }


        /// <summary>
        /// Draws the background for the room and the room.
        /// </summary>
        /// <param name="engine"></param>
        public void Draw(Engine engine)
        {
            engine.spriteBatch.Begin();
            engine.spriteBatch.Draw(Background.TextureMain, new Rectangle(0, 0, (int)engine.windowSize.X, (int)engine.windowSize.Y), Color.White);
            engine.spriteBatch.End();

            engine.primManager.Reset3D(engine);

            DrawRoom(engine);
        }


        /// <summary>
        /// Saves the layout.
        /// </summary>
        public void SaveLayout(Engine engine, String FileName)
        {
            StreamWriter _stream = new StreamWriter("Content/Texts/" + FileName + ".lyo");
            _stream.WriteLine(Width);
            _stream.WriteLine(Depth);
            _stream.WriteLine(Height);
            for (int w = 0; w < Width; w++)
            {
                for (int d = 0; d < Depth; d++)
                {
                    for (int h = 0; h < Height; h++)
                    {
                        _stream.WriteLine(GridArray[w, d, h]);
                    }
                }
            }
            _stream.Close();
        }


        /// <summary>
        /// Loads a layout.
        /// </summary>
        ///
        public bool LoadLayout(Engine engine, String FileName)
        {
            StreamReader _stream;

            try
            {
                _stream = new StreamReader("Content/Texts/" + FileName + ".lyo");
            }
            catch (System.IO.FileNotFoundException)
            {
                return false;
            }

            Width = int.Parse(_stream.ReadLine());
            Depth = int.Parse(_stream.ReadLine());
            Height = int.Parse(_stream.ReadLine());

            GridArray = new int[Width, Depth, Height];

            for (int w = 0; w < Width; w++)
            {
                for (int d = 0; d < Depth; d++)
                {
                    for (int h = 0; h < Height; h++)
                    {
                        GridArray[w, d, h] = int.Parse(_stream.ReadLine());
                    }
                }
            }
            _stream.Close();

            return true;
        }


        /// <summary>
        /// Fills the borders of the room with blocks.
        /// </summary>
        /// <param name="BlockNumber">The index of the type of block with respect to the current blockset.</param>
        public void FillBorders(int BlockNumber)
        {
            for (int w = 0; w < Width; w++)
            {
                for (int d = 0; d < Depth; d++)
                {
                    GridArray[w, d, 0] = BlockNumber;
                }

                for (int h = 0; h < Height; h++)
                {
                    GridArray[w, this.Depth - 1, h] = BlockNumber;
                }
            }

            for (int d = 0; d < Depth; d++)
            {
                for (int h = 0; h < Height; h++)
                {
                    GridArray[0, d, h] = BlockNumber;
                    GridArray[this.Width - 1, d, h] = BlockNumber;
                }
            }
        }
    }
}
