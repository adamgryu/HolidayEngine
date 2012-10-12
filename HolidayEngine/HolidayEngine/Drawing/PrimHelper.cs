using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HolidayEngine.Drawing
{
    public static class PrimHelper
    {

        public static void DrawCube(PrimManager PrimManager, Vector3 BottomLeftFront, Vector3 TopRightBack, TextureData Tex)
        {
            VertexIndexData _cube = PrimHelper.GenerateCubeVertices(BottomLeftFront, TopRightBack, Tex);
            PrimManager.DrawVertices(_cube, Tex.TextureMain);
        }

        public static LineData GenerateLineVertices(Vector3 StartPos, Vector3 EndPos, Color Colour)
        {
            List<VertexPositionColor> _vert = new List<VertexPositionColor> {
                    new VertexPositionColor(StartPos, Colour),
                    new VertexPositionColor(EndPos, Colour) };
            List<short> _indx = new List<short> {0,1};
            return new LineData(_vert, _indx);
        }

        public static LineData GenerateWireCubeVertices(Vector3 BottomLeftFront, Vector3 TopRightBack, Color color)
        {
            List<VertexPositionColor> vert = new List<VertexPositionColor>();
            List<short> indx = new List<short>();

            Vector3 topLeftFront = new Vector3(BottomLeftFront.X, BottomLeftFront.Y, TopRightBack.Z);
            Vector3 bottomLeftFront = BottomLeftFront;
            Vector3 topRightFront = new Vector3(TopRightBack.X, BottomLeftFront.Y, TopRightBack.Z);
            Vector3 bottomRightFront = new Vector3(TopRightBack.X, BottomLeftFront.Y, BottomLeftFront.Z);
            Vector3 topLeftBack = new Vector3(BottomLeftFront.X, TopRightBack.Y, TopRightBack.Z);
            Vector3 bottomLeftBack = new Vector3(BottomLeftFront.X, TopRightBack.Y, BottomLeftFront.Z);
            Vector3 topRightBack = TopRightBack;
            Vector3 bottomRightBack = new Vector3(TopRightBack.X, TopRightBack.Y, BottomLeftFront.Z);

            vert.Add(new VertexPositionColor(topLeftFront,color));
            vert.Add(new VertexPositionColor(topRightFront,color));
            vert.Add(new VertexPositionColor(topRightBack, color));
            vert.Add(new VertexPositionColor(topLeftBack, color));
            vert.Add(new VertexPositionColor(bottomLeftFront, color));
            vert.Add(new VertexPositionColor(bottomRightFront, color));
            vert.Add(new VertexPositionColor(bottomRightBack, color));
            vert.Add(new VertexPositionColor(bottomLeftBack, color));

            indx.Add(0);
            indx.Add(1);
            indx.Add(1);
            indx.Add(2);
            indx.Add(2);
            indx.Add(3);
            indx.Add(3);
            indx.Add(0);

            indx.Add(4);
            indx.Add(5);
            indx.Add(5);
            indx.Add(6);
            indx.Add(6);
            indx.Add(7);
            indx.Add(7);
            indx.Add(4);

            indx.Add(0);
            indx.Add(4);
            indx.Add(1);
            indx.Add(5);
            indx.Add(2);
            indx.Add(6);
            indx.Add(3);
            indx.Add(7);

            return new LineData(vert, indx);
        }

        public static LineData GenerateGridVertices(Vector3 BottomLeftCorner, Vector3 XLength, Vector3 YLength, int XTileNum, int YTileNum, Color Colour)
        {
            List<VertexPositionColor> vert = new List<VertexPositionColor>();
            List<short> indx = new List<short>();

            Vector3 _yFullLength = YLength * YTileNum;
            for (int xx = 0; xx <= XTileNum; xx++)
            {
                Vector3 _start = XLength * xx;
                vert.Add(new VertexPositionColor(BottomLeftCorner + _start, Colour));
                vert.Add(new VertexPositionColor(BottomLeftCorner + _start + _yFullLength, Colour));
                indx.Add((short)(xx * 2));
                indx.Add((short)(xx * 2 + 1));

            }

            Vector3 _xFullLength = XLength * XTileNum;
            for (int yy = 0; yy <= YTileNum; yy++)
            {
                Vector3 _start = YLength * yy;
                vert.Add(new VertexPositionColor(BottomLeftCorner + _start, Colour));
                vert.Add(new VertexPositionColor(BottomLeftCorner + _start + _xFullLength, Colour));
                indx.Add((short)(yy * 2 + XTileNum * 2 + 2));
                indx.Add((short)(yy * 2 + XTileNum * 2 + 3));
            }
            return new LineData(vert, indx);
        }

        public static VertexIndexData GenerateWallVertices(Vector3 BottomLeftCorner, Vector3 TopRightCorner, Vector3 Normal, TextureData Tex)
        {
            Vector3 topLeft = new Vector3(BottomLeftCorner.X, BottomLeftCorner.Y, TopRightCorner.Z);
            Vector3 bottomLeft = BottomLeftCorner;
            Vector3 topRight = TopRightCorner;
            Vector3 bottomRight = new Vector3(TopRightCorner.X, TopRightCorner.Y, BottomLeftCorner.Z);

            Vector2 textureTopLeft = Tex.TopLeftPercent;
            Vector2 textureTopRight = new Vector2(Tex.BottomRightPercent.X, Tex.TopLeftPercent.Y);
            Vector2 textureBottomLeft = new Vector2(Tex.TopLeftPercent.X, Tex.BottomRightPercent.Y);
            Vector2 textureBottomRight = Tex.BottomRightPercent;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[4];
            short[] indices = new short[6];

            vertices[0] =
                new VertexPositionNormalTexture(
                topLeft, Normal, textureTopLeft);
            vertices[1] =
                new VertexPositionNormalTexture(
                topRight, Normal, textureTopRight);
            vertices[2] =
                new VertexPositionNormalTexture(
                bottomLeft, Normal, textureBottomLeft);
            vertices[3] =
                new VertexPositionNormalTexture(
                bottomRight, Normal, textureBottomRight);

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 2;
            indices[4] = 1;
            indices[5] = 3;

            return new VertexIndexData(vertices, indices);
        }

        public static VertexIndexData GenerateFloorVertices(Vector3 BottomLeftCorner, Vector3 TopRightCorner, Vector3 Normal, TextureData Tex)
        {
            Vector3 topLeft = new Vector3(BottomLeftCorner.X, TopRightCorner.Y, BottomLeftCorner.Z);
            Vector3 bottomLeft = BottomLeftCorner;
            Vector3 topRight = new Vector3(TopRightCorner.X, TopRightCorner.Y, BottomLeftCorner.Z);
            Vector3 bottomRight = new Vector3(TopRightCorner.X, BottomLeftCorner.Y, BottomLeftCorner.Z);

            Vector2 textureTopLeft = Tex.TopLeftPercent;
            Vector2 textureTopRight = new Vector2(Tex.BottomRightPercent.X, Tex.TopLeftPercent.Y);
            Vector2 textureBottomLeft = new Vector2(Tex.TopLeftPercent.X, Tex.BottomRightPercent.Y);
            Vector2 textureBottomRight = Tex.BottomRightPercent;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[4];
            short[] indices = new short[6];

            vertices[0] =
                new VertexPositionNormalTexture(
                topLeft, Normal, textureTopLeft);
            vertices[1] =
                new VertexPositionNormalTexture(
                topRight, Normal, textureTopRight);
            vertices[2] =
                new VertexPositionNormalTexture(
                bottomLeft, Normal, textureBottomLeft);
            vertices[3] =
                new VertexPositionNormalTexture(
                bottomRight, Normal, textureBottomRight);

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 2;
            indices[4] = 1;
            indices[5] = 3;

            return new VertexIndexData(vertices, indices);
        }

        /// <summary>
        /// Generates vertices for a cube.
        /// </summary>
        /// <param name="BottomLeftFront"> The bottom left front corner of the cube.</param>
        /// <param name="TopRightBack">The top right back corner of the cube.</param>
        /// <param name="Tex">The texture to cover the cube.</param>
        /// <returns>Returns a VertexIndexData instance containing the information.</returns>
        public static VertexIndexData GenerateCubeVertices(Vector3 BottomLeftFront, Vector3 TopRightBack, TextureData Tex)
        {
            // NOTE: the Front is the side that is facing the camera, which should be in -Y zone, thus the side with lowest Y.
            //       the Left has smaller X value than the Right

            Vector3 topLeftFront = new Vector3(BottomLeftFront.X, BottomLeftFront.Y, TopRightBack.Z);
            Vector3 bottomLeftFront = BottomLeftFront;
            Vector3 topRightFront = new Vector3(TopRightBack.X, BottomLeftFront.Y, TopRightBack.Z);
            Vector3 bottomRightFront = new Vector3(TopRightBack.X, BottomLeftFront.Y, BottomLeftFront.Z);
            Vector3 topLeftBack = new Vector3(BottomLeftFront.X, TopRightBack.Y, TopRightBack.Z);
            Vector3 bottomLeftBack = new Vector3(BottomLeftFront.X, TopRightBack.Y, BottomLeftFront.Z);
            Vector3 topRightBack = TopRightBack;
            Vector3 bottomRightBack = new Vector3(TopRightBack.X, TopRightBack.Y, BottomLeftFront.Z);

            Vector2 textureTopLeft = Tex.TopLeftPercent;
            Vector2 textureTopRight = new Vector2(Tex.BottomRightPercent.X, Tex.TopLeftPercent.Y);
            Vector2 textureBottomLeft = new Vector2(Tex.TopLeftPercent.X, Tex.BottomRightPercent.Y);
            Vector2 textureBottomRight = Tex.BottomRightPercent;

            Vector3 frontNormal = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 backNormal = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 topNormal = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 bottomNormal = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 leftNormal = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f);

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[24];
            short[] indices = new short[36];

            vertices[0] =
                new VertexPositionNormalTexture(
                topLeftFront, frontNormal, textureTopLeft);
            vertices[1] =
                new VertexPositionNormalTexture(
                topRightFront, frontNormal, textureTopRight);
            vertices[2] =
                new VertexPositionNormalTexture(
                bottomLeftFront, frontNormal, textureBottomLeft);
            vertices[3] =
                new VertexPositionNormalTexture(
                bottomRightFront, frontNormal, textureBottomRight);

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 2;
            indices[4] = 1;
            indices[5] = 3;

            vertices[4] =
                new VertexPositionNormalTexture(
                topRightBack, backNormal, textureTopLeft);
            vertices[5] =
                new VertexPositionNormalTexture(
                topLeftBack, backNormal, textureTopRight);
            vertices[6] =
                new VertexPositionNormalTexture(
                bottomRightBack, backNormal, textureBottomLeft);
            vertices[7] =
                new VertexPositionNormalTexture(
                bottomLeftBack, backNormal, textureBottomRight);

            indices[6] = 4;
            indices[7] = 5;
            indices[8] = 6;
            indices[9] = 6;
            indices[10] = 5;
            indices[11] = 7;

            vertices[8] =
                new VertexPositionNormalTexture(
                topLeftBack, leftNormal, textureTopLeft);
            vertices[9] =
                new VertexPositionNormalTexture(
                topLeftFront, leftNormal, textureTopRight);
            vertices[10] =
                new VertexPositionNormalTexture(
                bottomLeftBack, leftNormal, textureBottomLeft);
            vertices[11] =
                new VertexPositionNormalTexture(
                bottomLeftFront, leftNormal, textureBottomRight);

            indices[12] = 8;
            indices[13] = 9;
            indices[14] = 10;
            indices[15] = 10;
            indices[16] = 9;
            indices[17] = 11;

            vertices[12] =
                new VertexPositionNormalTexture(
                topRightFront, rightNormal, textureTopLeft);
            vertices[13] =
                new VertexPositionNormalTexture(
                topRightBack, rightNormal, textureTopRight);
            vertices[14] =
                new VertexPositionNormalTexture(
                bottomRightFront, rightNormal, textureBottomLeft);
            vertices[15] =
                new VertexPositionNormalTexture(
                bottomRightBack, rightNormal, textureBottomRight);

            indices[18] = 12;
            indices[19] = 13;
            indices[20] = 14;
            indices[21] = 14;
            indices[22] = 13;
            indices[23] = 15;

            vertices[16] =
                new VertexPositionNormalTexture(
                topLeftBack, topNormal, textureTopLeft);
            vertices[17] =
                new VertexPositionNormalTexture(
                topRightBack, topNormal, textureTopRight);
            vertices[18] =
                new VertexPositionNormalTexture(
                topLeftFront, topNormal, textureBottomLeft);
            vertices[19] =
                new VertexPositionNormalTexture(
                topRightFront, topNormal, textureBottomRight);

            indices[24] = 16;
            indices[25] = 17;
            indices[26] = 18;
            indices[27] = 18;
            indices[28] = 17;
            indices[29] = 19;

            vertices[20] =
                new VertexPositionNormalTexture(
                bottomLeftFront, bottomNormal, textureTopLeft);
            vertices[21] =
                new VertexPositionNormalTexture(
                bottomRightFront, bottomNormal, textureTopRight);
            vertices[22] =
                new VertexPositionNormalTexture(
                bottomLeftBack, bottomNormal, textureBottomLeft);
            vertices[23] =
                new VertexPositionNormalTexture(
                bottomRightBack, bottomNormal, textureBottomRight);

            indices[30] = 20;
            indices[31] = 21;
            indices[32] = 22;
            indices[33] = 22;
            indices[34] = 21;
            indices[35] = 23;

            return new VertexIndexData(vertices, indices);
        }
    }
}
