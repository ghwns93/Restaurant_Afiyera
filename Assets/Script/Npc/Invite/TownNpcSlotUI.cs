using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TownNpcSlotUI : MonoBehaviour
{

    [Header("미해금 표기 설정")]
    [SerializeField] private GameObject lockedPanel;

    [Header("해금 표기 설정")]
    [SerializeField] private RectTransform backgroundPanel;
    [SerializeField] private Image npcIconImage;
    [SerializeField] private TextMeshProUGUI npcName;
    [SerializeField] private GameObject matPanel;
    [SerializeField] private List<Image> matList;

    public static event Action OnStateChanged;

    [Header("선택 이미지")]
    [SerializeField] private GameObject selectedImage;

    [Header("이미지 사이즈 조정")]
    [Range(0.1f, 2.0f)]
    [SerializeField] private float spriteRate = 1.0f;
    [SerializeField] private float imageOffsetY = 0.0f;

    private NpcSpecialEventData eventData;

    private bool selected = false;

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
        npcIconImage.gameObject.SetActive(isUnlocked);
        npcName.gameObject.SetActive(isUnlocked);
        matPanel.gameObject.SetActive(isUnlocked);
        selectedImage.SetActive(false);

        if (isUnlocked)
        {
            npcIconImage.sprite = data.NpcInfo.npcIconImage;
            npcName.text = data.NpcInfo.npcName;
            eventData = data;

            FitNpcImageSize();

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

            if(NpcSpecialEventManager.Instance.GetSelectedId() == eventData) OnActiveButton();
            else InActiveButton();
        }
    }

    private void FitNpcImageSize()
    {
        // 사이즈 조정
        float spriteWidth = npcIconImage.sprite.rect.size.x;
        float spriteHeight = npcIconImage.sprite.rect.size.y;

        float newWidth = spriteWidth * spriteRate;
        float newHeight = spriteHeight * spriteRate;

        Debug.Log($"Sprite Size: {spriteWidth} x {spriteHeight}, New Size: {newWidth} x {newHeight}");

        npcIconImage.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);


        // 위치 조정
        float floatBack = backgroundPanel.rect.height;
        float floatImage = npcIconImage.rectTransform.rect.height;

        float targetPosY = backgroundPanel.anchoredPosition.y - (floatBack * 0.5f) + (floatImage * 0.5f) + (imageOffsetY * 0.5f);

        npcIconImage.rectTransform.anchoredPosition = new Vector2(npcIconImage.rectTransform.anchoredPosition.x, targetPosY);
    }

    public void SelectButtonClick()
    {
        if (selected)
        {
            NpcSpecialEventManager.Instance.ResetEvent();
            OnStateChanged?.Invoke();
        }
        else
        {
            if (CheckAllIngredientsCollected())
            {
                NpcSpecialEventManager.Instance.SetSelectedEventId(eventData);
                OnStateChanged?.Invoke();
                OnActiveButton();
            }
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
        // 이미지 변경
        selectedImage.SetActive(false);

        selected = false;
    }

    private void OnActiveButton()
    {
        selectedImage.SetActive(true);

        selected = true;
    }
}