using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class TestingTransform : MonoBehaviour
{
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 pos = Vector3.zero;
        if (stream.isWriting)
        {
            pos = this.transform.localPosition;
            stream.Serialize(ref pos);
        }
        else
        {
            stream.Serialize(ref pos);
            this.transform.localPosition = pos;
        }
    }
}