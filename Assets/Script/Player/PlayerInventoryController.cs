using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    private static PlayerInventoryController instance;
    public static PlayerInventoryController Instance => instance;

    [SerializeField] private int selectedSlotIndex = 0; // 현재 선택된 슬롯 번호 (0 ~ 9)
    private const int MaxQuickSlots = 10;                // 1~9 및 0번까지 총 10개의 슬롯

    // 선택된 슬롯이 변경되었을 때벤트를 날려 UI를 갱신할 수 있습니다.
    public delegate void OnSelectedSlotChanged(int slotIndex);
    public event OnSelectedSlotChanged onSelectedSlotChangedCallback;

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

    private void Update()
    {
        HandleNumberKeyInput();
        HandleMouseScrollInput();
    }

    // 숫자 키 입력 처리 (1~9, 그리고 0)
    private void HandleNumberKeyInput()
    {
        // 1~9 키 처리
        for (int i = 0; i < 9; i++)
        {
            KeyCode alphaKey = KeyCode.Alpha1 + i;
            KeyCode keypadKey = KeyCode.Keypad1 + i;

            if (Input.GetKeyDown(alphaKey) || Input.GetKeyDown(keypadKey))
            {
                SelectSlot(i);
                break;
            }
        }

        // '0' 키 또는 숫자패드 '0' 입력 시 10번째 슬롯(인덱스 9) 선택
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeypadZeroCheck()))
        {
            SelectSlot(9);
        }
    }

    // 숫자패드 0 키 체크 헬퍼
    private KeyCode KeypadZeroCheck()
    {
        return KeyCode.Keypad0;
    }

    // 마우스 휠 입력 처리 (10칸 순환)
    private void HandleMouseScrollInput()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

        if (scrollDelta != 0f)
        {
            int totalSlots = MaxQuickSlots;
            if (InventoryManager.Instance != null)
            {
                // 인벤토리 전체 칸 수와 10칸 중 작은 값 혹은 전체 칸으로 설정 가능
                totalSlots = Mathf.Min(MaxQuickSlots, InventoryManager.Instance.GetSlots().Count);
            }

            if (scrollDelta < 0f)
            {
                // 휠을 아래로 내릴 때: index++
                selectedSlotIndex++;
                if (selectedSlotIndex >= totalSlots)
                {
                    selectedSlotIndex = 0; // 끝에 도달하면 처음(1번 슬롯)으로 순환
                }
            }
            else if (scrollDelta > 0f)
            {
                // 휠을 위로 올릴 때: index--
                selectedSlotIndex--;
                if (selectedSlotIndex < 0)
                {
                    selectedSlotIndex = totalSlots - 1; // 처음에 도달하면 맨 끝(10번 슬롯)으로 순환
                }
            }

            SelectSlot(selectedSlotIndex);
        }
    }

    // 특정 슬롯을 선택 상태로 지정
    private void SelectSlot(int index)
    {
        int inventoryLimit = InventoryManager.Instance != null ? InventoryManager.Instance.GetSlots().Count : MaxQuickSlots;

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

        var slots = InventoryManager.Instance.GetSlots();

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

        var slots = InventoryManager.Instance.GetSlots();

        if (selectedSlotIndex < slots.Count)
        {
            return slots[selectedSlotIndex];
        }

        return null;
    }
}