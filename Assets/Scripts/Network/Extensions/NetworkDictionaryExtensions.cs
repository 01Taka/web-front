using Fusion;
using System;
using System.Collections.Generic;

public static class NetworkDictionaryExtensions
{
    /// <summary>
    /// NetworkDictionary に TryGetValue 相当の処理を追加。
    /// </summary>
    public static bool TryGetValueSafe<TKey, TValue>(
        this NetworkDictionary<TKey, TValue> dict,
        TKey key,
        out TValue value)
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : unmanaged
    {
        if (dict.ContainsKey(key))
        {
            value = dict[key];
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// NetworkDictionary のすべての値を取得する
    /// </summary>
    public static TValue[] GetValues<TKey, TValue>(
        this NetworkDictionary<TKey, TValue> dict)
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : unmanaged
    {
        var values = new TValue[dict.Count];
        int i = 0;
        foreach (var kvp in dict)
        {
            values[i] = kvp.Value;
            i++;
        }
        return values;
    }

    /// <summary>
    /// NetworkDictionary のすべてのキーを取得する
    /// </summary>
    public static TKey[] GetKeys<TKey, TValue>(
        this NetworkDictionary<TKey, TValue> dict)
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : unmanaged
    {
        var keys = new TKey[dict.Count];
        int i = 0;
        foreach (var kvp in dict)
        {
            keys[i] = kvp.Key;
            i++;
        }
        return keys;
    }
}