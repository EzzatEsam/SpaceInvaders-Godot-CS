using Godot;
using System;

public class Weapon : Node2D
{
    [Export]
    PackedScene WeaponBullet;
    [Export]
    private float AttackSpeed;
    [Export]
    NodePath TimerPath;
    private Timer _timer;
    private bool CanFire = false;
    private AudioStreamPlayer2D streamPlayer2D;

    
    public override void _Ready()
    {
        _timer = GetNode<Timer>(TimerPath);
        streamPlayer2D = GetNode<AudioStreamPlayer2D>("FireSound");
        _timer.OneShot = true;
        _timer.WaitTime = 1/AttackSpeed;
        CanFire = true;
        _timer.Connect("timeout", this, nameof(ReEnableAttack));

    }

    private void ReEnableAttack()
    {
        CanFire = true;
    }

    public bool Fire(Vector2 dir)
    {
        if (CanFire)
        {
            
            _timer.Start();
            CanFire = false;
            var blt = WeaponBullet.Instance() as Bullet;
            blt.GetParent()?.RemoveChild(blt);
            GetTree().Root.AddChild(blt);
            blt.Velocity = dir * blt.Speed;
            blt.GlobalPosition = GlobalPosition;
            blt.GlobalRotation = dir.Angle() + Mathf.Pi/2;
            if (streamPlayer2D.Stream != null)
            {
                streamPlayer2D.Play();
            }
            return true;
        }
        return false;

    }


}
