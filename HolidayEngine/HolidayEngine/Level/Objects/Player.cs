using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HolidayEngine.Drawing;
using HolidayEngine.Level.Objects.PlayerAI;

namespace HolidayEngine.Level.Objects
{

    public class Player : WorldCube
    {
        /// <summary>
        /// The animation set to use for this player.
        /// </summary>
        public Animation animation;

        /// <summary>
        /// The AI this player uses.
        /// </summary>
        public ParentAI AI;

        /// <summary>
        /// Uses auto control movement.
        /// </summary>
        public bool AutoControl = true;


        /// <summary>
        /// The constructor for a player.
        /// </summary>
        public Player(Vector3 Position, ParentAI AI, Animation animation)
            : base(Position)
        {
            this.AI = AI;
            this.animation = animation;

            UsesIce = true;
            UsesLadders = true;
        }


        /// <summary>
        /// The update method for this player.
        /// </summary>
        public override void Update(Engine engine)
        {
            base.Update(engine);

            if (Mode == CubeMode.Normal)
                AI.Update(engine, this);

            if (Mode == CubeMode.Moving)
                animation.Update();
        }


        public override void NewStep(Engine engine)
        {
            base.NewStep(engine);

            if (AnimationStep == 0)
            {
                // Checks to see if the player needs to grab onto a ladder.
                if (Cube.IsMoveSafe(engine, GridPosition - Vector3.UnitZ))
                {
                    if (LadderVector != Vector3.Zero)
                        if (Block.VectorToSide[LadderVector] < 4)
                            animation.SetAnimation(Block.VectorToSide[LadderVector] + 4);
                        else
                        {
                            Vector3 _dir = Vector3.Normalize(Direction);
                            if (Block.VectorToSide[-_dir] < 4)
                                animation.SetAnimation(Block.VectorToSide[-_dir] + 13);
                        }
                }

                // The player must be falling.
                else
                {
                    Vector3 _dir = Vector3.Normalize(Direction);
                    if (Block.VectorToSide[-_dir] < 4)
                        animation.SetAnimation(Block.VectorToSide[-_dir]);
                    else if (Block.VectorToSide[-_dir] == 4)
                        animation.SetAnimation(0);
                }
            }
        }

        public override void SetAnimation(int AnimationNumber)
        {
            animation.SetAnimation(AnimationNumber);
        }


        public override void Falling()
        {
            animation.SetAnimation(8);
        }

        public override void Sliding()
        {
            Vector3 _dir = Vector3.Normalize(Direction);
            if (Block.VectorToSide[-_dir] < 4)
                animation.SetAnimation(Block.VectorToSide[-_dir] + 9);
        }

        public override void FoundCollision(Engine engine, Cube collision, Vector3 direction)
        {
            collision.Collide(engine, this, direction);
        }


        /// <summary>
        /// Moves the player one unit in a specific direction.
        /// </summary>
        /// <param name="engine">The current engine state.</param>
        /// <param name="direction">A unit direction to move.</param>
        public void Walk(Engine engine, Vector3 direction)
        {
            bool _downLadder = false;
            if (AutoControl)
                if (LadderVector != Vector3.Zero)
                    if (Block.VectorToSide[LadderVector] < 4)
                        if (direction == LadderVector)
                            if (!Cube.IsMoveLadderGrab(engine, GridPosition + direction))
                                if (Move(engine, GridPosition - Vector3.UnitZ))
                                {
                                    animation.SetAnimation(Block.VectorToSide[direction] + 4);
                                    _downLadder = true;
                                }

            if (!_downLadder)
            {
                // Tries moving the player.
                if (!Move(engine, GridPosition + direction))
                {
                    // If failed, then looks for a ladder.
                    int _side = Block.VectorToSide[direction];

                    // Gets the block the player would run into.
                    Block _b = engine.room.GetGridBlockSafe(GridPosition + direction);

                    // If the block exists.
                    if (_b != null)

                        // If the side is a ladder side.
                        if (_b.SideProperty[_side] == 1)
                        {
                            // Walk up ladder.
                            if (Move(engine, GridPosition + Vector3.UnitZ))
                            {
                                animation.SetAnimation(Block.VectorToSide[-direction] + 4);
                                MovementChain.Enqueue(direction);
                                AnimationChain.Enqueue(Block.VectorToSide[-direction]);
                            }
                        }
                }
                // If moving properly, then set the animation.
                else
                {
                    if (LadderVector == Vector3.Zero)
                    {
                        animation.SetAnimation(Block.VectorToSide[-direction]);
                    }
                    else
                    {
                        int _dir = Block.VectorToSide[LadderVector];
                        if (_dir < 4)
                            animation.SetAnimation(_dir + 4);
                        else
                            animation.SetAnimation(Block.VectorToSide[-direction] + 13);
                    }
                }
            }
        }


        /// <summary>
        /// Draws the player.
        /// </summary>
        public override void Draw(Engine engine)
        {
            // Gets the bottom left vector of the billboard.
            Vector3 _pos = new Vector3(DrawPosition.X, DrawPosition.Y + Block.Size / 2, DrawPosition.Z);

            // Creates a wall.
            VertexIndexData _wall = PrimHelper.GenerateWallVertices(_pos, _pos + new Vector3(Block.Size, 0, Block.Size), -Vector3.UnitY, animation.texture);

            // Draws the wall.
            engine.primManager.DrawVertices(_wall, animation.Texture);
        }
    }
}
