using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("선택 표시 UI")]
    [SerializeField] private RectTransform selectorRectTransform; // 이동할 화살표(선택 UI)의 RectTransform
    [SerializeField] private List<RectTransform> slotRectTransforms; // 각 인벤토리 슬롯들의 RectTransform 목록 (1번부터 0번까지 순서대로)
    [SerializeField] private float moveSpeed = 15f; // 값이 클수록 빠르게 이동합니다.

    [Header("아이템 슬롯 UI")]
    [SerializeField] private Transform slotParent;   // 그리드 레이아웃 그룹이 있는 부모 객체
    [SerializeField] private GameObject slotPrefab;  // 슬롯 UI 프리팹
    [SerializeField] private int initialSlotCount = 10; // 초기 슬롯 개수

    private List<InventorySlotUI> slotUIList = new List<InventorySlotUI>();
    private Coroutine moveCoroutine;

    private void Start()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChangedCallback += UpdateUI;
            InitSlots();
            UpdateUI();
        }

        if (PlayerInventoryController.Instance != null)
        {
            PlayerInventoryController.Instance.onSelectedSlotChangedCallback += OnSelectedSlotChanged;
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
        // 기존 생성된 슬롯 UI 청소
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }
        slotUIList.Clear();

        // 인벤토리 매니저의 슬롯 개수(유동적으로 늘어난 크기)만큼 UI 슬롯 생성
        List<ItemSlot> slots = InventoryManager.Instance.GetSlots(initialSlotCount);
        for (int i = 0; i < slots.Count; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotParent);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
            slotUIList.Add(slotUI);
        }
    }

    private void UpdateUI()
    {
        List<ItemSlot> slots = InventoryManager.Instance.GetSlots(initialSlotCount);

        // 만약 유동적으로 인벤토리 칸 수가 변경되었다면 UI를 재구축
        if (slotUIList.Count != slots.Count)
        {
            InitSlots();
        }

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (!slots[i].IsEmpty)
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
        if (slotIndex < 0) slotIndex = 0; // 음수 인덱스 방지

        if (slotIndex >= slotRectTransforms.Count) slotIndex = slotRectTransforms.Count - 1;

        // 목표 위치(슬롯의 로컬 좌표 기준 또는 부모 기준 위치) 계산
        // 만약 슬롯과 셀렉터가 같은 부모를 공유하고 있다면 anchoredPosition을 사용합니다.
        Vector2 targetPosition = slotRectTransforms[slotIndex].anchoredPosition;

        // 기존에 실행 중이던 이동 코루틴이 있다면 중지하여 겹침 방지
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // 새로운 이동 코루틴 시작
        moveCoroutine = StartCoroutine(SmoothMoveCoroutine(targetPosition));
    }

    private IEnumerator SmoothMoveCoroutine(Vector2 targetPos)
    {
        // Vector2.Lerp 또는 무한 루프 내 Approach 방식을 사용하여 스르륵 이동
        while (Vector2.Distance(selectorRectTransform.anchoredPosition, targetPos) > 0.1f)
        {
            selectorRectTransform.anchoredPosition = Vector2.Lerp(
                selectorRectTransform.anchoredPosition,
                targetPos,
                Time.deltaTime * moveSpeed
            );
            yield return null; // 다음 프레임까지 대기
        }

        // 미세한 오차 보정을 위해 최종 위치에 정확히 안착
        selectorRectTransform.anchoredPosition = targetPos;
        moveCoroutine = null;
    }
}