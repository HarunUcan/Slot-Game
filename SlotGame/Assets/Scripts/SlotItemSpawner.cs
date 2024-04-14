using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotItemSpawner : MonoBehaviour
{
    private int _spawnCountPerRow = 7;
    [SerializeField] private int _rowIndex;
    [SerializeField] private SlotItem[] _slotItems;
    public static Dictionary<int, List<GameObject>> SlotRowsPool = new Dictionary<int, List<GameObject>>(); // key: row index, value: list of slot items in that row

    private void Awake()
    {
        if (!SlotRowsPool.ContainsKey(_rowIndex))
        {
            SlotRowsPool.Add(_rowIndex, new List<GameObject>());
        }

        foreach (var slotItem in _slotItems)
        {
            for(int i = 0; i < _spawnCountPerRow; i++)
            {
                Transform spawnerTransform = this.transform;
                var slotItemObj = Instantiate(slotItem.ItemObj, spawnerTransform.position, Quaternion.identity);
                slotItemObj.TryGetComponent(out SlotItemInfos slotItemInfos);

                // Set the slot item infos
                slotItemInfos.ItemType = slotItem.ItemType;
                slotItemInfos.X3 = slotItem.X3;
                slotItemInfos.X4 = slotItem.X4;
                slotItemInfos.X5 = slotItem.X5;
                slotItemInfos.RowIndex = _rowIndex;

                // Add the slot item to the pool
                SlotRowsPool[_rowIndex].Add(slotItemObj);

            }

        }
    }

    private void Start()
    {
        foreach (var slotItem in SlotRowsPool[_rowIndex])
        {
            slotItem.SetActive(false);
        }
    }

}
