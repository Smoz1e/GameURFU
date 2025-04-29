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

        // Отрисовка пуль
        foreach (var bullet in model.Bullets)
        {
            bullet.Draw(spriteBatch);
        }
    }
}