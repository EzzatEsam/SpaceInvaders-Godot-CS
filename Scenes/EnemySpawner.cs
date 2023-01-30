using Godot;
using System;
using System.Collections.Generic;

public class EnemySpawner : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    [Export]
    int NumberToSpawnX, NumberToSpawnY, NumberToSpawnB;
    [Export]
    float EnemySpeed = 120f;


    [Export]
    float InnerDistance;
    [Export]
    PackedScene FirstEnemy, SecondEnemy, BossEnemy;
    private Node2D StartPos, InitialPos, FinalPos1, FinalPos2;
    private List<Enemy> currentEnemies = new List<Enemy>();
    Vector2 movmentVector = Vector2.Left;
    private int LevelIndex = 0;
    private EnemyType CurrentWaveType;
    public override void _EnterTree()
    {
        base._EnterTree();
        StaticRefs.enemySpawner = this;
    }
    public override void _Ready()
    {
        StartPos = GetNode("StartPosition") as Node2D;
        InitialPos = GetNode("InitialPos") as Node2D;
        FinalPos1 = GetNode("FinalPos1") as Node2D;
        FinalPos2 = GetNode("FinalPos2") as Node2D;

    }

    public void SpawnWave()
    {
        CurrentWaveType = EnemyType.WAVE;
        for (int i = 0; i < NumberToSpawnX; i++)
        {
            for (int j = 0; j < NumberToSpawnY; j++)
            {
                var pos = StartPos.GlobalPosition + i * Vector2.Right * InnerDistance + j * Vector2.Down * InnerDistance;
                var it = SpawnObject(FirstEnemy);
                (it as Enemy).TweenMoveTo(pos, 1.5f);
                currentEnemies.Add(it as Enemy);

            }
        }
    }

    Node2D SpawnObject(PackedScene _object)
    {
        var it = _object.Instance() as Node2D;
        AddChild(it);
        it.GlobalPosition = InitialPos.GlobalPosition;
        (it as Enemy).healthSystem.MaxHealth += (it as Enemy).healthSystem.MaxHealth * LevelIndex;
        GD.Print((it as Enemy).healthSystem.MaxHealth);
        return it;
    }

    void BeginMovement()
    {
        if (CurrentWaveType == EnemyType.WAVE)
        {
            movmentVector *= -1;
            foreach (var item in currentEnemies)
            {
                item.Velocity = movmentVector * EnemySpeed + Vector2.Down * EnemySpeed / 3.6f;
            }
            var timer = GetTree().CreateTimer(4.2f);
            timer.Connect("timeout", this, nameof(BeginMovement));
        }


    }

    public void BeginSpawning()
    {
        SpawnWave();
        BeginMovement();
        // SpawnBoss();
    }

    public void OnEnemyDeleted(Enemy enemy)
    {
        currentEnemies.Remove(enemy);
        if (currentEnemies.Count == 0)
        {
            MoveToNextPhase();
        }
    }

    void MoveToNextPhase()
    {
        if (!StaticRefs.gameManager.IsGameOver)
        {
            switch (CurrentWaveType)
            {
                case EnemyType.WAVE:
                    //SpawnSecondEnemies();

                    movmentVector = Vector2.Left;
                    GetTree().CreateTimer(2.5f).Connect("timeout", this, nameof(SpawnSecondEnemies));
                    break;
                case EnemyType.KAMIKAZE:
                    GetTree().CreateTimer(2.5f).Connect("timeout", this, nameof(SpawnBoss));

                    //SpawnBoss();
                    break;
                case EnemyType.BOSS:
                    LevelIndex++;
                    GetTree().CreateTimer(2.5f).Connect("timeout", this, nameof(BeginSpawning));
                    break;
                default:
                    break;
            }
        }
    }

    void SpawnBoss()
    {
        CurrentWaveType = EnemyType.BOSS;

        var boss = SpawnObject(BossEnemy) as Enemy;
        currentEnemies.Add(boss);

    }



    void SpawnSecondEnemies()
    {
        
        CurrentWaveType = EnemyType.KAMIKAZE;
        var rnd = new RandomNumberGenerator();
        rnd.Randomize();
        for (int i = 0; i < NumberToSpawnB; i++)
        {
            var it = SpawnObject(SecondEnemy);

            it.GlobalPosition = new Vector2(rnd.RandfRange(FinalPos1.GlobalPosition.x, FinalPos2.GlobalPosition.x),
            it.GlobalPosition.y);
            currentEnemies.Add(it as Enemy);
        }

        for (int i = 0; i < NumberToSpawnB; i++)
        {
            var pos = new Vector2(rnd.RandfRange(FinalPos1.GlobalPosition.x, FinalPos2.GlobalPosition.x),
            FinalPos1.GlobalPosition.y);
            (currentEnemies[i] as Enemy).TweenMoveToAdv(pos, (float)i + rnd.RandfRange(0.5f, 1.5f), 5.6f);
            (currentEnemies[i] as Enemy).FireTargetedAfterSeconds((float)i + rnd.RandfRange(0.8f, 1f));
        }

    }

    public Vector2 PickRandomLocation()
    {
        var rnd = new RandomNumberGenerator();
        rnd.Randomize();
        return new Vector2(rnd.RandfRange(FinalPos1.GlobalPosition.x + 80, FinalPos2.GlobalPosition.x - 80),
          rnd.RandfRange(StartPos.GlobalPosition.y, FinalPos2.GlobalPosition.y - 220));
    }



}


