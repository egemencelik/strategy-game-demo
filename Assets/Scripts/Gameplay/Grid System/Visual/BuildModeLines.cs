using Event_Manager;
using Event_Manager.Events;
using UnityEngine;

namespace Gameplay.Grid_System.Visual
{
    /// <summary>
    /// Grid lines that activates when user enters build mode
    /// </summary>
    public class BuildModeLines : MonoBehaviour
    {
        #region Events

        private void OnEnable()
        {
            EventManager.AddListener<ItemToBuildSelectedEvent>(OnItemSelected);
            EventManager.AddListener<ItemToBuildDeselectedEvent>(OnItemDeselected);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<ItemToBuildSelectedEvent>(OnItemSelected);
            EventManager.RemoveListener<ItemToBuildDeselectedEvent>(OnItemDeselected);
        }

        private void OnItemSelected(ItemToBuildSelectedEvent eventParam)
        {
            spriteRenderer.enabled = true;
        }

        private void OnItemDeselected(ItemToBuildDeselectedEvent eventParam)
        {
            spriteRenderer.enabled = false;
        }

        #endregion

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
        }
    }
}