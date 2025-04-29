using Microsoft.Xna.Framework;

public class BotModel
{
    public Vector2 Position { get; set; }
    public float Speed { get; set; } = 100f; // Скорость бота
    public Vector2 Direction { get; set; } = Vector2.Zero; // Направление движения
    public float Rotation { get; set; } = 0f; // Угол поворота спрайта

    public BotModel(Vector2 startPosition)
    {
        Position = startPosition;
    }
}