/* ****************************************************************
 * 
 * Name: Operation
 * 
 * Date Created: 2015-02-23
 * 
 * Original Team: Maps
 * 
 * Description: Data structure for storing a tile change operation.
 * 
 * Change Log
 * ================================================================
 * Name             Date        Comment
 * ---------------- ----------- -----------------------------------
 * D. York          2015-02-23  Created
 */

namespace Assets.Scripts.Maps {
    public class Operation : MapDebuggable {
        private int x;
        private int y;
        private byte prevId;
        private byte newTileId;
        private static byte[,] tiles = World.Instance.Tiles;

        public Operation(int x, int y, byte prevId, byte newTileId) {
            this.x = x;
            this.y = y;
            this.prevId = prevId;
            this.newTileId = newTileId;
        }

        public static void Undo(Operation operation) {
            ////Debug.Log("Changing (" + operation.x + "," + operation.y + ") to " + operation.prevId);
            tiles[operation.x, operation.y] ^= tiles[operation.x, operation.y];
            tiles[operation.x, operation.y] |= operation.prevId;
            MapDisplayAPI.ApplySinglePixel(operation.x, operation.y, operation.prevId);
        }

        public static void Redo(Operation operation) {
            ////Debug.Log("Changing (" + operation.x + "," + operation.y + ") to " + operation.newTileId);
            tiles[operation.x, operation.y] ^= tiles[operation.x, operation.y];
            tiles[operation.x, operation.y] |= operation.newTileId;
            MapDisplayAPI.ApplySinglePixel(operation.x, operation.y, operation.newTileId);
        }

    }
}
