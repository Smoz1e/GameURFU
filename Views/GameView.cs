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

            // Текстовая отрисовка волны и убитых ботов
            if (model.GameTextFont != null)
            {
                string waveText = $"Wave: {model.CurrentWave}";
                string killedText = $"Bots killed: {model.BotsKilled}";
                spriteBatch.DrawString(model.GameTextFont, waveText, new Vector2(40, 40), Color.Yellow);
                spriteBatch.DrawString(model.GameTextFont, killedText, new Vector2(40, 90), Color.Orange);
                // Отображение оставшихся патронов
                int ammoLeft = PlayerModel.MaxShotsBeforeReload - model.PlayerModel.ShotsFired;
                string ammoText = $"Патроны: {ammoLeft}";
                spriteBatch.DrawString(model.GameTextFont, ammoText, new Vector2(40, 140), Color.White);
                // Надпись по центру экрана при перезарядке
                if (model.PlayerModel.IsReloading)
                {
                    string reloadText = "Перезаряжаюсь";
                    Vector2 textSize = model.GameTextFont.MeasureString(reloadText);
                    Vector2 centerPos = new Vector2(
                        (spriteBatch.GraphicsDevice.Viewport.Width - textSize.X) / 2,
                        (spriteBatch.GraphicsDevice.Viewport.Height - textSize.Y) / 2
                    );
                    spriteBatch.DrawString(model.GameTextFont, reloadText, centerPos, Color.Red);
                }
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
        // Надписи уровней сложности с новым шрифтом
        if (model.TextMenuFont != null)
        {
            // Центрирование текста по кнопке
            Vector2 easySize = model.TextMenuFont.MeasureString("Easy");
            Vector2 mediumSize = model.TextMenuFont.MeasureString("Medium");
            Vector2 hardSize = model.TextMenuFont.MeasureString("Hard");
            Vector2 easyPos = new Vector2(easyRect.X + (easyRect.Width - easySize.X) / 2, easyRect.Y + (easyRect.Height - easySize.Y) / 2);
            Vector2 mediumPos = new Vector2(mediumRect.X + (mediumRect.Width - mediumSize.X) / 2, mediumRect.Y + (mediumRect.Height - mediumSize.Y) / 2);
            Vector2 hardPos = new Vector2(hardRect.X + (hardRect.Width - hardSize.X) / 2, hardRect.Y + (hardRect.Height - hardSize.Y) / 2);
            spriteBatch.DrawString(model.TextMenuFont, "Easy", easyPos, Color.Black);
            spriteBatch.DrawString(model.TextMenuFont, "Medium", mediumPos, Color.Black);
            spriteBatch.DrawString(model.TextMenuFont, "Hard", hardPos, Color.Black);
        }
        // Текст (можно заменить на отрисовку псевдотекста или добавить SpriteFont)
        // Здесь для простоты просто прямоугольники разного цвета
    }
}
