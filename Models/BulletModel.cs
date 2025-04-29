using Microsoft.Xna.Framework;

public class BulletModel
{
    public Vector2 Position { get; set; }
    public Vector2 Direction { get; set; }
    public float Speed { get; set; }

    public BulletModel(Vector2 startPosition, Vector2 direction, float speed)
    {
        Position = startPosition;
        Direction = direction;
        Speed = speed;
    }
}