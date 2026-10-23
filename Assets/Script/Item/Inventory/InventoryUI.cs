using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("원형 인벤토리 세팅")]
    [SerializeField] private RectTransform slotParent;   // Vertical Layout Group이 부착된 부모
    [SerializeField] private GameObject slotPrefab;  // 슬롯 UI 프리팹
    [SerializeField] private float moveSpeed = 15f;    // 이동 속도

    [Header("슬롯 제한")]
    [SerializeField] private int maxSlotCount = 10;    // 최대 슬롯 개수

    private List<InventorySlotUI> slotUIList = new List<InventorySlotUI>();
    private Coroutine moveCoroutine;
    private VerticalLayoutGroup layoutGroup;

    private void Start()
    {
        if (slotParent != null)
        {
            layoutGroup = slotParent.GetComponent<VerticalLayoutGroup>();
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChangedCallback += UpdateUI;
            InitSlots();
            UpdateUI();
        }

        if (PlayerInventoryController.Instance != null)
        {
            PlayerInventoryController.Instance.onSelectedSlotChangedCallback += OnSelectedSlotChanged;
            OnSelectedSlotChanged(PlayerInventoryController.Instance.SelectedSlotIndex);
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChangedCallback -= UpdateUI;
        }

        if (PlayerInventoryController.Instance != null)
        {
            PlayerInventoryController.Instance.onSelectedSlotChangedCallback -= OnSelectedSlotChanged;
        }
    }

    private void InitSlots()
    {
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }
        slotUIList.Clear();

        List<ItemSlot> slots = InventoryManager.Instance.GetSlots(ItemType.Tool);
        for (int i = 0; i < slots.Count; i++)
        {
            if (i >= maxSlotCount) break;

            GameObject slotObj = Instantiate(slotPrefab, slotParent);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
            slotUIList.Add(slotUI);
        }
    }

    private void UpdateUI()
    {
        List<ItemSlot> slots = InventoryManager.Instance.GetSlots(ItemType.Tool);

        if (slotUIList.Count != slots.Count && slotUIList.Count < maxSlotCount)
        {
            InitSlots();
        }

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i < slots.Count && !slots[i].IsEmpty)
            {
                slotUIList[i].SetItem(slots[i].ItemData, slots[i].Quantity);
            }
            else
            {
                slotUIList[i].ClearSlot();
            }
        }
    }

    private void OnSelectedSlotChanged(int slotIndex)
    {
        int totalCount = slotUIList.Count;
        if (totalCount == 0) return;

        slotIndex = Mathf.Clamp(slotIndex, 0, totalCount - 1);

        // 1. 자식의 높이(Height)와 VerticalLayoutGroup의 Spacing 가져오기
        float childHeight = 0f;
        if (totalCount > 0 && slotUIList[0] != null)
        {
            RectTransform childRect = slotUIList[0].GetComponent<RectTransform>();
            childHeight = childRect.rect.height;
        }

        float spacing = (layoutGroup != null) ? layoutGroup.spacing : 0f;

        // 2. 한 칸당 이동할 거리 (자식 y 크기 + spacing 수치)
        float step = childHeight + spacing;

        // 3. 중앙 인덱스 계산 (예: 5개일 때 5 / 2 = 2)
        int centerIndex = totalCount / 2;

        // 4. 중앙(2번)일 때 PosY = 0,
        // 2 -> 1 -> 0 일 때 -(step) 만큼 차감
        // 2 -> 3 -> 4 일 때 +(step) 만큼 증가
        float targetY = (slotIndex - centerIndex) * step;

        Vector2 targetPosition = new Vector2(0f, targetY);

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(SmoothMoveCoroutine(targetPosition));
    }

    private IEnumerator SmoothMoveCoroutine(Vector2 targetPos)
    {
        SFXPlayer.Play(SoundName.SFX_ToolSelect);

        while (Vector2.Distance(slotParent.anchoredPosition, targetPos) > 0.1f)
        {
            slotParent.anchoredPosition = Vector2.Lerp(
                slotParent.anchoredPosition,
                targetPos,
                Time.deltaTime * moveSpeed
            );
            yield return null;
        }

        slotParent.anchoredPosition = targetPos;
        moveCoroutine = null;
    }
}