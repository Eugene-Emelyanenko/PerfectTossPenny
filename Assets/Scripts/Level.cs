using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    [SerializeField] private Image levelImage;
    [SerializeField] private Sprite completedSprite;
    [SerializeField] private Sprite unclockedSprite;
    [SerializeField] private Sprite lockedSprite;

    [SerializeField] private TextMeshProUGUI levelNumberText;
    [SerializeField] private TextMeshProUGUI levelPriceText;

    public Button levelButton;

    public void SetUp(LevelData data)
    {
        gameObject.name = $"Level_{data.levelNumber}";
        if(data.isUnlocked)
        {
            levelPriceText.gameObject.SetActive(false);
            levelNumberText.gameObject.SetActive(true);
            levelNumberText.text = data.levelNumber.ToString();
            levelImage.sprite = unclockedSprite;
            if(data.isCompleted)
            {
                levelNumberText.gameObject.SetActive(false);
                levelImage.sprite = completedSprite;
            }
        }
        else
        {
            levelPriceText.gameObject.SetActive(true);
            levelNumberText.gameObject.SetActive(false);
            levelImage.sprite = lockedSprite;
            levelPriceText.text = data.price.ToString();
        }
    }
}
