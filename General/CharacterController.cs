using Godot;
using System;

public class CharacterController : KinematicBody2D
{
    public Vector2 Velocity { get => _movement; set => _movement = value; }
    private Vector2 _movement;

    public override void _PhysicsProcess(float delta)
    {
        //GD.Print(_movement);
        _movement = MoveAndSlide(_movement);
    }


}
