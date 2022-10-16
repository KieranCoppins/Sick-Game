using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// A custom comparer class that ensures the largest value of the key value pair appears first in the array
public class KeyValuePairComparer<T, P> : IComparer<KeyValuePair<T, P>> where P : IComparable<P>
{
    bool ascending;
    public KeyValuePairComparer(bool ascending = true) {
        this.ascending = ascending;
    }
    int IComparer<KeyValuePair<T, P>>.Compare(KeyValuePair<T, P> x, KeyValuePair<T, P> y)
    {
        if (ascending)
        {
            if (x.Value.CompareTo(y.Value) > 0)
                return -1;
            else if (x.Value.CompareTo(y.Value) == 0)
                return 0;
            else
                return 1;
        }
        else
        {
            if (x.Value.CompareTo(y.Value) > 0)
                return 1;
            else if (x.Value.CompareTo(y.Value) == 0)
                return 0;
            else
                return -1;
        }
    }
}
