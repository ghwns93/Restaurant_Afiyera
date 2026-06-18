using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingResourceButton : MonoBehaviour
{
    [SerializeField] private ItemData _data;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _count;

    public ItemData Data { get { return _data; } }

    private void Awake()
    {
        this._icon.sprite = _data.icon;
        this._name.text = _data.itemName;
        this._count.text = "-";

        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        
    }

    public void ActiveAndSetCount(int count)
    {
        this._count.text = count.ToString();
        this.gameObject.SetActive(true);
    }
}