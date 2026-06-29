using System.Collections.Generic;
using UnityEngine;

public class CookingResourcePanelController : MonoBehaviour
{
    [SerializeField] private List<CookingResourceButton> _buttons;

    private InventoryManager _IM;


    private void Start()
    {
        _IM = InventoryManager.Instance;
        List<ItemSlot> slots = new List<ItemSlot>(_IM.slots);

        CookingResourceButton temp = null;

        foreach (ItemSlot slot in slots)
        {
            temp = _buttons.Find(x => x.Data.id == slot.itemData.id);
            if (temp != null)
            {
                temp.ActiveAndSetCount(slot.quantity);
            }
        }
    }
}