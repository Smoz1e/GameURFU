using Microsoft.Xna.Framework;

public class AmmunitionModel
{
    public Vector2 Position;
    public bool IsCollected = false;
    public AmmunitionModel(Vector2 pos)
    {
        Position = pos;
    }
}
