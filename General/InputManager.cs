using Godot;
using System;

public class InputManager : Node2D
{
    [Export]
    bool CanMoveVertically = true;
    public Vector2 CurrentMovement { get { return _movement; } }
    public event Action OnNormalFire;
    public event Action OnAltFire; 
    private Vector2 _movement = new Vector2();
    private bool paused = false;
    private bool gameStarted = false;
    public override void _Ready()
    {

    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
         if (@event is InputEventKey)
         {
            if (!gameStarted && @event.IsPressed())
            {
                gameStarted = true;
                StaticRefs.gameManager.OnGameStart();
            }
         }

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (!gameStarted)
        {
            return;
        }
        if (!paused)
        {
            var vec1 = Vector2.Left * (Input.GetActionStrength("move_left") - Input.GetActionStrength("move_right"));
            var vec2 = CanMoveVertically ? Vector2.Up * Input.GetActionStrength("move_up") + Input.GetActionStrength("move_down") * Vector2.Down : Vector2.Zero;
            _movement = (vec1 + vec2).Normalized();
            if (Input.IsActionPressed("ship_normal_fire"))
            {
                OnNormalFire?.Invoke();
            }
            if (Input.IsActionPressed("ship_alt_fire"))
            {
                OnAltFire?.Invoke();
            }
        }
        if (Input.IsActionJustPressed("game_pause"))
        {
            paused = !paused;
            StaticRefs.gameManager.ShowPausedMenu(paused);
            GetTree().Paused = paused;
        }

    }
}
