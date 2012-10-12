using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace HolidayEngine.Drawing
{
    public class LineData
    {
        public List<VertexPositionColor> VertexList;
        public List<short> IndexList;

        public LineData()
        {
            this.VertexList = new List<VertexPositionColor>();
            this.IndexList = new List<short>();
        }

        public LineData(List<VertexPositionColor> VertexList, List<short> IndexList)
        {
            this.VertexList = VertexList;
            this.IndexList = IndexList;
        }

        public LineData(VertexPositionColor[] VertexList, short[] IndexList)
        {
            this.VertexList = VertexList.ToList<VertexPositionColor>();
            this.IndexList = IndexList.ToList<short>();
        }

        public VertexPositionColor[] VertexArray
        {
            get { return VertexList.ToArray(); }
            set { VertexList = value.ToList<VertexPositionColor>(); }
        }

        public short[] IndexArray
        {
            get { return IndexList.ToArray(); }
            set { IndexList = value.ToList<short>(); }
        }

        public void AddData(LineData lineData)
        {
            int _count = this.VertexList.Count;
            this.VertexList.AddRange(lineData.VertexList);
            foreach (short index in lineData.IndexList)
            {
                this.IndexList.Add((short)(index + _count));
            }
        }

        public void AddData(VertexPositionColor[] VertexList, short[] IndexList)
        {
            int _count = this.VertexList.Count;
            this.VertexList.AddRange(VertexList.ToList<VertexPositionColor>());
            foreach (short index in IndexList)
            {
                this.IndexList.Add((short)(index + _count));
            }
        }
    }
}
