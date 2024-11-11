using FlatRedBall;
using System;

namespace RockBlaster.Screens;

public partial class GameScreen
{
    void CustomActivity(bool firstTimeCalled)
    {
        InputActivity();
        RemovalActivity();
        GameOverActivity();
    }

    void GameOverActivity()
    {
        if (GameOverHud.Visible == false && PlayerList.Count == 0)
        {
            GameOverHud.Visible = true;
            StopGame();
        }
    }

    void InputActivity()
    {
        EnterButtonActivity();
        EscapeButtonActivity();
        PauseButtonActivity();
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

    void EnterButtonActivity()
    {
        if (!EnterButton.WasJustPressed) return;

        if (GameOverHud.Visible == true)
        {
            RestartGame();
        }
        else if (StartMenu.Visible == true)
        {
            StartGame();
        }
    }

    void EscapeButtonActivity()
    {
        if (!EscapeButton.WasJustPressed) return;

        if (StartMenu.Visible || GameOverHud.Visible)
            FlatRedBallServices.Game.Exit();
        else
        {
            if (IsPaused)
                UnpauseThisScreen();
            ExitToStartMenu();
        }
    }

    void PauseButtonActivity()
    {
        if (!PauseButton.WasJustPressed || StartMenu.Visible || GameOverHud.Visible) return;

        if (!IsPaused)
            PauseThisScreen();
        else if (IsPaused)
            UnpauseThisScreen();
    }
}