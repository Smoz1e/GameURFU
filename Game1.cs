using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace The_War_of__Layers
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private PlayerController _playerController;
        private PlayerModel _playerModel;
        private PlayerView _playerView;

        private Texture2D _introImage;
        private Texture2D _backgroundTexture;
        private Texture2D _menuBackground;
        private Texture2D _startButtonTexture;
        private Texture2D _settingsButtonTexture;
        private Texture2D _exitButtonTexture;
        private Rectangle _startButtonRect;
        private Rectangle _settingsButtonRect;
        private Rectangle _exitButtonRect;

        private enum GameState { Intro, Menu, Playing }
        private GameState _currentState = GameState.Intro;

        private float _introTimer = 0f;
        private const float IntroDuration = 3f;

        private List<BotController> _botControllers; // Список контроллеров ботов
        private List<BotModel> _botModels; // Список моделей ботов
        private BotView _botView; // Общий вид для всех ботов

        private const float SpaceBetweenBots = 100f;

        private MouseState _previousMouseState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;

            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _backgroundTexture = Content.Load<Texture2D>("testFon");
            _introImage = Content.Load<Texture2D>("Icon");
            _menuBackground = Content.Load<Texture2D>("menuBackground");
            _startButtonTexture = Content.Load<Texture2D>("startButton");
            _settingsButtonTexture = Content.Load<Texture2D>("settingsButton");
            _exitButtonTexture = Content.Load<Texture2D>("exitButton");

            var botTexture = Content.Load<Texture2D>("bot");

            // Создаем модель, вид и контроллер игрока
            var playerTexture = Content.Load<Texture2D>("playerTest");
            var bulletTexture = Content.Load<Texture2D>("bullet");
            _playerModel = new PlayerModel(new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), 300f);
            _playerView = new PlayerView(playerTexture, 100f, 100f, bulletTexture);
            _playerController = new PlayerController(_playerModel, _playerView);

            // Создаем список ботов
            _botModels = new List<BotModel>
            {
                new BotModel(new Vector2(100, 100)),
                new BotModel(new Vector2(500, 200)),
                new BotModel(new Vector2(800, 500))
            };

            _botView = new BotView(botTexture);
            _botControllers = new List<BotController>();
            foreach (var botModel in _botModels)
            {
                _botControllers.Add(new BotController(botModel));
            }

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (_currentState == GameState.Playing)
                {
                    _currentState = GameState.Menu;
                }
                else
                {
                    Exit();
                }
            }

            MouseState currentMouseState = Mouse.GetState();

            if (_currentState == GameState.Intro)
            {
                _introTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_introTimer >= IntroDuration || Keyboard.GetState().GetPressedKeys().Length > 0 || currentMouseState.LeftButton == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    _currentState = GameState.Menu;
                }
            }
            else if (_currentState == GameState.Menu)
            {
                bool isStartHovered = _startButtonRect.Contains(currentMouseState.Position);
                bool isSettingsHovered = _settingsButtonRect.Contains(currentMouseState.Position);
                bool isExitHovered = _exitButtonRect.Contains(currentMouseState.Position);

                if (isStartHovered && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    _currentState = GameState.Playing;
                }
                else if (isExitHovered && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    Exit();
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    _currentState = GameState.Playing;
                }
            }
            else if (_currentState == GameState.Playing)
            {
                _playerController.Update(gameTime, _graphics);

                for (int i = _botControllers.Count - 1; i >= 0; i--)
                {
                    var botController = _botControllers[i];
                    botController.Update(gameTime, _playerModel.Position, _botModels.ToArray(), SpaceBetweenBots);

                    // Проверка столкновения игрока с ботом
                    Rectangle playerRect = new Rectangle(
                        (int)(_playerModel.Position.X - 50),
                        (int)(_playerModel.Position.Y - 50),
                        100,
                        100
                    );

                    Rectangle botRect = new Rectangle(
                        (int)(_botModels[i].Position.X - 50),
                        (int)(_botModels[i].Position.Y - 50),
                        100,
                        100
                    );

                    if (playerRect.Intersects(botRect))
                    {
                        Exit();
                    }

                    // Проверка столкновения пуль с ботами
                    for (int j = _playerModel.Bullets.Count - 1; j >= 0; j--)
                    {
                        var bullet = _playerModel.Bullets[j];
                        Rectangle bulletRect = new Rectangle(
                            (int)(bullet.Position.X - 7.5f), // Половина ширины пули
                            (int)(bullet.Position.Y - 7.5f), // Половина высоты пули
                            15, // Ширина пули
                            15  // Высота пули
                        );

                        if (bulletRect.Intersects(botRect))
                        {
                            // Удаляем бота и пулю
                            _botControllers.RemoveAt(i);
                            _botModels.RemoveAt(i);
                            _playerModel.Bullets.RemoveAt(j);
                            break;
                        }
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
                _spriteBatch.Draw(_introImage, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            }
            else if (_currentState == GameState.Menu)
            {
                _spriteBatch.Draw(_menuBackground, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

                Color startButtonColor = _startButtonRect.Contains(Mouse.GetState().Position) ? Color.LightGray : Color.White;
                Color settingsButtonColor = _settingsButtonRect.Contains(Mouse.GetState().Position) ? Color.LightGray : Color.White;
                Color exitButtonColor = _exitButtonRect.Contains(Mouse.GetState().Position) ? Color.LightGray : Color.White;

                _spriteBatch.Draw(_startButtonTexture, _startButtonRect, startButtonColor);
                _spriteBatch.Draw(_settingsButtonTexture, _settingsButtonRect, settingsButtonColor);
                _spriteBatch.Draw(_exitButtonTexture, _exitButtonRect, exitButtonColor);
            }
            else if (_currentState == GameState.Playing)
            {
                _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

                foreach (var botModel in _botModels)
                {
                    _botView.Draw(_spriteBatch, botModel, 100f, 100f);
                }

                _playerController.Draw(_spriteBatch);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
