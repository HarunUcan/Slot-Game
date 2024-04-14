using UnityEngine;

public class SlotItemInfos : MonoBehaviour
{
    public SlotItemType ItemType;
   
    public float X3;
    public float X4;
    public float X5;

    [HideInInspector]
    public int RowIndex;
    [HideInInspector]
    public int TargetLineIndex;
}
