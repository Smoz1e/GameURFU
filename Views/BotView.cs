using System;
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

        // Отрисовка спрайта бота
        spriteBatch.Draw(
            _texture,
            model.Position,
            new Rectangle(0, 0, sourceWidth, sourceHeight),
            Color.White,
            model.Rotation,
            new Vector2(sourceWidth / 2, sourceHeight / 2),
            new Vector2(desiredWidth / sourceWidth, desiredHeight / sourceHeight),
            SpriteEffects.None,
            0f
        );

        // Отрисовка круга-коллайдера
        DrawCircle(spriteBatch, model.Position, model.ColliderRadius, Color.Red * 0.3f, 32);
    }

    // Вспомогательная функция для отрисовки круга
    private void DrawCircle(SpriteBatch spriteBatch, Vector2 center, float radius, Color color, int segments)
    {
        Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        float increment = MathHelper.TwoPi / segments;
        var lastPoint = center + radius * new Vector2((float)Math.Cos(0), (float)Math.Sin(0));
        for (int i = 1; i <= segments; i++)
        {
            float angle = increment * i;
            var nextPoint = center + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            DrawLine(spriteBatch, pixel, lastPoint, nextPoint, color);
            lastPoint = nextPoint;
        }
    }

    // Вспомогательная функция для отрисовки линии
    private void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 end, Color color)
    {
        Vector2 edge = end - start;
        float angle = (float)Math.Atan2(edge.Y, edge.X);
        spriteBatch.Draw(texture, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), 2), null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
    }
}