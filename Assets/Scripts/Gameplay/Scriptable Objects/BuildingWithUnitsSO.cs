using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Scriptable_Objects
{
    [CreateAssetMenu(menuName = "Building with Units")]
    public class BuildingWithUnitsSO : BuildingSO
    {
        [Header("Units")]
        public List<UnitSO> units;
    }
}