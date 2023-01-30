using Godot;
using System;

public class Explosion : Node2D
{
    Particles2D particles;
    Timer timer;
    public override void _Ready()
    {
        particles = GetNode("Explosion") as Particles2D;
        timer = GetNode("Timer") as Timer;
        
        timer.Connect("timeout" , this,nameof(DestroyObject));
    }

    void DestroyObject() {
        QueueFree();
    }

    public void Explode() {
        particles.Emitting = true;
        timer.Start();
    }
}
