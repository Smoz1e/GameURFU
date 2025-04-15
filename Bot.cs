using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Bot
{
    public Vector2 Position { get; set; }
    public float Speed { get; set; } = 20f; // Скорость бота
    private Texture2D _texture;

    public Bot(Texture2D texture, Vector2 startPosition)
    {
        _texture = texture;
        Position = startPosition;
    }

    public void Update(GameTime gameTime, Vector2 playerPosition)
    {
        // Рассчитываем направление к игроку
        Vector2 direction = playerPosition - Position;
        if (direction.Length() > 0)
        {
            direction.Normalize();
        }

        // Обновляем позицию бота
        Position += direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, Color.White);
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
            0f,
            new Vector2(sourceWidth / 2, sourceHeight / 2), // точка вращения (центр)
            new Vector2(desiredWidth / sourceWidth, desiredHeight / sourceHeight), // масштаб
            SpriteEffects.None,
            0f
        );
    }
}