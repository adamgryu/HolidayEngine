using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HolidayEngine.Level.Objects
{
    public enum CubeMode
    {
        Normal,
        Moving
    }

    public class WorldCube : Cube
    {
        /// <summary>
        /// The sets the draw position as well.
        /// </summary>
        public override Vector3 AllPosition
        {
            set { base.GridPosition = value; DrawPosition = value * Block.Size; }
        }

        /// <summary>
        /// The drawing position of the cube.
        /// To be updated by the real position.
        /// </summary>
        public Vector3 DrawPosition;

        /// <summary>
        /// The number of steps it takes in an animation.
        /// </summary>
        public short Speed = 16;

        /// <summary>
        /// The direction the block is moving during an animation.
        /// </summary>
        public Vector3 Direction = Vector3.Zero;

        /// <summary>
        /// The number of steps left in the current animation.
        /// </summary>
        public short AnimationStep = 0;

        /// <summary>
        /// The animation mode the cube is in.
        /// </summary>
        public CubeMode Mode = CubeMode.Normal;


        /// <summary>
        /// A property of the block:
        /// Does it respond to ice?
        /// </summary>
        public bool UsesIce = false;

        /// <summary>
        /// A property of the block:
        /// Does it respond to ladders?
        /// </summary>
        public bool UsesLadders = false;

        /// <summary>
        /// Holds ladder information.
        /// If it is the zero vector, no ladder is being used.
        /// Otherwise, it represents the normal from the ladder face.
        /// </summary>
        public Vector3 LadderVector = Vector3.Zero;

        /// <summary>
        /// A property of the block:
        /// Does gravity apply to it?
        /// </summary>
        public bool UsesGravity = true;


        /// <summary>
        /// Any vectors in this list will be executed on
        /// a new step taken.
        /// </summary>
        public Queue<Vector3> MovementChain = new Queue<Vector3>();

        /// <summary>
        /// Any new animations to be played with the movements in the queue.
        /// </summary>
        public Queue<int> AnimationChain = new Queue<int>();



        /// <summary>
        /// The constructor for this moving cube.
        /// </summary>
        /// <param name="Position">The grid position to start at.</param>
        public WorldCube(Vector3 Position)
            : base(Position)
        {
            DrawPosition = Position * Block.Size;
        }


        /// <summary>
        /// The script this class runs every new step that is taken.
        /// </summary>
        public virtual void NewStep(Engine engine)
        {
            // Checks to see if any moves are enquened.
            if (MovementChain.Count > 0)
            {
                if (AnimationChain.Count > 0)
                    SetAnimation(AnimationChain.Dequeue());

                if (!Move(engine, GridPosition + MovementChain.Dequeue()))
                    // If the move fails, it moves right on to the next move.
                    NewStep(engine);
            }

            // Otherwise, checks if the player needs to fall.
            else
            {
                LadderVector = Vector3.Zero;

                // Allows the block to use ladders or fall if it uses gravity.
                if (UsesGravity)
                {
                    // Only bothers to check if the fall will be sucessful.
                    if (Cube.IsMoveSafe(engine, GridPosition - Vector3.UnitZ))
                    {
                        // Checks to see if the cube can grab a ladder.
                        if (UsesLadders)
                        {
                            Block _b = engine.room.GetGridBlockSafe(GridPosition + Vector3.UnitZ);
                            if (_b != null)
                                if (_b.SideProperty[4] == 1)
                                    LadderVector = -Vector3.UnitZ;

                            _b = engine.room.GetGridBlockSafe(GridPosition + Vector3.UnitY);
                            if (_b != null)
                                if (_b.SideProperty[0] == 1)
                                    LadderVector = -Vector3.UnitY;

                            _b = engine.room.GetGridBlockSafe(GridPosition - Vector3.UnitY);
                            if (_b != null)
                                if (_b.SideProperty[2] == 1)
                                    LadderVector = Vector3.UnitY;

                            _b = engine.room.GetGridBlockSafe(GridPosition + Vector3.UnitX);
                            if (_b != null)
                                if (_b.SideProperty[3] == 1)
                                    LadderVector = -Vector3.UnitX;

                            _b = engine.room.GetGridBlockSafe(GridPosition - Vector3.UnitX);
                            if (_b != null)
                                if (_b.SideProperty[1] == 1)
                                    LadderVector = Vector3.UnitX;
                        }

                        // Lets the cube fall.
                        if (LadderVector == Vector3.Zero)
                        {
                            Move(engine, GridPosition - Vector3.UnitZ);
                            // We know this will be successful, so we use the falling animation.
                            Falling();
                        }
                    }
                }

                // Checks to see if the cube will slip on ice.
                if (UsesIce && LadderVector == Vector3.Zero && AnimationStep == 0)
                {
                    Block _bl = engine.room.GetGridBlockSafe(GridPosition - Vector3.UnitZ);
                    if (_bl != null)
                        if (_bl.SideProperty[5] == 2)
                        {
                            Vector3 _dir = Vector3.Normalize(Direction);
                            if (Move(engine, GridPosition + _dir))
                                // If the move is successful, use the falling animation.
                                Sliding();
                        }
                }
            }
        }


        /// <summary>
        /// Allows the child cube to have animations for falling.
        /// </summary>
        public virtual void SetAnimation(int AnimationNumber)
        {

        }


        /// <summary>
        /// Allows the child cube to have animations for falling.
        /// </summary>
        public virtual void Falling()
        {

        }


        /// <summary>
        /// Allows the child cube to have animations for sliding.
        /// </summary>
        public virtual void Sliding()
        {

        }


        /// <summary>
        /// Allows a collision event to occur.
        /// </summary>
        public virtual void FoundCollision(Engine engine, Cube collision, Vector3 direction)
        {

        }


        /// <summary>
        /// Moves the cube to the place vector.
        /// The vector should be a grid vector.
        /// Returns if the move was sucessful.
        /// </summary>
        public bool Move(Engine engine, Vector3 place)
        {
            if (place != GridPosition)
                if (Cube.IsMoveBlockSafe(engine, place))
                {
                    Cube _placeCube = Cube.IsMoveCubeSafe(engine, place);
                    if (_placeCube == null)
                    {
                        Mode = CubeMode.Moving;
                        Direction = (place * Block.Size - DrawPosition) / Speed;
                        AnimationStep = Speed;
                        GridPosition = place;
                        return true;
                    }
                    else
                    {
                        FoundCollision(engine, _placeCube, Vector3.Normalize(place * Block.Size - DrawPosition));
                        return false;
                    }
                }
                else
                    return false;
            return true;
        }


        /// <summary>
        /// The update method for this block.
        /// </summary>
        public override void Update(Engine engine)
        {
            if (Mode == CubeMode.Moving)
            {
                if (AnimationStep > 0)
                {
                    DrawPosition += Direction;
                }
                else
                {
                    Mode = CubeMode.Normal;
                    DrawPosition = GridPosition * Block.Size;
                    NewStep(engine);
                }

                AnimationStep--;
            }
        }
    }
}
