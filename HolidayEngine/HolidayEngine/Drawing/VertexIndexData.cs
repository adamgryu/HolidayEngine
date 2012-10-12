using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HolidayEngine.Drawing
{
    public class VertexIndexData
    {
        public List<VertexPositionNormalTexture> VertexList;
        public List<short> IndexList;

        public VertexIndexData()
        {
            this.VertexList = new List<VertexPositionNormalTexture>();
            this.IndexList = new List<short>();
        }

        public VertexIndexData(List<VertexPositionNormalTexture> VertexList, List<short> IndexList)
        {
            this.VertexList = VertexList;
            this.IndexList = IndexList;
        }

        public VertexIndexData(VertexPositionNormalTexture[] VertexList, short[] IndexList)
        {
            this.VertexList = VertexList.ToList<VertexPositionNormalTexture>();
            this.IndexList = IndexList.ToList<short>();
        }

        public VertexPositionNormalTexture[] VertexArray
        {
            get { return VertexList.ToArray(); }
            set { VertexList = value.ToList<VertexPositionNormalTexture>(); }
        }

        public short[] IndexArray
        {
            get { return IndexList.ToArray(); }
            set { IndexList = value.ToList<short>(); }
        }

        public void AddData(VertexIndexData vertexIndexData)
        {
            int _count = this.VertexList.Count;
            this.VertexList.AddRange(vertexIndexData.VertexList);
            foreach (short index in vertexIndexData.IndexList)
            {
                this.IndexList.Add((short)(index + _count));
            }
        }

        public void AddData(VertexPositionNormalTexture[] VertexList, short[] IndexList)
        {
            int _count = this.VertexList.Count;
            this.VertexList.AddRange(VertexList.ToList<VertexPositionNormalTexture>());
            foreach (short index in IndexList)
            {
                this.IndexList.Add((short)(index + _count));
            }
        }
    }
}