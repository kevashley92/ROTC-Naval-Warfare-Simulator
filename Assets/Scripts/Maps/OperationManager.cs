/* ****************************************************************
 * 
 * Name: OperationManager
 * 
 * Date Created: 2015-02-23
 * 
 * Original Team: Maps
 * 
 * Description: Class to manage changing tiles on the map.
 * 
 * Change Log
 * ================================================================
 * Name             Date        Comment
 * ---------------- ----------- -----------------------------------
 * D. York          2015-02-23  Created
 */

using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Maps;

public class OperationManager : MapDebuggable {
    private int currentPosition;
    private const int MAX_LIST_SIZE = 10;
    private List<LinkedList<Operation>> OperationList;
    private static byte[,] tiles;

    public OperationManager() {
        OperationList = new List<LinkedList<Operation>>();
        currentPosition = 0;
        OperationList.Add(new LinkedList<Operation>());
        tiles = null;
    }

    public void Redo() {
        if (currentPosition < (OperationList.Count - 1)) {
            Debug("Redo operation " + currentPosition);
            var RedoOperation = OperationList[currentPosition];
            foreach (Operation operation in RedoOperation) {
                Operation.Redo(operation);
            }
            currentPosition++;
        }
    }

    public void Undo() {
        if (currentPosition > 0) {
            currentPosition--;
            Debug("Undo operation " + currentPosition);
            var UndoOperation = OperationList[currentPosition];
            foreach (Operation operation in UndoOperation) {
                Operation.Undo(operation);
            }
        }
    }

    public void EndDraw() {
        if (currentPosition == (MAX_LIST_SIZE)) {
            Debug("Remove first");
            OperationList.RemoveAt(0);
        } else if (currentPosition < (OperationList.Count - 1)) {
            Debug("Chop end");
            OperationList.RemoveRange(currentPosition + 1, (OperationList.Count - 1) - currentPosition);
            currentPosition++;
        } else {
            Debug("Add to end");
            currentPosition++;
        }
        Debug("Current Position: " + currentPosition);
        Debug("Number of elements: " + OperationList.Count);
        OperationList.Add(new LinkedList<Operation>());
    }

    public void BeginDraw() {
        if (tiles == null)
            tiles = World.Instance.Tiles;
        OperationList[currentPosition].Clear();
    }

    public void changeTile(int x, int y, byte newTileId) {
        byte prevId = tiles[x, y];
        if (prevId != newTileId) {
            tiles[x, y] ^= tiles[x, y];
            tiles[x, y] |= newTileId;
            MapDisplayAPI.ApplySinglePixel(x, y, newTileId);
            OperationList[currentPosition].AddLast(new Operation(x, y, prevId, newTileId));
        }
    }

    public bool InBounds(int x, int y) {
        if (x >= 0 && x < tiles.GetLength(0)
            && y >= 0 && y < tiles.GetLength(1)) {
            return true;
        }
        return false;
    }

    public bool InBounds(Point point) {
        if (point.x >= 0 && point.x < tiles.GetLength(0)
            && point.y >= 0 && point.y < tiles.GetLength(1)) {
            return true;
        }
        return false;
    }

    public void Reset() {
        tiles = World.Instance.Tiles;
        currentPosition = 0;
        OperationList.Clear();
        OperationList.Add(new LinkedList<Operation>());
    }
}