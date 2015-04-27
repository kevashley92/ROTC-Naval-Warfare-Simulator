/* ****************************************************************
 * 
 * Name: Point
 * 
 * Date Created: 2015-02-09
 * 
 * Original Team: Maps
 * 
 * Description: 
 * 
 * Change Log
 * ================================================================
 * Name             Date        Comment
 * ---------------- ----------- -----------------------------------
 * D. York          2015-02-09  Created
 * R. Lodico        2015-02-11  Added change log, normalized style
 */
using System;
using UnityEngine;


[Serializable]
public class Point : MapDebuggable {
    public float x { get; private set; }
    public float y { get; private set; }
    public float z;
    public Point(float x, float y) {
        this.x = x;
        this.y = y;
        this.z = 0;
    }
    public Point(float x, float y, float z){
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Point(Vector3 v){
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }
}
