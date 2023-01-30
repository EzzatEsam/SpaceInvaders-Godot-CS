using Godot;
using System;

public interface IDestructible 
{
    HealthSystem healthSystem {get; set;}
}

public interface IScoreObject
{
    int ObjectScore {get; set;}
}