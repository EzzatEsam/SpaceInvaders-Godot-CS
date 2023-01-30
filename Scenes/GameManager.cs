using Godot;
using System;

public class GameManager : Node2D
{
    [Export]
    NodePath PausedMenuPath, GameOverPath, GameStartPath, AnimationPlayerPath, ExplosionAudioPlayerPath;
    [Export]
    NodePath ScoreLabelPath, PlayerLivesLabelPath;
    private Label pausedMenu, gameOvermenu, gameStartLabel;
    private AnimationPlayer animationPlayer;
    private Explosion PlayerExplosion;
    private Label scoreLabel, livesLabel;
    private AudioStreamPlayer2D ExplosionAudioPlayer;
    public bool IsGameOver { get { return _gameOver; } }
    private bool _gameOver = false;
    private int Score = 0;
    private int PlayerLives = 3;
    public override void _Ready()
    {
        StaticRefs.gameManager = this;
        pausedMenu = GetNode(PausedMenuPath) as Label;
        gameOvermenu = GetNode(GameOverPath) as Label;
        scoreLabel = GetNode(ScoreLabelPath) as Label;
        livesLabel = GetNode(PlayerLivesLabelPath) as Label;
        gameStartLabel = GetNode(GameStartPath) as Label;
        ExplosionAudioPlayer = GetNode(ExplosionAudioPlayerPath) as AudioStreamPlayer2D;
        PlayerExplosion = GetNode<Explosion>("ExplosionPlayer");
        animationPlayer = GetNode(AnimationPlayerPath) as AnimationPlayer;
        scoreLabel.Text = (0).ToString();
        livesLabel.Text = 3.ToString();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public void ShowPausedMenu(bool t)
    {
        pausedMenu.Visible = t;
    }
    public void IncreaseScore(int sc)
    {
        Score += sc;
        scoreLabel.Text = Score.ToString();
    }
    public void OnPlayerDied()
    {
        PlayerLives--;
        livesLabel.Text = PlayerLives.ToString();
        if (PlayerLives == 0)
        {
            GameOver();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (_gameOver && @event is InputEventKey && Input.IsActionJustPressed("ship_normal_fire"))
        {
            GetTree().ReloadCurrentScene();
        }
    }
    void GameOver()
    {
        PlayerExplosion.GlobalPosition = StaticRefs.player.GlobalPosition;
        PlayerExplosion.Explode();
        StaticRefs.player.QueueFree();
        StaticRefs.player = null;
        gameOvermenu.Visible = true;
        _gameOver = true;
        ExplosionAudioPlayer.Play();

    }

    void ResetLevel()
    {

    }

    public void OnGameStart()
    {
        StaticRefs.enemySpawner.BeginSpawning();
        animationPlayer.Play("RESET");
    }
}
