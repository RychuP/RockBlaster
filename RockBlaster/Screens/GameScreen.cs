using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Gui;
using FlatRedBall.Math;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Localization;
using Microsoft.Xna.Framework;
using RockBlaster.DataTypes;
using Microsoft.Xna.Framework.Input;

namespace RockBlaster.Screens;

public partial class GameScreen
{
    IPressableInput RestartInput;

    private void CustomInitialize()
    {
        ResetScore();
        GumScreen.TryAgainButton.Click += TryAgainButton_OnClick;
        RestartInput = InputManager.Keyboard.GetKey(Keys.Enter);
    }

    private void CustomActivity(bool firstTimeCalled)
    {
        RemovalActivity();
        EndGameActivity();
    }

    private void CustomDestroy()
    {
        
    }

    private static void CustomLoadStaticContent(string contentManagerName)
    {
        
    }

    void ResetScore()
    {
        GlobalData.PlayerData.Score = 0;
        Score.Text = "0";
    }

    void RestartGame()
    {
        GameOverHud.Visible = false;
        RockSpawner.Restart();
        GumScreen.HealthPercent = 100;
        ResetScore();
        Factories.PlayerFactory.CreateNew();
    }

    void RemovalActivity()
    {
        // reverse loop since we're going to Destroy
        for (int i = BulletList.Count - 1; i > -1; i--)
        {
            float absoluteX = Math.Abs(BulletList[i].X);
            float absoluteY = Math.Abs(BulletList[i].Y);

            const float removeBeyond = 600;
            if (absoluteX > removeBeyond || absoluteY > removeBeyond)
            {
                BulletList[i].Destroy();
            }
        }

        for (int i = RockList.Count - 1; i > -1; i--)
        {
            float absoluteX = Math.Abs(RockList[i].X);
            float absoluteY = Math.Abs(RockList[i].Y);

            const float removeBeyond = 600;
            if (absoluteX > removeBeyond || absoluteY > removeBeyond)
            {
                RockList[i].Destroy();
            }
        }
    }

    void EndGameActivity()
    {
        // If the list has 0 ships, then all have been killed
        if (GameOverHud.Visible == false)
        {
            if (PlayerList.Count != 0) return;

            GameOverHud.Visible = true;
            RockSpawner.Stop();

            while (RockList.Count > 0)
                RockList[0].Destroy();

            while (BulletList.Count > 0)
                BulletList[0].Destroy();
        }
        else
        {
            if (RestartInput.WasJustPressed)
            {
                RestartGame();
            }
        }
    }

    void TryAgainButton_OnClick(IWindow window)
    {
        RestartGame();
    }
}