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
using FlatRedBall.Gui;
using Microsoft.Xna.Framework.Input;

namespace RockBlaster.Screens;

public partial class GameScreen
{
    void RegisterEventHandlers()
    {
        // entity value changed events
        Player1.HealthChanged += Player_OnHealthChanged;
        RockSpawner.DifficultyChanged += RockSpawner_OnDifficultyChanged;

        // difficulty window events
        GumScreen.DifficultySlider.ThumbInstance.PositionChanged += DifficultySlider_ValueChanged;
        GumScreen.DifficultyDescriptionContainer.Click += DifficultyDescriptionContainer_OnClick;
        GumScreen.DifficultyDescriptionContainer.RollOn += (w) => SetCursorToHand();
        GumScreen.DifficultyDescriptionContainer.RollOff += (w) => SetCursorToArrow();

        // healing window events
        GumScreen.HealingSlider.ThumbInstance.PositionChanged += HealingSlider_ValueChanged;
        GumScreen.RepairDescriptionContainer.Click += HealingDescription_OnClick;
        GumScreen.RepairDescriptionContainer.RollOn += (w) => SetCursorToHand();
        GumScreen.RepairDescriptionContainer.RollOff += (w) => SetCursorToArrow();

        // tryagain button events
        GumScreen.TryAgainButton.Click += TryAgainButton_OnClick;
        GumScreen.TryAgainButton.RollOn += (w) => SetCursorToHand();
        GumScreen.TryAgainButton.RollOff += (w) => SetCursorToArrow();

        // start button events
        GumScreen.StartButton.Click += StartButton_OnClick;
        GumScreen.StartButton.RollOn += (w) => SetCursorToHand();
        GumScreen.StartButton.RollOff += (w) => SetCursorToArrow();
    }

    void OnBulletVsRockCollided (Bullet bullet, Rock rock) 
    {
        bullet.Destroy();
        rock.TakeHit();
        Score += rock.PointValue;
        HealingScore -= rock.PointValue;
        RocksDestroyed++;
    }
    static void OnPlayerVsRockCollided (Player player, Rock rock) 
    {
        if (player.IsDamageReceivingEnabled)
        {
            player.Health--;
            rock.TakeHit();
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
            CalculateNewHealingScore();
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

    void HealingSlider_ValueChanged(object o, EventArgs e)
    {
        var range = MaxBaseHealingRate - MinBaseHealingRate;
        var percent = GumScreen.HealingSlider.SliderPercent / 100;
        var value = percent * range;
        BaseHealingScore = MinBaseHealingRate + (int)value;
        CalculateNewHealingScore();
        UpdateHealingValueText();
    }

    void DifficultySlider_ValueChanged(object o, EventArgs e)
    {
        var range = RockSpawner.MaxDifficulty - RockSpawner.MinDifficulty;
        var percent = GumScreen.DifficultySlider.SliderPercent / 100;
        var value = RockSpawner.MinDifficulty + percent * range;
        RockSpawner.SetInitialDifficulty(value);
        CalculateNewHealingScore();
        UpdateDifficultyValueText();
    }

    void DifficultyDescriptionContainer_OnClick(IWindow window)
    {
        GumScreen.DifficultySliderPercent = 0;
    }

    void HealingDescription_OnClick(IWindow window)
    {
        GumScreen.HealingSliderPercent = 50;
    }

    void RockSpawner_OnDifficultyChanged(object o, DifficultyEventArgs e)
    {
        UpdateDifficultyInfo();
    }
}