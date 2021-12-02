using Gameplay.Grid_System.Core;
using UnityEngine;

namespace Gameplay.Interfaces
{
    public interface IMovement
    {
        bool IsMoving { get; }
        Vector2 TargetDestination { get; set; }
        bool Move(GridObject target);
    }
}