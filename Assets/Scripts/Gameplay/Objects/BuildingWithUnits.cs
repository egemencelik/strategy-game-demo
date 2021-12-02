using Event_Manager;
using Event_Manager.Events;
using Gameplay.Grid_System.Core;
using Gameplay.Scriptable_Objects;
using Helpers;
using UnityEngine;

namespace Gameplay.Objects
{
    public class BuildingWithUnits : Building
    {
        #region Events

        protected override void OnEnable()
        {
            base.OnEnable();
            EventManager.AddListener<ItemToBuildSelectedEvent>(OnItemSelect);
            EventManager.AddListener<ItemToBuildDeselectedEvent>(OnItemDeselect);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventManager.RemoveListener<ItemToBuildSelectedEvent>(OnItemSelect);
            EventManager.RemoveListener<ItemToBuildDeselectedEvent>(OnItemDeselect);
        }

        private void OnItemSelect(ItemToBuildSelectedEvent eventParam)
        {
            spawnPoint.gameObject.SetActive(true);
        }

        private void OnItemDeselect(ItemToBuildDeselectedEvent eventParam)
        {
            spawnPoint.gameObject.SetActive(false);
        }

        protected override void OnItemChanged(SelectedObjectChangedEvent eventParam)
        {
            base.OnItemChanged(eventParam);
            if (eventParam.selectedObject != this) spawnPoint.gameObject.SetActive(false);
        }

        #endregion

        [SerializeField]
        private Transform spawnPoint;

        private Vector3 SpawnPointPosition => spawnPoint.position;
        public Vector3 SpawnPointLocalPosition => spawnPoint.localPosition;
        public BuildingWithUnitsSO UnitSoSpawnerSo => PlacedObjectSo as BuildingWithUnitsSO;


        protected override void Awake()
        {
            base.Awake();
            spawnPoint.position = GridSystem.Instance.GetSnappedPosition(SpawnPointPosition);
        }

        public void Spawn(UnitSO unitSo)
        {
            var origin = GridSystem.Instance.GetGridPosition(SpawnPointPosition);
            var spawnedUnit = ObjectPool.Instance.SpawnFromPool(PoolType.Soldier).GetComponent<Unit>();
            spawnedUnit.Setup(unitSo, origin);
            GridSystem.Instance.AddUnitToGridObject(spawnedUnit, SpawnPointPosition);
        }

        public override void Select()
        {
            base.Select();
            spawnPoint.gameObject.SetActive(true);
        }
    }
}