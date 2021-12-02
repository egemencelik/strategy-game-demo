using System;
using Helpers;
using TMPro;
using UnityEngine;

namespace Gameplay.Grid_System.Core
{
    public class Grid
    {
        private readonly GridObject[,] gridArray;

        public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid, int, int, GridObject> createGridObject)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            OriginPosition = originPosition;
            CenterPosition = new Vector3(width * cellSize / 2, height * cellSize / 2, 0);

            gridArray = new GridObject[width, height];

            for (var x = 0; x < gridArray.GetLength(0); x++)
            for (var y = 0; y < gridArray.GetLength(1); y++)
                gridArray[x, y] = createGridObject(this, x, y);


            if (GridSystem.Instance.showGridNumbers)
            {
                var debugTextArray = new TextMeshPro[width, height];

                for (var x = 0; x < gridArray.GetLength(0); x++)
                for (var y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = Utils.CreateWorldText(gridArray[x, y]?.ToString(), GridSystem.Instance.debugTextsParent, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f,
                        3.5f, Color.white);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }

                Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
            }
        }

        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }
        private Vector3 OriginPosition { get; }
        public Vector3 CenterPosition { get; }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y) * CellSize + OriginPosition;
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - OriginPosition).x / CellSize);
            y = Mathf.FloorToInt((worldPosition - OriginPosition).y / CellSize);
        }

        public GridObject GetGridObject(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height) return gridArray[x, y];

            return default;
        }

        public GridObject GetGridObject(Vector3 worldPosition)
        {
            GetXY(worldPosition, out var x, out var y);
            return GetGridObject(x, y);
        }
    }
}