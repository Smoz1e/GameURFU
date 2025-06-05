using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

public class BotController
{
    private BotModel _model;

    public BotController(BotModel model)
    {
        _model = model;
    }

    public void Update(GameTime gameTime, Vector2 playerPosition, BotModel[] otherBots, float spaceBetweenBots, List<Rectangle> obstacles)
    {
        // Рассчитываем направление к игроку
        var toPlayer = playerPosition - _model.Position;
        var moveDir = Vector2.Zero;
        if (toPlayer.Length() > 0)
            moveDir = Vector2.Normalize(toPlayer);

        // Суммируем силы отталкивания от других ботов
        var repel = Vector2.Zero;
        int repelCount = 0;
        float repelRadius = spaceBetweenBots * 0.9f; // чуть меньше, чтобы не было "липкости"
        foreach (var bot in otherBots)
        {
            if (bot != _model)
            {
                float dist = Vector2.Distance(_model.Position, bot.Position);
                if (dist < repelRadius && dist > 0.01f)
                {
                    var away = _model.Position - bot.Position;
                    away.Normalize();
                    // Чем ближе — тем сильнее отталкивание
                    float force = (repelRadius - dist) / repelRadius;
                    repel += away * force;
                    repelCount++;
                }
            }
        }
        if (repelCount > 0)
        {
            repel /= repelCount;
            // Усиливаем эффект
            repel *= 1.5f;
        }

        // Итоговое направление: к игроку + отталкивание
        var finalDir = moveDir + repel;
        if (finalDir.Length() > 0)
            finalDir.Normalize();
        _model.Direction = finalDir;

        // Предполагаемое новое положение
        var newPos = _model.Position + _model.Direction * _model.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        float botRadius = _model.ColliderRadius;
        bool collision = false;
        if (GameControllerStatic.IsCollisionStatic != null)
        {
            collision = GameControllerStatic.IsCollisionStatic(newPos, botRadius);
        }
        if (!collision)
        {
            _model.Position = newPos;
        }
        // Обновляем угол поворота
        if (_model.Direction.Length() > 0)
            _model.Rotation = (float)Math.Atan2(_model.Direction.Y, _model.Direction.X);
    }

    private bool CircleIntersectsRectangle(Vector2 circleCenter, float radius, Rectangle rect)
    {
        float closestX = MathHelper.Clamp(circleCenter.X, rect.Left, rect.Right);
        float closestY = MathHelper.Clamp(circleCenter.Y, rect.Top, rect.Bottom);
        float dx = circleCenter.X - closestX;
        float dy = circleCenter.Y - closestY;
        return (dx * dx + dy * dy) < (radius * radius);
    }
}