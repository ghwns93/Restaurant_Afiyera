using System.Linq;
using UnityEngine;

public class HomeMaterialUISetting : MonoBehaviour
{
    [SerializeField] private GameObject materialBtn;

    private void Start()
    {
        if(InventoryManager.Instance != null)
        {
            SetMaterialButton();
        }
    }

    private void SetMaterialButton()
    {
        var list = InventoryManager.Instance.slots.Where(slot => slot.quantity > 0).ToList();

        foreach(var slot in list)
        {
            GameObject btn = Instantiate(materialBtn, transform);

            var homeButton = btn.GetComponent<HomeMaterialUiButton>();
            homeButton.itemInfo = slot.itemData;
            homeButton.SetButton();
        }
    }
}
