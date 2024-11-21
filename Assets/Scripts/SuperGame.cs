using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SuperGame : MonoBehaviour
{
    [SerializeField] private GameObject superGamePanel;
    [SerializeField] private DialogPanel dialogPanel;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button plusSuperGameButton;
    [SerializeField] private Button minusSuperGameButton;
    [SerializeField] private TextMeshProUGUI superGamePlayPriceText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Transform obstaclesContainer; // Контейнер с кнопками
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite defaultSprite;
    private List<Button> selectedButtons = new List<Button>(); // Список для хранения выбранных кнопок

    public int superGamePlayPrice = 30;

    private int superGameTime;
    private int currentSuperGameTime = 0;

    private void Start()
    {
        dialogPanel.gameObject.SetActive(false);
        EnableMenu(false);
        InitializeButtons();
    }

    public void EnableMenu(bool isOpen)
    {
        superGamePanel.SetActive(isOpen);
        superGamePlayPriceText.text = superGamePlayPrice.ToString();
        superGameTime = PlayerPrefs.GetInt("SuperGameTime", 30);
        currentSuperGameTime = superGameTime;
        PlayerPrefs.SetInt("CurrentSuperGameTime", currentSuperGameTime);
        PlayerPrefs.Save();
        timeText.text = ConvertTime(superGameTime);
        plusSuperGameButton.interactable = false;
        if (superGameTime > 0)
            minusSuperGameButton.interactable = true;
        else
            minusSuperGameButton.interactable = false;

        List<ObstacleData> obstacleDataList = ObstacleDataManager.LoadObstaclesData();
        for (int i = 0; i < obstacleDataList.Count; i++)
        {
            if (!obstacleDataList[i].isPurchased)
                obstaclesContainer.GetChild(i).GetComponent<Button>().interactable = false;
            else
                obstaclesContainer.GetChild(i).GetComponent<Button>().interactable = true;
        }
    }

    private void InitializeButtons()
    {
        foreach (Transform child in obstaclesContainer)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnButtonClicked(button));
            }
        }

        // Загружаем и выбираем ранее сохраненные кнопки
        if (PlayerPrefs.HasKey("SuperGameFirstSelectedObstacle"))
        {
            int firstIndex = PlayerPrefs.GetInt("SuperGameFirstSelectedObstacle", 0);
            SelectButton(obstaclesContainer.GetChild(firstIndex).GetComponent<Button>());
        }

        if (PlayerPrefs.HasKey("SuperGameLastSelectedObstacle"))
        {
            int lastIndex = PlayerPrefs.GetInt("SuperGameLastSelectedObstacle", 1);
            SelectButton(obstaclesContainer.GetChild(lastIndex).GetComponent<Button>());
        }
        else
        {
            // Если кнопки не были сохранены, выбираем первые две кнопки
            if (obstaclesContainer.childCount >= 2)
            {
                SelectButton(obstaclesContainer.GetChild(0).GetComponent<Button>());
                SelectButton(obstaclesContainer.GetChild(1).GetComponent<Button>());
            }
        }
    }

    private void OnButtonClicked(Button clickedButton)
    {
        if (selectedButtons.Contains(clickedButton))
        {
            return;
        }
        else
        {
            if (selectedButtons.Count < 2)
            {
                SelectButton(clickedButton);
            }
            else
            {
                DeselectButton(selectedButtons[0]); // Снимаем выбор с первой кнопки
                SelectButton(clickedButton); // Выбираем новую кнопку
            }
        }
    }

    private void SelectButton(Button button)
    {
        selectedButtons.Add(button);
        button.GetComponent<Image>().sprite = selectedSprite;

        // Сохраняем индексы выбранных кнопок в PlayerPrefs
        int buttonIndex = button.transform.GetSiblingIndex();

        if (selectedButtons.Count == 1)
        {
            PlayerPrefs.SetInt("SuperGameFirstSelectedObstacle", buttonIndex);
        }
        else if (selectedButtons.Count == 2)
        {
            PlayerPrefs.SetInt("SuperGameLastSelectedObstacle", buttonIndex);
        }

        PlayerPrefs.Save();
    }

    private void DeselectButton(Button button)
    {
        selectedButtons.Remove(button);
        button.GetComponent<Image>().sprite = defaultSprite;

        // Обновляем сохранённые индексы
        if (selectedButtons.Count == 1)
        {
            int remainingButtonIndex = selectedButtons[0].transform.GetSiblingIndex();
            PlayerPrefs.SetInt("SuperGameFirstSelectedObstacle", remainingButtonIndex);
            PlayerPrefs.DeleteKey("SuperGameLastSelectedObstacle");
        }
        else
        {
            PlayerPrefs.DeleteKey("SuperGameFirstSelectedObstacle");
            PlayerPrefs.DeleteKey("SuperGameLastSelectedObstacle");
        }

        PlayerPrefs.Save();
    }

    public void PlusSuperGameTime()
    {
        superGameTime++;
        minusSuperGameButton.interactable = true;
        if (superGameTime == PlayerPrefs.GetInt("SuperGameTime", 30))
        {
            plusSuperGameButton.interactable = false;
        }      
        timeText.text = ConvertTime(superGameTime);
    }

    public void MinusSuperGameTime()
    {
        superGameTime--;
        plusSuperGameButton.interactable = true;
        if(superGameTime == 0)
        {
            minusSuperGameButton.interactable = false;
        }
        timeText.text = ConvertTime(superGameTime);
    }

    private string ConvertTime(int totalTime)
    {
        float minutes = Mathf.FloorToInt(totalTime / 60);
        float seconds = Mathf.FloorToInt(totalTime % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private int ConvertToSeconds(string timeString)
    {
        string[] timeParts = timeString.Split(':');

        // Преобразуем минуты и секунды в int
        int minutes = int.Parse(timeParts[0]);
        int seconds = int.Parse(timeParts[1]);

        return minutes * 60 + seconds;
    }

    public void Play()
    {
        dialogPanel.gameObject.SetActive(true);
        int coins = Coins.GetCoins();
        if (coins < superGamePlayPrice)
        {
            dialogPanel.OpenNotEnoughtDialogFrame();
        }
        else
        {
            coins -= superGamePlayPrice;
            Coins.SaveCoins(coins);
            UpdateCoinsText();
            PlayerPrefs.SetInt("SelectedLevel", 0);
            superGameTime = ConvertToSeconds(timeText.text);
            PlayerPrefs.SetInt("CurrentSuperGameTime", superGameTime);
            PlayerPrefs.Save();
            SoundManager.Instance.PlayClip(SoundManager.Instance.buySound);
            dialogPanel.Close();
            SceneManager.LoadScene("Game");
        }
    }

    private void UpdateCoinsText()
    {
        coinsText.text = Coins.GetCoins().ToString();
    }
}
