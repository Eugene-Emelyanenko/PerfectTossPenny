using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public int price;
    public bool isCompleted;
    public bool isUnlocked;
    public int time;

    public LevelData(int levelNumber, int price, bool isCompleted, bool isUnlocked, int time)
    {
        this.levelNumber = levelNumber;
        this.price = price;
        this.isCompleted = isCompleted;
        this.isUnlocked = isUnlocked;
        this.time = time;
    }
}

public static class LevelDataManager
{
    public static void SaveLevelsData(List<LevelData> levelDataList)
    {
        string json = JsonUtility.ToJson(new LevelDataListWrapper(levelDataList));
        PlayerPrefs.SetString("LevelsData", json);
        PlayerPrefs.Save();
    }

    public static List<LevelData> LoadLevelsData()
    {
        if (PlayerPrefs.HasKey("LevelsData"))
        {
            string json = PlayerPrefs.GetString("LevelsData");
            LevelDataListWrapper wrapper = JsonUtility.FromJson<LevelDataListWrapper>(json);
            return wrapper.levelDataList;
        }
        else
        {
            return new List<LevelData>();
        }
    }
}

[System.Serializable]
public class LevelDataListWrapper
{
    public List<LevelData> levelDataList;

    public LevelDataListWrapper(List<LevelData> levelDataList)
    {
        this.levelDataList = levelDataList;
    }
}
public class Levels : MonoBehaviour
{
    [SerializeField] private GameObject levelsMenu;
    [SerializeField] private DialogPanel dialogPanel;

    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private Transform levelsContainer;
    [SerializeField] private TextMeshProUGUI coinsText;

    public int levelCount = 30;

    private List<LevelData> levelDataList = new List<LevelData>();

    void Start()
    {
        dialogPanel.gameObject.SetActive(false);
        EnableMenu(false);
    }

    public void EnableMenu(bool isOpen)
    {
        levelsMenu.SetActive(isOpen);

        levelDataList = LevelDataManager.LoadLevelsData();
        if (levelDataList.Count == 0)
            CreateDefaultData();

        DisplayLevels();
    }

    private void CreateDefaultData()
    {
        levelDataList.Clear();

        for (int i = 1; i <= levelCount; i++)
        {
            int price = 10 + (i - 1) / 7 * 10;

            int time = 30 + ((i - 1) / 5) * 5;

            bool isUnlocked = (i == 1);
            bool isCompleted = false;

            levelDataList.Add(new LevelData(i, price, isCompleted, isUnlocked, time));
        }

        LevelDataManager.SaveLevelsData(levelDataList);
    }

    private void DisplayLevels()
    {
        foreach (Transform level in levelsContainer)
        {
            Destroy(level.gameObject);
        }

        foreach (LevelData data in levelDataList)
        {
            GameObject levelObject = Instantiate(levelPrefab, levelsContainer);
            Level level = levelObject.GetComponent<Level>();
            level.SetUp(data);
            level.levelButton.onClick.RemoveAllListeners();
            int price = data.price;
            int coins = Coins.GetCoins();
            level.levelButton.onClick.AddListener(() =>
            {
                if(!data.isUnlocked)
                {
                    dialogPanel.gameObject.SetActive(true);
                    if (coins < price)
                    {
                        dialogPanel.OpenNotEnoughtDialogFrame();
                    }
                    else
                    {
                        data.isUnlocked = true;
                        coins -= price;
                        Coins.SaveCoins(coins);
                        UpdateCoinsText();
                        LevelDataManager.SaveLevelsData(levelDataList);
                        SoundManager.Instance.PlayClip(SoundManager.Instance.buySound);
                        dialogPanel.Close();
                        DisplayLevels();
                    }
                }
                else
                {
                    PlayerPrefs.SetInt("SelectedLevel", data.levelNumber);
                    PlayerPrefs.Save();
                    SceneManager.LoadScene("Game");
                }
            });
        }
    }

    private void UpdateCoinsText()
    {
        coinsText.text = Coins.GetCoins().ToString();
    }
}
