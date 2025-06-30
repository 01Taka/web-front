using System;
using UnityEngine;

public static class GameMath
{
    /// <summary>
    /// 加重相乗平均を計算します。
    /// </summary>
    /// <param name="a">値A（正）</param>
    /// <param name="b">値B（正）</param>
    /// <param name="weightA">Aの重み</param>
    /// <param name="weightB">Bの重み</param>
    /// <param name="strictMode">trueなら不正値で例外、falseなら極小値に置き換え</param>
    /// <returns>加重相乗平均</returns>
    public static float WeightedGeometricMean(float a, float b, float weightA, float weightB, bool strictMode = true)
    {
        float totalWeight = weightA + weightB;
        if (totalWeight == 0f)
        {
            if (strictMode)
                throw new ArgumentException("Total weight must not be zero.");
            else
                totalWeight = Mathf.Epsilon; // avoid divide by zero
        }

        weightA /= totalWeight;
        weightB /= totalWeight;

        if (strictMode)
        {
            if (a <= 0f || b <= 0f)
                throw new ArgumentException("Inputs a and b must be positive in strict mode.");
        }
        else
        {
            a = Mathf.Max(a, 1e-6f);
            b = Mathf.Max(b, 1e-6f);
        }

        return Mathf.Pow(a, weightA) * Mathf.Pow(b, weightB);
    }
}
