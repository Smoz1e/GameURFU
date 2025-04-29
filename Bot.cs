using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class Bot
{
    public Vector2 Position { get; set; }
    public float Speed { get; set; } = 100f; // Скорость бота
    private Texture2D _texture;
    private Vector2 _direction = Vector2.Zero; // Направление движения
    private float _rotation = 0f; // Угол поворота спрайта

    public Bot(Texture2D texture, Vector2 startPosition)
    {
        _texture = texture;
        Position = startPosition;
    }

    public void Update(GameTime gameTime, Vector2 playerPosition, Bot[] otherBots, float spaceBetweenBots)
    {
        // Рассчитываем направление к игроку
        Vector2 newDirection = playerPosition - Position;
        if (newDirection.Length() > 0)
        {
            newDirection.Normalize();
        }

        // Проверяем пересечение с другими ботами
        foreach (var bot in otherBots)
        {
            if (bot != this && Vector2.Distance(Position, bot.Position) < spaceBetweenBots)
            {
                // Отталкиваем бота в противоположную сторону
                Vector2 avoidDirection = Position - bot.Position;
                if (avoidDirection.Length() > 0)
                {
                    avoidDirection.Normalize();
                }
                newDirection += avoidDirection;

                // Корректируем позицию для предотвращения наложения
                Position += avoidDirection * (spaceBetweenBots - Vector2.Distance(Position, bot.Position));
            }
        }

        if (newDirection.Length() > 0)
        {
            newDirection.Normalize();
        }

        _direction = newDirection;

        // Обновляем позицию бота
        Position += _direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Рассчитываем угол поворота
        _rotation = (float)Math.Atan2(_direction.Y, _direction.X);
    }

    public void Draw(SpriteBatch spriteBatch, float desiredWidth, float desiredHeight)
    {
        int sourceWidth = _texture.Width;
        int sourceHeight = _texture.Height;

        spriteBatch.Draw(
            _texture,
            Position,
            new Rectangle(0, 0, sourceWidth, sourceHeight), // область текстуры для отрисовки
            Color.White,
            _rotation, // Угол поворота
            new Vector2(sourceWidth / 2, sourceHeight / 2), // Точка вращения (центр)
            new Vector2(desiredWidth / sourceWidth, desiredHeight / sourceHeight), // Масштаб
            SpriteEffects.None,
            0f
        );
    }
}