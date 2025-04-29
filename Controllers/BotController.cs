using Microsoft.Xna.Framework;
using System;

public class BotController
{
    private BotModel _model;

    public BotController(BotModel model)
    {
        _model = model;
    }

    public void Update(GameTime gameTime, Vector2 playerPosition, BotModel[] otherBots, float spaceBetweenBots)
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
                // Отталкиваем бота в противоположную сторону
                Vector2 avoidDirection = _model.Position - bot.Position;
                if (avoidDirection.Length() > 0)
                {
                    avoidDirection.Normalize();
                }
                newDirection += avoidDirection;

                // Корректируем позицию для предотвращения наложения
                _model.Position += avoidDirection * (spaceBetweenBots - Vector2.Distance(_model.Position, bot.Position));
            }
        }

        if (newDirection.Length() > 0)
        {
            newDirection.Normalize();
        }

        _model.Direction = newDirection;

        // Обновляем позицию бота
        _model.Position += _model.Direction * _model.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Рассчитываем угол поворота
        _model.Rotation = (float)Math.Atan2(_model.Direction.Y, _model.Direction.X);
    }
}