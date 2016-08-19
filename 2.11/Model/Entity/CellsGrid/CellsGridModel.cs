using System.Collections.Generic;

namespace Model.Entity.CellsGrid
{
    public class CellsGridModel
    {
        public int NoHorizontalCells { get; set; }
        public int NoVerticalCells { get; set; }
        public List<InsightControl> InsightControls { get; set; }
    }
}
