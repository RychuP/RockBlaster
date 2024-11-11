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
using Microsoft.Xna.Framework.Input;

namespace RockBlaster.Entities;

public partial class Player
{
    public event EventHandler<HealthEventArgs> HealthChanged;

    // two offsets used to detect when player ship goes off screen
    // and needs to be repositioned on the opposite side
    float _offsetX;
    float _offsetY;

    // bullet delay
    double _lastShotTime;

    // keep on shooting when true
    bool _autoFire;

    bool _isStopped = true;

    public I1DInput TurningInput { get; set; }
    
    public IPressableInput ShootingInput { get; set; }

    public bool NeedsHealing { get => Health < StartingHealth; }

    int _health;
    public int Health
    {
        get => _health;
        set
        {
            if (_health == value) return;
            var prevHealth = _health;
            _health = value;
            OnHealthChanged(prevHealth, _health);
        }
    }

    /// <summary>
    /// Initialization logic which is executed only one time for this Entity (unless the Entity is pooled).
    /// This method is called when the Entity is added to managers. Entities which are instantiated but not
    /// added to managers will not have this method called.
    /// </summary>
    private void CustomInitialize()
    {
        AssignInput();
        CalculateRepositionOffsets();
    }

    private void CustomActivity()
    {
        MovementActivity();
        ShootingActivity();
    }

    private void CustomDestroy()
    {
        
    }

    private static void CustomLoadStaticContent(string contentManagerName)
    {
        
    }

    void AssignInput()
    {
        TurningInput = InputManager.Keyboard.Get1DInput(Keys.Left, Keys.Right);
        ShootingInput = InputManager.Keyboard.GetKey(Keys.Space);
    }

    void CalculateRepositionOffsets()
    {
        _offsetX = Camera.Main.OrthogonalWidth / 2;
        _offsetY = Camera.Main.OrthogonalHeight / 2;
    }

    void MovementActivity()
    {
        // Negative value is needed so that holding "left" turns to the left
        RotationZVelocity = -TurningInput.Value * TurningSpeed;

        if (!_isStopped)
            Acceleration = RotationMatrix.Up * MovementSpeed;

        // reposition the ship when off bounds
        if (X < -_offsetX - CircleInstanceRadius || X > _offsetX + CircleInstanceRadius) X = -X;
        if (Y < -_offsetY - CircleInstanceRadius || Y > _offsetY + CircleInstanceRadius) Y = -Y;
    }

    void ShootingActivity()
    {
        if (ShootingInput.WasJustPressed)
            _autoFire = !_autoFire;

        if (!_autoFire || TimeManager.CurrentScreenSecondsSince(_lastShotTime) < TimeBetweenShots)
            return;

        FireBullet(RotationMatrix.Right * 6);
        FireBullet(-RotationMatrix.Right * 6);

        BulletSound.Play();
        _lastShotTime = TimeManager.CurrentScreenTime;
    }

    void FireBullet(Vector3 offset)
    {
        Bullet bullet = Factories.BulletFactory.CreateNew();
        bullet.Position = Position;
        bullet.Position += RotationMatrix.Up * 12;
        bullet.Position += offset;
        bullet.RotationZ = RotationZ;
        bullet.Velocity = RotationMatrix.Up * bullet.MovementSpeed;
    }

    public void Heal()
    {
        Health++;
    }

    public void StartFlying()
    {
        _isStopped = false;
    }

    public void Stop()
    {
        _autoFire = false;
        _isStopped = true;
    }

    void OnHealthChanged(int prevHealth, int newHealth)
    {
        if (newHealth < prevHealth)
        {
            HitSound.Play();
            if (_health <= 0)
                Destroy();
        }
        else if (newHealth == prevHealth + 1) 
        {
            HealSound.Play();
        }

        var args = new HealthEventArgs()
        {
            PrevHealth = prevHealth,
            NewHealth = newHealth,
        };
        HealthChanged?.Invoke(this, args);
    }
}

public class HealthEventArgs : EventArgs
{
    public int PrevHealth { get; init; }
    public int NewHealth { get; init; }
}