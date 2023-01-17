using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// A custom comparer class for lists of key value pairs
public class KeyValuePairComparer<T, P> : IComparer<KeyValuePair<T, P>> where P : IComparable<P>
{
    private bool _ascending;

    public KeyValuePairComparer(bool ascending = true) {
        _ascending = ascending;
    }
    int IComparer<KeyValuePair<T, P>>.Compare(KeyValuePair<T, P> x, KeyValuePair<T, P> y)
    {
        if (_ascending)
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
