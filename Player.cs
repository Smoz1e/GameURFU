using System; // Для Math
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Player
{
    public Vector2 Position { get; private set; }
    public float Speed { get; private set; }
    private Texture2D _texture;
    private float _desiredWidth;
    private float _desiredHeight;
    private float _rotation; // Угол поворота игрока
    private Texture2D _bulletTexture; // Текстура пули
    public List<Bullet> Bullets { get; private set; } // Список пуль

    private MouseState _previousMouseState; // Добавлено: Предыдущее состояние мыши

    public Player(Texture2D texture, Vector2 startPosition, float speed, float desiredWidth, float desiredHeight, Texture2D bulletTexture)
    {
        _texture = texture ?? throw new ArgumentNullException(nameof(texture));
        Position = startPosition;
        Speed = speed;
        _desiredWidth = desiredWidth;
        _desiredHeight = desiredHeight;
        _rotation = 0f;
        _bulletTexture = bulletTexture;
        Bullets = new List<Bullet>();
        _previousMouseState = Mouse.GetState(); // Инициализация состояния мыши
    }

    public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        float updatedSpeed = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        var kstate = Keyboard.GetState();
        var mouseState = Mouse.GetState();

        // Управление игроком
        Vector2 movement = Vector2.Zero;
        if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.W)) movement.Y -= 1; // Вверх
        if (kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.S)) movement.Y += 1; // Вниз
        if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A)) movement.X -= 1; // Влево
        if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D)) movement.X += 1; // Вправо

        if (movement != Vector2.Zero)
        {
            movement.Normalize();
            Position += movement * updatedSpeed;
        }

        // Ограничение движения игрока в пределах экрана
        ClampPosition(graphics);

        // Вычисление угла поворота к курсору
        Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
        Vector2 directionToMouse = mousePosition - Position;
        _rotation = (float)Math.Atan2(directionToMouse.Y, directionToMouse.X);

        // Стрельба
        if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            Vector2 bulletDirection = directionToMouse;
            if (bulletDirection.Length() > 0)
            {
                bulletDirection.Normalize();
            }

            // Удаляем старую пулю, если она существует
            if (Bullets.Count > 0)
            {
                Bullets.Clear(); // Удаляем все существующие пули
            }

            // Создаем новую пулю
            Bullets.Add(new Bullet(
                _bulletTexture,
                Position,
                bulletDirection,
                500f, // Скорость пули
                16,   // Ширина одного кадра (например, 16 пикселей)
                16,   // Высота одного кадра (например, 16 пикселей)
                0.1f  // Время между кадрами (например, 0.1 секунды)
            ));
        }

        // Обновление пуль
        for (int i = Bullets.Count - 1; i >= 0; i--)
        {
            Bullets[i].Update(gameTime);

            // Удаляем пулю, если она вышла за пределы экрана
            if (Bullets[i].Position.X < 0 || Bullets[i].Position.X > graphics.PreferredBackBufferWidth ||
                Bullets[i].Position.Y < 0 || Bullets[i].Position.Y > graphics.PreferredBackBufferHeight)
            {
                Bullets.RemoveAt(i);
            }
        }

        // Обновляем предыдущее состояние мыши
        _previousMouseState = mouseState;
    }

    private void ClampPosition(GraphicsDeviceManager graphics)
    {
        int c = 6;
        Position = new Vector2(
            MathHelper.Clamp(Position.X, _texture.Width / (2 * c), graphics.PreferredBackBufferWidth - _texture.Width / (2 * c)),
            MathHelper.Clamp(Position.Y, _texture.Height / (2 * c), graphics.PreferredBackBufferHeight - _texture.Height / (2 * c))
        );
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_texture == null) return;

        int sourceWidth = _texture.Width;
        int sourceHeight = _texture.Height;

        spriteBatch.Draw(
            _texture,
            Position,
            new Rectangle(0, 0, sourceWidth, sourceHeight),
            Color.White,
            _rotation, // Угол поворота
            new Vector2(sourceWidth / 2, sourceHeight / 2), // Точка вращения (центр)
            new Vector2(_desiredWidth / sourceWidth, _desiredHeight / sourceHeight), // Масштаб
            SpriteEffects.None,
            0f
        );

        // Отрисовка пуль
        foreach (var bullet in Bullets)
        {
            bullet.Draw(spriteBatch);
        }
    }
}