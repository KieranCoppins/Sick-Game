using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class of static generic helpers that help with dealing with generics
public static class GenericHelpers
{
    /// <summary>
    /// Check if type toCheck is derived from generic where generic is an unbound generic class
    /// </summary>
    /// <param name="generic"></param>
    /// <param name="toCheck"></param>
    /// <returns></returns>
    public static bool IsSubClassOfRawGeneric(System.Type generic, System.Type toCheck)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
                return true;
            toCheck = toCheck.BaseType;
        }
        return false;
    }

    /// <summary>
    /// Returns type T in Generic<T>, finds the first base generic it can find
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static System.Type GetGenericType(object obj)
    {
        System.Type type = null;

        System.Type current = obj.GetType();
        while (type == null && current != typeof(object))
        {
            if (current.IsGenericType)
            {
                type = current.GetGenericTypeDefinition().GetGenericArguments()[0].GetType();
            }
            current = current.BaseType;
        }
        return type;
    }
}
