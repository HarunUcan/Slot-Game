using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlotItemMovement : MonoBehaviour
{
    private Vector3 _startPosition;
    public bool CanMove = false;

    private void Awake()
    {
        _startPosition = transform.position;
    }
    void Start()
    {
    }

    void Update()
    {
        if (CanMove)
        {
            CanMove = false;
            transform.position = _startPosition;
            var targetLineIndex = GetComponent<SlotItemInfos>().TargetLineIndex;
            MoveTo(targetLineIndex, 0.3f);
        }

    }

    public void MoveTo(int targetLineIndex, float duration)
    {

        if (targetLineIndex == 0)
            transform.DOMoveY(-3.5f, duration).SetEase(Ease.Linear).OnComplete(() => { transform.position = _startPosition; SlotItemSpawner.SlotRowsPool[GetComponent<SlotItemInfos>().RowIndex].Add(gameObject); gameObject.SetActive(false); }); 
        else if (targetLineIndex == 1)
            transform.DOMoveY(3f, duration).SetEase(Ease.Linear).OnComplete(() => { SlotItemSpawner.SlotRowsPool[GetComponent<SlotItemInfos>().RowIndex].Add(gameObject); });
        else if (targetLineIndex == 2)
            transform.DOMoveY(1.2f, duration).SetEase(Ease.Linear).OnComplete(() => { SlotItemSpawner.SlotRowsPool[GetComponent<SlotItemInfos>().RowIndex].Add(gameObject); });
        else if (targetLineIndex == 3)
            transform.DOMoveY(-0.5f, duration).SetEase(Ease.Linear).OnComplete(() => { SlotItemSpawner.SlotRowsPool[GetComponent<SlotItemInfos>().RowIndex].Add(gameObject); });
        
    }
    public void ResetPosition()
    {
        transform.position = _startPosition;
        GetComponent<SlotItemInfos>().TargetLineIndex = 0;  
        gameObject.SetActive(false);
    }
}
