using System;

namespace Gameplay.Grid_System.Pathfinding
{
    /// <summary>
    /// Holds grid object's pathfinding info.
    /// </summary>
    public class AstarTile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Cost { get; set; }
        public int Distance { get; set; }
        public int CostDistance => Cost + Distance;
        public AstarTile Parent { get; set; }

        public void SetDistance(int targetX, int targetY)
        {
            Distance = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
        }

        public override string ToString()
        {
            return X + ", " + Y;
        }
    }
}
