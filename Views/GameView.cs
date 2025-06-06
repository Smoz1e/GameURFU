using System;
using System.Collections.Generic;
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
            if (model.IsGameInfoModalOpen)
            {
                DrawGameInfoModal(spriteBatch, model);
            }
        }
        else if (model.CurrentState == GameState.Playing)
        {
            spriteBatch.Draw(model.BackgroundTexture, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.White);
            foreach (var botModel in model.BotModels)
            {
                botView.Draw(spriteBatch, botModel, 100f, 100f);
            }
            // Отрисовка боеприпасов
            foreach (var ammo in model.AmmunitionControllers)
            {
                ammo.Draw(spriteBatch);
            }
            playerController.Draw(spriteBatch);

            int barWidth =300;
            int barHeight = 40;
            int margin = 40;
            int x = spriteBatch.GraphicsDevice.Viewport.Width - barWidth - margin;
            int y = margin;
            float healthPercent = (float)model.PlayerModel.Health / PlayerModel.MaxHealth;
            Rectangle bgRect = new Rectangle(x, y, barWidth, barHeight);
            Rectangle fgRect = new Rectangle(x, y, (int)(barWidth * healthPercent), barHeight);
            spriteBatch.Draw(model.PixelTexture, bgRect, Color.DarkRed * 0.5f);
            spriteBatch.Draw(model.PixelTexture, fgRect, Color.Red);
            // Сердечко
            if (model.PixelTexture != null && model.GameTextFont != null)
            {
                Texture2D heartTexture = model.FullHeartTexture; 
                if (heartTexture != null)
                {
                    int heartSize = barHeight; 
                    int heartX = x - heartSize - 10; 
                    int heartY = y;
                    spriteBatch.Draw(heartTexture, new Rectangle(heartX, heartY, heartSize, heartSize), Color.White);
                }
            }
            if (model.GameTextFont != null)
            {
                string hpText = $"HP: {model.PlayerModel.Health}/{PlayerModel.MaxHealth}";
                Vector2 textSize = model.GameTextFont.MeasureString(hpText);
                Vector2 textPos = new Vector2(x + (barWidth - textSize.X) / 2, y + (barHeight - textSize.Y) / 2);
                spriteBatch.DrawString(model.GameTextFont, hpText, textPos, Color.White);
            }

            if (model.GameTextFont != null)
            {
                string waveText = $"Wave: {model.CurrentWave}";
                string killedText = $"Bots killed: {model.BotsKilled}";
                spriteBatch.DrawString(model.GameTextFont, waveText, new Vector2(40, 40), Color.Yellow);
                spriteBatch.DrawString(model.GameTextFont, killedText, new Vector2(40, 90), Color.Orange);

                int ammoLeft = PlayerModel.MaxShotsBeforeReload - model.PlayerModel.ShotsFired;
                string ammoText = $"Патроны: {ammoLeft}";
                spriteBatch.DrawString(model.GameTextFont, ammoText, new Vector2(40, 140), Color.White);

                string magText = $"Магазины: {model.PlayerModel.Magazines}";
                spriteBatch.DrawString(model.GameTextFont, magText, new Vector2(40, 180), Color.LightBlue);

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

                if (model.IsVictory)
                {
                    string victoryText = "Вы победили!";
                    Vector2 textSize = model.GameTextFont.MeasureString(victoryText);
                    Vector2 centerPos = new Vector2(
                        (spriteBatch.GraphicsDevice.Viewport.Width - textSize.X) / 2,
                        (spriteBatch.GraphicsDevice.Viewport.Height - textSize.Y) / 2 - 100
                    );
                    spriteBatch.DrawString(model.GameTextFont, victoryText, centerPos, Color.LimeGreen);
                }
            }

            if (model.GameTextFont != null)
            {
                string waveText = $"Wave: {model.CurrentWave}";
                string killedText = $"Bots killed: {model.BotsKilled}";
                spriteBatch.DrawString(model.GameTextFont, waveText, new Vector2(40, 40), Color.Yellow);
                spriteBatch.DrawString(model.GameTextFont, killedText, new Vector2(40, 90), Color.Orange);
                int ammoLeft = PlayerModel.MaxShotsBeforeReload - model.PlayerModel.ShotsFired;
                string ammoText = $"Патроны: {ammoLeft}";
                spriteBatch.DrawString(model.GameTextFont, ammoText, new Vector2(40, 140), Color.White);
                string magText = $"Магазины: {model.PlayerModel.Magazines}";
                spriteBatch.DrawString(model.GameTextFont, magText, new Vector2(40, 180), Color.LightBlue);
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
                if (model.IsVictory)
                {
                    string victoryText = "Вы победили!";
                    Vector2 textSize = model.GameTextFont.MeasureString(victoryText);
                    Vector2 centerPos = new Vector2(
                        (spriteBatch.GraphicsDevice.Viewport.Width - textSize.X) / 2,
                        (spriteBatch.GraphicsDevice.Viewport.Height - textSize.Y) / 2 - 100
                    );
                    spriteBatch.DrawString(model.GameTextFont, victoryText, centerPos, Color.LimeGreen);
                }
            }

            if (model.CrosshairTexture != null)
            {
                var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
                int crosshairSize = 20; // размер прицела
                int crosshairX = mouseState.X - crosshairSize / 2;
                int crosshairY = mouseState.Y - crosshairSize / 2;
                spriteBatch.Draw(model.CrosshairTexture, new Rectangle(crosshairX, crosshairY, crosshairSize, crosshairSize), Color.White);
            }
        }
        spriteBatch.End();
    }

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

    private void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 end, Color color)
    {
        Vector2 edge = end - start;
        float angle = (float)Math.Atan2(edge.Y, edge.X);
        spriteBatch.Draw(texture, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), 2), null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
    }

    private void DrawDifficultyModal(SpriteBatch spriteBatch, GameModel model)
    {
        int modalWidth = 500;
        int modalHeight = 350;
        int x = (spriteBatch.GraphicsDevice.Viewport.Width - modalWidth) / 2;
        int y = (spriteBatch.GraphicsDevice.Viewport.Height - modalHeight) / 2;
        spriteBatch.Draw(model.PixelTexture, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.Black * 0.5f);
        spriteBatch.Draw(model.PixelTexture, new Rectangle(x, y, modalWidth, modalHeight), Color.White);
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
        if (model.TextMenuFont != null)
        {
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
    }

    // Модальное окно смысла игры
    private void DrawGameInfoModal(SpriteBatch spriteBatch, GameModel model)
    {
        int modalWidth = 700;
        int modalHeight = 350;
        int x = (spriteBatch.GraphicsDevice.Viewport.Width - modalWidth) / 2;
        int y = (spriteBatch.GraphicsDevice.Viewport.Height - modalHeight) / 2;
        // Полупрозрачный фон
        spriteBatch.Draw(model.PixelTexture, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.Black * 0.5f);
        // Полупрозрачное белое окно
        spriteBatch.Draw(model.PixelTexture, new Rectangle(x, y, modalWidth, modalHeight), Color.White * 0.92f);
        // Текст смысла игры с переносами и более читаемым шрифтом
        SpriteFont infoFont = model.GameTextFont ?? model.GameTextFont;
        if (infoFont != null)
        {
            string info = model.GameInfoText;
            int padding = 40;
            int textAreaWidth = modalWidth - 2 * padding;
            var lines = WrapText(infoFont, info, textAreaWidth);
            float lineHeight = infoFont.LineSpacing;
            float totalTextHeight = lines.Count * lineHeight;
            float startY = y + 40 + (120 - totalTextHeight) / 2;
            for (int i = 0; i < lines.Count; i++)
            {
                Vector2 textSize = infoFont.MeasureString(lines[i]);
                Vector2 textPos = new Vector2(x + (modalWidth - textSize.X) / 2, startY + i * lineHeight);
                spriteBatch.DrawString(infoFont, lines[i], textPos, Color.Black);
            }
        }
        // Кнопка "Прочитал"
        int buttonWidth = 220;
        int buttonHeight = 60;
        int buttonX = x + (modalWidth - buttonWidth) / 2;
        int buttonY = y + modalHeight - buttonHeight - 30;
        Rectangle buttonRect = new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight);
        Color buttonColor = buttonRect.Contains(Mouse.GetState().Position) ? Color.LightGray : Color.Silver;
        spriteBatch.Draw(model.PixelTexture, buttonRect, buttonColor * 0.92f);
        if (infoFont != null)
        {
            string btnText = "Прочитал";
            Vector2 btnSize = infoFont.MeasureString(btnText);
            Vector2 btnPos = new Vector2(buttonX + (buttonWidth - btnSize.X) / 2, buttonY + (buttonHeight - btnSize.Y) / 2);
            spriteBatch.DrawString(infoFont, btnText, btnPos, Color.Black);
        }
    }

    // Вспомогательная функция для переноса текста по ширине
    private List<string> WrapText(SpriteFont font, string text, int maxWidth)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        string currentLine = "";
        foreach (var word in words)
        {
            string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
            if (font.MeasureString(testLine).X > maxWidth)
            {
                if (!string.IsNullOrEmpty(currentLine))
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    lines.Add(word);
                    currentLine = "";
                }
            }
            else
            {
                currentLine = testLine;
            }
        }
        if (!string.IsNullOrEmpty(currentLine))
            lines.Add(currentLine);
        return lines;
    }
}
