using Event_Manager;
using Event_Manager.Events;
using Gameplay.Grid_System.Pathfinding;
using Gameplay.Interfaces;
using Gameplay.Objects;
using Gameplay.Scriptable_Objects;
using Helpers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.Grid_System.Core
{
    public class GridSystem : Singleton<GridSystem>
    {
        #region Events

        private void OnEnable()
        {
            EventManager.AddListener<ItemToBuildSelectedEvent>(OnItemSelected);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<ItemToBuildSelectedEvent>(OnItemSelected);
        }

        private void OnItemSelected(ItemToBuildSelectedEvent eventParam)
        {
            placedObjectSo = eventParam.PlacedObjectSo;
            if (placedObjectSo.GetType() == typeof(BuildingWithUnitsSO)) buildingWithUnits = placedObjectSo.prefab.GetComponent<BuildingWithUnits>();
        }

        #endregion

        [SerializeField]
        private GameObject tilePrefab;

        [SerializeField]
        private Transform tilesParent, builtObjectsParent;

        [Header("Grid")]
        [SerializeField]
        private int gridWidth = 20;

        [SerializeField]
        private int gridHeight = 20;

        [SerializeField]
        private float cellSize = 4f;

        [Header("Debug")]
        public bool showGridNumbers;

        public Transform debugTextsParent;
        private BuildingWithUnits buildingWithUnits;

        private PlacedObjectSO placedObjectSo;

        private PlacedObject selectedObject;

        public Grid Grid { get; private set; }

        // Function to create grid objects.
        private GridObject CreateGridObject(Grid grid, int x, int y)
        {
            var buildTileLines = Instantiate(tilePrefab, new Vector3(x * grid.CellSize, y * grid.CellSize), Quaternion.identity, tilesParent);
            buildTileLines.name = $"Tile {x}, {y}";
            return new GridObject(grid, x, y);
        }

        private void Awake()
        {
            Grid = new Grid(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), CreateGridObject);
            placedObjectSo = null;
        }

        private void Start()
        {
            EventManager.Trigger(new GridInitializedEvent { grid = Grid });
        }


        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // try to build if object to build is selected
                if (placedObjectSo != null)
                {
                    var mousePosition = Utils.GetMouseWorldPosition();
                    Grid.GetXY(mousePosition, out var x, out var y);

                    var placedObjectOrigin = new Vector2Int(x, y);

                    var gridPositionList = placedObjectSo.GetGridPositionList(placedObjectOrigin);

                    // check if can build
                    if (CanBuildInMousePosition())
                    {
                        var buildingWorldPosition = Grid.GetWorldPosition(x, y);
                        var building = PlacedObject.Create(buildingWorldPosition, placedObjectOrigin, placedObjectSo, builtObjectsParent) as Building;

                        // set gridobjects under the building
                        foreach (var gridPosition in gridPositionList) Grid.GetGridObject(gridPosition.x, gridPosition.y).SetBuilding(building);

                        // set spawn point if object to build has units
                        if (buildingWithUnits != null)
                        {
                            Grid.GetXY(GetMouseWorldSnappedPosition() + buildingWithUnits.SpawnPointLocalPosition, out var x2, out var y2);
                            var spawnPointGridObject = Grid.GetGridObject(x2, y2);
                            spawnPointGridObject.IsSpawnPoint = true;
                            spawnPointGridObject.SetBuilding(building);
                        }

                        // clear object to build
                        DeselectObjectType();
                    }
                    else
                    {
                        Utils.ShowFloatingText("Can't build here!", "red", mousePosition);
                    }
                }
                else // if not trying to build, select object
                {
                    // Return if mouse over UI element.
                    if (EventSystem.current.IsPointerOverGameObject()) return;

                    var mousePosition = Utils.GetMouseWorldPosition();
                    selectedObject = null;
                    var gridObj = Grid.GetGridObject(mousePosition);

                    if (gridObj != null) selectedObject = gridObj.GetPlacedObject();

                    if (selectedObject != null) selectedObject.Select();


                    EventManager.Trigger(new SelectedObjectChangedEvent { selectedObject = selectedObject });
                }
            }


            if (Input.GetMouseButtonDown(1))
            {
                // cancel build
                if (placedObjectSo != null) DeselectObjectType();

                // Return if mouse over UI element.
                if (EventSystem.current.IsPointerOverGameObject()) return;

                var gridObj = GetGridObjectAtMousePosition();
                if (gridObj == null) return;

                if (selectedObject != null)
                {
                    // check if there is object in position
                    if (gridObj.LatestPlacedObject != null)
                    {
                        switch (selectedObject)
                        {
                            // check if selected object can attack
                            case IAttack attacker:
                                attacker.Attack(gridObj.LatestPlacedObject);
                                break;
                            // check if selected object can move
                            case IMovement movement:
                            {
                                var closest = Pathfinder.GetClosestGridObject(Grid, selectedObject, gridObj.LatestPlacedObject);
                                movement.Move(closest);
                                break;
                            }
                        }
                    }
                    else
                    {
                        // check if selected object can move
                        if (selectedObject is IMovement movement) movement.Move(gridObj);
                    }
                }
            }
        }


        private void DeselectObjectType()
        {
            placedObjectSo = null;
            buildingWithUnits = null;
            EventManager.Trigger(new ItemToBuildDeselectedEvent());
        }

        public void AddUnitToGridObject(Unit unit, Vector3 pos)
        {
            Grid.GetXY(pos, out var x, out var y);
            Grid.GetGridObject(x, y).AddUnit(unit);
        }

        public bool CanBuildInMousePosition()
        {
            var spawnPointAvailable = true;

            if (buildingWithUnits != null) spawnPointAvailable = CanBuildInPosition(GetMouseWorldSnappedPosition() + buildingWithUnits.SpawnPointLocalPosition);

            var mousePosition = Utils.GetMouseWorldPosition();
            Grid.GetXY(mousePosition, out var x, out var y);
            var positions = placedObjectSo.GetGridPositionList(new Vector2Int(x, y));
            foreach (var pos in positions)
            {
                var obj = Grid.GetGridObject(pos.x, pos.y);
                if (obj == null) return false;
                if (!obj.CanBuild) return false;
            }

            return spawnPointAvailable;
        }

        private bool CanBuildInPosition(Vector3 pos)
        {
            var gridPos = GetGridPosition(pos);
            var obj = Grid.GetGridObject(gridPos.x, gridPos.y);
            return obj != null && obj.CanBuild;
        }

        public Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            Grid.GetXY(worldPosition, out var x, out var z);
            return new Vector2Int(x, z);
        }

        public Vector3 GetSnappedPosition(Vector3 pos)
        {
            Grid.GetXY(pos, out var x, out var y);
            return Grid.GetWorldPosition(x, y);
        }

        public Vector3 GetMouseWorldSnappedPosition()
        {
            var mousePosition = Utils.GetMouseWorldPosition();
            Grid.GetXY(mousePosition, out var x, out var y);


            if (placedObjectSo != null)
            {
                var placedObjectWorldPosition = Grid.GetWorldPosition(x, y);
                return placedObjectWorldPosition;
            }

            return mousePosition;
        }

        private GridObject GetGridObjectAtMousePosition()
        {
            var mousePosition = Utils.GetMouseWorldPosition();
            return Grid.GetGridObject(mousePosition);
        }
    }
}