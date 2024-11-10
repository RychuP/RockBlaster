using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using Microsoft.Xna.Framework;

namespace RockBlaster.Entities
{
    public partial class Rock
    {
        /// <summary>
        /// Initialization logic which is executed only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
        {
            
        }

        private void CustomActivity()
        {
            
        }

        private void CustomDestroy()
        {
            
        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {
            
        }

        public void TakeHit()
        {
            if (CurrentRockSizeState == RockSize.Size4)
            {
                BreakIntoPieces(RockSize.Size3);
            }
            else if (CurrentRockSizeState == RockSize.Size3)
            {
                BreakIntoPieces(RockSize.Size2);
            }
            //else if (CurrentRockSizeState == RockSize.Size2)
            //{
            //    BreakIntoPieces(RockSize.Size1);
            //}
            // don't break into pieces if at size 1

            SmashSound.Play();
            Destroy();
        }

        void BreakIntoPieces(RockSize newRockState)
        {
            for (int i = 0; i < NumberOfRocksToBreakInto; i++)
            {
                Rock newRock = Factories.RockFactory.CreateNew();

                // Let's make the positions random so that they appear in a random arrangement
                newRock.Position = Position;
                newRock.Position.X += -1 + 2 * FlatRedBallServices.Random.NextSingle();
                newRock.Position.Y += -1 + 2 * FlatRedBallServices.Random.NextSingle();

                float randomAngle = FlatRedBallServices.Random.NextSingle() * (float)Math.PI * 2;

                float speed = FlatRedBallServices.Random.NextSingle() * RandomSpeedOnBreak;
                newRock.Velocity = FlatRedBall.Math.MathFunctions.AngleToVector(randomAngle) * speed;
                newRock.CurrentRockSizeState = newRockState;
            }
        }
    }
}
