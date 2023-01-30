using Godot;
using System;

public class Bullet : Area2D
{
    [Export]
    public int Damage = 50;
    [Export]
    PackedScene ExplosionEffect;
    [Export]
    private float DeathAfter = 3.5f;
    [Export]
    bool IsPermenmenant = false;

    private Timer deathTimer;
    [Export]
    public float Speed = 400;
    public Vector2 Velocity;
    public override void _Ready()
    {
        deathTimer = GetNode<Timer>("Timer");
        if (!IsPermenmenant)
        {
            deathTimer.Start(DeathAfter);
            deathTimer.Connect("timeout", this, nameof(Die));
        }

        Monitoring = true;
        Connect("body_entered", this, nameof(OnHitObject));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    private void Die()
    {
        QueueFree();
    }



    private void OnHitObject(PhysicsBody2D body)
    {

        if (body != null)
        {
            if (body is IDestructible)
            {
                (body as IDestructible).healthSystem.TakeDamage(Damage);
                if (ExplosionEffect != null)
                {
                    var exp = ExplosionEffect.Instance() as Node2D;
                    GetTree().Root.AddChild(exp);
                    exp.GlobalPosition = GlobalPosition;
                    exp.GlobalRotation = GlobalRotation;
                }
                QueueFree();
            }
            
        }
    }



    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        Position += Velocity * delta;
    }
}
