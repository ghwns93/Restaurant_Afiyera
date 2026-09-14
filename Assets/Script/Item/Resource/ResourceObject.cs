using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ResourceObject : MonoBehaviour
{
    [SerializeField] private ResourceType resourceType; // 이 자원의 종류 (Tree, Rock, Lake, Animal 등)
    [SerializeField] private ItemData rewardItemData;    // 획득할 재료 (도구 데이터에 지정 안 했을 경우 여기서 기본 제공 가능)
    [SerializeField] private int rewardItemCount = 1;

    public ResourceType ResourceType => resourceType;
    public ItemData RewardItemData => rewardItemData;
    public int RewardItemCount => rewardItemCount;

    private void Awake()
    {
        // 2D Trigger Collider 필수 체크
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ResourceGatherController gatherController = other.GetComponent<ResourceGatherController>();
            if (gatherController != null)
            {
                gatherController.EnterResourceRange(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ResourceGatherController gatherController = other.GetComponent<ResourceGatherController>();
            if (gatherController != null)
            {
                gatherController.ExitResourceRange(this);
            }
        }
    }
}