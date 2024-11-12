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

    int _rocksDestroyed = 0;
    /// <summary>
    /// Number of <see cref="Rock"/> instances destroyed.
    /// </summary>
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
    /// <summary>
    /// Amount of <see cref="Score"/> points needed to increase <see cref="Player.Health"/> by 1 point.
    /// </summary>
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
    /// <summary>
    /// Current accumulation of rock points.
    /// </summary>
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
        UpdateDifficultyValueText();
        StartMenu.Visible = true;
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

    /// <summary>
    /// Actions to be performed when starting game from the Start menu
    /// </summary>
    void StartGame()
    {
        StartMenu.Visible = false;
        Player1.StartFlying();
        RockSpawner.Start();
    }

    void ExitToStartMenu()
    {
        StopGame();
        DestroyPlayer();
        CreateNewPlayer();
        SetStartValues();
        StartMenu.Visible = true;
    }

    void CreateNewPlayer()
    {
        Player1 = Factories.PlayerFactory.CreateNew();
        Player1.HealthChanged += Player_OnHealthChanged;
    }

    void DestroyPlayer()
    {
        Player1.HealthChanged -= Player_OnHealthChanged;
        Player1.Destroy();
    }

    void UpdateDifficultyInfo()
    {
        GumScreen.DifficultyInfo.ScoreText = $"{RockSpawner.Difficulty:0.00}";
    }

    void AssignInput()
    {
        EnterButton = InputManager.Keyboard.GetKey(Keys.Enter);
        EscapeButton = InputManager.Keyboard.GetKey(Keys.Escape);
        PauseButton = InputManager.Keyboard.GetKey(Keys.P);
    }

    void UpdateHealingValueText()
    {
        GumScreen.HealingValueText = BaseHealingScore.ToString();
    }

    void UpdateDifficultyValueText()
    {
        GumScreen.DifficultyValueText = $"{RockSpawner.InitialDifficulty:0.00}";
    }

    void SetStartValues()
    {
        RockSpawner.ResetDifficulty();
        CalculateNewHealingScore();
        Player1.Reset();
        RocksDestroyed = 0;
        Score = 0;
    }

    void ShowRockValuesInStartMenu()
    {
        GumScreen.SmallRockValueText = Rock.RockSize.Size2.PointValue.ToString();
        GumScreen.MediumRockValueText = Rock.RockSize.Size3.PointValue.ToString();
        GumScreen.LargeRockValueText = Rock.RockSize.Size4.PointValue.ToString();
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

    void CalculateNewHealingScore()
    {
        HealingScore = Convert.ToInt32(BaseHealingScore * RockSpawner.Difficulty);
    }

    void SetCursorToHand()
    {
        Microsoft.Xna.Framework.Input.Mouse.SetCursor(MouseCursor.Hand);
    }

    void SetCursorToArrow()
    {
        Microsoft.Xna.Framework.Input.Mouse.SetCursor(MouseCursor.Arrow);
    }
}