using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HolidayEngine.Drawing;
using System.IO;

namespace HolidayEngine.Level
{
    /// <summary>
    /// The blockset class holds a list of blocks and a tileset for which they belong.
    /// Note that each blockset must have two blocks, a 'null' and a 'standard' block.
    /// </summary>
    public class Blockset
    {
        public String Name;
        public Tileset TilesetMain;
        public List<Block> Blocks = new List<Block>();

        public Blockset(String Name, Tileset TilesetMain)
        {
            this.Name = Name;
            this.TilesetMain = TilesetMain;
            Blocks.Add(new Block("null", 0, TilesetMain, false, false));
            Blocks.Add(new Block("null_solid", 0, TilesetMain, false, true));
            Blocks.Add(new Block("standard", 1, TilesetMain, true, true));
        }

        public void SaveBlockSet(Engine engine, String FileName)
        {
            StreamWriter _stream = new StreamWriter("Content/Texts/" + FileName + ".bls");
            _stream.WriteLine(engine.textureManager.TilesetList.FindIndex(TilesetMain.Equals));
            foreach(Block block in Blocks)
            {
                if (block != null)
                {
                    _stream.WriteLine(block.Name + "#"
                        + block.Tex[0].ToString() + "#"
                        + block.Tex[1].ToString() + "#"
                        + block.Tex[2].ToString() + "#"
                        + block.Tex[3].ToString() + "#"
                        + block.Tex[4].ToString() + "#"
                        + block.Tex[5].ToString() + "#"
                        + block.Culling.ToString() + "#"
                        + block.Solid.ToString() + "#"
                        + block.SideProperty[0].ToString() + "#"
                        + block.SideProperty[1].ToString() + "#"
                        + block.SideProperty[2].ToString() + "#"
                        + block.SideProperty[3].ToString() + "#"
                        + block.SideProperty[4].ToString() + "#"
                        + block.SideProperty[5].ToString());
                }
                else
                    _stream.WriteLine("%");

            }
            _stream.Close();
        }

        public static Blockset LoadBlockSet(Engine engine, String FileName)
        {
            StreamReader _stream;

            try
            {
                _stream = new StreamReader("Content/Texts/" + FileName + ".bls");
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }
            Blockset _return = new Blockset(FileName, engine.textureManager.TilesetList[int.Parse(_stream.ReadLine())]);
            _return.Blocks.Clear();

            String line;
            do
            {
                line = _stream.ReadLine();
                if (line != null && line != "%")
                {
                    String[] split = line.Split('#');
                    int[] sides = new int[6];
                    for (int i = 1; i < 7; i++)
                    {
                        sides[i - 1] = int.Parse(split[i]);
                    }
                    short[] prop = new short[6];
                    for (int i = 9; i < 15; i++)
                    {
                        prop[i - 9] = short.Parse(split[i]);
                    }
                    _return.Blocks.Add(new Block(split[0], sides, prop, _return.TilesetMain, bool.Parse(split[7]), bool.Parse(split[8])));
                }
            }
            while (line != null);
            _stream.Close();

            return _return;
        }
    }
}
