using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace The_War_of__Layers
{
    public class Game1 : Game
    {

        //* Добавляем игрока

        Texture2D PlayerTexture;
        Vector2 PlayerPosition;
        float PlayerSpeed;

        // Желаемые размеры на экране (можно изменить)
        float desiredWidth = 100f;  // пример: 100 пикселей в ширину
        float desiredHeight = 100f;  // пример: 100 пикселей в высоту

        //* Для отрисовки

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        //* Текстуры для интро и фона
        private Texture2D _introImage;
        private Texture2D _backgroundTexture;
        
        //* Состояние игры
        private enum GameState { Intro, Playing }
        private GameState _currentState = GameState.Intro;
        
        //* Таймер для заставки
        private float _introTimer = 0f;
        private const float IntroDuration = 3f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;

            //* Установка разрешения и полноэкранного режима 
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; 
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height; 
            _graphics.IsFullScreen = true; 
            _graphics.ApplyChanges(); 
        }

        protected override void Initialize()
        {
            PlayerPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                                   _graphics.PreferredBackBufferHeight / 2);
            PlayerSpeed = 300f;
            // TODO: Инициализация объектов
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backgroundTexture = Content.Load<Texture2D>("testFon");
            _introImage = Content.Load<Texture2D>("Icon");
            PlayerTexture = Content.Load<Texture2D>("playerTest");
        }

        protected override void Update(GameTime gameTime)
        {
            //* Выход из игры
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_currentState == GameState.Intro)
            {
                //* Обновляем таймер
                _introTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                // Проверяем, закончилось ли время заставки или пользователь нажал кнопку
                if (_introTimer >= IntroDuration || 
                    Keyboard.GetState().GetPressedKeys().Length > 0 ||
                    Mouse.GetState().LeftButton == ButtonState.Pressed ||
                    GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    _currentState = GameState.Playing;
                }
            }
            else
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                            Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                float updatedPlayerSpeed = PlayerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                var kstate = Keyboard.GetState();
                
                //* Передвижение

                if (kstate.IsKeyDown(Keys.Up))
                {
                    PlayerPosition.Y -= updatedPlayerSpeed;
                }
                
                if (kstate.IsKeyDown(Keys.Down))
                {
                    PlayerPosition.Y += updatedPlayerSpeed;
                }
                
                if (kstate.IsKeyDown(Keys.Left))
                {
                    PlayerPosition.X -= updatedPlayerSpeed;
                }
                if (kstate.IsKeyDown(Keys.Right))
                {
                    PlayerPosition.X += updatedPlayerSpeed;
                }
                
                //* Убираем выход за экран
                int c = 6;
                if (PlayerPosition.X > _graphics.PreferredBackBufferWidth - PlayerTexture.Width / (2 * c))
                {
                    PlayerPosition.X = _graphics.PreferredBackBufferWidth - PlayerTexture.Width / (2 * c);
                }
                else if (PlayerPosition.X < PlayerTexture.Width / (2 * c))
                {
                    PlayerPosition.X = PlayerTexture.Width / (2 * c);
                }

                if (PlayerPosition.Y > _graphics.PreferredBackBufferHeight - PlayerTexture.Height / (2 * c))
                {
                    PlayerPosition.Y = _graphics.PreferredBackBufferHeight - PlayerTexture.Height / (2 * c);
                }
                else if (PlayerPosition.Y < PlayerTexture.Height / (2 * c))
                {
                    PlayerPosition.Y = PlayerTexture.Height / (2 * c);
                }
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            
            if (_currentState == GameState.Intro)
            {
                //* Рисуем интро по центру экрана
                _spriteBatch.Draw(
                    _introImage,
                    new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), 
                    Color.White);
            }
            else
            {
                //* Рисуем фон на весь экран
                _spriteBatch.Draw(
                    _backgroundTexture, 
                    new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), 
                    Color.White);


                
                // Задаем желаемые размеры (можно изменить на нужные)
                int sourceWidth = PlayerTexture.Width;  // или любой другой размер
                int sourceHeight = PlayerTexture.Height;  // или любой другой размер
                    
                //*  Рисуем игрока
                _spriteBatch.Draw(
                    PlayerTexture,
                    PlayerPosition,
                    new Rectangle(0, 0, sourceWidth, sourceHeight),  // область текстуры для отрисовки
                    Color.White,
                    0f,
                    new Vector2(sourceWidth / 2, sourceHeight / 2),  // точка вращения (центр)
                    new Vector2(desiredWidth / sourceWidth, desiredHeight / sourceHeight),  // масштаб
                    SpriteEffects.None,
                    0f
                );
            }
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}