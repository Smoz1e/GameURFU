using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

public class PlayerView
{
    public Texture2D _texture { get; private set; } // Изменено на public с private set
    public Texture2D _bulletTexture { get; private set; } // Изменено на public с private set
    private float _desiredWidth;
    private float _desiredHeight;

    public PlayerView(Texture2D texture, float desiredWidth, float desiredHeight, Texture2D bulletTexture)
    {
        _texture = texture ?? throw new ArgumentNullException(nameof(texture));
        _desiredWidth = desiredWidth;
        _desiredHeight = desiredHeight;
        _bulletTexture = bulletTexture;
    }

    public void Draw(SpriteBatch spriteBatch, PlayerModel model)
    {
        if (_texture == null) return;

        int sourceWidth = _texture.Width;
        int sourceHeight = _texture.Height;

        // Отрисовка игрока
        spriteBatch.Draw(
            _texture,
            model.Position,
            new Rectangle(0, 0, sourceWidth, sourceHeight),
            Color.White,
            model.Rotation,
            new Vector2(sourceWidth / 2, sourceHeight / 2),
            new Vector2(_desiredWidth / sourceWidth, _desiredHeight / sourceHeight),
            SpriteEffects.None,
            0f
        );

        // Временная отрисовка синего коллайдера (круга)
        float radius = Math.Min(_desiredWidth, _desiredHeight) / 3.0f;
        int segments = 32;
        Vector2 center = model.Position;
        DrawCircle(spriteBatch, center, radius, Color.Blue * 0.7f, segments);

        // Отрисовка пуль
        foreach (var bullet in model.Bullets)
        {
            bullet.Draw(spriteBatch);
        }
    }

    // Вспомогательная функция для отрисовки круга
    private void DrawCircle(SpriteBatch spriteBatch, Vector2 center, float radius, Color color, int segments)
    {
        Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        float increment = MathHelper.TwoPi / segments;
        Vector2 lastPoint = center + radius * new Vector2((float)Math.Cos(0), (float)Math.Sin(0));
        for (int i = 1; i <= segments; i++)
        {
            float angle = increment * i;
            Vector2 nextPoint = center + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
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