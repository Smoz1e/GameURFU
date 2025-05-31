using System;
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

            if (model.IsSettingsModalOpen)
            {
                DrawDifficultyModal(spriteBatch, model);
            }
        }
        else if (model.CurrentState == GameState.Playing)
        {
            spriteBatch.Draw(model.BackgroundTexture, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.White);
            // Отрисовка препятствий красным цветом
            // foreach (var rect in model.Obstacles)
            // {
            //     int cornerRadius = 30; // радиус скругления углов
            //     // Основной прямоугольник (без углов)
            //     Rectangle coreRect = new Rectangle(rect.X + cornerRadius, rect.Y, rect.Width - 2 * cornerRadius, rect.Height);
            //     spriteBatch.Draw(model.PixelTexture, coreRect, Color.Red * 0.7f);
            //     coreRect = new Rectangle(rect.X, rect.Y + cornerRadius, rect.Width, rect.Height - 2 * cornerRadius);
            //     spriteBatch.Draw(model.PixelTexture, coreRect, Color.Red * 0.7f);
            //     // 4 скругленных угла
            //     DrawCircleQuarter(spriteBatch, model.PixelTexture, new Vector2(rect.Left + cornerRadius, rect.Top + cornerRadius), cornerRadius, Color.Red * 0.7f, 180, 270);
            //     DrawCircleQuarter(spriteBatch, model.PixelTexture, new Vector2(rect.Right - cornerRadius, rect.Top + cornerRadius), cornerRadius, Color.Red * 0.7f, 270, 360);
            //     DrawCircleQuarter(spriteBatch, model.PixelTexture, new Vector2(rect.Left + cornerRadius, rect.Bottom - cornerRadius), cornerRadius, Color.Red * 0.7f, 90, 180);
            //     DrawCircleQuarter(spriteBatch, model.PixelTexture, new Vector2(rect.Right - cornerRadius, rect.Bottom - cornerRadius), cornerRadius, Color.Red * 0.7f, 0, 90);
            // }
            foreach (var botModel in model.BotModels)
            {
                botView.Draw(spriteBatch, botModel, 100f, 100f);
            }
            playerController.Draw(spriteBatch);

            // Отрисовка номера волны без использования шрифта
            string waveText = $"Волна: {model.CurrentWave}";
            int x = 40, y = 40;
            int rectWidth = 200, rectHeight = 40;
            // Полупрозрачный фон
            spriteBatch.Draw(model.PixelTexture, new Rectangle(x - 10, y - 10, rectWidth, rectHeight), Color.Black * 0.5f);
            // "Псевдотекст": рисуем желтые прямоугольники по количеству волны (например, палочки)
            for (int i = 0; i < model.CurrentWave; i++)
            {
                spriteBatch.Draw(model.PixelTexture, new Rectangle(x + 10 + i * 20, y + 10, 10, 20), Color.Yellow);
            }
        }
        spriteBatch.End();
    }

    // Отрисовка четверти круга (для скругленных углов)
    private void DrawCircleQuarter(SpriteBatch spriteBatch, Texture2D pixel, Vector2 center, int radius, Color color, float startAngleDeg, float endAngleDeg)
    {
        int segments = 8;
        float startRad = MathHelper.ToRadians(startAngleDeg);
        float endRad = MathHelper.ToRadians(endAngleDeg);
        float increment = (endRad - startRad) / segments;
        Vector2 lastPoint = center + radius * new Vector2((float)Math.Cos(startRad), (float)Math.Sin(startRad));
        for (int i = 1; i <= segments; i++)
        {
            float angle = startRad + increment * i;
            Vector2 nextPoint = center + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            DrawLine(spriteBatch, pixel, lastPoint, nextPoint, color);
            lastPoint = nextPoint;
        }
    }

    // Вспомогательная функция для отрисовки линии
    private void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 end, Color color)
    {
        Vector2 edge = end - start;
        float angle = (float)Math.Atan2(edge.Y, edge.X);
        spriteBatch.Draw(texture, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), 2), null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
    }

    // Модальное окно выбора сложности
    private void DrawDifficultyModal(SpriteBatch spriteBatch, GameModel model)
    {
        int modalWidth = 500;
        int modalHeight = 350;
        int x = (spriteBatch.GraphicsDevice.Viewport.Width - modalWidth) / 2;
        int y = (spriteBatch.GraphicsDevice.Viewport.Height - modalHeight) / 2;
        // Полупрозрачный фон
        spriteBatch.Draw(model.PixelTexture, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.Black * 0.5f);
        // Белое окно
        spriteBatch.Draw(model.PixelTexture, new Rectangle(x, y, modalWidth, modalHeight), Color.White);
        // Кнопки
        int buttonWidth = 400;
        int buttonHeight = 70;
        int spacing = 20;
        Rectangle easyRect = new Rectangle(x + 50, y + 60, buttonWidth, buttonHeight);
        Rectangle mediumRect = new Rectangle(x + 50, y + 60 + buttonHeight + spacing, buttonWidth, buttonHeight);
        Rectangle hardRect = new Rectangle(x + 50, y + 60 + 2 * (buttonHeight + spacing), buttonWidth, buttonHeight);
        Color easyColor = model.SelectedDifficulty == DifficultyLevel.Easy ? Color.LightGreen : Color.LightGray;
        Color mediumColor = model.SelectedDifficulty == DifficultyLevel.Medium ? Color.Orange : Color.LightGray;
        Color hardColor = model.SelectedDifficulty == DifficultyLevel.Hard ? Color.Red : Color.LightGray;
        spriteBatch.Draw(model.PixelTexture, easyRect, easyColor);
        spriteBatch.Draw(model.PixelTexture, mediumRect, mediumColor);
        spriteBatch.Draw(model.PixelTexture, hardRect, hardColor);
        // Текст (можно заменить на отрисовку псевдотекста или добавить SpriteFont)
        // Здесь для простоты просто прямоугольники разного цвета
    }
}
