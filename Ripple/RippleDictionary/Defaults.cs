namespace RippleDictionary
{
    public class Defaults
    {
        #region Fields
        private static int row, column, id;
        private static bool subTile = false;
        #endregion

        #region Constructors
        public Defaults(int row, int column, int id, bool subTile = false)
        {
            Defaults.row = row;
            Defaults.column = column;
            Defaults.id = id;
            Defaults.subTile = subTile;
            Defaults.Tile = new Tile(XMLElementsAndAttributes.Tile, XMLElementsAndAttributes.Tile, TileType.OnlyMedia, "#FFFFFF", GetStyle(), GetCoordinate(), TileAction.Standard, null);
        } 
	    #endregion

        #region Objects
        public static Tile Tile;
        #endregion

        #region Helpers
        private static Coordinate GetCoordinate()
        {
            int rowId = (id % column == 0 ? id / column : (id / column) + 1); // Avoiding Ceiling()
            int columnId = (id % column == 0 ? column : id % column);

            return new Coordinate((columnId - 1), (row - rowId + 1));
        }

        private static Style GetStyle()
        {
            double width, height = 1.0 / (double)row;

            if (subTile == true)
            {
                width = 1;
            }
            else
            {
                width = 1.0 / (double)column;
            }

            return new Style(width, height);
        }
        #endregion
    }
}
