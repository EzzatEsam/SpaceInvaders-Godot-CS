using Godot;
using System;

public class Enemy : CharacterController, IDestructible, IScoreObject
{
    [Export]
    Vector2 MinMaxTimeToFire;
    [Export]
    int Score;
    [Export]
    NodePath ExplosionPath , HitAnimationPlayerPath;
    Explosion DeathExplosion;
    public HealthSystem healthSystem { get; set; }
    public int ObjectScore { get { return Score; } set { Score = value; } }
    private Timer AttackTimer, MoveTimer;
    [Export]
    public EnemyType enemyType = EnemyType.WAVE;
    RandomNumberGenerator numberGenerator = new RandomNumberGenerator();
    private Weapon weapon;
    AnimationPlayer HitAnimation;
    public override void _Ready()
    {
        if (enemyType != EnemyType.STATIC)
        {
            AttackTimer = GetNode("AttackTimer") as Timer;
            MoveTimer = GetNode("MoveTimer") as Timer;
            weapon = GetNode("Weapon") as Weapon;
            
        }
        DeathExplosion = GetNode<Explosion>(ExplosionPath);
        healthSystem = GetNode("HealthSystem") as HealthSystem;
        healthSystem.OnDeath += this.OnDeath;
        healthSystem.OnTakeDamage += this.OnGetHurt;

        numberGenerator.Randomize();
        if (HitAnimationPlayerPath != null)
        {
            HitAnimation = GetNode<AnimationPlayer>(HitAnimationPlayerPath);
        }
        if (enemyType == EnemyType.WAVE)
        {
            AttackTimer.Connect("timeout", this, nameof(Fire));
            AttackTimer.Start(numberGenerator.RandfRange(MinMaxTimeToFire.x, MinMaxTimeToFire.y));
        }
        if (enemyType == EnemyType.KAMIKAZE)
        {
            AttackTimer.Connect("timeout", this, nameof(TargetedFire));
        }
        if (enemyType == EnemyType.BOSS)
        {
            AttackTimer.Connect("timeout", this, nameof(TargetedFire));
            AttackTimer.OneShot = false;
            AttackTimer.WaitTime = numberGenerator.RandfRange(MinMaxTimeToFire.x, MinMaxTimeToFire.y) / 3;
            AttackTimer.Start();
            MoveTimer.Connect("timeout", this, nameof(MoveToRandomLocation));
            MoveToRandomLocation();
        }

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        var col = GetLastSlideCollision();
        if (col != null && col.Collider is Player)
        {
            (col.Collider as Player).healthSystem.TakeDamage(400);
            if (!(enemyType == EnemyType.BOSS || enemyType ==EnemyType.STATIC ))
            {
                OnDeath();
            }
            
        }
    }

    public void Fire()
    {
        weapon.Fire(Vector2.Down);
        AttackTimer.Start(numberGenerator.RandfRange(MinMaxTimeToFire.x, MinMaxTimeToFire.y));
    }

    void MoveToRandomLocation()
    {
        var rnd = new RandomNumberGenerator();
        rnd.Randomize();
        TweenMoveTo(StaticRefs.enemySpawner.PickRandomLocation(), 6.8f);
        MoveTimer.Start(6.8f + rnd.RandfRange(0.2f, 2f));
    }

    public void TargetedFire()
    {
        if (StaticRefs.player != null)
        {
            var dir = StaticRefs.player.GlobalPosition - GlobalPosition;
            dir = dir.Normalized();
            weapon.Fire(dir);
        }
        if (enemyType == EnemyType.BOSS)
        {
            AttackTimer.Start();
        }

    }

    public void FireTargetedAfterSeconds(float t)
    {
        AttackTimer.Start(t);

    }



    private void OnDeath()
    {
        StaticRefs.gameManager.IncreaseScore(ObjectScore);
        if (enemyType != EnemyType.STATIC)
        {
           StaticRefs.enemySpawner.OnEnemyDeleted(this); 
        }
        DeathExplosion.GetParent()?.RemoveChild(DeathExplosion); ;
        GetTree().Root.AddChild(DeathExplosion);
        DeathExplosion.GlobalPosition = GlobalPosition;
        DeathExplosion.Explode();
        QueueFree();
    }
    public void OnDeathNoScore()
    {
        StaticRefs.enemySpawner.OnEnemyDeleted(this);
        DeathExplosion.GetParent().RemoveChild(DeathExplosion); ;
        GetTree().Root.AddChild(DeathExplosion);
        DeathExplosion.GlobalPosition = GlobalPosition;
        DeathExplosion.Explode();
        QueueFree();
    }

    private void OnGetHurt()
    {
        HitAnimation.Play("FlashWhite");
    }

    public void TweenMoveTo(Vector2 pos, float t)
    {
        //GD.Print(t);
        var tween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(trans: Tween.TransitionType.Quint);
        tween.TweenProperty(this, "global_position", pos, t);

    }

    public void TweenMoveToAdv(Vector2 pos, float AfterTime, float t)
    {
        MoveTimer.Connect("timeout", this, nameof(TweenMoveTo), new Godot.Collections.Array { pos, t });
        MoveTimer.OneShot = true;
        MoveTimer.Start(AfterTime);
    }
}

public enum EnemyType
{
    WAVE,
    KAMIKAZE,
    BOSS,
    STATIC
}
