using System;
using UnityEngine;
using UnityEngine.UI;

public class HomeMaterialUiButton : MonoBehaviour
{
    [NonSerialized]
    public ItemData itemInfo;

    public void SetButton()
    {
        SetSprite();
    }

    private void SetSprite()
    {
        Image image = GetComponent<Image>();

        if (image != null)
        {
            image.sprite = itemInfo.icon;
        }
    }
}
