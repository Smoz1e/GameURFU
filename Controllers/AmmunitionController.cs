using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class AmmunitionController
{
    public AmmunitionModel Model;
    public AmmunitionView View;
    public AmmunitionController(AmmunitionModel model, AmmunitionView view)
    {
        Model = model;
        View = view;
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        View.Draw(spriteBatch, Model);
    }
}
