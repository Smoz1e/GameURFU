using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace The_War_of__Layers
{
    public class Game1 : Game
    {

        //* Добавляем игрока

        Texture2D PlayerTexture;
        Vector2 PlayerPosition;
        float PlayerSpeed;

        float desiredWidth = 100f;  //пиксели в ширину
        float desiredHeight = 100f;  // пиксели в высоту

        //* Для отрисовки

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        //* Текстуры для интро и фона и меню игры
        private Texture2D _introImage;
        private Texture2D _backgroundTexture;
        private Texture2D _menuBackground;
        private Texture2D _startButtonTexture;
        //* Новые текстуры для кнопок
        private Texture2D _settingsButtonTexture;
        private Texture2D _exitButtonTexture;
        private Rectangle _settingsButtonRect;
        private Rectangle _exitButtonRect;

        // Шрифт для заголовка
        // private SpriteFont _menuFont;

        //* Состояние игры
        private enum GameState { Intro, Menu, Playing }
        private GameState _currentState = GameState.Intro;
        
        //* Таймер для заставки
        private float _introTimer = 0f;
        private const float IntroDuration = 3f;

        //* Кнопка "Начать игру"
        private Rectangle _startButtonRect;
        private bool _isButtonHovered;
        private MouseState _previousMouseState;

        private List<Bot> _bots;
        private Texture2D _botTexture;

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

            _startButtonRect = new Rectangle();
            // TODO: Инициализация объектов
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backgroundTexture = Content.Load<Texture2D>("testFon");
            _introImage = Content.Load<Texture2D>("Icon");
            PlayerTexture = Content.Load<Texture2D>("playerTest");

            _menuBackground = Content.Load<Texture2D>("menuBackground");
            _startButtonTexture = Content.Load<Texture2D>("startButton");
            _settingsButtonTexture = Content.Load<Texture2D>("settingsButton");
            _exitButtonTexture = Content.Load<Texture2D>("exitButton");

            _botTexture = Content.Load<Texture2D>("bot"); // Загрузка текстуры бота

            // Создаем список ботов
            _bots = new List<Bot>
            {
                new Bot(_botTexture, new Vector2(100, 100)),
                // new Bot(_botTexture, new Vector2(500, 200)),
                // new Bot(_botTexture, new Vector2(800, 500))
            };

            // загрузка шрифта
            // _menuFont = Content.Load<SpriteFont>("MenuFont");

            // Установка размеров и позиций кнопок
            int buttonWidth = 200;
            int buttonHeight = 125;
            _startButtonRect = new Rectangle(
                (_graphics.PreferredBackBufferWidth - buttonWidth) / 2,
                (_graphics.PreferredBackBufferHeight - buttonHeight) / 2 - 150,
                buttonWidth,
                buttonHeight
            );
            _settingsButtonRect = new Rectangle(
                (_graphics.PreferredBackBufferWidth - buttonWidth) / 2,
                (_graphics.PreferredBackBufferHeight - buttonHeight) / 2,
                buttonWidth,
                buttonHeight
            );
            _exitButtonRect = new Rectangle(
                (_graphics.PreferredBackBufferWidth - buttonWidth) / 2,
                (_graphics.PreferredBackBufferHeight - buttonHeight) / 2 + 150,
                buttonWidth,
                buttonHeight
            );
        }

        protected override void Update(GameTime gameTime)
        {
            //* Выход из игры
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (_currentState == GameState.Playing)
                {
                    _currentState = GameState.Menu; // Возврат в меню при нажатии ESC
                }
                else
                {
                    Exit(); // Выход из игры, если уже в меню
                }
            }

            MouseState currentMouseState = Mouse.GetState();
            
            if (_currentState == GameState.Intro)
            {
                //* Обновляем таймер
                _introTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                // Проверяем, закончилось ли время заставки или бфло нажатие на кнопку
                if (_introTimer >= IntroDuration || 
                    Keyboard.GetState().GetPressedKeys().Length > 0 ||
                    currentMouseState.LeftButton == ButtonState.Pressed ||
                    GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    _currentState = GameState.Menu; // Переходим в меню вместо сразу в игру
                }
            }
            else if (_currentState == GameState.Menu)
            {
                // Проверка наведения на кнопки
                _isButtonHovered = _startButtonRect.Contains(currentMouseState.Position);
                bool isSettingsHovered = _settingsButtonRect.Contains(currentMouseState.Position);
                bool isExitHovered = _exitButtonRect.Contains(currentMouseState.Position);

                // Проверка клика по кнопкам
                if (_isButtonHovered && currentMouseState.LeftButton == ButtonState.Pressed &&
                    _previousMouseState.LeftButton == ButtonState.Released)
                {
                    _currentState = GameState.Playing;
                }
                else if (isSettingsHovered && currentMouseState.LeftButton == ButtonState.Pressed &&
                         _previousMouseState.LeftButton == ButtonState.Released)
                {
                    // Логика для настроек
                }
                else if (isExitHovered && currentMouseState.LeftButton == ButtonState.Pressed &&
                         _previousMouseState.LeftButton == ButtonState.Released)
                {
                    Exit();
                }
                
                // Начать игру по нажатию Enter или Start
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) || 
                    GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    _currentState = GameState.Playing;
                }
            }
            else if (_currentState == GameState.Playing)
            {
                float updatedPlayerSpeed = PlayerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                var kstate = Keyboard.GetState();

                //* Передвижение игрока
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

                // *Обновляем позиции ботов
                foreach (var bot in _bots)
                {
                    bot.Update(gameTime, PlayerPosition);

                    // Проверяем пересечение игрока и бота
                    Rectangle playerRect = new Rectangle(
                        (int)(PlayerPosition.X - desiredWidth / 2) + 20,
                        (int)(PlayerPosition.Y - desiredHeight / 2) + 20,
                        (int)desiredWidth - 2 * 20,
                        (int)desiredHeight - 2 * 20
                    );

                    Rectangle botRect = new Rectangle(
                        (int)(bot.Position.X - desiredWidth / 2) + 20,
                        (int)(bot.Position.Y - desiredHeight / 2) + 20,
                        (int)desiredWidth - 2 * 20,
                        (int)desiredHeight - 2 * 20
                    );

                    if (playerRect.Intersects(botRect))
                    {
                        System.Console.WriteLine("Game over");
                        Exit();
                    }
                }
            }
            
            _previousMouseState = currentMouseState;
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

            else if (_currentState == GameState.Menu)
            {
                // Рисуем фон меню
                _spriteBatch.Draw(
                    _menuBackground,
                    new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                    Color.White
                );

                // Отрисовка кнопоук
                Color startButtonColor = _isButtonHovered ? Color.LightGray : Color.White;
                Color settingsButtonColor = _settingsButtonRect.Contains(Mouse.GetState().Position) ? Color.LightGray : Color.White;
                Color exitButtonColor = _exitButtonRect.Contains(Mouse.GetState().Position) ? Color.LightGray : Color.White;

                _spriteBatch.Draw(_startButtonTexture, _startButtonRect, startButtonColor);
                _spriteBatch.Draw(_settingsButtonTexture, _settingsButtonRect, settingsButtonColor);
                _spriteBatch.Draw(_exitButtonTexture, _exitButtonRect, exitButtonColor);
            }
            
            else
            {
                //* Рисуем фон на весь экран
                _spriteBatch.Draw(
                    _backgroundTexture, 
                    new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), 
                    Color.White
                );

                // Рисуем ботов
                foreach (var bot in _bots)
                {
                    bot.Draw(_spriteBatch, desiredWidth, desiredHeight);
                }

                // Маштаб игрока
                int sourceWidth = PlayerTexture.Width; 
                int sourceHeight = PlayerTexture.Height;

                //* Рисуем игрока
                _spriteBatch.Draw(
                    PlayerTexture,
                    PlayerPosition,
                    new Rectangle(0, 0, sourceWidth, sourceHeight),  // область текстуры для отрисовки
                    Color.White,
                    0f,
                    new Vector2(sourceWidth / 2, sourceHeight / 2),  // Центр
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