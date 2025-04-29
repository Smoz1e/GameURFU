using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics; // Добавлено для использования SpriteBatch

public class BulletController
{
    private BulletModel _model;
    private BulletView _view;

    public BulletController(BulletModel model, BulletView view)
    {
        _model = model;
        _view = view;
    }

    public void Update(GameTime gameTime)
    {
        // Обновляем позицию пули
        _model.Position += _model.Direction * _model.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Обновляем анимацию
        _view.UpdateAnimation(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _view.Draw(spriteBatch, _model.Position);
    }

    public bool IsOutOfBounds(int screenWidth, int screenHeight)
    {
        return _model.Position.X < 0 || _model.Position.X > screenWidth ||
               _model.Position.Y < 0 || _model.Position.Y > screenHeight;
    }

    public Vector2 Position => _model.Position;
}