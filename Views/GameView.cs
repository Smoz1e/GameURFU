using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class GameView
{
    public void Draw(SpriteBatch spriteBatch, GameModel model, PlayerController playerController, BotView botView)
    {
        spriteBatch.Begin();
        if (model.CurrentState == GameState.Intro)
        {
            spriteBatch.Draw(model.IntroImage, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.White);
        }
        else if (model.CurrentState == GameState.Menu)
        {
            spriteBatch.Draw(model.MenuBackground, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.White);
            Color startButtonColor = model.StartButtonRect.Contains(Mouse.GetState().Position) ? Color.LightGray : Color.White;
            Color settingsButtonColor = model.SettingsButtonRect.Contains(Mouse.GetState().Position) ? Color.LightGray : Color.White;
            Color exitButtonColor = model.ExitButtonRect.Contains(Mouse.GetState().Position) ? Color.LightGray : Color.White;
            spriteBatch.Draw(model.StartButtonTexture, model.StartButtonRect, startButtonColor);
            spriteBatch.Draw(model.SettingsButtonTexture, model.SettingsButtonRect, settingsButtonColor);
            spriteBatch.Draw(model.ExitButtonTexture, model.ExitButtonRect, exitButtonColor);
        }
        else if (model.CurrentState == GameState.Playing)
        {
            spriteBatch.Draw(model.BackgroundTexture, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.White);
            foreach (var botModel in model.BotModels)
            {
                botView.Draw(spriteBatch, botModel, 100f, 100f);
            }
            playerController.Draw(spriteBatch);
        }
        spriteBatch.End();
    }
}
