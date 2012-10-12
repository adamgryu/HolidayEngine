using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace HolidayEngine.Drawing
{
    public class TextureManager
    {
        public Dictionary<String, TextureData> Dic = new Dictionary<String, TextureData>();
        public List<Tileset> TilesetList = new List<Tileset>();
        public Dictionary<String, Animation> AniDic = new Dictionary<String, Animation>();

        public TextureManager()
        {
            // Nothing thus far.
        }

        public void AddTexture(TextureData Tex)
        {
            Dic.Add(Tex.Name, Tex);
        }

        public void LoadTexturesFromFile(String Filename, ContentManager Content)
        {
            // The filename should not have the extension.

            TextReader rawTextureFile = new StreamReader("Content/Texts/" + Filename + ".tin");
            String line;
            do
            {
                line = rawTextureFile.ReadLine();
                if (line != null)
                {
                    // Loads a tileset
                    if (line[0] == '#')
                    {
                        String[] _lineSplit = line.Split('#');

                        Texture2D _texture = Content.Load<Texture2D>("Tilesets/" + _lineSplit[2]);

                        LoadTileset(_lineSplit[2], _texture, int.Parse(_lineSplit[1]));
                    }

                    // Loads an animation.
                    else if (line[0] == '@')
                    {
                        // Splits the data.
                        String[] _lineSplit = line.Split('@');

                        // Loads the texture.
                        Texture2D _texture = Content.Load<Texture2D>("Animations/" + _lineSplit[2]);

                        // Gets the grid width.
                        int gridWidth = int.Parse(_lineSplit[1]);

                        float frameSize = _texture.Width / gridWidth;

                        // Creates a new animation.
                        Animation _ani = new Animation(_texture);

                        for (int i = 3; i < _lineSplit.Length; i++)
                        {
                            // Creates a new list to hold texture data.
                            List<TextureData> _smallAni = new List<TextureData>();

                            // Gets the frames in this smaller animation.
                            int _frames = int.Parse(_lineSplit[i]);

                            // Loads individual frames.
                            for (int f = 0; f < _frames; f++)
                            {
                                TextureData _tex = new TextureData(
                                    _lineSplit[2] + (i-3).ToString() + "_" + f.ToString(),
                                    _texture,
                                    new Vector2((float)f / (float)gridWidth, frameSize * (i-3) / (float)_texture.Height),
                                    new Vector2(1f / (float)gridWidth, frameSize / (float)_texture.Height));

                                _smallAni.Add(_tex);
                            }

                            _ani.AniList.Add(_smallAni);
                        }

                        AniDic.Add(_lineSplit[2], _ani);
                    }

                    // Loads a single image
                    else
                    {
                        Texture2D _texture = Content.Load<Texture2D>("Textures/" + line);
                        AddTexture(new TextureData(line, _texture));
                    }
                }
            }
            while (line != null);

            rawTextureFile.Close();
        }

        private void LoadTileset(String Filename, Texture2D Texture, int GridWidth)
        {
            // The filename should not have the extension.

            Tileset _newTileset = new Tileset(Filename, Texture, Texture.Width / GridWidth);
            TilesetList.Add(_newTileset);

            TextReader tilesetFile = new StreamReader("Content/Texts/" + Filename + ".tlin");
            String line;
            int lineNum = 0;
            do
            {
                line = tilesetFile.ReadLine();
                if (line != null)
                {
                    String[] _lineSplit = line.Split(' ');
                    for (int i = 0; i < _lineSplit.Length; i++)
                    {
                        TextureData _newTexture = new TextureData(_lineSplit[i], Texture, new Vector2((float)i / (float)GridWidth, (float)lineNum / (float)GridWidth), new Vector2(1f / (float)GridWidth, 1f / (float)GridWidth));
                        _newTileset.Tiles.Add(_newTexture);
                    }

                    for (int i = _lineSplit.Length; i < GridWidth; i++)
                    {
                        _newTileset.Tiles.Add(null);
                    }
                }
                lineNum += 1;
            }
            while (line != null);

            tilesetFile.Close();
        }
    }
}
