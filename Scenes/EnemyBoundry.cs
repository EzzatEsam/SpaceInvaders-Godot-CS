using Godot;
using System;

public class EnemyBoundry : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("body_entered" ,this , nameof(OnEnemyEntered));
    }

    void OnEnemyEntered( PhysicsBody2D body) {
        if (body !=null)
        {
            if (body is Enemy)
            {
                (body as Enemy).OnDeathNoScore();
            }
        }
    }
}
