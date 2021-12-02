using System.Collections.Generic;
using Gameplay.Grid_System.Pathfinding;
using Gameplay.Objects;
using UnityEngine;

namespace Gameplay.Grid_System.Core
{
    public class GridObject
    {
        private readonly Grid grid;

        private readonly List<Unit> unitsOnTile;
        private readonly int x;
        private readonly int y;

        public GridObject(Grid grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            unitsOnTile = new List<Unit>();
            Tile = new AstarTile { X = x, Y = y, Parent = null, Cost = 0 };
            Position = new Vector2(x, y);
        }

        public AstarTile Tile { get; }
        public Vector2 Position { get; }
        public bool IsSpawnPoint { get; set; }
        public bool CanBuild => Building == null && unitsOnTile.Count < 1;
        public bool CanWalk => CanBuild || !CanBuild && IsSpawnPoint;

        private Unit LatestUnit => unitsOnTile.Count < 1 ? null : unitsOnTile[unitsOnTile.Count - 1];

        private Building Building { get; set; }
        public PlacedObject LatestPlacedObject => unitsOnTile.Count < 1 ? Building as PlacedObject : unitsOnTile[unitsOnTile.Count - 1];

        public override string ToString()
        {
            return $"{x},{y}";
        }

        public PlacedObject GetPlacedObject()
        {
            if (unitsOnTile.Count > 0) return LatestUnit;
            return Building;
        }

        public void SetBuilding(Building building)
        {
            Building = building;
        }

        public void AddUnit(Unit unit)
        {
            if (unitsOnTile.Contains(unit)) return;
            unitsOnTile.Add(unit);
        }

        public void RemoveUnit(Unit unit)
        {
            if (!unitsOnTile.Contains(unit)) return;
            unitsOnTile.Remove(unit);
        }
    }
}