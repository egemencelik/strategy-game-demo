using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Grid_System.Core;
using Gameplay.Objects;
using Helpers;
using UnityEngine;
using Grid = Gameplay.Grid_System.Core.Grid;

namespace Gameplay.Grid_System.Pathfinding
{
    /// <summary>
    /// Static class that does pathfinding calculations.
    /// </summary>
    public static class Pathfinder
    {
        /// <summary>
        /// Returns available tiles around current tile.
        /// </summary>
        /// <param name="grid">Grid to search.</param>
        /// <param name="currentTile">Current tile to search around.</param>
        /// <param name="targetTile">Target tile.</param>
        /// <returns>List of available tiles.</returns>
        private static List<AstarTile> GetAvailableTiles(Grid grid, AstarTile currentTile, AstarTile targetTile)
        {
            // create 4 way tiles
            var possibleTiles = new List<AstarTile>()
            {
                new AstarTile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new AstarTile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new AstarTile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new AstarTile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
            };

            possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

            var maxX = grid.Width - 1;
            var maxY = grid.Height - 1;

            // return tiles that are in grid area and walkable
            return possibleTiles
                .Where(tile => tile.X.IsBetween(0, maxX))
                .Where(tile => tile.Y.IsBetween(0, maxY))
                .Where(tile => grid.GetGridObject(tile.X, tile.Y).CanWalk)
                .ToList();
        }

        public static List<GridObject> FindPath(Grid grid, AstarTile startTile, AstarTile finishTile)
        {
            // Init
            startTile.SetDistance(finishTile.X, finishTile.Y);
            var activeTiles = new List<AstarTile>();
            var path = new List<GridObject>();
            activeTiles.Add(startTile);
            var visitedTiles = new List<AstarTile>();

            while (activeTiles.Any())
            {
                // get smallest costdistance
                var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();
                
                // create path if reach target
                if (checkTile.X == finishTile.X && checkTile.Y == finishTile.Y)
                {
                    var tile = checkTile;

                    // create path
                    while (true)
                    {
                        var gridObject = grid.GetGridObject(tile.X, tile.Y);
                        path.Add(gridObject);
                        tile = tile.Parent;
                        if (tile == null)
                        {
                            return path;
                        }
                    }
                }
                
                visitedTiles.Add(checkTile);
                activeTiles.Remove(checkTile);

                var walkableTiles = GetAvailableTiles(grid, checkTile, finishTile);

                foreach (var walkableTile in walkableTiles)
                {
                    // check if visited before
                    if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                        continue;

                    // check if active
                    if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                    {
                        var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
                        if (existingTile.CostDistance > checkTile.CostDistance)
                        {
                            activeTiles.Remove(existingTile);
                            activeTiles.Add(walkableTile);
                        }
                    }
                    else
                    {
                        activeTiles.Add(walkableTile);
                    }
                }
            }

            return path;
        }

        /// <summary>
        /// Returns closest grid object to the target.
        /// </summary>
        /// <param name="grid">Grid to search.</param>
        /// <param name="start">Current object.</param>
        /// <param name="target">Target object.</param>
        /// <returns>Returns closest grid object to the target.</returns>
        public static GridObject GetClosestGridObject(Grid grid, PlacedObject start, PlacedObject target)
        {
            var startPosition = start.Origin;
            var targetPosition = target.Origin;

            // get closest point
            var closest = target.GetGridPositionList().OrderBy(pos => Vector2Int.Distance(startPosition, pos)).First();

            var x = closest.x;
            var y = closest.y;

            var xDif = closest.x - startPosition.x;
            var yDif = closest.y - startPosition.y;

            // get closest in 4 way
            if (Math.Abs(xDif) > Math.Abs(yDif))
            {
                if (startPosition.x > targetPosition.x)
                {
                    x++;
                }
                else
                {
                    x--;
                }
            }
            else
            {
                if (startPosition.y > targetPosition.y)
                {
                    y++;
                }
                else
                {
                    y--;
                }
            }

            return grid.GetGridObject(x, y);
        }
    }
}