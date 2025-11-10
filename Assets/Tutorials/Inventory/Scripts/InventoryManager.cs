using System.Collections.Generic;
using UnityEngine;

namespace Tutorials.Inventory.Scripts
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_InventoryPrefab;

        [SerializeField] private GameObject m_EmptySlotContainer;
        [SerializeField] private GameObject m_FilledSlotContainer;

        [SerializeField] private int m_MaxSlots = 10;
        [SerializeField] private int m_CurrentSlots = 10;//for adding scrolling functionality

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
            Remove(1, 7);//remove seven apples
            Remove(2, 1);//and one slime
            //apple is item type 1
            //slime is item type 2
        }

        /// <summary>
        /// Adds items to the inventory
        /// </summary>
        /// <param name="itemID">The type of item to add to the inventory</param>
        /// <param name="quantity">The amount of the item to add to the inventory</param>
        public void Add(int itemID, int quantity)
        {
            //Overflow item amount e.g., if 20 is tried to add to a stack of 15 the excess would be 5
            int excess = 0;

            if (m_EmptySlots.Count > 0)
            {
                //checks slots which are already populated with that item type
                foreach (GameObject slot in m_FilledSlots)
                {
                    InventorySlot invSlot = slot.GetComponent<InventorySlot>();

                    if (invSlot.ItemID == itemID)//checks slots of the item type to get added
                    {
                        slot.transform.SetAsLastSibling();//moves the new slot to the end of the menu
                    
                        //updates the item stack and returns any overflow of items which couldn't be added to the stack
                        excess = invSlot.AddItems(quantity);

                        //if there are no items left needing population - end the loop
                        if (excess <= 0)
                        {
                            return;
                        }
                        //if there are items needing population, continue looping and make a new stack
                    }
                }
            
                //If this is a new item, and we have slots left to fill
                if (m_FilledSlots.Count < m_CurrentSlots)
                {
                    //Remove an empty slot from our empty slot list
                    GameObject slot = m_EmptySlots[0];
                    m_EmptySlots.RemoveAt(0);

                    slot.GetComponent<InventorySlot>().InitItemSlot(GetItemByID(itemID), excess > 0 ? excess : quantity);
                    slot.transform.SetParent(m_FilledSlotContainer.transform);

                    //Puts the filled slot into the filled slot list instead thus, adding the item to it
                    m_FilledSlots.Add(slot);
                }
                else
                {
                    /*
                        TODO add scroll functionality here:
                        would be an if statement like if(current slots > page slots)
                        move the screen down every time it is over page slots
                        this whole thing would need to be in if current slots < max slots
                        then return if current slots == max slots
                    */
                }
            }
        }

        /// <summary>
        /// Removes items from the inventory
        /// </summary>
        /// <param name="itemID">The type of item to remove from the inventory</param>
        /// <param name="quantity">The amount of the item to remove from the inventory</param>
        public void Remove(int itemID, int quantity)
        {
            List<GameObject> slotsToRemove = new List<GameObject>();

            //Validates that there are enough items left in the array to be removed
            if (m_FilledSlots.Count <= 0)
            {
                Debug.LogWarning("Insufficient items to remove");
                return;
            }
    
            //Checks slots which are already populated with that item type
            foreach (GameObject slot in m_FilledSlots)
            {
                InventorySlot invSlot = slot.GetComponent<InventorySlot>();

                
                //checks slots of the same item type to be removed
                if (invSlot.ItemID == itemID && quantity > 0)
                {
                    quantity = invSlot.RemoveItems(quantity);

                    if (quantity >= 0)
                    {
                        //Queues them up for removal if they’re going to need to be removed 
                        slotsToRemove.Add(slot);

                        if (m_FilledSlots.Count == 0 && quantity > 0)
                        {
                            Debug.LogWarning("Insufficient items to remove");
                        }
                    }
                }
            }
        
            //moves every slot to remove into the empty slot array
            //this is in a separate foreach so the collection isn't being edited whilst you're iterating through it
            foreach (GameObject slot in slotsToRemove)//in slotsToRemove as opposed to filled slots like before
            {
                slot.transform.SetParent(m_EmptySlotContainer.transform);
                m_FilledSlots.Remove(slot);//can be edited now as another collection is being used to iterate
                m_EmptySlots.Add(slot);
            }
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
}
