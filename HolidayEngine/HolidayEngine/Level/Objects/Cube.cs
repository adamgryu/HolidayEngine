using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HolidayEngine.Drawing;

namespace HolidayEngine.Level.Objects
{
    public class Cube
    {
        /// <summary>
        /// The sets the draw position as well.
        /// </summary>
        public virtual Vector3 AllPosition
        {
            set { GridPosition = value; }
        }

        /// <summary>
        /// The position of the cube in the room.
        /// </summary>
        public Vector3 GridPosition;

        /// <summary>
        /// If this cube is solid or not.
        /// </summary>
        public bool Solid;

        public Cube(Vector3 Position)
        {
            this.GridPosition = Position;
            this.Solid = true;
        }

        /// <summary>
        /// Checks the room and cube objects to find if a given space is free to move to.
        /// Takes into account grid-size, cube entities, and solid blocks.
        /// </summary>
        public static bool IsMoveSafe(Engine engine, Vector3 position)
        {
            foreach (Cube _c in engine.cubeManager.CubeList)
            {
                if (_c.GridPosition == position)
                    if (_c.Solid)
                        return false;
            }

            Block _b = engine.room.GetGridBlockSafe(position);
            if (_b == null)
                return false;
            else if (_b.Solid)
                return false;

            return true;
        }


        /// <summary>
        /// Checks if a certain spot is safe blockwise.
        /// </summary>
        public static bool IsMoveBlockSafe(Engine engine, Vector3 position)
        {
            Block _b = engine.room.GetGridBlockSafe(position);
            if (_b == null)
                return false;
            else if (_b.Solid)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a spot will allow grabbing onto of a ladder.
        /// </summary>
        public static bool IsMoveLadderGrab(Engine engine, Vector3 position)
        {
            Block _b = engine.room.GetGridBlockSafe(position + Vector3.UnitZ);
            if (_b != null)
                if (_b.SideProperty[4] == 1)
                    return true;

            _b = engine.room.GetGridBlockSafe(position + Vector3.UnitY);
            if (_b != null)
                if (_b.SideProperty[0] == 1)
                    return true;

            _b = engine.room.GetGridBlockSafe(position - Vector3.UnitY);
            if (_b != null)
                if (_b.SideProperty[2] == 1)
                    return true;

            _b = engine.room.GetGridBlockSafe(position + Vector3.UnitX);
            if (_b != null)
                if (_b.SideProperty[3] == 1)
                    return true;

            _b = engine.room.GetGridBlockSafe(position - Vector3.UnitX);
            if (_b != null)
                if (_b.SideProperty[1] == 1)
                    return true;

            return false;
        }


        /// <summary>
        /// Checks if a certain spot is safe blockwise.
        /// Considers a space free if it is off the map.
        /// </summary>
        public static bool IsMoveBlockSafeIgnoreEdges(Engine engine, Vector3 position)
        {
            Block _b = engine.room.GetGridBlockSafe(position);
            if (_b == null)
                return true;
            else if (_b.Solid)
                return false;

            return true;
        }


        /// <summary>
        /// Checks if a certain spot is safe cubewise.
        /// </summary>
        public static Cube IsMoveCubeSafe(Engine engine, Vector3 position)
        {
            foreach (Cube _c in engine.cubeManager.CubeList)
            {
                if (_c.GridPosition == position)
                    if (_c.Solid)
                        return _c;
            }

            return null;
        }


        /// <summary>
        /// Runs when another cube steps on this cube.
        /// </summary>
        public virtual void Overlap(Engine engine, Cube otherCube)
        {

        }

        /// <summary>
        /// Runs when another cube tries to move onto this cube.
        /// </summary>
        public virtual void Collide(Engine engine, WorldCube otherCube, Vector3 direction)
        {
            
        }

        /// <summary>
        /// The update function for this cube.
        /// </summary>
        public virtual void Update(Engine engine)
        {

        }

        /// <summary>
        /// The draw function for this cube.
        /// </summary>
        public virtual void Draw(Engine engine)
        {

        }
    }
}
