using UnityEngine;

public class MouseToGridAPI : MapDebuggable {
    private static Camera mapCamera;
    public static Camera MapCamera {
        get {
            if (mapCamera == null) {
                mapCamera = GameObject.Find("MapCamera").camera;
            }

            return mapCamera;
        }
    }

    private static GameObject mapObject;
    public static GameObject MapObject {
        get {
            if (mapObject == null) {
                mapObject = GameObject.Find("MapObject").gameObject;
            }

            return mapObject;
        }
    }

    public static Vector3 GetWorldCoordinate(Vector3 mousePos) {
        Vector3 worldMouse = mousePos;
        worldMouse.z = MapObject.transform.localPosition.z;
        worldMouse = MapCamera.ScreenToWorldPoint(worldMouse);

        ////Debug.Log("Mouse position in world  [" + worldMouse.x + "," + worldMouse.y + "," + worldMouse.z + "]");

        return worldMouse;
    }

    public static Vector3 GetGridCoordinateFromUnit(Vector3 unit)
    {
        Vector3 gridPosition = unit;
        gridPosition.x = Mathf.Floor(unit.x);
        gridPosition.y = Mathf.Floor(World.Instance.Height - unit.y);
        return gridPosition;
    }

    public static Vector3 GetGridCoordinate(Vector3 mousePos) {
        Vector3 worldMouse = mousePos;
        //worldMouse.z = -MapCamera.transform.localPosition.z;
        worldMouse.z = 0;
        worldMouse = MapCamera.ScreenToWorldPoint(worldMouse);
        ////Debug.Log("Mouse position in world  [" + worldMouse.x + "," + worldMouse.y + "," + worldMouse.z + "]");

        Vector3 gridPosition = new Vector3();
        gridPosition.x = Mathf.Floor(worldMouse.x - MapObject.transform.position.x);
        gridPosition.y = Mathf.Floor(World.Instance.Height + MapObject.transform.position.y - worldMouse.y);
        // Warning, the next line will spam the output when uncommented, printing
        // at least once every frame.
        ////Debug.Log("Corresponding grid       [" + gridPosition.x + "," + gridPosition.y + "]");

        return gridPosition;
    }

    public static MarineGridCoordinate GetMGRSFromMouse(Vector3 mousePos) {
        return GetMGRSFromGrid(GetGridCoordinate(mousePos));
    }

    public static MarineGridCoordinate GetMGRSFromGrid(Vector3 pos) {
        MarineGridCoordinate mgrs = World.Instance.StartMarineGridCoordinate;
        if (mgrs == null) {
            return null;
        }

        int sX = World.Instance.StartX;
        int sY = World.Instance.StartY;

        int gX = sX - (int)pos.x;
        int gY = sY - (int)pos.y;

        ////Debug.Log("Corresponding mgrs       [" + (mgrs.X - gX) + "," + (mgrs.Y + gY - 1) + "]");

        return new MarineGridCoordinate((ushort)(mgrs.X - gX), (ushort)(mgrs.Y + gY - 1));
    }

    public static Coordinate GetCoordinateFromMouse(Vector3 mousePos) {
        return GetCoordinateFromGrid(GetGridCoordinate(mousePos));
    }

    public static Coordinate GetCoordinateFromGrid(Vector3 pos) {
        Coordinate start = World.Instance.StartCoordinate;
        if (start == null) {
            return null;
        }

        Coordinate delta = new Coordinate(0, 0, pos.x, 0, 0, pos.y);

        Coordinate ret = new Coordinate(start.Latitude + delta.Latitude, start.Longitude - delta.Longitude);

        ////Debug.Log("Corresponding  coor       [" + 
        //    ret.LatitudeDegree + "d" + ret.LatitudeMinute + "m" +  ret.LatitudeMinute + "s" + "," + 
        //    ret.LongitudeDegree + "d" + ret.LongitudeMinute + "m" + ret.LongitudeSecond + "s" + "]");

        return ret;
    }

    public static Vector2 GetGridFromMGRS(MarineGridCoordinate pos) {
        MarineGridCoordinate mgrs = World.Instance.StartMarineGridCoordinate;
        if (mgrs == null) {
            return new Vector2(-1, -1);
        }

        int sX = World.Instance.StartX;
        int sY = World.Instance.StartY;

        int dX = mgrs.X - pos.X;
        int dY = mgrs.Y - pos.Y;

        ////Debug.Log("Corresponding grid       [" + (sX - dX) + "," + (sY + dY - 1) + "]");

        return new Vector2(sX - dX, sY + dY - 1);
    }

    public static Vector3 UnitToWorldCoordinate(Vector3 unit)
    {
        Vector3 ret = unit;
        ret.y = Mathf.Floor(World.Instance.Height - unit.y);
        return ret;
    }
}
