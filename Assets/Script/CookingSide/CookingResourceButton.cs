using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CookingResourceButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ItemData _data;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _count;

    public ItemData Data { get { return _data; } }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CookingWorkerManager.Instance.CheckIsWorkerCanWork()) return;

        InventoryManager.Instance.ReduceItem(this._data,1);
        this._count.text = InventoryManager.Instance.GetItem(this._data).ToString();
        CookingWorkerManager.Instance.StartWorking(_data);
    }

    private void Awake()
    {
        this._icon.sprite = _data.icon;
        this._name.text = _data.itemName;
        this._count.text = "-";

        this.gameObject.SetActive(false);
    }

    public void ActiveAndSetCount(int count)
    {
        this._count.text = count.ToString();
        this.gameObject.SetActive(true);
    }
}