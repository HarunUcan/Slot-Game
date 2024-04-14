using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer _ln;
    [SerializeField] private int _lineIndex;
    [SerializeField] private List<Transform> _Points;
    [SerializeField] private Transform _endTransform;
    private Transform[,] _PointsTransforms = new Transform[3, 5];


    public static readonly int[,] PayLines = new int[20, 5] {
        {1,1,1,1,1},
        {0,0,0,0,0},
        {2,2,2,2,2},
        {0,1,2,1,0},
        {2,1,0,1,2},
        {1,0,0,0,1},
        {1,2,2,2,1},
        {0,0,1,2,2},
        {2,2,1,0,0},
        {1,2,1,0,1},
        {1,0,1,2,1},
        {0,1,1,1,0},
        {2,1,1,1,2},
        {0,1,0,1,0},
        {2,1,2,1,2},
        {1,1,0,1,1},
        {1,1,2,1,1},
        {0,0,2,0,0},
        {2,2,0,2,2},
        {0,2,2,2,0}
    };

    private void Awake()
    {
        _ln = GetComponent<LineRenderer>();
        _ln.positionCount = 7;
        _ln.startWidth = 0.08f;
        _ln.endWidth = 0.08f;
        _ln.startColor = GetComponent<SpriteRenderer>().color;
        _ln.endColor = GetComponent<SpriteRenderer>().color;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                _PointsTransforms[i, j] = _Points[i * 5 + j];
            }
        }

        _ln.SetPosition(0, this.transform.position);
        var randPosYOffset = Random.Range(-0.15f, 0.15f);
        var randPosXOffset = Random.Range(-0.15f, 0.15f);
        for (int i = 1; i < 6; i++)
        {
            Vector3 pos = new Vector3(_PointsTransforms[PayLines[_lineIndex, i - 1], i - 1].position.x + randPosXOffset, _PointsTransforms[PayLines[_lineIndex, i - 1], i - 1].position.y + randPosYOffset, _PointsTransforms[PayLines[_lineIndex, i - 1], i - 1].position.z);
            _ln.SetPosition(i, pos);
        }
        _ln.SetPosition(6, _endTransform.position);
    }

    



}
