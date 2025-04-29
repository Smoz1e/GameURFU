using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class PlayerModel
{
    public Vector2 Position { get; set; }
    public float Speed { get; set; }
    public float Rotation { get; set; }
    public List<Bullet> Bullets { get; private set; }

    public PlayerModel(Vector2 startPosition, float speed)
    {
        Position = startPosition;
        Speed = speed;
        Rotation = 0f;
        Bullets = new List<Bullet>();
    }
}