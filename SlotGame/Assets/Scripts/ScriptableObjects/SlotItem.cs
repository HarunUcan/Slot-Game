using UnityEngine;

[CreateAssetMenu(fileName = "NewSlotItem", menuName = "SlotItem")]
public class SlotItem : ScriptableObject
{
    public GameObject ItemObj;
    public SlotItemType ItemType;

    [Header("Payouts")]
    public float X3;
    public float X4;
    public float X5;
}


