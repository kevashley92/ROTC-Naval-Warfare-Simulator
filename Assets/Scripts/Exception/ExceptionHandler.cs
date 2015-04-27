using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ExceptionHandler
{
    private static ExceptionHandler _this;
    private static List<String> ExceptionsThrown;
    private ExceptionHandler()
    {
        ExceptionsThrown = new List<String>();
    }

    private static void Init() 
    {
        if (_this == null)
        {
            _this = new ExceptionHandler();
        }
    }

    public static void Throw(BaseException e) 
    {
        e.GetType();
    }
}