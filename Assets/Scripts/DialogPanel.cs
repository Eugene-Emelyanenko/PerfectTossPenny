using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogPanel : MonoBehaviour
{
    [SerializeField] private GameObject notEnoughtDialogFrame;
    [SerializeField] private TextMeshProUGUI coinsText;
    public TextMeshProUGUI notEnoghtBalanceText;

    public void OpenNotEnoughtDialogFrame()
    {
        notEnoughtDialogFrame.SetActive(true);

        UpdateCoinsText();
    }

    private void UpdateCoinsText()
    {
        int coins = Coins.GetCoins();
        notEnoghtBalanceText.text = coins.ToString();
        coinsText.text = coins.ToString();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
