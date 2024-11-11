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
using RockBlaster.Entities;

namespace RockBlaster.Screens;

public partial class GameScreen
{
    IPressableInput EnterButton;
    IPressableInput EscapeButton;
    IPressableInput PauseButton;

    float Difficulty => RockSpawner.RocksPerSecond;

    int _rocksDestroyed = 0;
    int RocksDestroyed
    {
        get => _rocksDestroyed;
        set
        {
            if (_rocksDestroyed == value) return;
            if (value < _rocksDestroyed && value != 0)
                throw new ArgumentException("Reducing counter is not allowed (unless 0).");
            _rocksDestroyed = value;
            OnRocksDestroyedChanged();
        }
    }

    int _healingScore = 0;
    int HealingScore
    {
        get => _healingScore;
        set
        {
            if (_healingScore == value) return;
            _healingScore = value;
            OnHealingScoreChanged();
        }
    }

    int _score = 0;
    int Score
    {
        get => _score;
        set
        {
            if (_score == value) return;
            if (value < _score && value != 0)
                throw new ArgumentException("Reducing score is not allowed (unless 0).");
            _score = value;
            OnScoreChanged();
        }
    }

    private void CustomInitialize()
    {
        AssignInput();
        RegisterEventHandlers();
        SetStartValues();
        ShowRockValuesInStartMenu();
        UpdateHealingValueText();
        StartMenu.Visible = true;
    }

    private void CustomActivity(bool firstTimeCalled)
    {
        UpdateDifficultyInfo();
        RemovalActivity();
        StartGameActivity();
        GameOverActivity();
        EscapeButtonActivity();
        PauseButtonActivity();
    }

    private void CustomDestroy()
    {
        
    }

    private static void CustomLoadStaticContent(string contentManagerName)
    {
        
    }

    /// <summary>
    /// Actions to be performed when restarting game from the GameOver menu
    /// </summary>
    void RestartGame()
    {
        GameOverHud.Visible = false;
        CreateNewPlayer();
        Player1.StartFlying();
        RockSpawner.Start();
        SetStartValues();
    }

    void CreateNewPlayer()
    {
        Player1 = Factories.PlayerFactory.CreateNew();
        Player1.HealthChanged += Player_OnHealthChanged;
    }

    /// <summary>
    /// Actions to be performed when starting game from the Start menu
    /// </summary>
    void StartGame()
    {
        StartMenu.Visible = false;
        Player1.StartFlying();
        RockSpawner.Start();
    }

    void UpdateDifficultyInfo()
    {
        GumScreen.DifficultyInfo.ScoreText = $"{Difficulty:0.00}";
    }

    void AssignInput()
    {
        EnterButton = InputManager.Keyboard.GetKey(Keys.Enter);
        EscapeButton = InputManager.Keyboard.GetKey(Keys.Escape);
        PauseButton = InputManager.Keyboard.GetKey(Keys.P);
    }

    void EscapeButtonActivity()
    {
        if (!EscapeButton.WasJustPressed) return;

        if (StartMenu.Visible || GameOverHud.Visible)
        {
            FlatRedBallServices.Game.Exit();
        }
        // stop the game and show start menu
        else
        {
            StopGame();
            Player1.HealthChanged -= Player_OnHealthChanged;
            Player1.Destroy();
            CreateNewPlayer();
            SetStartValues();
            UpdateDifficultyInfo();
            StartMenu.Visible = true;
        }
    }

    void PauseButtonActivity()
    {
        if (!PauseButton.WasJustPressed || StartMenu.Visible || GameOverHud.Visible) return;

        if (!IsPaused)
        {
            PauseThisScreen();
        }
        else if (IsPaused)
        {
            UnpauseThisScreen();
        }
    }

    void UpdateHealingValueText()
    {
        GumScreen.HealingValueText = BaseHealingScore.ToString();
    }

    void SetStartValues()
    {
        GumScreen.HealthPercent = 100;
        HealingScore = Convert.ToInt32(BaseHealingScore * Difficulty);
        Player1.Health = Player1.StartingHealth;
        Player1.Y = Player1.StartY;
        RocksDestroyed = 0;
        Score = 0;
    }

    void ShowRockValuesInStartMenu()
    {
        GumScreen.SmallRockValueText = Rock.RockSize.Size2.PointValue.ToString();
        GumScreen.MediumRockValueText = Rock.RockSize.Size3.PointValue.ToString();
        GumScreen.LargeRockValueText = Rock.RockSize.Size4.PointValue.ToString();
    }

    void RegisterEventHandlers()
    {
        Player1.HealthChanged += Player_OnHealthChanged;
        GumScreen.TryAgainButton.Click += TryAgainButton_OnClick;
        GumScreen.StartButton.Click += StartButton_OnClick;
        GumScreen.HealingSlider.ThumbInstance.PositionChanged += HealingSlider_OnSliderPercentChanged;
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

    void DestroyRocksAndBullets()
    {
        while (RockList.Count > 0)
            RockList[0].Destroy();

        while (BulletList.Count > 0)
            BulletList[0].Destroy();
    }

    void StopGame()
    {
        DestroyRocksAndBullets();
        RockSpawner.Stop();
    }

    void GameOverActivity()
    {
        if (GameOverHud.Visible == false)
        {
            // check if a player is alive
            if (PlayerList.Count != 0) return;

            GameOverHud.Visible = true;
            StopGame();
        }
        else if (GameOverHud.Visible == true && EnterButton.WasJustPressed)
        {
            RestartGame();
        }
    }

    void StartGameActivity()
    {
        if (StartMenu.Visible == true)
        {
            if (EnterButton.WasJustPressed)
                StartGame();
        }
    }

    void OnRocksDestroyedChanged()
    {
        GumScreen.RockCounter.ScoreText = RocksDestroyed.ToString();
    }

    void OnHealingScoreChanged()
    {
        if (HealingScore <= 0)
        {
            HealingScore = Convert.ToInt32(BaseHealingScore * Difficulty);
            if (Player1.NeedsHealing)
                Player1.Heal();
        }
        GumScreen.HealCountdown.ScoreText = HealingScore.ToString();
    }

    void OnScoreChanged()
    {
        GumScreen.ScoreCounter.ScoreText = Score.ToString();
    }

    void TryAgainButton_OnClick(IWindow window)
    {
        RestartGame();
    }

    void StartButton_OnClick(IWindow window)
    {
        StartGame();
    }

    void Player_OnHealthChanged(object o, HealthEventArgs e)
    {
        if (o is not Player player) return;
        var percent = 100 * e.NewHealth / (float)player.StartingHealth;
        GumScreen.HealthPercent = percent;
        GumScreen.HealthValue = e.NewHealth.ToString();
        GumScreen.HealthValueColor = percent < 42 ?
            GumRuntimes.TextRuntime.ColorCategory.Black :
            GumRuntimes.TextRuntime.ColorCategory.White;
        if (e.NewHealth <= 0)
            player.HealthChanged -= Player_OnHealthChanged;
    }

    void HealingSlider_OnSliderPercentChanged(object o, EventArgs e)
    {
        var range = MaxBaseHealingRate - MinBaseHealingRate;
        var percent = GumScreen.HealingSlider.SliderPercent / 100;
        var value = percent * range;
        BaseHealingScore = MinBaseHealingRate + (int)value;
        HealingScore = Convert.ToInt32(BaseHealingScore * Difficulty);
        UpdateHealingValueText();
    }
}