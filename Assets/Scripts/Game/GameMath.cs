using System;
using UnityEngine;

public static class GameMath
{
    /// <summary>
    /// ���d���敽�ς��v�Z���܂��B
    /// </summary>
    /// <param name="a">�lA�i���j</param>
    /// <param name="b">�lB�i���j</param>
    /// <param name="weightA">A�̏d��</param>
    /// <param name="weightB">B�̏d��</param>
    /// <param name="strictMode">true�Ȃ�s���l�ŗ�O�Afalse�Ȃ�ɏ��l�ɒu������</param>
    /// <returns>���d���敽��</returns>
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
