using System;
using UnityEngine;
using UnityEngine.UI;

public class HomeMaterialUiButton : MonoBehaviour
{
    [NonSerialized]
    public ItemData itemInfo;

    private bool selected = false;
    private Image image;

    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color unselectedColor = Color.white;

    public void SetButton()
    {
        SetSprite();
    }

    private void SetSprite()
    {
        if (image == null) image = GetComponent<Image>();
        if (image != null) image.sprite = itemInfo.icon;
    }

    public void OnClick()
    {
        if (BuffSelectManager.Instance != null && itemInfo.buffEffect != null)
        {
            if (!selected)
            {
                bool result = BuffSelectManager.Instance.AddBuff(itemInfo.buffEffect);

                if (result)
                {
                    //선택 되었을때
                    selected = true;
                    image.color = selectedColor;
                }
            }
            else
            {
                BuffSelectManager.Instance.RemoveBuff(itemInfo.buffEffect);

                //선택 되지 않았을때
                selected = false;
                image.color = unselectedColor;
            }
        }
    }
}
