using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketBox : MonoBehaviour
{
    [SerializeField] private Image boxIcon;
    [SerializeField] private GameObject priceObject;
    [SerializeField] private TextMeshProUGUI priceText;
    public Button button;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite selectedSprite;
    public BoxData boxData;

    public void SetUp(BoxData data)
    {
        boxData = data;
        boxIcon.sprite = Resources.Load<Sprite>($"Box/{boxData.index}");
        priceText.text = boxData.price.ToString();

        priceObject.SetActive(true);
        Deselect();

        if(boxData.isUnlocked)
        {
            priceObject.SetActive(false);
            if (boxData.isSelected)
            {
                Select();
            }
        }
    }

    public void Select()
    {
        button.image.sprite = selectedSprite;
    }

    public void Deselect()
    {
        button.image.sprite = defaultSprite;
    }
}
