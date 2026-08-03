using UnityEngine;

public class ShopUnlockableItem : MonoBehaviour
{
    [Header("해금 고유 ID")]
    [SerializeField] private string unlockID;

    [Header("기본 해금 여부 (첫 시작 시)")]
    [SerializeField] private bool isUnlockedByDefault = false;

    [Header("해금 비용")]
    [SerializeField] private int unlockCost = 0;

    [Header("상태에 따른 연출 (선택 사항)")]
    [SerializeField] private GameObject lockedVisual;   // 잠겼을 때 켤 오브젝트 (예: 자물쇠 아이콘)
    [SerializeField] private GameObject unlockedVisual; // 해금됐을 때 켤 오브젝트

    public string UnlockID => unlockID;

    public int UnlockCost { get => unlockCost; set => unlockCost = value; }
    public bool IsUnlockedByDefault { get => isUnlockedByDefault; set => isUnlockedByDefault = value; }

    private void Start()
    {
        // 기본 해금 항목 설정
        if (IsUnlockedByDefault && !ShopUnlockManager.Instance.IsUnlocked(unlockID))
        {
            ShopUnlockManager.Instance.Unlock(unlockID);
        }

        RefreshUI();
    }

    private void OnEnable()
    {
        ShopUnlockManager.OnItemUnlocked += HandleItemUnlocked;
    }

    private void OnDisable()
    {
        ShopUnlockManager.OnItemUnlocked -= HandleItemUnlocked;
    }

    private void HandleItemUnlocked(string id)
    {
        if (id == unlockID)
        {
            RefreshUI();
        }
    }

    // 해금 상태에 따라 비주얼 갱신
    public void RefreshUI()
    {
        bool isUnlocked = ShopUnlockManager.Instance.IsUnlocked(unlockID);

        if (lockedVisual != null) lockedVisual.SetActive(!isUnlocked);
        if (unlockedVisual != null) unlockedVisual.SetActive(isUnlocked);
    }
}
