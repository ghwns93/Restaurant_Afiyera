using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UI;

public class TownNpcSlotUI : MonoBehaviour
{
    [SerializeField] private Image npcIconImage;
    [SerializeField] private List<Image> matList;

    [Header("미해금 표기 설정")]
    [SerializeField] private GameObject lockedPanel;

    public static event Action OnStateChanged;

    [Header("버튼 이미지")]
    [SerializeField] private Image targetImage;

    [Header("활성화 버튼 색상")]
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Sprite activeSprite;

    [Header("비활성화 버튼 색상")]
    [SerializeField] private Color inactiveColor = Color.white;
    [SerializeField] private Sprite inactiveSprite;

    private NpcSpecialEventData eventData;

    private void OnEnable()
    {
        // 이벤트 구독
        TownNpcSlotUI.OnStateChanged += InActiveButton;
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        TownNpcSlotUI.OnStateChanged -= InActiveButton;
    }

    public void Setup(NpcSpecialEventData data, bool isUnlocked)
    {
        lockedPanel.SetActive(!isUnlocked);

        if (isUnlocked)
        {
            npcIconImage.sprite = data.NpcSprite;
            eventData = data;

            var requiredIngredients = data.RequiredIngredients;

            for (int i = 0; i < matList.Count; i++)
            {
                if (i < requiredIngredients.Count)
                {
                    matList[i].sprite = requiredIngredients[i].Icon;
                    matList[i].gameObject.SetActive(true);
                }
                else
                {
                    matList[i].gameObject.SetActive(false);
                }
            }

            InActiveButton();
        }
    }

    public void SelectButtonClick()
    {
        if (CheckAllIngredientsCollected())
        {
            NpcSpecialEventManager.Instance.SetSelectedEventId(eventData.EventId);
            OnStateChanged?.Invoke();
            OnActiveButton();
        }
    }

    private bool CheckAllIngredientsCollected()
    {
        if (eventData == null) return false;
        foreach (var ingredient in eventData.RequiredIngredients)
        {
            if (InventoryManager.Instance.GetItemCount(ingredient) == 0)
            {
                return false; // 하나라도 부족하면 false 반환
            }
        }
        return true; // 모든 재료가 충분하면 true 반환
    }

    private void InActiveButton()
    {
        if (targetImage == null) return;

        // 색상 변경
        targetImage.color = inactiveColor;

        //// 이미지 변경 (Sprite가 할당되어 있는 경우에만)
        //Sprite targetSprite = isActive ? activeSprite : inactiveSprite;
        //if (targetSprite != null)
        //{
        //    targetImage.sprite = targetSprite;
        //}
    }

    private void OnActiveButton()
    {
        if (targetImage == null) return;

        targetImage.color = activeColor;
    }
}