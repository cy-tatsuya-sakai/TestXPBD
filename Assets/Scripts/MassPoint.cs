using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 質点
/// </summary>
public class MassPoint
{
    public float invMass
    {
        get => isKinematic ? 0.0f : _invMass;
        set => _invMass = value;
    }

    public Vector2 position;
    public Vector2 prevPosition;
    public Vector2 nextPosition;
    public Vector2 velocity;
    public bool    isKinematic;

    private float _invMass;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MassPoint(float invMass, Vector2 pos, bool isKinematic = false)
    {
        Init(invMass, pos, isKinematic);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(float invMass, Vector2 pos, bool isKinematic = false)
    {
        _invMass = invMass;
        position = prevPosition = nextPosition = pos;
        velocity = Vector2.zero;
        this.isKinematic = isKinematic;
    }

    /// <summary>
    /// 拘束計算の前処理
    /// ヤコビ法でやるとしたら多分こう
    /// </summary>
    public void Prepare()
    {
        nextPosition = position;
    }

    /// <summary>
    /// 拘束計算の適用
    /// ヤコビ法でやるとしたら多分こう
    /// </summary>
    public void Apply()
    {
        position = nextPosition;
    }

    /// <summary>
    /// 座標を更新
    /// </summary>
    public void UpdatePosition(float dt)
    {
        if(dt <= 0.0f)  { return; }
        if(isKinematic) { return; }

        prevPosition = position;
        position += velocity * dt;
    }

    /// <summary>
    /// 速度を更新
    /// </summary>
    public void UpdateVelocity(float dt, float gravity)
    {
        velocity = (position - prevPosition) * (1.0f / dt);
        velocity.y += gravity * dt;
    }

    /// <summary>
    /// 速度の減衰
    /// </summary>
    public void SolveVelocity(float dampCoeff, float dt)
    {
        var v = velocity.magnitude;
        if(v <= 0.0f) { return; }

        var n = velocity / v;
        var dv = -v * Mathf.Min(1.0f, dampCoeff * dt * invMass);
        velocity += n * dv;
    }

    /// <summary>
    /// 座標を設定。つかみ処理用
    /// </summary>
    public void SetPosition(Vector2 pos)
    {
        position = prevPosition = pos;
    }
}
