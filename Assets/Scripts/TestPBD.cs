#define XPBD    // アンコメントするとそれぞれ有効になります
// #define SUBSTEP //

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// PBDのテスト
/// </summary>
public class TestPBD : MonoBehaviour
{
    private const int   POINT_NUM   = 10;
    private const float GRAVITY     = 9.8f * 2; // 適当。見た目重視
    private const float LENGTH      = 1.0f;
    private const float STIFFNESS   = 0.1f;                             // PBDの固さ
    private const Const.Compliance COMPLIANCE   = Const.Compliance.Fat; // XPBDの固さ

    [SerializeField]                        private SpriteLine  _sprLine;
    [SerializeField, Range(1, 1000)]        private int         _step = 10;
    [SerializeField, Range(0.0f, 2.0f)]     private float       _globalDampingCoeff = 0.0f; // 範囲はデモを参考に…正解はよく分からない
    [SerializeField, Range(0.0f, 100.0f)]   private float       _edgeDampingCoeff = 0.0f;   // https://matthias-research.github.io/pages/challenges/pendulum.html

    private List<MassPoint>     _massPointList;
    private List<IConstraint>   _constraintList;
    private Vector2 _pullPosition;
    private bool    _isPull;

    void Start()
    {
        // ひも生成
        _massPointList  = new List<MassPoint>(POINT_NUM);
        _constraintList = new List<IConstraint>(POINT_NUM - 1);

        for(int i = 0; i < POINT_NUM; i++)
        {
            var invMass = i == POINT_NUM - 1 ? 0.1f : 1.0f; // 先端だけちょっと重くしてみる
            _massPointList.Add(new MassPoint(invMass, new Vector2(i, 0.0f), i == 0));
        }

        for(int i = 0; i < POINT_NUM - 1; i++)
        {
            var a = _massPointList[i];
            var b = _massPointList[i + 1];
#if XPBD
            var compliance = Const.GetCompliance(COMPLIANCE);
            _constraintList.Add(new DistanceConstraint_XPBD(LENGTH, compliance, a, b));
#else
            _constraintList.Add(new DistanceConstraint_PBD(LENGTH, STIFFNESS, a, b));
#endif
        }
    }

    void Update()
    {
        // マウスの引っ張り
        _isPull = Input.GetMouseButton(0);
        _pullPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        // マウスの引っ張り
        PullLastPoint();

        // シミュレーション全体
#if SUBSTEP
        dt = dt / _step;
        for(int i = 0; i < _step; i++)
        {
            Simulate(dt, 1);
        }
#else
        Simulate(dt, _step);
#endif
    }

    /// <summary>
    /// シミュレーション全体
    /// </summary>
    private void Simulate(float dt, int iter)
    {
        // 座標更新
        foreach(var massPoint in _massPointList)
        {
            massPoint.UpdatePosition(dt);
        }

        // 拘束計算の前処理
        // foreach(var massPoint in _massPointList)
        // {
        //     massPoint.Prepare();
        // }
        foreach(var constraint in _constraintList)
        {
            constraint.InitLambda();
        }

        // 拘束計算
        for(int i = 0; i < iter; i++)
        {
            foreach(var constraint in _constraintList)
            {
                constraint.SolvePosition(dt);
            }
            // foreach(var massPoint in _massPointList)
            // {
            //     massPoint.Apply();
            // }
        }

        // 速度を更新
        foreach(var massPoint in _massPointList)
        {
            massPoint.UpdateVelocity(dt, -GRAVITY);
        }

        // 速度の減衰各種
        foreach(var massPoint in _massPointList)
        {
            massPoint.SolveVelocity(dt, _globalDampingCoeff);
        }
        foreach(var constraint in _constraintList)
        {
            constraint.SolveVelocity(dt, _edgeDampingCoeff);
        }
    }

    /// <summary>
    /// マウスの引っ張り
    /// </summary>
    private void PullLastPoint()
    {
        var massPoint = _massPointList[_massPointList.Count - 1];
        if(_isPull)
        {
            massPoint.SetPosition(_pullPosition);
        }
        massPoint.isKinematic = _isPull;
    }

    void LateUpdate()
    {
        // ひも描画
        var posList = _massPointList.Select(x => x.position).ToList();
        _sprLine.Draw(posList);
    }
}
