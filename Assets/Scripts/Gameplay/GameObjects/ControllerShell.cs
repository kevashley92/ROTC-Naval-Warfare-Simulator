using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ControllerShell
{
    private static Dictionary<string, Dictionary<string, ISetGetValues>> _cc;
    private static int _index = 0;

    public static void Clear() {
        _cc = new Dictionary<string, Dictionary<string, ISetGetValues>>();
        _index = 0;
    }

    public static void DumpGuidList()
    {
        Clear();
        List<GameObject> list = GuidList.GetAllObjects();
        foreach (GameObject go in list)
        {
            Dictionary<String, ISetGetValues> shell = new Dictionary<string, ISetGetValues>();
            ISetGetValues[] cList = go.GetComponents<Controller>();
            foreach (ISetGetValues sgc in cList)
            {
                if (sgc.GetType().Name == "MoverController") {
                    ((MoverController)sgc).x_pos = go.transform.position.x;
                    ((MoverController)sgc).y_pos = go.transform.position.y;
                }
                shell.Add(sgc.GetType().Name, sgc);
            }
            _cc.Add(go.tag + "-" + (_index++).ToString(), shell);
        }
    }

    public static IDictionary Container
    {
        get
        {
            return _cc;
        }
        private set
        {

        }
    }

    public static List<string> Keys
    {
        get
        {
            return new List<string>(_cc.Keys);
        }
        private set
        {

        }
    }
}

