using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Scriptable_Objects
{
    public class PlacedObjectSO : ScriptableObject
    {
        public string objectName;
        public Sprite sprite;
        public GameObject prefab;

        [Header("Size")]
        public int width;

        public int height;

        [Header("Stats")]
        public int health;

        public List<Vector2Int> GetGridPositionList(Vector2Int offset)
        {
            var gridPositionList = new List<Vector2Int>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gridPositionList.Add(offset + new Vector2Int(x, y));
                }
            }

            return gridPositionList;
        }
    }
}