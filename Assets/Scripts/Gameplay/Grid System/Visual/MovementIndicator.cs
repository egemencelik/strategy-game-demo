using Event_Manager;
using Event_Manager.Events;
using Gameplay.Interfaces;
using Helpers;
using UnityEngine;

namespace Gameplay.Grid_System.Visual
{
    /// <summary>
    /// Singleton class that shows selected unit's target point on the grid.
    /// </summary>
    public class MovementIndicator : Singleton<MovementIndicator>
    {
        #region Events

        private void OnEnable()
        {
            EventManager.AddListener<SelectedObjectChangedEvent>(OnItemChanged);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<SelectedObjectChangedEvent>(OnItemChanged);
        }

        private void OnItemChanged(SelectedObjectChangedEvent eventParam)
        {
            spriteRenderer.enabled = false;

            if (eventParam.selectedObject is IMovement movement)
            {
                if (movement.IsMoving)
                {
                    spriteRenderer.enabled = true;
                    transform.position = movement.TargetDestination;
                }
            }
        }

        #endregion

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Activate(IMovement movement)
        {
            spriteRenderer.enabled = true;
            transform.position = movement.TargetDestination;
        }

        public void Deactivate()
        {
            spriteRenderer.enabled = false;
        }
    }
}