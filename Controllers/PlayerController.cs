using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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

    public void Update(GameTime gameTime, GraphicsDeviceManager graphics, List<Rectangle> obstacles)
    {
        float updatedSpeed = _model.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        var kstate = Keyboard.GetState();
        var mouseState = Mouse.GetState();

        Vector2 movement = Vector2.Zero;
        if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.W)) movement.Y -= 1;
        if (kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.S)) movement.Y += 1;
        if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A)) movement.X -= 1;
        if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D)) movement.X += 1;

        Vector2 oldPosition = _model.Position;
        if (movement != Vector2.Zero)
        {
            movement.Normalize();
            Vector2 newPos = _model.Position + movement * updatedSpeed;
            bool collided = false;
            if (GameControllerStatic.IsCollisionStatic != null)
            {
                collided = GameControllerStatic.IsCollisionStatic(newPos, _model.ColliderRadius);
            }
            if (!collided)
            {
                _model.Position = newPos;
            }
        }

        ClampPosition(graphics);

        Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
        Vector2 directionToMouse = mousePosition - _model.Position;
        _model.Rotation = (float)Math.Atan2(directionToMouse.Y, directionToMouse.X);

        if (_model.IsReloading)
        {
            _model.ReloadTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_model.ReloadTimer >= PlayerModel.ReloadDuration)
            {
                if (_model.Magazines > 0)
                {
                    _model.Magazines--;
                    _model.IsReloading = false;
                    _model.ReloadTimer = 0f;
                    _model.ShotsFired = 0;
                }
                else
                {
                    _model.IsReloading = false;
                    _model.ReloadTimer = 0f;
                }
            }
        }

        if (!_model.IsReloading && kstate.IsKeyDown(Keys.R) && _model.Magazines > 0 && _model.ShotsFired > 0)
        {
            _model.IsReloading = true;
            _model.ReloadTimer = 0f;
        }

        if (!_model.IsReloading && mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            if (_model.ShotsFired < PlayerModel.MaxShotsBeforeReload && (_model.Magazines > 0 || _model.ShotsFired < PlayerModel.MaxShotsBeforeReload))
            {
                Vector2 bulletDirection = directionToMouse;
                if (bulletDirection.Length() > 0)
                {
                    bulletDirection.Normalize();
                }

                Vector2 gunOffset = new Vector2(30, 10); // подберите значение под вашу модель
                Vector2 rotatedOffset = Vector2.Transform(gunOffset, Matrix.CreateRotationZ(_model.Rotation));
                Vector2 bulletStart = _model.Position + rotatedOffset;

                var bulletModel = new BulletModel(bulletStart, bulletDirection, 500f);
                var bulletView = new BulletView(_view._bulletTexture, 16, 16, 0.1f);
                var bulletController = new BulletController(bulletModel, bulletView);

                _model.Bullets.Add(bulletController);
                _model.ShotsFired++;
                if (_model.ShotsFired >= PlayerModel.MaxShotsBeforeReload)
                {
                    if (_model.Magazines > 0)
                    {
                        _model.IsReloading = true;
                        _model.ReloadTimer = 0f;
                    }
                }
            }
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

    private bool CircleIntersectsRectangle(Vector2 circleCenter, float radius, Rectangle rect)
    {
        float closestX = MathHelper.Clamp(circleCenter.X, rect.Left, rect.Right);
        float closestY = MathHelper.Clamp(circleCenter.Y, rect.Top, rect.Bottom);
        float dx = circleCenter.X - closestX;
        float dy = circleCenter.Y - closestY;
        return (dx * dx + dy * dy) < (radius * radius);
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