using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class AmmunitionView
{
    private Texture2D _texture;
    private float _size;
    public AmmunitionView(Texture2D texture, float size = 50f)
    {
        _texture = texture;
        _size = size;
    }
    public void Draw(SpriteBatch spriteBatch, AmmunitionModel model)
    {
        if (model.IsCollected) return;
        int sourceWidth = _texture.Width;
        int sourceHeight = _texture.Height;
        spriteBatch.Draw(
            _texture,
            model.Position,
            new Rectangle(0, 0, sourceWidth, sourceHeight),
            Color.White,
            0f,
            new Vector2(sourceWidth / 2, sourceHeight / 2),
            new Vector2(_size / sourceWidth, _size / sourceHeight),
            SpriteEffects.None,
            0f
        );
    }
}
