using Godot;
using System;

public class SceneCamera : Camera2D
{
    [Export]
    float  MaxRollRadians = 0.1f , ShakeIntensity;
    [Export]
    Vector2 MaxOffset = new Vector2(100, 75);

    OpenSimplexNoise noise = new OpenSimplexNoise();
    private float shakeTime = 0;
    int ny = 0;
    public override void _EnterTree()
    {
        base._EnterTree();
        StaticRefs.sceneCamera = this;
    }
    public override void _Ready()
    {
        var random = new Random();
        noise.Seed = random.Next();
        noise.Period = 4;
        noise.Octaves = 2;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (shakeTime > 0)
        {
            
            shakeTime -= delta;
            shakeTime = Mathf.Clamp(shakeTime,0,3f);
            Shake();
        }
        
    }

    void Shake(){
        var amount =ShakeIntensity;
        ny++;
        Rotation = noise.GetNoise2d(noise.Seed,ny) * amount*MaxRollRadians;
        Offset = MaxOffset * amount * noise.GetNoise2d(noise.Seed,ny)  ;

    }
    public void ShakeForSeconds(float time = 0.4f , float intensity = 10f ) {
        this.ShakeIntensity = intensity ;
        shakeTime = time;
        //GD.Print("shake");
    }
}
