using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HolidayEngine.Level.Objects.PlayerAI
{
    public class PlayerAI : ParentAI
    {
        public Vector3 Ladder = Vector3.Zero;

        public PlayerAI()
        {

        }

        public override void Update(Engine engine, Player player)
        {
            if (engine.inputManager.IsKeyDown(InputManager.KeyRight))
                player.Walk(engine, Vector3.UnitX);
            else if (engine.inputManager.IsKeyDown(InputManager.KeyLeft))
                player.Walk(engine, -Vector3.UnitX);
            else if (engine.inputManager.IsKeyDown(InputManager.KeyForward))
                player.Walk(engine, Vector3.UnitY);
            else if (engine.inputManager.IsKeyDown(InputManager.KeyBackward))
                player.Walk(engine, -Vector3.UnitY);
            else if (engine.inputManager.IsKeyDown(InputManager.KeyFall))
            {
                // If auto control is on, it moves to the only spot you can't get to with the arrow keys.
                if (player.AutoControl)
                {
                    // Push off of a surface.
                    if (player.LadderVector != Vector3.Zero)
                    {
                        // If the pushed spot can be reached by ladder grab, fall instead.
                        if (Cube.IsMoveLadderGrab(engine, player.GridPosition + player.LadderVector))
                        {
                            if (player.Move(engine, player.GridPosition - Vector3.UnitZ))
                                player.Falling();
                        }
                        // If it can't be reached by a ladder grab, then jump there.
                        else
                        {
                            if (player.Move(engine, player.GridPosition + player.LadderVector))
                                if (player.LadderVector == -Vector3.UnitZ)
                                    player.Falling();
                        }
                    }
                    // If on the ground, jump!
                    else
                    {
                        if (player.Move(engine, player.GridPosition + Vector3.UnitZ))
                            player.Falling();
                    }
                }

                // Otherwise, it is just a fall/jump button.
                else
                    if (player.Move(engine, player.GridPosition - Vector3.UnitZ))
                        player.Falling();
                    else
                        if (player.Move(engine, player.GridPosition + Vector3.UnitZ))
                            player.Falling();

            }
        }
    }
}
