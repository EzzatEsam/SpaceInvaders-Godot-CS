using Godot;
using System;

public class Player : CharacterController, IDestructible
{
    [Export]
    private float Speed = 100f;
    [Export]
    NodePath WeaponPath;
    private InputManager inputManager;
    private Weapon weapon;
    private AnimationPlayer animationPlayer;

    public HealthSystem healthSystem { get; set; }

    public override void _EnterTree()
    {
        base._EnterTree();
        StaticRefs.player = this;
    }

    public override void _Ready()
    {
        inputManager = GetNode<InputManager>("InputManager");
        healthSystem = GetNode<HealthSystem>("HealthSystem");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        weapon = GetNode<Weapon>(WeaponPath);
        inputManager.OnNormalFire +=Fire;
        healthSystem.OnDeath  += OnTakeDamage;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        base.Velocity = inputManager.CurrentMovement * Speed;
    }
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

    }
    public void Fire(){
        if (weapon.Fire(Vector2.Up))
        {
            animationPlayer?.Play("Fire");
        }
        
    }
    void ReEnableHealth() {
        //GD.Print("why");
        healthSystem.IsShielded = false;
    }

    public void OnTakeDamage() {
        //StaticRefs.sceneCamera.ShakeForSeconds();
        healthSystem.Reset();
        StaticRefs.sceneCamera.ShakeForSeconds();
        StaticRefs.gameManager.OnPlayerDied();
        healthSystem.IsShielded = true;
        GetTree().CreateTimer(2).Connect("timeout",this,nameof(ReEnableHealth));


    }
}
