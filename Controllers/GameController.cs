using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

public class GameController
{
    private GameModel _model;
    private GameView _view;
    private PlayerController _playerController;
    private BotView _botView;
    private MouseState _previousMouseState;
    private GraphicsDeviceManager _graphics;
    private AmmunitionView _ammunitionView;

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
        _model.TextMenuFont = Content.Load<SpriteFont>("TextMenu");
        _model.GameTextFont = Content.Load<SpriteFont>("GameText");
        var botTexture = Content.Load<Texture2D>("bot");
        var playerTexture = Content.Load<Texture2D>("playerTest");
        var bulletTexture = Content.Load<Texture2D>("bullet");
        var ammunitionTexture = Content.Load<Texture2D>("Ammunition");
        _ammunitionView = new AmmunitionView(ammunitionTexture, 50f);
        _model.FullHeartTexture = Content.Load<Texture2D>("full_heath");
        _model.PlayerModel = new PlayerModel(new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), 300f);
        var playerView = new PlayerView(playerTexture, 100f, 100f, bulletTexture);
        _playerController = new PlayerController(_model.PlayerModel, playerView); _model.BotModels = new List<BotModel>();
        _botView = new BotView(botTexture);
        _model.BotControllers = new List<BotController>();
        SpawnBotWave(_model.BotsInWave);
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
        _model.Obstacles.Add(new Rectangle(300, 100, 120, 450));
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

        // Куст
        _model.Obstacles.Add(new Rectangle(1500, 970, 50, 50));

        // Палатки
        _model.Obstacles.Add(new Rectangle(110, 650, 150, 150));
       _model.Obstacles.Add(new Rectangle(380, 530, 160, 140));
    }

    // Проверка пересечения круга и прямоугольника
    private bool CircleIntersectsRectangle(Vector2 circleCenter, float radius, Rectangle rect)
    {
        float closestX = MathHelper.Clamp(circleCenter.X, rect.Left, rect.Right);
        float closestY = MathHelper.Clamp(circleCenter.Y, rect.Top, rect.Bottom);
        float dx = circleCenter.X - closestX;
        float dy = circleCenter.Y - closestY;
        return (dx * dx + dy * dy) < (radius * radius);
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

                if (_model.IsSettingsModalOpen)
                {
                    // Обработка кликов по модальному окну выбора сложности
                    HandleDifficultyModalClick(currentMouseState);
                }
                else
                {
                    if (isStartHovered && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                    {
                        ResetGameState();
                        _model.CurrentState = GameState.Playing;
                    }
                    else if (isSettingsHovered && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                    {
                        _model.IsSettingsModalOpen = true;
                    }
                    else if (isExitHovered && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                    {
                        System.Environment.Exit(0);
                    }
                    if (keyboardState.IsKeyDown(Keys.Enter) || gamePadState.Buttons.Start == ButtonState.Pressed)
                    {
                        ResetGameState();
                        _model.CurrentState = GameState.Playing;
                    }
                }
                break;
            case GameState.Playing:
                _playerController.Update(gameTime, _graphics, _model.Obstacles);
                CheckAmmunitionPickup();
                for (int i = _model.BotControllers.Count - 1; i >= 0; i--)
                {
                    var botController = _model.BotControllers[i];
                    botController.Update(gameTime, _model.PlayerModel.Position, _model.BotModels.ToArray(), _model.SpaceBetweenBots, _model.Obstacles);

                    // Проверка столкновения игрока с ботом (по кругам)
                    float dist = Vector2.Distance(_model.PlayerModel.Position, _model.BotModels[i].Position);
                    float sumRadius = _model.PlayerModel.ColliderRadius + _model.BotModels[i].ColliderRadius;
                    if (dist < sumRadius)
                    {
                        _model.PlayerModel.TakeDamage(1);
                        if (_model.PlayerModel.IsDead)
                        {
                            _model.CurrentState = GameState.Menu;
                        }
                    }
                    // Проверка столкновения пуль с ботами (по кругам)
                    for (int j = _model.PlayerModel.Bullets.Count - 1; j >= 0; j--)
                    {
                        var bullet = _model.PlayerModel.Bullets[j];
                        float bulletRadius = 7.5f; // радиус пули (можно вынести в модель)
                        float botRadius = _model.BotModels[i].ColliderRadius;
                        float distBullet = Vector2.Distance(bullet.Position, _model.BotModels[i].Position);
                        if (distBullet < bulletRadius + botRadius)
                        {
                            // Спавн боеприпаса с шансом (1-3 за волну)
                            TrySpawnAmmunition(_model.BotModels[i].Position);
                            _model.BotControllers.RemoveAt(i);
                            _model.BotModels.RemoveAt(i);
                            _model.PlayerModel.Bullets.RemoveAt(j);
                            _model.BotsKilled++;
                            // Если это был последний бот в волне, доспавнить недостающие боеприпасы
                            if (_model.BotControllers.Count == 0 && _ammoSpawnedThisWave < _ammoToSpawnThisWave)
                            {
                                int toSpawn = _ammoToSpawnThisWave - _ammoSpawnedThisWave;
                                for (int k = 0; k < toSpawn; k++)
                                {
                                    // Спавним боеприпас в случайной позиции на экране
                                    Vector2 spawnPos = new Vector2(
                                        _rnd.Next(60, _graphics.PreferredBackBufferWidth - 60),
                                        _rnd.Next(60, _graphics.PreferredBackBufferHeight - 60)
                                    );
                                    TryForceSpawnAmmunition(spawnPos);
                                }
                            }
                            break;
                        }
                    }
                }
                // Проверка на новую волну
                if (_model.BotControllers.Count == 0)
                {
                    _model.CurrentWave++;
                    _model.BotsInWave++;
                    SpawnBotWave(_model.BotsInWave);
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

    private Random _rnd = new Random();

    private int botRadius = 50;
    private int minBotDistanceToObstacle = 100;
    private int _ammoToSpawnThisWave = 0;
    private int _ammoSpawnedThisWave = 0;

    // Новый метод для форсированного спавна боеприпаса (без проверки лимита)
    private void TryForceSpawnAmmunition(Vector2 pos)
    {
        var ammo = new AmmunitionModel(pos);
        var controller = new AmmunitionController(ammo, _ammunitionView);
        _model.AmmunitionControllers.Add(controller);
        _ammoSpawnedThisWave++;
    }

    // Исправленный TrySpawnAmmunition (без инициализации лимита)
    private void TrySpawnAmmunition(Vector2 pos)
    {
        if (_ammoSpawnedThisWave < _ammoToSpawnThisWave)
        {
            var ammo = new AmmunitionModel(pos);
            var controller = new AmmunitionController(ammo, _ammunitionView);
            _model.AmmunitionControllers.Add(controller);
            _ammoSpawnedThisWave++;
        }
    }

    private void SpawnBotWave(int count)
    {
        _model.BotModels.Clear();
        _model.BotControllers.Clear();
        _model.AmmunitionControllers.Clear();
        _ammoToSpawnThisWave = _rnd.Next(1, 4);
        _ammoSpawnedThisWave = 0;
        int botsToSpawn = _model.BotsStartCount + (_model.CurrentWave - 1) * _model.BotsPerWave;
        float speedMultiplier = 1f + (_model.CurrentWave - 1) * (_model.BotSpeedMultiplier - 1f) / 5f;
        if (_model.SelectedDifficulty == DifficultyLevel.Easy)
            speedMultiplier = 1f;
        else if (_model.SelectedDifficulty == DifficultyLevel.Medium)
            speedMultiplier = 1f + (_model.CurrentWave - 1) * 0.3f;
        else if (_model.SelectedDifficulty == DifficultyLevel.Hard)
            speedMultiplier = 1f + (_model.CurrentWave - 1) * 0.6f;
        for (int b = 0; b < botsToSpawn; b++)
        {
            Vector2 pos;
            bool valid;
            int attempts = 0;
            do
            {
                pos = new Vector2(
                    _rnd.Next(botRadius, _graphics.PreferredBackBufferWidth - botRadius),
                    _rnd.Next(botRadius, _graphics.PreferredBackBufferHeight - botRadius)
                );
                valid = true;
                foreach (var obs in _model.Obstacles)
                {
                    Rectangle expanded = new Rectangle(
                        obs.X - minBotDistanceToObstacle - botRadius,
                        obs.Y - minBotDistanceToObstacle - botRadius,
                        obs.Width + 2 * (minBotDistanceToObstacle + botRadius),
                        obs.Height + 2 * (minBotDistanceToObstacle + botRadius)
                    );
                    if (CircleIntersectsRectangle(pos, 1, expanded))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    foreach (var other in _model.BotModels)
                    {
                        if (Vector2.Distance(pos, other.Position) < botRadius * 2)
                        {
                            valid = false;
                            break;
                        }
                    }
                    int minBotDistanceToPlayer = 400; 
                    if (Vector2.Distance(pos, _model.PlayerModel.Position) < minBotDistanceToPlayer)
                    {
                        valid = false;
                    }
                }
                attempts++;
            } while (!valid && attempts < 100);
            var bot = new BotModel(pos);
            bot.Speed *= speedMultiplier;
            _model.BotModels.Add(bot);
            _model.BotControllers.Add(new BotController(bot));
        }
    }

    // Обработка клика по модальному окну выбора сложности
    private void HandleDifficultyModalClick(MouseState mouseState)
    {
        int modalWidth = 500;
        int modalHeight = 350;
        int x = (_graphics.PreferredBackBufferWidth - modalWidth) / 2;
        int y = (_graphics.PreferredBackBufferHeight - modalHeight) / 2;
        int buttonWidth = 400;
        int buttonHeight = 70;
        int spacing = 20;
        Rectangle easyRect = new Rectangle(x + 50, y + 60, buttonWidth, buttonHeight);
        Rectangle mediumRect = new Rectangle(x + 50, y + 60 + buttonHeight + spacing, buttonWidth, buttonHeight);
        Rectangle hardRect = new Rectangle(x + 50, y + 60 + 2 * (buttonHeight + spacing), buttonWidth, buttonHeight);
        if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            if (easyRect.Contains(mouseState.Position))
            {
                _model.SelectedDifficulty = DifficultyLevel.Easy;
                _model.IsSettingsModalOpen = false;
            }
            else if (mediumRect.Contains(mouseState.Position))
            {
                _model.SelectedDifficulty = DifficultyLevel.Medium;
                _model.IsSettingsModalOpen = false;
            }
            else if (hardRect.Contains(mouseState.Position))
            {
                _model.SelectedDifficulty = DifficultyLevel.Hard;
                _model.IsSettingsModalOpen = false;
            }
        }
    }

    private void SetDifficultyParams()
    {
        switch (_model.SelectedDifficulty)
        {
            case DifficultyLevel.Easy:
                _model.BotsStartCount = 1;
                _model.BotsPerWave = 1;
                _model.BotSpeedMultiplier = 1f;
                break;
            case DifficultyLevel.Medium:
                _model.BotsStartCount = 3;
                _model.BotsPerWave = 2;
                _model.BotSpeedMultiplier = 1.5f;
                break;
            case DifficultyLevel.Hard:
                _model.BotsStartCount = 5;
                _model.BotsPerWave = 3;
                _model.BotSpeedMultiplier = 2.2f;
                break;
        }
    }

    // Сброс состояния игры при старте новой игры
    private void ResetGameState()
    {
        SetDifficultyParams();
        _model.CurrentWave = 1;
        _model.BotsInWave = 1;
        _model.BotsKilled = 0; // Сброс счетчика убитых ботов
        // Сброс игрока
        _model.PlayerModel.Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        _model.PlayerModel.Rotation = 0f;
        _model.PlayerModel.Bullets.Clear();
        _model.PlayerModel.Magazines = 4; // стартовое значение
        _model.PlayerModel.ShotsFired = 0;
        _model.PlayerModel.IsReloading = false;
        _model.PlayerModel.ReloadTimer = 0f;
        _model.PlayerModel.Health = PlayerModel.MaxHealth;
        _model.PlayerModel.IsDead = false;
        // Сброс ботов
        _model.BotModels.Clear();
        _model.BotControllers.Clear();
        SpawnBotWave(_model.BotsInWave);
    }

    // Проверка подбора боеприпасов игроком
    private void CheckAmmunitionPickup()
    {
        int pickedUp = 0;
        for (int i = _model.AmmunitionControllers.Count - 1; i >= 0; i--)
        {
            var ammo = _model.AmmunitionControllers[i];
            if (!ammo.Model.IsCollected && Vector2.Distance(_model.PlayerModel.Position, ammo.Model.Position) < 60f)
            {
                pickedUp++;
                ammo.Model.IsCollected = true;
                _model.AmmunitionControllers.RemoveAt(i);
            }
        }
        if (pickedUp > 0)
        {
            _model.PlayerModel.Magazines = Math.Min(_model.PlayerModel.Magazines + pickedUp, PlayerModel.MaxMagazines);
        }
    }
}