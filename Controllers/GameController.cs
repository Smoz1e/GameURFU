using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

public static class GameControllerStatic
{
    public static Func<Vector2, float, bool> IsCollisionStatic;
}

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
        GameControllerStatic.IsCollisionStatic = IsCollision;
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
        _model.CrosshairTexture = Content.Load<Texture2D>("crosshairs_green1");
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

        _model.CollisionMapTexture = Content.Load<Texture2D>("collisionMap"); 
        _model.CollisionMaskWidth = _model.CollisionMapTexture.Width;
        _model.CollisionMaskHeight = _model.CollisionMapTexture.Height;
        _model.CollisionMaskData = new Color[_model.CollisionMaskWidth * _model.CollisionMaskHeight];
        _model.CollisionMapTexture.GetData(_model.CollisionMaskData);
        _model.Obstacles.Clear(); 
    }

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
                    HandleDifficultyModalClick(currentMouseState);
                }
                else if (_model.IsGameInfoModalOpen)
                {
                    HandleGameInfoModalClick(currentMouseState);
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
                float baseScreenHeight = 1080f;
                float scale = _graphics.PreferredBackBufferHeight / baseScreenHeight;
                _model.PlayerModel.ColliderRadius = 25f * scale;
                foreach (var bot in _model.BotModels)
                {
                    bot.ColliderRadius = 25f * scale;
                }
                _playerController.Update(gameTime, _graphics, _model.Obstacles);
                CheckAmmunitionPickup();
                for (int i = _model.BotControllers.Count - 1; i >= 0; i--)
                {
                    var botController = _model.BotControllers[i];
                    botController.Update(gameTime, _model.PlayerModel.Position, _model.BotModels.ToArray(), _model.SpaceBetweenBots, _model.Obstacles);

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
                    for (int j = _model.PlayerModel.Bullets.Count - 1; j >= 0; j--)
                    {
                        var bullet = _model.PlayerModel.Bullets[j];
                        float bulletRadius = 7.5f; 
                        float botRadius = _model.BotModels[i].ColliderRadius;
                        float distBullet = Vector2.Distance(bullet.Position, _model.BotModels[i].Position);
                        if (distBullet < bulletRadius + botRadius)
                        {
                            TrySpawnAmmunition(_model.BotModels[i].Position);
                            _model.BotControllers.RemoveAt(i);
                            _model.BotModels.RemoveAt(i);
                            _model.PlayerModel.Bullets.RemoveAt(j);
                            _model.BotsKilled++;
                            if (_model.BotControllers.Count == 0 && _ammoSpawnedThisWave < _ammoToSpawnThisWave)
                            {
                                int toSpawn = _ammoToSpawnThisWave - _ammoSpawnedThisWave;
                                for (int k = 0; k < toSpawn; k++)
                                {
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
                if (_model.BotControllers.Count == 0)
                {
                    if (_model.CurrentWave >= 7)
                    {
                        _model.IsVictory = true;
                    }
                    else
                    {
                        _model.CurrentWave++;
                        _model.BotsInWave++;
                        SpawnBotWave(_model.BotsInWave);
                    }
                }
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    _model.CurrentState = GameState.Menu;
                }
                break;
        }

        if (_model.IsVictory)
        {

            if (Keyboard.GetState().GetPressedKeys().Length > 0 || Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                ResetGameState(); 
                _model.CurrentState = GameState.Menu; 
            }
        }

        _previousMouseState = currentMouseState;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _view.Draw(spriteBatch, _model, _playerController, _botView);
    }

    private Random _rnd = new Random();

    private int botRadius = 50;
    private int _ammoToSpawnThisWave = 0;
    private int _ammoSpawnedThisWave = 0;

    private void TryForceSpawnAmmunition(Vector2 pos)
    {
        var ammo = new AmmunitionModel(pos);
        var controller = new AmmunitionController(ammo, _ammunitionView);
        _model.AmmunitionControllers.Add(controller);
        _ammoSpawnedThisWave++;
    }

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
                valid = !IsCollision(pos, botRadius);
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

    private void HandleGameInfoModalClick(MouseState mouseState)
    {
        int modalWidth = 700;
        int modalHeight = 350;
        int x = (_graphics.PreferredBackBufferWidth - modalWidth) / 2;
        int y = (_graphics.PreferredBackBufferHeight - modalHeight) / 2;
        int buttonWidth = 220;
        int buttonHeight = 60;
        int buttonX = x + (modalWidth - buttonWidth) / 2;
        int buttonY = y + modalHeight - buttonHeight - 30;
        Rectangle buttonRect = new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight);
        if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            if (buttonRect.Contains(mouseState.Position))
            {
                _model.IsGameInfoModalOpen = false;
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

    private void ResetGameState()
    {
        SetDifficultyParams();
        _model.CurrentWave = 1;
        _model.BotsInWave = 1;
        _model.BotsKilled = 0; 
        _model.PlayerModel.Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        _model.PlayerModel.Rotation = 0f;
        _model.PlayerModel.Bullets.Clear();
        _model.PlayerModel.Magazines = 4; 
        _model.PlayerModel.ShotsFired = 0;
        _model.PlayerModel.IsReloading = false;
        _model.PlayerModel.ReloadTimer = 0f;
        _model.PlayerModel.Health = PlayerModel.MaxHealth;
        _model.PlayerModel.IsDead = false;
        _ammoSpawnedThisWave = 0;
        _model.IsVictory = false; 
        _model.BotModels.Clear();
        _model.BotControllers.Clear();
        SpawnBotWave(_model.BotsInWave);
    }

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

    private bool IsCollision(Vector2 pos, float radius = 0)
    {
        if (_model.CollisionMaskData == null) return false;
        int w = _model.CollisionMaskWidth;
        int h = _model.CollisionMaskHeight;
        int px = (int)(pos.X / _graphics.PreferredBackBufferWidth * w);
        int py = (int)(pos.Y / _graphics.PreferredBackBufferHeight * h);
        if (px < 0 || py < 0 || px >= w || py >= h) return true;
        if (radius <= 1)
        {
            var c = _model.CollisionMaskData[py * w + px];
            return c.R > 200 && c.G > 200 && c.B > 200;
        }
        int rPix = (int)(radius / _graphics.PreferredBackBufferWidth * w);
        for (int dx = -rPix; dx <= rPix; dx++)
        for (int dy = -rPix; dy <= rPix; dy++)
        {
            int tx = px + dx;
            int ty = py + dy;
            if (tx < 0 || ty < 0 || tx >= w || ty >= h) continue;
            if (dx * dx + dy * dy <= rPix * rPix)
            {
                var c = _model.CollisionMaskData[ty * w + tx];
                if (c.R > 200 && c.G > 200 && c.B > 200) return true;
            }
        }
        return false;
    }
}