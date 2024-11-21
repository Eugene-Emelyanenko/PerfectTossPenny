using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ObstacleData
{
    public int obstacleNumber;
    public int price;
    public bool isPurchased;
    public bool isSelected;

    public ObstacleData(int obstacleNumber, int price, bool isPurchased, bool isSelected)
    {
        this.obstacleNumber = obstacleNumber;
        this.price = price;
        this.isPurchased = isPurchased;
        this.isSelected = isSelected;
    }
}

public static class ObstacleDataManager
{
    public static void SaveObstaclesData(List<ObstacleData> obstacleDataList)
    {
        string json = JsonUtility.ToJson(new ObstacleDataListWrapper(obstacleDataList));
        PlayerPrefs.SetString("ObstaclesData", json);
        PlayerPrefs.Save();
    }

    public static List<ObstacleData> LoadObstaclesData()
    {
        if (PlayerPrefs.HasKey("ObstaclesData"))
        {
            string json = PlayerPrefs.GetString("ObstaclesData");
            ObstacleDataListWrapper wrapper = JsonUtility.FromJson<ObstacleDataListWrapper>(json);
            return wrapper.obstacleDataList;
        }
        else
        {
            return new List<ObstacleData>();
        }
    }
}

[System.Serializable]
public class ObstacleDataListWrapper
{
    public List<ObstacleData> obstacleDataList;

    public ObstacleDataListWrapper(List<ObstacleData> obstacleDataList)
    {
        this.obstacleDataList = obstacleDataList;
    }
}

public class Market : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private GameObject marketPanel;
    [SerializeField] private DialogPanel dialogPanel;
    [SerializeField] private TextMeshProUGUI buyButtonText;
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("SuperGame Time")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button plusSuperGameTimeButton;
    [SerializeField] private Button minusSuperGameTimeButton;
    [SerializeField] private TextMeshProUGUI superGameTimePriceText;

    [Header("Obstacles")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private Transform obstacleContainer;

    [Header("Values")]
    public int superGameTimePrice = 100;

    private int superGameTime;
    private int totalPrice;
    private List<GameObject> obstaclesList = new List<GameObject>();
    private List<ObstacleData> obstacleDataList = new List<ObstacleData>();
    public List<MarketObstacle> selectedObstacles = new List<MarketObstacle>();

    private void Start()
    {
        marketPanel.SetActive(false);

        superGameTimePriceText.text = superGameTimePrice.ToString();
        dialogPanel.gameObject.SetActive(false);

        coinsText.text = PlayerPrefs.GetInt("Coins", 0).ToString();

        plusSuperGameTimeButton.onClick.RemoveAllListeners();
        plusSuperGameTimeButton.onClick.AddListener(PlusSuperGameTime);

        minusSuperGameTimeButton.onClick.RemoveAllListeners();
        minusSuperGameTimeButton.onClick.AddListener(MinusSuperGameTime);
    }

    public void EnableMenu(bool isOpen)
    {
        marketPanel.SetActive(isOpen);

        superGameTime = PlayerPrefs.GetInt("SuperGameTime", 30);
        minusSuperGameTimeButton.interactable = false;
        timeText.text = ConvertTime(superGameTime);
        totalPrice = 0;
        buyButtonText.text = $"BUY FOR: {totalPrice.ToString()}";
        UpdateCoinsText();

        obstacleDataList = ObstacleDataManager.LoadObstaclesData();
        if (obstacleDataList.Count == 0)
            CreateDefaultObstacleData();
        DisplayObstacles();
    }

    private void CreateDefaultObstacleData()
    {
        Debug.Log("Generate Default Obstacles Data");

        obstacleDataList.Add(new ObstacleData(0, 50, true, true));
        obstacleDataList.Add(new ObstacleData(1, 50, true, true));
        obstacleDataList.Add(new ObstacleData(2, 50, false, false));
        obstacleDataList.Add(new ObstacleData(3, 50, false, false));
        obstacleDataList.Add(new ObstacleData(4, 50, false, false));
        obstacleDataList.Add(new ObstacleData(5, 50, false, false));

        ObstacleDataManager.SaveObstaclesData(obstacleDataList);
    }

    private void DisplayObstacles()
    {
        obstaclesList.Clear();
        selectedObstacles.Clear();

        foreach (Transform obstacle in obstacleContainer)
        {
            Destroy(obstacle.gameObject);
        }

        foreach (ObstacleData data in obstacleDataList)
        {
            GameObject obstacleObject = Instantiate(obstaclePrefab, obstacleContainer);
            obstaclesList.Add(obstacleObject);
            MarketObstacle marketObstacle = obstacleObject.GetComponent<MarketObstacle>();
            marketObstacle.SetUp(data);
            marketObstacle.button.onClick.RemoveAllListeners();
            marketObstacle.button.onClick.AddListener(() => OnClick(marketObstacle));
            if (data.isSelected)
            {
                selectedObstacles.Add(marketObstacle);
                marketObstacle.Select();
            }
        }

        if(selectedObstacles.Count == 0)
        {
            OnClick(obstaclesList[0].GetComponent<MarketObstacle>());
            OnClick(obstaclesList[1].GetComponent<MarketObstacle>());
        }
        else if(selectedObstacles.Count == 1)
        {
            foreach (GameObject obstacle in obstaclesList)
            {
                MarketObstacle marketObstacle = obstacle.GetComponent<MarketObstacle>();
                if (marketObstacle.data.isPurchased && !marketObstacle.data.isSelected)
                {
                    OnClick(marketObstacle);
                    break;
                }
            }
        }

        UpdateBuyButton();
    }

    private void OnClick(MarketObstacle marketObstacle)
    {
        if (selectedObstacles.Contains(marketObstacle))
        {
            return;
        }
        else
        {
            if (selectedObstacles.Count < 2)
            {
                SelectButton(marketObstacle);
            }
            else
            {
                DeselectButton(selectedObstacles[0]);
                SelectButton(marketObstacle);
            }
        }
    }

    private void SelectButton(MarketObstacle marketObstacle)
    {
        selectedObstacles.Add(marketObstacle);
        marketObstacle.Select();

        if (!marketObstacle.data.isPurchased)
        {
            totalPrice += marketObstacle.data.price;
        }
        else
        {
            marketObstacle.data.isSelected = true;
            ObstacleDataManager.SaveObstaclesData(obstacleDataList);

            int buttonIndex = marketObstacle.data.obstacleNumber;

            if (selectedObstacles.Count == 1)
            {
                PlayerPrefs.SetInt("FirstSelectedObstacle", buttonIndex);
            }
            else if (selectedObstacles.Count == 2)
            {
                PlayerPrefs.SetInt("LastSelectedObstacle", buttonIndex);
            }

            PlayerPrefs.Save();
        }

        UpdateBuyButton();
    }

    private void DeselectButton(MarketObstacle marketObstacle)
    {
        selectedObstacles.Remove(marketObstacle);
        marketObstacle.Deselect();

        if (!marketObstacle.data.isPurchased)
        {
            totalPrice -= marketObstacle.data.price;
        }
        else
        {
            marketObstacle.data.isSelected = false;
            ObstacleDataManager.SaveObstaclesData(obstacleDataList);
        }

        // Обновляем сохранённые индексы
        if (selectedObstacles.Count == 1)
        {
            int remainingButtonIndex = selectedObstacles[0].data.obstacleNumber;
            PlayerPrefs.SetInt("FirstSelectedObstacle", remainingButtonIndex);
            PlayerPrefs.DeleteKey("LastSelectedObstacle");
        }
        else
        {
            PlayerPrefs.DeleteKey("FirstSelectedObstacle");
            PlayerPrefs.DeleteKey("LastSelectedObstacle");
        }

        PlayerPrefs.Save();

        UpdateBuyButton();
    }

    private void UpdateBuyButton()
    {
        buyButtonText.text = $"BUY FOR: {totalPrice.ToString()}";
    }

    private void PlusSuperGameTime()
    {
        superGameTime++;
        totalPrice += superGameTimePrice;
        if (superGameTime > PlayerPrefs.GetInt("SuperGameTime", 30))
            minusSuperGameTimeButton.interactable = true;
        timeText.text = ConvertTime(superGameTime);
        UpdateBuyButton();
    }

    private void MinusSuperGameTime()
    {
        if (superGameTime > PlayerPrefs.GetInt("SuperGameTime", 30))
        {
            superGameTime--;
            totalPrice -= superGameTimePrice;
        }
        if (superGameTime <= PlayerPrefs.GetInt("SuperGameTime", 30))
            minusSuperGameTimeButton.interactable = false;
        timeText.text = ConvertTime(superGameTime);
        UpdateBuyButton();
    }

    private string ConvertTime(int totalTime)
    {
        float minutes = Mathf.FloorToInt(totalTime / 60);
        float seconds = Mathf.FloorToInt(totalTime % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void Buy()
    {
        if (totalPrice <= 0)
        {
            totalPrice = 0;
            return;
        }

        dialogPanel.gameObject.SetActive(true);

        int coins = Coins.GetCoins();

        if (coins < totalPrice)
        {
            dialogPanel.OpenNotEnoughtDialogFrame();
        }
        else
        {
            coins -= totalPrice;
            Coins.SaveCoins(coins);
            UpdateCoinsText();

            // Обновляем статус покупки времени и препятствий
            PlayerPrefs.SetInt("SuperGameTime", superGameTime);

            foreach (ObstacleData data in obstacleDataList)
            {
                data.isSelected = false;
                PlayerPrefs.DeleteKey("FirstSelectedObstacle");
                PlayerPrefs.DeleteKey("LastSelectedObstacle");
            }

            foreach (var obstacle in selectedObstacles)
            {
                obstacle.data.isPurchased = true;
                obstacle.data.isSelected = true;
            }
            PlayerPrefs.SetInt("FirstSelectedObstacle", selectedObstacles[0].data.obstacleNumber);
            PlayerPrefs.SetInt("LastSelectedObstacle", selectedObstacles[1].data.obstacleNumber);

            ObstacleDataManager.SaveObstaclesData(obstacleDataList);

            PlayerPrefs.Save();
            SoundManager.Instance.PlayClip(SoundManager.Instance.buySound);
            dialogPanel.Close();
            EnableMenu(true);
        }
    }

    private void UpdateCoinsText()
    {
        coinsText.text = Coins.GetCoins().ToString();
    }
}
