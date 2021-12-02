using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Grid_System.Core;
using Gameplay.Grid_System.Pathfinding;
using Gameplay.Grid_System.Visual;
using Gameplay.Interfaces;
using Gameplay.Scriptable_Objects;
using Helpers;
using UnityEngine;

namespace Gameplay.Objects
{
    public class Unit : PlacedObject, IAttack, IMovement
    {
        private UnitSO UnitSO => PlacedObjectSo as UnitSO;
        public int Damage => UnitSO.damage;

        private float attackCooldown => UnitSO.attackCooldown;
        private float movementSpeed => UnitSO.movementSpeed;
        public bool IsMoving => currentPath.Count > 0;

        public Vector2 TargetDestination { get; set; }


        private List<GridObject> currentPath;
        private int currentPathIndex;
        private GridObject currentGridObject;
        private WaitForSeconds moveCooldown;
        private WaitForSeconds attackCooldownWait;
        private Coroutine currentMoveCoroutine, currentWaitForRangeCoroutine, currentAttackCoroutine;

        private PlacedObject currentTarget;

        public override void Setup(PlacedObjectSO placedObjectSo, Vector2Int origin)
        {
            base.Setup(placedObjectSo, origin);
            transform.position = new Vector3(origin.x, origin.y);
            currentGridObject = GridSystem.Instance.Grid.GetGridObject(origin.x, origin.y);
            currentPath = new List<GridObject>();
            moveCooldown = new WaitForSeconds(5f / movementSpeed);
            attackCooldownWait = new WaitForSeconds(attackCooldown);
            spriteRenderer.sprite = placedObjectSo.sprite;
        }

        protected override void DestroySelf()
        {
            base.DestroySelf();
            currentGridObject.RemoveUnit(this);
        }


        #region Attack

        public void Attack(PlacedObject target)
        {
            // return if target is itself
            if (target == this) return;

            // stop current coroutines
            if (currentWaitForRangeCoroutine != null) StopCoroutine(currentWaitForRangeCoroutine);
            if (currentAttackCoroutine != null) StopCoroutine(currentAttackCoroutine);

            currentTarget = target;
            var closest = Pathfinder.GetClosestGridObject(GridSystem.Instance.Grid, this, target);

            // return if can't move to the target point
            if (!Move(closest)) return;

            currentWaitForRangeCoroutine = StartCoroutine(WaitForAttackRange());
        }

        private IEnumerator WaitForAttackRange()
        {
            while (IsMoving)
            {
                yield return null;
            }

            // attack if damage greater than 0
            if (Damage > 0)
                currentAttackCoroutine = StartCoroutine(StartAttack());

            yield return null;
        }

        private IEnumerator StartAttack()
        {
            while (currentTarget.Health > 0)
            {
                currentTarget.Health -= Damage;
                Utils.ShowFloatingText(Damage.ToString(), "red", TargetDestination);
                yield return attackCooldownWait;
            }

            yield return null;
        }

        #endregion

        #region Movement

        public bool Move(GridObject target)
        {
            TargetDestination = target.Position;
            MovementIndicator.Instance.Activate(this);

            // stop current coroutines
            if (currentMoveCoroutine != null) StopCoroutine(currentMoveCoroutine);
            if (currentWaitForRangeCoroutine != null) StopCoroutine(currentWaitForRangeCoroutine);
            if (currentAttackCoroutine != null) StopCoroutine(currentAttackCoroutine);

            currentPath = Pathfinder.FindPath(GridSystem.Instance.Grid, currentGridObject.Tile, target.Tile);

            // return if can't find path
            if (currentPath.Count == 0 && target.Position != (Vector2)transform.position)
            {
                Utils.ShowFloatingText("Can't find path!", "red", transform.position);
                ResetPath();
                return false;
            }

            currentPath.Reverse();
            currentPathIndex = 0;
            currentMoveCoroutine = StartCoroutine(GoToNextCell());
            return true;
        }

        private IEnumerator GoToNextCell()
        {
            while (currentPathIndex < currentPath.Count)
            {
                currentGridObject.RemoveUnit(this);
                transform.position = currentPath[currentPathIndex].Position;
                currentGridObject = currentPath[currentPathIndex];
                origin = currentGridObject.Position.ToVector2Int();
                currentPathIndex++;
                currentGridObject.AddUnit(this);
                yield return moveCooldown;
            }

            ResetPath();
        }

        private void ResetPath()
        {
            foreach (var cell in currentPath.ToList())
            {
                currentPath.Remove(cell);
            }

            MovementIndicator.Instance.Deactivate();
        }

        #endregion
    }
}