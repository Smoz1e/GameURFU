using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Bullet
{
    public Vector2 Position { get; private set; }
    public Vector2 Direction { get; private set; }
    public float Speed { get; private set; }
    private Texture2D _texture;

    private int _frameWidth; // Ширина одного кадра
    private int _frameHeight; // Высота одного кадра
    private int _currentFrame; // Текущий кадр
    private int _totalFrames; // Общее количество кадров
    private float _frameTime; // Время между кадрами
    private float _elapsedTime; // Время, прошедшее с последнего обновления кадра

    public Bullet(Texture2D texture, Vector2 startPosition, Vector2 direction, float speed, int frameWidth, int frameHeight, float frameTime)
    {
        _texture = texture;
        Position = startPosition;
        Direction = direction;
        Speed = speed;

        _frameWidth = 8 ;
        _frameHeight = 10;
        _currentFrame = 0;
        _totalFrames = _texture.Width / 2; // Количество кадров по горизонтали
        _frameTime = frameTime;
        _elapsedTime = 0f;
    }

    public void Update(GameTime gameTime)
    {
        // Обновляем позицию пули
        Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Обновляем анимацию
        _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_elapsedTime >= _frameTime)
        {
            _currentFrame = (_currentFrame + 1) % _totalFrames; // Переход к следующему кадру
            _elapsedTime = 0f;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Определяем область текстуры для текущего кадра
        Rectangle sourceRectangle = new Rectangle(
            _currentFrame * _frameWidth, // Смещение по X для текущего кадра
            0,                          // Y всегда 0, так как кадры расположены горизонтально
            _frameWidth,                // Ширина одного кадра
            _frameHeight                // Высота одного кадра
        );

        spriteBatch.Draw(
            _texture,
            Position,
            sourceRectangle,
            Color.White,
            MathHelper.PiOver2, // Поворот на -90 градусов
            new Vector2(_frameWidth / 2, _frameHeight / 2), // Центр пули
            1f,
            SpriteEffects.None,
            0f
        );
    }
}