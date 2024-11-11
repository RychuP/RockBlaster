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

namespace RockBlaster.Entities;

public partial class Star
{
    Instruction _twinkleInstruction;

    /// <summary>
    /// Initialization logic which is executed only one time for this Entity (unless the Entity is pooled).
    /// This method is called when the Entity is added to managers. Entities which are instantiated but not
    /// added to managers will not have this method called.
    /// </summary>
    private void CustomInitialize()
    {
        Twinkle();
    }

    private void CustomActivity()
    {
        if (TimeManager.CurrentTime > _twinkleInstruction.TimeToExecute + 1)
            Twinkle();
    }

    private void CustomDestroy()
    {
        
    }

    private static void CustomLoadStaticContent(string contentManagerName)
    {
        
    }

    void Twinkle()
    {
        var state = CurrentStarSizeState == StarSize.HalfSize ?
                StarSize.FullSize : StarSize.HalfSize;
        double time = FlatRedBallServices.Random.NextDouble() * 2 + 1;
        _twinkleInstruction = InterpolateToState(state, time);
    }
}