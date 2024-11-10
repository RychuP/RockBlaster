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
using RockBlaster.Screens;
using System.Runtime.CompilerServices;
using FlatRedBall.Screens;
using System.Linq;
using static RockBlaster.Entities.Star;

namespace RockBlaster.Entities
{
    public partial class StarSpawner
    {
        /// <summary>
        /// Initialization logic which is executed only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
        {
            PerformSpawn();
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

        void PerformSpawn()
        {
            for (int i = 0; i < StarCountToSpawn; i++)
            {
                var pos = GetRandomStarPosition();
                if (pos == Vector3.Zero) continue;
                var star = Factories.StarFactory.CreateNew();
                var roll = FlatRedBallServices.Random.Next(5);
                star.CurrentStarTypeState = roll switch
                {
                    0 => StarType.Blue,
                    1 => StarType.Pink,
                    2 => StarType.Green,
                    3 => StarType.LargeCross,
                    //4 => StarType.SmallCross,
                    _ => StarType.White
                };
                star.Position = pos;
            }
        }

        static Vector3 GetRandomStarPosition()
        {
            var starBounds = SpriteManager.ManagedPositionedObjects.Where(o => o is Star)
                .Select(o => (o as Star).CircleInstance).ToList();

            for (int i = 0; i < 100; i++)
            {
                var y = FlatRedBallServices.Random.NextSingle() * 550 - 275;
                var x = FlatRedBallServices.Random.NextSingle() * 750 - 375;
                var pos = new Vector3(x, y, 0);
                
                var result = starBounds.Find(r => r.IsPointInside(ref pos));

                if (result is not null) continue;
                else return pos;
            }

            return Vector3.Zero;
        }
    }
}