using Event_Manager;
using Event_Manager.Events;
using Gameplay.Grid_System.Core;
using Gameplay.Objects;
using Gameplay.Scriptable_Objects;
using UnityEngine;

namespace Gameplay.Grid_System.Visual
{
    public class BuildingGhost : MonoBehaviour
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
            RefreshVisual(eventParam.PlacedObjectSo);
        }

        private void OnItemDeselected(ItemToBuildDeselectedEvent eventParam)
        {
            RefreshVisual(null);
        }

        #endregion

        [SerializeField]
        private GameObject buildingVisual, spawnPointVisual;

        private SpriteRenderer visualSpriteRenderer;

        private void Awake()
        {
            visualSpriteRenderer = buildingVisual.GetComponent<SpriteRenderer>();
        }

        // Update position if active
        private void LateUpdate()
        {
            if (!buildingVisual.activeInHierarchy) return;
            var targetPosition = GridSystem.Instance.GetMouseWorldSnappedPosition();
            CheckPositionAvailable();
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
        }

        // Change visual
        private void RefreshVisual(PlacedObjectSO placed)
        {
            // Disable objects
            if (buildingVisual.activeInHierarchy)
            {
                buildingVisual.SetActive(false);
                spawnPointVisual.SetActive(false);
            }

            // Active objects if selected object is not null
            if (placed != null)
            {
                if (placed.GetType() == typeof(BuildingWithUnitsSO))
                {
                    // activate spawn point if object has units
                    var buildingWithUnits = placed.prefab.GetComponent<BuildingWithUnits>();
                    spawnPointVisual.transform.localPosition = buildingWithUnits.SpawnPointLocalPosition;
                    spawnPointVisual.SetActive(true);
                }

                buildingVisual.SetActive(true);
                visualSpriteRenderer.sprite = placed.sprite;
                buildingVisual.transform.localPosition = Vector3.zero;
                buildingVisual.transform.localEulerAngles = Vector3.zero;
                MakeVisualTransparent();
            }
        }

        private void CheckPositionAvailable()
        {
            if (!buildingVisual.activeInHierarchy) return;
            ChangeVisualColor(GridSystem.Instance.CanBuildInMousePosition());
        }

        private void ChangeVisualColor(bool avaiable)
        {
            var tempColor = avaiable ? Color.white : Color.red;
            tempColor.a = .5f;
            visualSpriteRenderer.color = tempColor;
        }

        private void MakeVisualTransparent()
        {
            var tempColor = visualSpriteRenderer.color;
            tempColor.a = .5f;
            visualSpriteRenderer.color = tempColor;
        }
    }
}