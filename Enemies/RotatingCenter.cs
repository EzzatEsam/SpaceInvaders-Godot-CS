using Godot;
using System;

public class RotatingCenter : Node2D
{
    
    public override void _Ready()
    {
        
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

    }
}
