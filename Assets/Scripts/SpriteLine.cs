using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 適当に2Dライン引く
/// </summary>
[RequireComponent(typeof(SortingGroup))]
public class SpriteLine : MonoBehaviour
{
    [SerializeField] private Gradient       _grad;
    [SerializeField] private SpriteRenderer _prefPoint;
    [SerializeField] private SpriteRenderer _prefEdge;
    [SerializeField] private float          _width = 0.2f;

    private List<SpriteRenderer> _pointList;
    private List<SpriteRenderer> _edgeList;

    void Awake()
    {
        _pointList = new List<SpriteRenderer>();
        _edgeList  = new List<SpriteRenderer>();
    }

    public void Draw(params List<Vector2>[] posListArray)
    {
        foreach(var item in _pointList)
        {
            item.enabled = false;
        }
        foreach(var item in _edgeList)
        {
            item.enabled = false;
        }

        foreach(var posList in posListArray)
        {
            Draw_(_grad, posList);
        }
    }

    private void Draw_(Gradient grad, List<Vector2> posList)
    {
        int num = posList.Count;
        for(int i = 0; i < num; i++)
        {
            var color = grad.Evaluate((float)i / (num - 1));

            // 節
            {
                if(_pointList.Count <= i)
                {
                    _pointList.Add(Instantiate(_prefPoint, transform));
                }

                var obj = _pointList[i];
                var pos = posList[i];
                obj.gameObject.SetActive(true);
                obj.enabled = true;
                obj.transform.position = pos;
                obj.transform.localScale = new Vector3(_width, _width, 1.0f);
                obj.color = color;
            }

            // 辺
            if(i < num - 1)
            {
                if(_edgeList.Count <= i)
                {
                    _edgeList.Add(Instantiate(_prefEdge, transform));
                }

                var obj = _edgeList[i];
                var a = posList[i];
                var b = posList[i + 1];
                var v = a - b;
                var r = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                obj.gameObject.SetActive(true);
                obj.enabled = true;
                obj.transform.SetPositionAndRotation((a + b) * 0.5f, Quaternion.Euler(0.0f, 0.0f, r));
                obj.transform.localScale = new Vector3(v.magnitude, _width, 1.0f);
                obj.color = color;
            }
        }
    }
}
