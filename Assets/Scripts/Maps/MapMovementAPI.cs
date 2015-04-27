using UnityEngine;
using System.Collections.Generic;

public class MapMovementAPI : MapDebuggable {
    private static Camera mapCamera;
    private static GameObject mapObject;

	public static float maxCamDistance = 120.0f;
	public static float minCamDistance = 7.0f;
	public static float defaultScale = 15.0f;

    static MapMovementAPI() {
        EnsureMapCamera();
        EnsureMapObject();
    }

    private static void EnsureMapCamera() {
        if (mapCamera == null) {
            mapCamera = GameObject.Find("MapCamera").camera;
        }
    }

    private static void EnsureMapObject() {
        if (mapObject == null) {
            mapObject = GameObject.Find("MapObject").gameObject;
        }
    }

    public static void MoveMap(Vector3 vector) {
        /* Zoom-based movement scaling. */
        var scaled = new Vector3(vector.x, vector.y, vector.z) * (mapCamera.orthographicSize + 100.0f) / 250.0f;
        var position = mapCamera.transform.localPosition;

        position.x = Mathf.Clamp(position.x + scaled.x, 0, World.Instance.Width);
        position.y = Mathf.Clamp(position.y + scaled.y, 0, World.Instance.Height);
        position.z = -(mapCamera.orthographicSize = Mathf.Clamp(mapCamera.orthographicSize + scaled.z, 1f, Mathf.Max(World.Instance.Height / 1.95f, World.Instance.Width / 1.95f)));

        mapCamera.transform.localPosition = position;

        /* Adjust unit zoom. */
        var units = GuidList.GetAllObjects();
        var ortho = mapCamera.orthographicSize;
        var ratio = 3.125f * Mathf.Pow(ortho, 0.404157f) + Mathf.Pow(ortho / 56, 1.45f);
        foreach (var unit in units) {
//            if (unit == null) {
//                continue;
//            }
//            unit.transform.localScale = new Vector3(ratio, ratio, 1);
//
//            var range = unit.transform.FindChild("Move Range Sprite");
//            if (range != null) {
//           		range.localScale = new Vector3(0.1155f * 15 / ratio, 0.1155f * 15 / ratio, 1);
//          	}
//			range = unit.transform.FindChild("Attack Range Sprite");
//			if (range != null) {
//				range.localScale = new Vector3(0.1155f * 15 / ratio, 0.1155f * 15 / ratio, 1);
//			}
//			range = unit.transform.FindChild("Radar Range Sprite");
//			if (range != null) {
//				range.localScale = new Vector3(0.1155f * 15 / ratio, 0.1155f * 15 / ratio, 1);
//			}
        }
    }
}
