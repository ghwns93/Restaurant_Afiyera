using TMPro;
using UnityEngine;

public class PlayerGoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText; // 골드 수량을 표시할 TextMeshProUGUI 컴포넌트

    private void OnEnable()
    {
        if(InventoryManager.Instance != null) InventoryManager.Instance.onInventoryChangedCallback += UpdateGoldUI;
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null) InventoryManager.Instance.onInventoryChangedCallback -= UpdateGoldUI;
    }

    private void Start()
    {
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        if (goldText != null && InventoryManager.Instance != null)
        {
            goldText.text = InventoryManager.Instance.GetGold().ToString("N0");
        }
    }
}
