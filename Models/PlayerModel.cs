using Microsoft.Xna.Framework; // Добавлено для использования Vector2
using System.Collections.Generic;

public class PlayerModel
{
    public Vector2 Position { get; set; }
    public float Speed { get; set; }
    public float Rotation { get; set; }
    public List<BulletController> Bullets { get; private set; }

    public PlayerModel(Vector2 startPosition, float speed)
    {
        Position = startPosition;
        Speed = speed;
        Rotation = 0f;
        Bullets = new List<BulletController>();
    }
}