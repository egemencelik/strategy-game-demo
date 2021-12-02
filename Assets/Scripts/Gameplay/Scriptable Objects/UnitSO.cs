using UnityEngine;

namespace Gameplay.Scriptable_Objects
{
    [CreateAssetMenu(menuName = "Unit")]
    public class UnitSO : PlacedObjectSO
    {
        public int damage;
        public float attackCooldown;
        public float movementSpeed;
    }
}