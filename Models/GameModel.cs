using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public enum GameState { Intro, Menu, Playing }

public class GameModel
{
    public GameState CurrentState = GameState.Intro;
    public float IntroTimer = 0f;
    public const float IntroDuration = 3f;

    public PlayerModel PlayerModel;
    public List<BotModel> BotModels = new List<BotModel>();
    public List<BotController> BotControllers = new List<BotController>();
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
}
