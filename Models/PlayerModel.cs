using Microsoft.Xna.Framework; // Добавлено для использования Vector2
using System.Collections.Generic;

public class PlayerModel
{
    public Vector2 Position { get; set; }
    public float Speed { get; set; }
    public float Rotation { get; set; }
    public List<BulletController> Bullets { get; private set; }

    public int ShotsFired = 0;
    public bool IsReloading = false;
    public float ReloadTimer = 0f;
    public const int MaxShotsBeforeReload = 25;
    public const float ReloadDuration = 2.5f;
    public int Magazines = 4;
    public const int MaxMagazines = 7;

    public PlayerModel(Vector2 startPosition, float speed)
    {
        Position = startPosition;
        Speed = speed;
        Rotation = 0f;
        Bullets = new List<BulletController>();
    }
}