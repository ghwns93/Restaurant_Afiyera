using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderGameManager : MonoBehaviour
{
    [Header("Data References")]
    [SerializeField] private List<CustomerData> customerPool;
    private List<FoodData> allFoods;

    [Header("UI - Customer Area (Top)")]
    [SerializeField] private Image customerImageDisplay;
    [SerializeField] private RectTransform chatContentParent; // Transform -> RectTransform으로 변경
    [SerializeField] private GameObject chatBubblePrefab;

    [Header("UI - Layout Options")]
    [SerializeField] private float bubbleSpacing = 15f; // 말풍선 간 세로 간격
    [SerializeField] private float sidePadding = 20f;   // 좌/우 여백

    [Header("UI - Keyword Area (Bottom Left)")]
    [SerializeField] private Transform keywordToggleParent;
    [SerializeField] private GameObject keywordTogglePrefab;
    [SerializeField] private List<FoodKeyWordName> foodKeyWordNames;

    [Header("UI - Menu Display Area (Bottom Right)")]
    [SerializeField] private Transform menuGridParent;
    [SerializeField] private GameObject menuItemPrefab;

    private CustomerData currentCustomer;
    private HashSet<FoodKeyword> selectedKeywords = new HashSet<FoodKeyword>();
    private Dictionary<FoodKeyword, GameObject> activeKeywordCheckImages = new Dictionary<FoodKeyword, GameObject>();
    private List<GameObject> activeMenuItems = new List<GameObject>();

    private FoodData SelectedFood; 

    [SerializeField] private Image selectedFoodMat1;
    [SerializeField] private Image selectedFoodMat2;
    [SerializeField] private Image selectedFoodCook;

    [SerializeField] private GameObject orderPanel;
    [SerializeField] private GameObject cookPanel;

    private void Start()
    {
        allFoods = CookingCookTypeManager.Instance.GetAllFoodData();
        cookPanel.SetActive(false);

        SpawnRandomCustomer();
    }

    public void SpawnRandomCustomer()
    {
        if (customerPool.Count == 0) return;

        // UI 초기화
        foreach (Transform child in chatContentParent) Destroy(child.gameObject);
        foreach (Transform child in keywordToggleParent) Destroy(child.gameObject);
        selectedKeywords.Clear();

        // Content 높이 초기화
        chatContentParent.sizeDelta = new Vector2(chatContentParent.sizeDelta.x, 0);

        currentCustomer = customerPool[Random.Range(0, customerPool.Count)];
        if (customerImageDisplay != null) customerImageDisplay.sprite = currentCustomer.customerSprite;

        StartCoroutine(RoutineShowDialogues());
        SetupKeywordToggles();
        UpdateMenuFilter();
    }

    private IEnumerator RoutineShowDialogues()
    {
        List<OrderDialogueData> dialogues = currentCustomer.GetDialogues();
        float currentY = -bubbleSpacing; // 상단 여백 시작 위치

        foreach (var dialogue in dialogues)
        {
            GameObject bubbleObj = Instantiate(chatBubblePrefab, chatContentParent);
            RectTransform bubbleRect = bubbleObj.GetComponent<RectTransform>();

            // 1. 텍스트 바인딩
            TMP_Text textComp = bubbleObj.GetComponentInChildren<TMP_Text>();
            if (textComp != null) textComp.text = dialogue.message;

            // 2. 말풍선 크기 강제 계산 (Text/Image 크기에 맞춤)
            Canvas.ForceUpdateCanvases();
            Transform bubbleContent = bubbleObj.transform.Find("BubbleContent");
            RectTransform contentRect = bubbleContent != null ? bubbleContent.GetComponent<RectTransform>() : bubbleRect;

            // TextMeshPro와 ContentSizeFilter 레이아웃 즉시 연산
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

            float bubbleWidth = contentRect.rect.width;
            float bubbleHeight = contentRect.rect.height;

            // 3. 좌/우 앵커 및 좌표 지정 (Pivot = Top-Left 기준)
            bubbleRect.anchorMin = new Vector2(0, 1);
            bubbleRect.anchorMax = new Vector2(0, 1);
            bubbleRect.pivot = new Vector2(0, 1);

            float parentWidth = chatContentParent.rect.width;
            float targetX = 0f;

            if (!dialogue.isCustomer)
            {
                // [나의 말] - 왼쪽 정렬
                targetX = sidePadding;
                Image img = bubbleContent?.GetComponent<Image>();
                if (img != null) img.color = new Color(1f, 0.95f, 0.6f);
            }
            else
            {
                // [손님의 말] - 오른쪽 정렬
                targetX = parentWidth - bubbleWidth - sidePadding;
                Image img = bubbleContent?.GetComponent<Image>();
                if (img != null) img.color = Color.white;
            }

            // 위치 적용
            bubbleRect.anchoredPosition = new Vector2(targetX, currentY);

            // 다음 말풍선을 위한 Y좌표 누적
            currentY -= (bubbleHeight + bubbleSpacing);

            // Content 전체 높이 갱신 (스크롤 범위 확보)
            chatContentParent.sizeDelta = new Vector2(chatContentParent.sizeDelta.x, Mathf.Abs(currentY));

            // 4. 애니메이션 연출
            yield return StartCoroutine(RoutineAnimateBubble(bubbleObj.transform));
            yield return new WaitForSeconds(0.4f);
        }
    }

    private IEnumerator RoutineAnimateBubble(Transform bubbleTransform)
    {
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;
        bubbleTransform.localScale = initialScale;

        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.1f;
            bubbleTransform.localScale = Vector3.LerpUnclamped(initialScale, targetScale, scale);
            yield return null;
        }
        bubbleTransform.localScale = targetScale;
    }

    private void SetupKeywordToggles()
    {
        foreach (Transform child in keywordToggleParent) Destroy(child.gameObject);
        activeKeywordCheckImages.Clear();

        foreach (FoodKeyword keyword in System.Enum.GetValues(typeof(FoodKeyword)))
        {
            GameObject buttonObj = Instantiate(keywordTogglePrefab, keywordToggleParent);
            Button btn = buttonObj.GetComponentInChildren<Button>();
            TMP_Text label = buttonObj.GetComponentInChildren<TMP_Text>();

            Transform checkImgTransform = buttonObj.transform.Find("CheckImage");
            GameObject checkIconObj = checkImgTransform != null ? checkImgTransform.gameObject : null;

            if (label != null)
            {
                foreach (var keyWordName in foodKeyWordNames)
                {
                    if (keyWordName.foodKeyWord == keyword)
                    {
                        label.text = keyWordName.foodKeyWordName;
                        break;
                    }
                }
            }

            if (checkIconObj != null)
            {
                checkIconObj.SetActive(false);
                activeKeywordCheckImages[keyword] = checkIconObj;
            }

            FoodKeyword currentKeyword = keyword;
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnKeywordButtonClicked(currentKeyword));
            }
        }
    }

    private void OnKeywordButtonClicked(FoodKeyword keyword)
    {
        if (selectedKeywords.Contains(keyword))
        {
            selectedKeywords.Remove(keyword);
            if (activeKeywordCheckImages.TryGetValue(keyword, out GameObject checkImg)) checkImg.SetActive(false);
        }
        else
        {
            selectedKeywords.Add(keyword);
            if (activeKeywordCheckImages.TryGetValue(keyword, out GameObject checkImg)) checkImg.SetActive(true);
        }

        UpdateMenuFilter();
    }

    private void UpdateMenuFilter()
    {
        foreach (var item in activeMenuItems) Destroy(item);
        activeMenuItems.Clear();

        foreach (var food in allFoods)
        {
            bool matchAll = true;
            foreach (var kw in selectedKeywords)
            {
                if (!food.keywords.Contains(kw))
                {
                    matchAll = false;
                    break;
                }
            }

            if (matchAll)
            {
                GameObject itemObj = Instantiate(menuItemPrefab, menuGridParent);
                Image img = itemObj.GetComponentInChildren<Image>();
                TMP_Text nameText = itemObj.GetComponentInChildren<TMP_Text>();

                if (img != null) img.sprite = food.iconPlated;
                if (nameText != null) nameText.text = food.foodName;

                Button btn = itemObj.GetComponentInChildren<Button>();
                if (btn != null) btn.onClick.AddListener(() => OnSelectFood(food));

                activeMenuItems.Add(itemObj);
            }
        }
    }

    private void OnSelectFood(FoodData selectedFood)
    {
        //if (selectedFood == currentCustomer.targetFood)
        //{
        //    Debug.Log("주문 성공!");
        //    Invoke(nameof(SpawnRandomCustomer), 1.5f);
        //}
        //else
        //{
        //    Debug.Log("주문 실패!");
        //}

        SelectedFood = selectedFood;

        var matData1 = IngredientDicManager.Instance.GetData(selectedFood.mat[0]);
        var matData2 = IngredientDicManager.Instance.GetData(selectedFood.mat[1]);
        var cookData = CookingCookTypeManager.Instance.GetCookTypeImage(selectedFood.cookType);

        selectedFoodMat1.sprite = matData1.icon;
        selectedFoodMat2.sprite = matData2.icon;
        selectedFoodCook.sprite = cookData;
    }

    public void OpenOrderPanel()
    {
        orderPanel.SetActive(true);
        cookPanel.SetActive(false);

        SelectedFood = null;

        selectedFoodMat1.sprite = null;
        selectedFoodMat2.sprite = null;
        selectedFoodCook.sprite = null;
    }

    public void OpenCookPanel()
    {
        if (SelectedFood == null)
        {
            Debug.Log("음식을 선택 해 주세요!");
            return;
        }

        orderPanel.SetActive(false);
        cookPanel.SetActive(true);
    }
}

[System.Serializable]
public struct FoodKeyWordName
{
    public FoodKeyword foodKeyWord;
    public string foodKeyWordName;
}