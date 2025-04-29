using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

public class PlayerController
{
    private PlayerModel _model;
    private PlayerView _view;
    private MouseState _previousMouseState;

    public PlayerController(PlayerModel model, PlayerView view)
    {
        _model = model;
        _view = view;
        _previousMouseState = Mouse.GetState();
    }

    public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        float updatedSpeed = _model.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        var kstate = Keyboard.GetState();
        var mouseState = Mouse.GetState();

        // Управление игроком
        Vector2 movement = Vector2.Zero;
        if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.W)) movement.Y -= 1;
        if (kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.S)) movement.Y += 1;
        if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A)) movement.X -= 1;
        if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D)) movement.X += 1;

        if (movement != Vector2.Zero)
        {
            movement.Normalize();
            _model.Position += movement * updatedSpeed;
        }

        // Ограничение движения игрока в пределах экрана
        ClampPosition(graphics);

        // Вычисление угла поворота к курсору
        Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
        Vector2 directionToMouse = mousePosition - _model.Position;
        _model.Rotation = (float)Math.Atan2(directionToMouse.Y, directionToMouse.X);

        // Стрельба
        if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            Vector2 bulletDirection = directionToMouse;
            if (bulletDirection.Length() > 0)
            {
                bulletDirection.Normalize();
            }

            var bulletModel = new BulletModel(_model.Position, bulletDirection, 500f);
            var bulletView = new BulletView(_view._bulletTexture, 16, 16, 0.1f);
            var bulletController = new BulletController(bulletModel, bulletView);

            _model.Bullets.Add(bulletController);
        }

        // Обновление пуль
        for (int i = _model.Bullets.Count - 1; i >= 0; i--)
        {
            _model.Bullets[i].Update(gameTime);

            if (_model.Bullets[i].IsOutOfBounds(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight))
            {
                _model.Bullets.RemoveAt(i);
            }
        }

        _previousMouseState = mouseState;
    }

    private void ClampPosition(GraphicsDeviceManager graphics)
    {
        int c = 6;
        _model.Position = new Vector2(
            MathHelper.Clamp(_model.Position.X, _view._texture.Width / (2 * c), graphics.PreferredBackBufferWidth - _view._texture.Width / (2 * c)),
            MathHelper.Clamp(_model.Position.Y, _view._texture.Height / (2 * c), graphics.PreferredBackBufferHeight - _view._texture.Height / (2 * c))
        );
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _view.Draw(spriteBatch, _model);

        foreach (var bullet in _model.Bullets)
        {
            bullet.Draw(spriteBatch);
        }
    }
}