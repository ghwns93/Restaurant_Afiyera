using UnityEngine;
using UnityEngine.EventSystems;

public class ShopDropReceiver : MonoBehaviour, IDropHandler
{
    [SerializeField] private int sellPriceMultiplier = 1; // 판매 가격 배율

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("[ShopDropReceiver] 상점 영역에 아이템 드랍됨!");

        GameObject draggedObj = eventData.pointerDrag;
        if (draggedObj == null) return;

        // 드래그된 오브젝트에서 인벤토리 슬롯 UI 컴포넌트 탐색
        InventorySlotUI draggedSlot = draggedObj.GetComponentInParent<InventorySlotUI>();
        if (draggedSlot == null) return;

        int slotIndex = draggedSlot.transform.GetSiblingIndex();

        if (InventoryManager.Instance != null)
        {
            var slotData = InventoryManager.Instance.slots[slotIndex];

            if (slotData != null && slotData.ItemData != null && slotData.Quantity > 0)
            {
                ItemData soldItem = slotData.ItemData;
                int soldCount = slotData.Quantity; // 전부 판매

                int unitPrice = soldItem.BaseSellPrice;

                if (soldItem.ItemType == ItemType.Ingredient)
                {
                    unitPrice = ShopManager.Instance.CheckIngredientPrice((ItemIngredientData)soldItem);
                }

                int totalEarnings = unitPrice * soldCount * sellPriceMultiplier;

                // 인벤토리에서 제거 및 골드 지급
                InventoryManager.Instance.ReduceItem(soldItem, soldCount);
                InventoryManager.Instance.AddGold(totalEarnings);

                if (ShopManager.Instance != null)
                {
                    ShopManager.Instance.UpdatePlayerGoldUI();
                }

                Debug.Log($"[상점] {soldItem.ItemName} (수량: {soldCount})을(를) 판매하여 총 {totalEarnings} 골드를 획득했습니다!");
            }
        }
    }
}