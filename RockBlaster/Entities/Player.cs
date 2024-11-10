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
    // two offsets used to detect when player ship goes off screen
    // and needs to be repositioned on the opposite side
    float _offsetX;
    float _offsetY;

    // bullet delay
    double _lastShotTime;

    public I1DInput TurningInput { get; set; }
    
    public IPressableInput ShootingInput { get; set; }

    int _health;
    public int Health
    {
        get => _health;
        set
        {
            if (_health == value) return;
            _health = value;
            OnHealthChanged();
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
        AddHealthBar();
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

    void AddHealthBar()
    {
        Health = StartingHealth;
        //var hudParent = gumAttachmentWrappers[0];
        //hudParent.ParentRotationChangesRotation = false;
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
        Acceleration = RotationMatrix.Up * MovementSpeed;

        // reposition the ship when off bounds
        if (X < -_offsetX - CircleInstanceRadius || X > _offsetX + CircleInstanceRadius) X = -X;
        if (Y < -_offsetY - CircleInstanceRadius || Y > _offsetY + CircleInstanceRadius) Y = -Y;
    }

    void ShootingActivity()
    {
        if (!ShootingInput.IsDown 
            || TimeManager.CurrentScreenSecondsSince(_lastShotTime) < TimeBetweenShots)
            return;

        // We'll create 2 bullets because it looks much cooler than 1
        Bullet firstBullet = Factories.BulletFactory.CreateNew();
        firstBullet.Position = Position;
        firstBullet.Position += RotationMatrix.Up * 12;
        // This is the bullet on the right side when the ship is facing up.
        // Adding along the Right vector will move it to the right relative to the ship
        firstBullet.Position += RotationMatrix.Right * 6;
        firstBullet.RotationZ = RotationZ;
        firstBullet.Velocity = RotationMatrix.Up * firstBullet.MovementSpeed;

        Bullet secondBullet = Factories.BulletFactory.CreateNew();
        secondBullet.Position = Position;
        secondBullet.Position += RotationMatrix.Up * 12;
        // This bullet is moved along the Right vector, but in the nevative
        // direction, making it the bullet on the left.
        secondBullet.Position -= RotationMatrix.Right * 6;
        secondBullet.RotationZ = RotationZ;
        secondBullet.Velocity = RotationMatrix.Up * secondBullet.MovementSpeed;

        BulletSound.Play();
        _lastShotTime = TimeManager.CurrentScreenTime;
    }

    void OnHealthChanged()
    {
        // Multiply the value by 100 so that full health is 100%
        HealthBarRuntimeInstance.PercentFull = 100 * Health / (float)StartingHealth;
        

        if (Health < StartingHealth)
        {
            HitSound.Play();
            if (_health <= 0)
                Destroy();
        }
    }
}