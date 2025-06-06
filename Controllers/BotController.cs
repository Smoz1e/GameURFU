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

    private List<Vector2> _path = null;
    private int _pathIndex = 0;
    private static Dictionary<(Point, Point), List<Vector2>> _pathCache = new();
    private static AStarPathfinder _astar = null;
    private float _lastPathfindTime = 0f;
    private float _stuckTime = 0f;
    private Vector2 _lastPosition = Vector2.Zero;
    private const float PathfindCooldown = 0.5f;  // частота пересчёта пути
    private const float StuckThreshold = 0.3f; //  время за которое бот считается застрявшим 
    private const float MinMoveDist = 2f;
    private static float _cellSize = 56f; // крупнее клетка для ускорения

    public static void InitAStar(int width, int height, float cellSize, Func<Vector2, float, bool> isCollision)
    {
        _cellSize = cellSize;
        _astar = new AStarPathfinder(width, height, cellSize, isCollision);
    }

    public void Update(GameTime gameTime, Vector2 playerPosition, BotModel[] otherBots, float spaceBetweenBots, List<Rectangle> obstacles)
    {
        if (_astar == null)
        {
            // Инициализация A при первом апдейте 
            _astar = new AStarPathfinder(1920, 1080, _cellSize, GameControllerStatic.IsCollisionStatic);
        }
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if ((_model.Position - _lastPosition).Length() < MinMoveDist)
            _stuckTime += elapsed;
        else
            _stuckTime = 0f;
        _lastPosition = _model.Position;
        _lastPathfindTime += elapsed;
        bool needPath = false;
        float botRadius = _model.ColliderRadius;
        Vector2 toPlayer = playerPosition - _model.Position;
        if (_path == null || _pathIndex >= (_path?.Count ?? 0))
            needPath = true;
        else {
            Vector2 next = _path[_pathIndex];
            if (GameControllerStatic.IsCollisionStatic != null && GameControllerStatic.IsCollisionStatic(next, botRadius))
                needPath = true;
        }
        // Пересчёт пути только если бот застрял или путь устарел не чаще PathfindCooldown
        if ((needPath && _lastPathfindTime > PathfindCooldown) || (_stuckTime > StuckThreshold && _lastPathfindTime > PathfindCooldown))
        {
            var key = (ToGridPoint(_model.Position), ToGridPoint(playerPosition));
            if (!_pathCache.TryGetValue(key, out _path))
            {
                _path = _astar.FindPath(_model.Position, playerPosition, botRadius);
                if (_path != null)
                    _pathCache[key] = _path;
            }
            _pathIndex = 0;
            _lastPathfindTime = 0f;
            _stuckTime = 0f;
        }
        // Движение по пути
        Vector2 moveDir = Vector2.Zero;
        if (_path != null && _pathIndex < _path.Count)
        {
            Vector2 target = _path[_pathIndex];
            Vector2 toTarget = target - _model.Position;
            if (toTarget.Length() < _cellSize * 0.5f)
            {
                _pathIndex++;
                if (_pathIndex < _path.Count)
                    target = _path[_pathIndex];
                toTarget = target - _model.Position;
            }
            if (toTarget.Length() > 0)
                moveDir = Vector2.Normalize(toTarget);
        }
        else
        {
            // fallback: по прямой к игроку
            if (toPlayer.Length() > 0)
                moveDir = Vector2.Normalize(toPlayer);
        }
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
            repel *= 1.5f;
        }

        // Итоговое направление: к игроку + отталкивание
        var finalDir = moveDir + repel;
        if (finalDir.Length() > 0)
            finalDir.Normalize();
        _model.Direction = finalDir;

        // Предполагаемое новое положение
        float margin = 30f; // отступ для раннего обхода
        bool usingAstar = _path != null && _pathIndex < (_path?.Count ?? 0);
        // Проверка: если бот близко к препятствию и НЕ по A* — пересчитать путь через A*
        if (!usingAstar)
        {
            var lookAheadPos = _model.Position + _model.Direction * (botRadius + margin);
            bool tooClose = false;
            if (GameControllerStatic.IsCollisionStatic != null)
            {
                tooClose = GameControllerStatic.IsCollisionStatic(lookAheadPos, botRadius);
            }
            if (tooClose && _lastPathfindTime > PathfindCooldown)
            {
                // Сбросить путь, чтобы A* пересчитал обход заранее
                _path = null;
                _pathIndex = 0;
                _lastPathfindTime = 0f;
            }
        }
        // Проверка столкновения для реального движения
        var newPos = _model.Position + _model.Direction * _model.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        bool collision = false;
        if (GameControllerStatic.IsCollisionStatic != null)
        {
            collision = GameControllerStatic.IsCollisionStatic(newPos, botRadius);
        }
        if (!collision)
        {
            _model.Position = newPos;
        }
        
        if (_model.Direction.Length() > 0)
            _model.Rotation = (float)Math.Atan2(_model.Direction.Y, _model.Direction.X);
    }

    private static Point ToGridPoint(Vector2 v)
    {
        return new Point((int)(v.X / _cellSize), (int)(v.Y / _cellSize));
    }

    private static Point ToPoint(Vector2 v) => new Point((int)v.X, (int)v.Y);

    private bool CircleIntersectsRectangle(Vector2 circleCenter, float radius, Rectangle rect)
    {
        float closestX = MathHelper.Clamp(circleCenter.X, rect.Left, rect.Right);
        float closestY = MathHelper.Clamp(circleCenter.Y, rect.Top, rect.Bottom);
        float dx = circleCenter.X - closestX;
        float dy = circleCenter.Y - closestY;
        return (dx * dx + dy * dy) < (radius * radius);
    }
}