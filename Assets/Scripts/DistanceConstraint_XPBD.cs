using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 距離拘束。XPBD版
/// </summary>
public class DistanceConstraint_XPBD : IConstraint
{
    public float length
    {
        get => _length;
        set => _length = Mathf.Max(value, 0.0f);
    }
    public float compliance
    {
        get => _compliance;
        set => _compliance = Mathf.Max(value, 0.0f);
    }

    private MassPoint _a, _b;
    private float     _length;
    private float     _compliance;
    private float     _lambda;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public DistanceConstraint_XPBD(float length, float compliance, MassPoint a, MassPoint b)
    {
        Init(length, compliance, a, b);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(float length, float compliance, MassPoint a, MassPoint b)
    {
        this.length = length;
        this.compliance = compliance;
        _a = a;
        _b = b;
    }

    /// <summary>
    /// ラムダをリセット
    /// </summary>
    public void InitLambda()
    {
        _lambda = 0.0f;
    }

    /// <summary>
    /// 拘束計算
    /// </summary>
    public void SolvePosition(float dt)
    {
        var sumMass = _a.invMass + _b.invMass;
        if(sumMass <= 0.0f) { return; }

        var v = _b.position - _a.position;
        var d = v.magnitude;
        if(d <= 0.0f) { return; }

        var constraint  = d - length;               // 目標の距離
        var compliance  = _compliance / (dt * dt);  // コンプライアンス値にdtを加味する
        var dLambda     = (constraint - compliance * _lambda) / (sumMass + compliance); // 今回近づける量

        v = (v / d) * dLambda;  // 拘束ベクトル
        _lambda += dLambda;     // ラムダを累積

        _a.position += v * _a.invMass;
        _b.position -= v * _b.invMass;
        // ヤコビ法でやるとしたら多分こう
        // _a.nextPosition += v * _a.invMass;
        // _b.nextPosition -= v * _b.invMass;
    }

    /// <summary>
    /// 速度の減衰。拘束方向に減衰する
    /// </summary>
    public void SolveVelocity(float dt, float dampCoeff)
    {
        var v = _b.position - _a.position;
        var d = v.magnitude;
        if(d <= 0.0f) { return; }

        var n = v / d;
        var v0  = Vector2.Dot(n, _a.velocity);
        var v1  = Vector2.Dot(n, _b.velocity);
        var dv0 = (v1 - v0) * Mathf.Min(0.5f, dampCoeff * dt * _a.invMass);
        var dv1 = (v0 - v1) * Mathf.Min(0.5f, dampCoeff * dt * _b.invMass);
        _a.velocity += n * dv0;
        _b.velocity += n * dv1;
    }
}
