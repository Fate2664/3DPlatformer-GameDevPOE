namespace Platformer
{
    //This scriptable object will hold all the data needed for a collectible.
    public abstract class CollectibleData : EntityData
    {
        public abstract bool TryCollect(PlayerController player);
    }
}
