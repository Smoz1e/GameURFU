using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public enum GameState { Intro, Menu, Playing }
public enum DifficultyLevel { Easy, Medium, Hard }

public class GameModel
{
    public GameState CurrentState = GameState.Intro;
    public float IntroTimer = 0f;
    public const float IntroDuration = 3f;

    public PlayerModel PlayerModel;
    public List<BotModel> BotModels = new List<BotModel>();
    public List<BotController> BotControllers = new List<BotController>();
    public List<AmmunitionController> AmmunitionControllers = new List<AmmunitionController>();
    public float SpaceBetweenBots = 100f;

    public List<Rectangle> Obstacles = new List<Rectangle>();

    public Texture2D IntroImage;
    public Texture2D BackgroundTexture;
    public Texture2D MenuBackground;
    public Texture2D StartButtonTexture;
    public Texture2D SettingsButtonTexture;
    public Texture2D ExitButtonTexture;
    public Rectangle StartButtonRect;
    public Rectangle SettingsButtonRect;
    public Rectangle ExitButtonRect;
    public Texture2D PixelTexture; // 1x1 пиксель для отрисовки препятствий
    public SpriteFont DebugFont;
    public SpriteFont TextMenuFont;
    public SpriteFont GameTextFont;

    public int CurrentWave = 1;
    public int BotsInWave = 1;

    public DifficultyLevel SelectedDifficulty = DifficultyLevel.Easy;
    public int BotsStartCount = 1;
    public int BotsPerWave = 1;
    public float BotSpeedMultiplier = 1f;
    public bool IsSettingsModalOpen = false;
    public int BotsKilled = 0;
    public Texture2D FullHeartTexture; // Текстура сердечка для отображения рядом с полоской здоровья
}
