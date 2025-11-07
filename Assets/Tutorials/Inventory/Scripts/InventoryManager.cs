using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject m_InventoryPrefab;

    [SerializeField] private GameObject m_EmptySlotContainer;
    [SerializeField] private GameObject m_FilledSlotContainer;

    [SerializeField] private int m_MaxSlots = 10;
    [SerializeField] private int m_CurrentSlots = 10;

    private ItemSO[] m_ItemDatabase;

    private List<GameObject> m_EmptySlots;
    private List<GameObject> m_FilledSlots;

    void Start()
    {
        //Debug Init - This should ideally be initialised elsewhere during setup.
        Init();
    }

    public void Init()
    {
        m_ItemDatabase = Resources.LoadAll<ItemSO>("");

        m_EmptySlots = new List<GameObject>();
        m_FilledSlots = new List<GameObject>();

        for (int i = 0; i < m_MaxSlots; i++)
        {
            GameObject slot = Instantiate(m_InventoryPrefab, m_EmptySlotContainer.transform);
            m_EmptySlots.Add(slot);
        }
    }

    public void TestAdd()
    {
        Add(1, 10);
        Add(2, 1);
    }

    public void TestRemove()
    {
        Remove(1, 7);
        Remove(2, 1);
    }

    public void Add(int itemID, int quantity)
    {
        int excess = 0;

        if (m_EmptySlots.Count > 0)
        {
            foreach (GameObject slot in m_FilledSlots)
            {
                InventorySlot invSlot = slot.GetComponent<InventorySlot>();

                if (invSlot.ItemID == itemID)
                {
                    slot.transform.SetAsLastSibling();
                    excess = invSlot.AddItems(quantity);

                    if (excess <= 0)
                    {
                        return;
                    }
                }
            }
        }

        if (m_EmptySlots.Count > 0 && m_FilledSlots.Count < m_CurrentSlots)
        {
            GameObject slot = m_EmptySlots[0];
            m_EmptySlots.RemoveAt(0);

            slot.GetComponent<InventorySlot>().InitItemSlot(GetItemByID(itemID), excess > 0 ? excess : quantity);
            slot.transform.SetParent(m_FilledSlotContainer.transform);

            m_FilledSlots.Add(slot);
        }
    }

    public void Remove(int itemID, int quantity)
    {
        List<GameObject> slotsToRemove = new List<GameObject>();

        if (m_FilledSlots.Count > 0)
        {
            int excess = quantity;

            foreach (GameObject slot in m_FilledSlots)
            {
                InventorySlot invSlot = slot.GetComponent<InventorySlot>();

                if (invSlot.ItemID == itemID && excess > 0)
                {
                    excess = invSlot.RemoveItems(excess);

                    if (excess >= 0)
                    {
                        slotsToRemove.Add(slot);

                        if (m_FilledSlots.Count == 0 && excess > 0)
                        {
                            Debug.LogWarning("Insufficient items to remove");
                        }
                    }
                }
            }

            foreach (GameObject slot in slotsToRemove)
            {
                slot.transform.SetParent(m_EmptySlotContainer.transform);
                m_FilledSlots.Remove(slot);
                m_EmptySlots.Add(slot);
            }

            return;

        }

        Debug.LogWarning("Insufficient items to remove");
    }

    private ItemSO GetItemByID(int id)
    {
        foreach (ItemSO item in m_ItemDatabase)
        {
            if (item.ItemID == id)
            {
                return item;
            }
        }
        Debug.LogError("Item ID " + id + " not found in database");
        return null;
    }
}
