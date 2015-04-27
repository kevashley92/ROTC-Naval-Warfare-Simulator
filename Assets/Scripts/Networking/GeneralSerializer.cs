
using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class GeneralSerializer : MonoBehaviour
{

    //Inspired by http://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp      
    public static object ConvertByteArrayToObject(byte[] data)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(data, 0, data.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        object obj = (object)binForm.Deserialize(memStream);

        return obj;
    }

    public static byte[] ConvertObjectToByteArray(object obj)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, obj);
        byte[] message = stream.ToArray();

        return message;
    }

    //Smuggles the serialized data through the network view
    public static NetworkPlayer Serialize(object obj)
    {
        //Might need to change default encoding to something else
        string str = Convert.ToBase64String(ConvertObjectToByteArray(obj));
        return new NetworkPlayer(str, 12345);
    }

    public static object Deserialize(NetworkPlayer obj)
    {
        string str = obj.ipAddress;
        byte[] arr = Convert.FromBase64String(str);
        return ConvertByteArrayToObject(arr);
    }

}
