using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    /// <summary>
    /// stringは空かどうか　確認
    /// </summary>
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if(stringToCheck == "")
        {
            Debug.Log(thisObject.name.ToString() + "の" +　fieldName + "は初期化されていません");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Listは空かエラーがあるか　確認
    /// </summary>
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        foreach(var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debug.Log(thisObject.name.ToString() + "の" + fieldName + "ではNULLフィールドがあります。");
                error = true;
            }
            else
            {
                count++;
            }
        }

        if(count==0)
        {
            Debug.Log(thisObject.name.ToString() + "の" + fieldName + "は初期化されていません");
            error=true;
        }
        return error;
    }
}
