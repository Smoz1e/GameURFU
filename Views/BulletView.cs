using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BulletView
{
    private Texture2D _texture;
    private int _frameWidth;
    private int _frameHeight;
    private int _currentFrame;
    private int _totalFrames;
    private float _frameTime;
    private float _elapsedTime;

    public BulletView(Texture2D texture, int frameWidth, int frameHeight, float frameTime)
    {
        _texture = texture;
        _frameWidth = frameWidth;
        _frameHeight = frameHeight;
        _currentFrame = 0;
        _totalFrames = texture.Width / frameWidth;
        _frameTime = frameTime;
        _elapsedTime = 0f;
    }

    public void UpdateAnimation(GameTime gameTime)
    {
        _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_elapsedTime >= _frameTime)
        {
            _currentFrame = (_currentFrame + 1) % _totalFrames;
            _elapsedTime = 0f;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        Rectangle sourceRectangle = new Rectangle(
            _currentFrame * _frameWidth,
            0,
            _frameWidth,
            _frameHeight
        );

        spriteBatch.Draw(
            _texture,
            position,
            sourceRectangle,
            Color.White,
            MathHelper.PiOver2,
            new Vector2(_frameWidth / 2, _frameHeight / 2),
            1f,
            SpriteEffects.None,
            0f
        );
    }
}