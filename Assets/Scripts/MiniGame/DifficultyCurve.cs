using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyCurve
{
    public static int day = 0;

    public static Vector3 GetNumberPatterns()
    {
        switch (day)
        {
            case 2:
                return new Vector3(10, 6, 2);

            case 3:
                return new Vector3(8, 6, 4);

            default:
                return new Vector3(6, 6, 6);
        }
    }
}