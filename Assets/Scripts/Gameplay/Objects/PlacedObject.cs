using System.Collections.Generic;
using Event_Manager;
using Event_Manager.Events;
using Gameplay.Scriptable_Objects;
using UnityEngine;

namespace Gameplay.Objects
{
    public class PlacedObject : MonoBehaviour
    {
        #region Static

        /// <summary>
        /// Static function to instantiate placed object.
        /// </summary>
        /// <param name="worldPosition">Position to instantiate.</param>
        /// <param name="origin">Origin of the object.</param>
        /// <param name="placedObjectSo">Scriptable object to get data from.</param>
        /// <param name="parent">Parent of the gameobject.</param>
        /// <returns>Instantiated object.</returns>
        public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectSO placedObjectSo, Transform parent)
        {
            var placedObjectTransform = Instantiate(placedObjectSo.prefab, worldPosition, Quaternion.identity, parent);

            var placedObject = placedObjectTransform.GetComponent<PlacedObject>();
            placedObject.Setup(placedObjectSo, origin);

            return placedObject;
        }

        #endregion

        #region Events

        public delegate void HealthChangedEvent();

        public event HealthChangedEvent OnHealthChanged;

        protected virtual void OnEnable()
        {
            EventManager.AddListener<SelectedObjectChangedEvent>(OnItemChanged);
        }

        protected virtual void OnDisable()
        {
            EventManager.RemoveListener<SelectedObjectChangedEvent>(OnItemChanged);
        }

        protected virtual void OnItemChanged(SelectedObjectChangedEvent eventParam)
        {
            if (eventParam.selectedObject != this)
                Deselect();
        }

        #endregion

        protected Vector2Int origin;
        protected SpriteRenderer spriteRenderer;
        private bool isSelected;

        public PlacedObjectSO PlacedObjectSo { get; private set; }

        public int Health
        {
            get => health;
            set
            {
                if (value != health)
                {
                    TakeDamageEffect(value);
                    health = value;
                    OnHealthChanged?.Invoke();
                    if (health <= 0) DestroySelf();
                }
            }
        }

        private int health;

        public Vector2Int Origin => origin;


        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual void Select()
        {
            spriteRenderer.color = Color.gray;
            isSelected = true;
        }

        private void Deselect()
        {
            spriteRenderer.color = Color.white;
            isSelected = false;
        }

        private void TakeDamageEffect(int value)
        {
            if (value > health) return;
            spriteRenderer.color = Color.red;
            Invoke(nameof(ResetColor), .5f);
        }

        private void ResetColor()
        {
            spriteRenderer.color = isSelected ? Color.gray : Color.white;
        }

        public virtual void Setup(PlacedObjectSO placedObjectSo, Vector2Int origin)
        {
            PlacedObjectSo = placedObjectSo;
            this.origin = origin;
            Health = placedObjectSo.health;
        }

        public List<Vector2Int> GetGridPositionList()
        {
            return PlacedObjectSo.GetGridPositionList(origin);
        }

        protected virtual void DestroySelf()
        {
            if (isSelected) EventManager.Trigger(new SelectedObjectChangedEvent());
            Destroy(gameObject);
        }
    }
}