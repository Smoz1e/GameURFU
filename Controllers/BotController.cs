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
        Vector2 newDirection = playerPosition - _model.Position;
        if (newDirection.Length() > 0)
        {
            newDirection.Normalize();
        }

        // Проверяем пересечение с другими ботами
        foreach (var bot in otherBots)
        {
            if (bot != _model && Vector2.Distance(_model.Position, bot.Position) < spaceBetweenBots)
            {
                Vector2 avoidDirection = _model.Position - bot.Position;
                if (avoidDirection.Length() > 0)
                {
                    avoidDirection.Normalize();
                }
                newDirection += avoidDirection;
                _model.Position += avoidDirection * (spaceBetweenBots - Vector2.Distance(_model.Position, bot.Position));
            }
        }

        if (newDirection.Length() > 0)
        {
            newDirection.Normalize();
        }

        _model.Direction = newDirection;

        // Предполагаемое новое положение
        Vector2 newPos = _model.Position + _model.Direction * _model.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        float botRadius = 50f;
        bool collision = false;
        foreach (var obs in obstacles)
        {
            if (CircleIntersectsRectangle(newPos, botRadius, obs))
            {
                collision = true;
                break;
            }
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