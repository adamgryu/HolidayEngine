using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HolidayEngine.Level.Objects
{
    public class CubeManager
    {
        public List<Cube> CubeList = new List<Cube>();
        List<Cube> CubeRemoveBuffer = new List<Cube>();
        List<Cube> CubeAddBuffer = new List<Cube>();

        public CubeManager()
        {
            // Do nothing.
        }

        public void Update(Engine engine)
        {
            foreach (Cube Cube in CubeList)
            {
                Cube.Update(engine);
            }

            // Updates the remove list buffer.
            foreach (Cube Cube in CubeRemoveBuffer)
            {
                CubeList.Remove(Cube);
            }
            CubeRemoveBuffer.Clear();

            // Updates the add list buffer.
            foreach (Cube Cube in CubeAddBuffer)
            {
                CubeList.Add(Cube);
            }
            CubeAddBuffer.Clear();
        }

        public void Draw(Engine engine)
        {
            foreach (Cube Cube in CubeList)
            {
                Cube.Draw(engine);
            }
        }

        public void AddCube(Cube Cube)
        {
            CubeAddBuffer.Add(Cube);
        }


        public void RemoveCube(Cube Cube)
        {
            CubeRemoveBuffer.Add(Cube);
        }

        public List<Cube> AllCubes
        {
            get {
                List<Cube> _cube = new List<Cube>();
                _cube.AddRange(CubeList);
                _cube.AddRange(CubeAddBuffer);
                return _cube;
            }
        }
    }
}
