using Gameplay.Objects;

namespace Gameplay.Interfaces
{
    public interface IAttack
    {
        int Damage { get; }
        void Attack(PlacedObject target);
    }
}