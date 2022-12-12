using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// XPBD関連の定義
/// </summary>
public static class Const
{
    /// <summary>
    /// コンプライアンス値
    /// </summary>
    public enum Compliance
    {
        Concrete,
        Wood,
        Leather,
        Tendon,
        Rubber,
        Muscle,
        Fat,
        Max,
    };

    /// <summary>
    /// Miles Macklin's blog (http://blog.mmacklin.com/2016/10/12/xpbd-slides-and-stiffness/)
    /// </summary>
    private static readonly float[] COMPLIANCE = new float[(int)Compliance.Max]
    {
        0.00000000004f, // 0.04 x 10^(-9) (M^2/N) Concrete
        0.00000000016f, // 0.16 x 10^(-9) (M^2/N) Wood
        0.000000001f,   // 1.0  x 10^(-8) (M^2/N) Leather
        0.000000002f,   // 0.2  x 10^(-7) (M^2/N) Tendon
        0.0000001f,     // 1.0  x 10^(-6) (M^2/N) Rubber
        0.00002f,       // 0.2  x 10^(-3) (M^2/N) Muscle
        0.0001f,        // 1.0  x 10^(-3) (M^2/N) Fat
    };

    /// <summary>
    /// コンプライアンス値を取得
    /// </summary>
    public static float GetCompliance(Compliance compliance)
    {
        var ret = compliance == Compliance.Max ? 0.0f : COMPLIANCE[(int)compliance];
        return ret;
    }
}
