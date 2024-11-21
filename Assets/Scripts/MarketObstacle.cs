using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MarketObstacle : MonoBehaviour
{
    [SerializeField] private Image obstacleIcon;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image obstacleButtonImage;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private GameObject price;
    public Button button;
    public ObstacleData data;

    public void SetUp(ObstacleData data)
    {
        this.data = data;
        gameObject.name = $"Obstacle_{data.obstacleNumber}";
        obstacleIcon.sprite = Resources.Load<Sprite>($"Obstacles/{data.obstacleNumber}");
        priceText.text = data.price.ToString();
        if (data.isPurchased)
        {
            price.SetActive(false);
        }
        if (data.isSelected)
            Select();
        else
            Deselect();
    }

    public void Select()
    {
        obstacleButtonImage.sprite = selectedSprite;
    }

    public void Deselect()
    {
        obstacleButtonImage.sprite = defaultSprite;
    }
}

