using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public int ItemID;
    public string ItemName;
    public Sprite ItemIcon;
    public int MaxStack;
}
