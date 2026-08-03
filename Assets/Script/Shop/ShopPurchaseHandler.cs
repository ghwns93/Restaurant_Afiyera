using UnityEngine;

// 소모품의 종류 분류 (프로젝트 환경에 맞게 수정 가능)
public enum ConsumableType
{
    Build,      // 건물
    Flavoring,  // 향신료
}

public class ShopPurchaseHandler : MonoBehaviour
{
    public static ShopPurchaseHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 상점 구매 버튼 클릭 시 호출하는 함수
    /// </summary>
    public void PurchaseConsumable(string unlockItemID, ConsumableType type, int amount)
    {
        // 각 타입별로 이미 존재하는 개별 매니저의 함수를 호출하여 저장합니다.
        switch (type)
        {
            case ConsumableType.Build:
                // 기존 포션 매니저 호출
                if (BuildableCountManager.Instance != null)
                {
                    int nodeId = ((BuildDicManager)BuildDicManager.Instance).GetNodeIdByUnlockID(unlockItemID);

                    BuildableCountManager.Instance.AddBuildableCount(nodeId, amount);
                }
                break;

            default:
                Debug.LogWarning($"[ShopPurchaseHandler] {type}에 해당하는 매니저가 정의되지 않았습니다.");
                break;
        }

        Debug.Log($"[구매 완료] {unlockItemID} ({type}) {amount}개 추가됨");
    }
}
