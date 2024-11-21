using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BoxData
{
    public int index;
    public int price;
    public int coinMultiplier;
    public bool isUnlocked;
    public bool isSelected;

    public BoxData(int index, int price, int coinMultiplier, bool isUnlocked, bool isSelected)
    {
        this.index = index;
        this.price = price;
        this.coinMultiplier = coinMultiplier;
        this.isUnlocked = isUnlocked;
        this.isSelected = isSelected;
    }
}

public static class BoxDataManager
{
    public readonly static string boxDataKey = "BoxData";
    public static void SaveBoxData(List<BoxData> boxDataList)
    {
        string json = JsonUtility.ToJson(new BoxDataListWrapper(boxDataList));
        PlayerPrefs.SetString(boxDataKey, json);
        PlayerPrefs.Save();
    }

    public static List<BoxData> LoadBoxData()
    {
        if (PlayerPrefs.HasKey(boxDataKey))
        {
            string json = PlayerPrefs.GetString(boxDataKey);
            BoxDataListWrapper wrapper = JsonUtility.FromJson<BoxDataListWrapper>(json);
            return wrapper.boxDataList;
        }
        else
        {
            return new List<BoxData>();
        }
    }
}

[System.Serializable]
public class BoxDataListWrapper
{
    public List<BoxData> boxDataList;

    public BoxDataListWrapper(List<BoxData> boxDataList)
    {
        this.boxDataList = boxDataList;
    }
}

public class BoxMarket : MonoBehaviour
{
    [SerializeField] private GameObject boxMarketPanel;
    [SerializeField] private Transform boxContainer;
    [SerializeField] private GameObject marketBoxPrefab;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI buyButtonText;
    [SerializeField] private DialogPanel dialogPanel;

    private List<BoxData> boxDataList = new List<BoxData>();
    private MarketBox selectedBox = null;

    private void Start()
    {
        EnablePanel(false);
    }

    public void EnablePanel(bool isOpen)
    {
        boxMarketPanel.SetActive(isOpen);

        UpdateCoinsText();

        buyButton.gameObject.SetActive(false);

        selectedBox = null;

        boxDataList = BoxDataManager.LoadBoxData();
        if (boxDataList.Count == 0)
            CreateDefaultBoxData();

        if(isOpen)
        {
            DisplayBoxes();
        }
        else
        {

        }
    }

    private void CreateDefaultBoxData()
    {
        boxDataList.Clear();
        for (int i = 0; i < 4; i++)
        {
            boxDataList.Add(new BoxData(i + 1, 50, i + 1, i == 0, i == 0));
        }
        BoxDataManager.SaveBoxData(boxDataList);
    }

    private void DisplayBoxes()
    {
        foreach (Transform t in boxContainer)
        {
            Destroy(t.gameObject);
        }

        foreach (BoxData data in boxDataList)
        {
            GameObject boxObject = Instantiate(marketBoxPrefab, boxContainer);
            boxObject.name = $"Box_{data.index}";
            MarketBox box = boxObject.GetComponent<MarketBox>();
            box.SetUp(data);
            box.button.onClick.RemoveAllListeners();
            box.button.onClick.AddListener( () =>
            {
                if(data.isSelected)
                {
                    Debug.Log($"{boxObject.name} is already selected. Returned");
                    buyButton.gameObject.SetActive(false);
                    DisplayBoxes();
                    return;
                }
                else if(data.isUnlocked)
                {
                    Debug.Log($"{boxObject.name} selected");
                    buyButton.gameObject.SetActive(false);
                    foreach (BoxData boxData in boxDataList)
                    {
                        boxData.isSelected = false;
                    }
                    data.isSelected = true;
                    BoxDataManager.SaveBoxData(boxDataList);
                    DisplayBoxes();
                }
                else if(!data.isUnlocked)
                {
                    Debug.Log($"Trying to unlock {boxObject.name}. Price: {data.price}. Current balance: {Coins.GetCoins()}");

                    selectedBox = box;

                    foreach (Transform t in boxContainer)
                    {
                        MarketBox currentBox = t.gameObject.GetComponent<MarketBox>();
                        currentBox.Deselect();
                    }

                    selectedBox.Select();

                    foreach (Transform t in boxContainer)
                    {
                        MarketBox currentBox = t.gameObject.GetComponent<MarketBox>();
                        if (currentBox.boxData.isSelected)
                            currentBox.Deselect();
                    }
                    buyButton.gameObject.SetActive(true);
                    buyButtonText.text = selectedBox.boxData.price.ToString();
                    buyButton.onClick.RemoveAllListeners();
                    buyButton.onClick.AddListener( () =>
                    {
                        int price = selectedBox.boxData.price;
                        int coins = Coins.GetCoins();

                        dialogPanel.gameObject.SetActive(true);

                        if (coins < price)
                        {
                            dialogPanel.OpenNotEnoughtDialogFrame();
                        }
                        else
                        {
                            coins -= price;
                            Coins.SaveCoins(coins);
                            UpdateCoinsText();

                            foreach (BoxData boxData in boxDataList)
                            {
                                boxData.isSelected = false;
                            }

                            data.isUnlocked = true;
                            data.isSelected = true;

                            BoxDataManager.SaveBoxData(boxDataList);

                            PlayerPrefs.Save();
                            SoundManager.Instance.PlayClip(SoundManager.Instance.buySound);
                            dialogPanel.Close();
                            EnablePanel(true);
                        }
                    });
                }
            });
        }
    }

    private void UpdateCoinsText()
    {
        coinsText.text = Coins.GetCoins().ToString();
    }
}
