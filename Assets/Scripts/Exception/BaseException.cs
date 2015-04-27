using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class BaseException {
    private int _level;
    private String _type;
    private String _message;

    public int Level
    {
        get
        {
            return _level;
        }
    }

    public String Type
    {
        get
        {
            return _type;
        }
    }

    private BaseException(int level, String type) 
    {
        _level = level;
        _type = type;
    }

    public static BaseException getByLevel(int level)
    {
        return new BaseException(0, "null");
    }

    public static BaseException getByType(String type)
    {
        return new BaseException(0, "null");
    }

}