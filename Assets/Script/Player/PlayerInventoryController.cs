using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    private static PlayerInventoryController instance;
    public static PlayerInventoryController Instance => instance;

    [SerializeField] private int selectedSlotIndex = 0; // 현재 선택된 슬롯 번호 (0 ~ 9)
    private int MaxQuickSlots = Enum.GetValues(typeof(ToolType)).Length;                // 1~9 및 0번까지 총 10개의 슬롯

    // 선택된 슬롯이 변경되었을 때벤트를 날려 UI를 갱신할 수 있습니다.
    public delegate void OnSelectedSlotChanged(int slotIndex);
    public event OnSelectedSlotChanged onSelectedSlotChangedCallback;

    public Dictionary<ToolType, ItemBaitData> equippedBaits = new Dictionary<ToolType, ItemBaitData>();

    public int SelectedSlotIndex => selectedSlotIndex;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SelectSlot(selectedSlotIndex);
    }

    private void Update()
    {
        if (!SystemController.Instance.IsSystemPaused) return;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            // Ctrl 키가 눌린 상태에서는 숫자키 입력을 무시 (미끼 선택 UI와 충돌 방지)
            return;
        }

        HandleMouseScrollInput();
    }

    // 마우스 휠 입력 처리 (10칸 순환)
    private void HandleMouseScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            if (scroll > 0f)
            {
                // 휠 위로 -> 이전 슬롯
                selectedSlotIndex = (SelectedSlotIndex - 1 + MaxQuickSlots) % MaxQuickSlots;
            }
            else if (scroll < 0f)
            {
                // 휠 아래로 -> 다음 슬롯
                selectedSlotIndex = (SelectedSlotIndex + 1) % MaxQuickSlots;
            }

            onSelectedSlotChangedCallback?.Invoke(SelectedSlotIndex);
        }
    }

    // 특정 슬롯을 선택 상태로 지정
    private void SelectSlot(int index)
    {
        int inventoryLimit = InventoryManager.Instance != null ? InventoryManager.Instance.GetSlots(MaxQuickSlots).Count : MaxQuickSlots;

        // 유효 범위 체크 (인벤토리에 존재하는 칸 까지만 선택 가능하도록 제한)
        if (index < 0 || index >= MaxQuickSlots || index >= inventoryLimit) return;

        selectedSlotIndex = index;
        //Debug.Log($"인벤토리 {selectedSlotIndex + 1}번 슬롯 선택됨");

        // 콜백 실행 (선택 효과 UI 갱신 등)
        onSelectedSlotChangedCallback?.Invoke(selectedSlotIndex);
    }

    // 현재 플레이어가 손에 든 아이템의 데이터 반환 (비어있으면 null)
    public ItemData GetCurrentlyEquippedItem()
    {
        if (InventoryManager.Instance == null) return null;

        var slots = InventoryManager.Instance.GetSlots(MaxQuickSlots);

        if (selectedSlotIndex < slots.Count && !slots[selectedSlotIndex].IsEmpty)
        {
            return slots[selectedSlotIndex].ItemData;
        }

        return null;
    }

    // 현재 플레이어가 손에 든 아이템의 슬롯 전체 정보 반환
    public ItemSlot GetCurrentlyEquippedSlot()
    {
        if (InventoryManager.Instance == null) return null;

        var slots = InventoryManager.Instance.GetSlots(MaxQuickSlots);

        if (selectedSlotIndex < slots.Count)
        {
            return slots[selectedSlotIndex];
        }

        return null;
    }

    public ItemBaitData GetCurrentlyEquippedBait(ToolType toolType)
    {
        return equippedBaits.TryGetValue(toolType, out var baitData) ? baitData : null;
    }

    public void EquipBaitForTool(ToolType toolType, ItemBaitData baitData)
    {
        if (baitData == null)
        {
            equippedBaits.Remove(toolType);
        }
        else
        {
            equippedBaits[toolType] = baitData;
        }
    }
}