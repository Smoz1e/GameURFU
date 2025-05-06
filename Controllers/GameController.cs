using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class GameController
{
    private GameModel _model;
    private GameView _view;
    private PlayerController _playerController;
    private BotView _botView;
    private MouseState _previousMouseState;
    private GraphicsDeviceManager _graphics;

    public GameController(GameModel model, GameView view, GraphicsDeviceManager graphics)
    {
        _model = model;
        _view = view;
        _graphics = graphics;
        _previousMouseState = Mouse.GetState();
    }

    public void Initialize()
    {
        // Initialization logic can be added here
    }

    public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
    {
        _model.BackgroundTexture = Content.Load<Texture2D>("testFon");
        _model.IntroImage = Content.Load<Texture2D>("Icon");
        _model.MenuBackground = Content.Load<Texture2D>("menuBackground");
        _model.StartButtonTexture = Content.Load<Texture2D>("startButton");
        _model.SettingsButtonTexture = Content.Load<Texture2D>("settingsButton");
        _model.ExitButtonTexture = Content.Load<Texture2D>("exitButton");
        var botTexture = Content.Load<Texture2D>("bot");
        var playerTexture = Content.Load<Texture2D>("playerTest");
        var bulletTexture = Content.Load<Texture2D>("bullet");
        _model.PlayerModel = new PlayerModel(new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), 300f);
        var playerView = new PlayerView(playerTexture, 100f, 100f, bulletTexture);
        _playerController = new PlayerController(_model.PlayerModel, playerView);
        _model.BotModels = new List<BotModel>
        {
            new BotModel(new Vector2(100, 100)),
            new BotModel(new Vector2(500, 200)),
            new BotModel(new Vector2(800, 500))
        };
        _botView = new BotView(botTexture);
        _model.BotControllers = new List<BotController>();
        foreach (var botModel in _model.BotModels)
        {
            _model.BotControllers.Add(new BotController(botModel));
        }
        int buttonWidth = 200;
        int buttonHeight = 125;
        _model.StartButtonRect = new Rectangle(
            (_graphics.PreferredBackBufferWidth - buttonWidth) / 2,
            (_graphics.PreferredBackBufferHeight - buttonHeight) / 2 - 150,
            buttonWidth,
            buttonHeight
        );
        _model.SettingsButtonRect = new Rectangle(
            (_graphics.PreferredBackBufferWidth - buttonWidth) / 2,
            (_graphics.PreferredBackBufferHeight - buttonHeight) / 2,
            buttonWidth,
            buttonHeight
        );
        _model.ExitButtonRect = new Rectangle(
            (_graphics.PreferredBackBufferWidth - buttonWidth) / 2,
            (_graphics.PreferredBackBufferHeight - buttonHeight) / 2 + 150,
            buttonWidth,
            buttonHeight
        );

        // Создание 1x1 пиксельной текстуры для отрисовки препятствий
        _model.PixelTexture = new Texture2D(_graphics.GraphicsDevice, 1, 1);
        _model.PixelTexture.SetData(new[] { Color.White });

        // Пример препятствий для теста (можно редактировать координаты и размеры)
        _model.Obstacles.Add(new Rectangle(0, 0, 100, 650));
        _model.Obstacles.Add(new Rectangle(100, 100, 100, 500));
        _model.Obstacles.Add(new Rectangle(200, 100, 80, 470));
        _model.Obstacles.Add(new Rectangle(300, 100, 120, 420));
        _model.Obstacles.Add(new Rectangle(400, 100, 80, 320));
        _model.Obstacles.Add(new Rectangle(480, 100, 60, 270));
        _model.Obstacles.Add(new Rectangle(540, 100, 60, 200));
        _model.Obstacles.Add(new Rectangle(540, 100, 60, 200));
        _model.Obstacles.Add(new Rectangle(640, 100, 100, 150));
        _model.Obstacles.Add(new Rectangle(740, 0, 80, 200));
        _model.Obstacles.Add(new Rectangle(840, 0, 80, 130));

        //Нижняя пропость
        _model.Obstacles.Add(new Rectangle(2000, 1100, 100, 500));
        _model.Obstacles.Add(new Rectangle(1900, 1000, 80, 500));
        _model.Obstacles.Add(new Rectangle(1800, 920, 100, 500));
        _model.Obstacles.Add(new Rectangle(1700, 920, 100, 500));
        _model.Obstacles.Add(new Rectangle(1600, 1100, 100, 500));
        _model.Obstacles.Add(new Rectangle(1550, 1300, 50, 500));
    }

    public void Update(GameTime gameTime)
    {
        // Управление состояниями игры
        var keyboardState = Keyboard.GetState();
        var gamePadState = GamePad.GetState(PlayerIndex.One);
        MouseState currentMouseState = Mouse.GetState();

        switch (_model.CurrentState)
        {
            case GameState.Intro:
                _model.IntroTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_model.IntroTimer >= GameModel.IntroDuration ||
                    keyboardState.GetPressedKeys().Length > 0 ||
                    currentMouseState.LeftButton == ButtonState.Pressed ||
                    gamePadState.Buttons.Start == ButtonState.Pressed)
                {
                    _model.CurrentState = GameState.Menu;
                }
                break;
            case GameState.Menu:
                bool isStartHovered = _model.StartButtonRect.Contains(currentMouseState.Position);
                bool isSettingsHovered = _model.SettingsButtonRect.Contains(currentMouseState.Position);
                bool isExitHovered = _model.ExitButtonRect.Contains(currentMouseState.Position);

                if (isStartHovered && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    _model.CurrentState = GameState.Playing;
                }
                else if (isExitHovered && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    System.Environment.Exit(0);
                }
                if (keyboardState.IsKeyDown(Keys.Enter) || gamePadState.Buttons.Start == ButtonState.Pressed)
                {
                    _model.CurrentState = GameState.Playing;
                }
                break;
            case GameState.Playing:
                _playerController.Update(gameTime, _graphics, _model.Obstacles);
                for (int i = _model.BotControllers.Count - 1; i >= 0; i--)
                {
                    var botController = _model.BotControllers[i];
                    botController.Update(gameTime, _model.PlayerModel.Position, _model.BotModels.ToArray(), _model.SpaceBetweenBots);

                    // Проверка столкновения игрока с ботом
                    Rectangle playerRect = new Rectangle(
                        (int)(_model.PlayerModel.Position.X - 50),
                        (int)(_model.PlayerModel.Position.Y - 50),
                        100, 100);
                    Rectangle botRect = new Rectangle(
                        (int)(_model.BotModels[i].Position.X - 50),
                        (int)(_model.BotModels[i].Position.Y - 50),
                        100, 100);
                    if (playerRect.Intersects(botRect))
                    {
                        System.Environment.Exit(0);
                    }
                    // Проверка столкновения пуль с ботами
                    for (int j = _model.PlayerModel.Bullets.Count - 1; j >= 0; j--)
                    {
                        var bullet = _model.PlayerModel.Bullets[j];
                        Rectangle bulletRect = new Rectangle(
                            (int)(bullet.Position.X - 7.5f),
                            (int)(bullet.Position.Y - 7.5f),
                            15, 15);
                        if (bulletRect.Intersects(botRect))
                        {
                            _model.BotControllers.RemoveAt(i);
                            _model.BotModels.RemoveAt(i);
                            _model.PlayerModel.Bullets.RemoveAt(j);
                            break;
                        }
                    }
                }
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    _model.CurrentState = GameState.Menu;
                }
                break;
        }
        _previousMouseState = currentMouseState;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _view.Draw(spriteBatch, _model, _playerController, _botView);
    }
}