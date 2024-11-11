using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using FlatRedBall.Audio;
using FlatRedBall.Screens;
using RockBlaster.Entities;
using RockBlaster.Screens;
namespace RockBlaster.Screens
{
    public partial class GameScreen
    {
        void OnBulletVsRockCollided (Bullet bullet, Rock rock) 
        {
            bullet.Destroy();
            rock.TakeHit();
            Score += rock.PointValue;
            HealingScore -= rock.PointValue;
            RocksDestroyed++;
        }
        void OnPlayerVsRockCollided (Player player, Rock rock) 
        {
            player.Health--;
            rock.TakeHit();
        }
    }
}