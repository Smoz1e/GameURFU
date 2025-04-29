using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BotView
{
    private Texture2D _texture;

    public BotView(Texture2D texture)
    {
        _texture = texture;
    }

    public void Draw(SpriteBatch spriteBatch, BotModel model, float desiredWidth, float desiredHeight)
    {
        int sourceWidth = _texture.Width;
        int sourceHeight = _texture.Height;

        spriteBatch.Draw(
            _texture,
            model.Position,
            new Rectangle(0, 0, sourceWidth, sourceHeight), // область текстуры для отрисовки
            Color.White,
            model.Rotation, // Угол поворота
            new Vector2(sourceWidth / 2, sourceHeight / 2), // Точка вращения (центр)
            new Vector2(desiredWidth / sourceWidth, desiredHeight / sourceHeight), // Масштаб
            SpriteEffects.None,
            0f
        );
    }
}