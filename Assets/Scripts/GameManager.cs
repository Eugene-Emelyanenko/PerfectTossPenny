using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator catAnimator;

    [Header("Variables")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private RectTransform timerTransform;
    [SerializeField] private Vector2 defaultTimerPos;
    [SerializeField] private Vector2 superGameTimerPos;
    public Arrow arrow;
    public int scoreToWin = 10;

    [Header("Hearts")]
    [SerializeField] Transform heartsContainer;
    [SerializeField] private Sprite heartFill;
    [SerializeField] private Sprite heartEmpty;

    [Header("Coins")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform coinSpawnPoint;
    [SerializeField] private Transform coinsContainer;

    [Header("Game Over Menu")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Image menuImage;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;
    [SerializeField] private TextMeshProUGUI menuText;
    [SerializeField] private TextMeshProUGUI menuScoreText;
    [SerializeField] private Button menuButton;

    [Header("Effect")]
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Sprite plusCoinSprite;
    [SerializeField] private Sprite takeDamageSprite;
    [SerializeField] private Transform plusCoinSpawnPoint;
    [SerializeField] private float yTakeDamagePos;

    [Header("Obstacles")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject cloudPrefab;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private float cloudMaxY;
    [SerializeField] private float cloudMinY;

    private Coin currentCoin;

    private int score;
    private int hearts;
    private int selectedLevel;
    private int activatedChildCount = 0;

    private void Start()
    {
        selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        if(selectedLevel == 0)
        {
            heartsContainer.gameObject.SetActive(false);
            timerTransform.anchoredPosition = superGameTimerPos;
            scoreToWin = 0;
        }
        else
        {
            heartsContainer.gameObject.SetActive(true);
            timerTransform.anchoredPosition = defaultTimerPos;
            scoreToWin = 10;
        }

        score = 0;
        hearts = 3;

        foreach (Transform coin in coinsContainer)
        {
            coin.gameObject.SetActive(false);
        }

        menuPanel.SetActive(false);

        UpdateUI();
        CreateNewCoin();

        SpawnObstacles();
    }

    private void SpawnObstacles()
    {
        List<Transform> shuffledSpawnPoints = new List<Transform>(spawnPoints);
        for (int i = 0; i < shuffledSpawnPoints.Count; i++)
        {
            Transform temp = shuffledSpawnPoints[i];
            int randomIndex = Random.Range(i, shuffledSpawnPoints.Count);
            shuffledSpawnPoints[i] = shuffledSpawnPoints[randomIndex];
            shuffledSpawnPoints[randomIndex] = temp;
        }

        GameObject firstObstacle = Instantiate(obstaclePrefab, shuffledSpawnPoints[0].position, Quaternion.identity);
        Obstacle obstacle_1 = firstObstacle.GetComponent<Obstacle>();
        GameObject secongObstacle = Instantiate(obstaclePrefab, shuffledSpawnPoints[1].position, Quaternion.identity);
        Obstacle obstacle_2 = secongObstacle.GetComponent<Obstacle>();

        if(selectedLevel != 0)
        {
            obstacle_1.spriteRenderer.sprite = Resources.Load<Sprite>($"Obstacles/{PlayerPrefs.GetInt("FirstSelectedObstacle", 0)}");
            obstacle_2.spriteRenderer.sprite = Resources.Load<Sprite>($"Obstacles/{PlayerPrefs.GetInt("LastSelectedObstacle", 1)}");
        }
        else
        {
            obstacle_1.spriteRenderer.sprite = Resources.Load<Sprite>($"Obstacles/{PlayerPrefs.GetInt("SuperGameFirstSelectedObstacle", 0)}");
            obstacle_2.spriteRenderer.sprite = Resources.Load<Sprite>($"Obstacles/{PlayerPrefs.GetInt("SuperGameLastSelectedObstacle", 1)}");
        }

        GameObject cloudObject = Instantiate(cloudPrefab, shuffledSpawnPoints[2].position, Quaternion.identity);
        cloudObject.transform.position = new Vector3(cloudObject.transform.position.x, Random.Range(cloudMinY, cloudMaxY), cloudObject.transform.position.z);
        cloudObject.transform.localScale = new Vector3(Random.Range(0, 2) == 0 ? -1 : 1, 1, 1);
    }

    public void Drop()
    {
        if(currentCoin != null)
        {
            catAnimator.SetTrigger("TossPenny");
            currentCoin.Toss(arrow.GetCurrentAngle());
            currentCoin = null;
            arrow.enabled = false;
            arrow.gameObject.SetActive(false);
        }       
    }

    private void UpdateUI()
    {
        if(selectedLevel == 0)
            scoreText.text = $"{score}";
        else
            scoreText.text = $"{score}/{scoreToWin}";
        foreach (Transform heart in heartsContainer)
            heart.GetComponent<Image>().sprite = heartEmpty;
        for (int i = 0; i < hearts; i++)
            heartsContainer.GetChild(i).GetComponent<Image>().sprite = heartFill;
    }

    public void TakeDamage()
    {
        hearts--;
        if (PlayerPrefs.GetInt("Vibrate", 1) == 1)
            Handheld.Vibrate();
        if(hearts <= 0 && selectedLevel != 0)
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.gameOverSound);
            hearts = 0;
            GameOver();
        }
        else
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.failSound);
        }

        UpdateUI();
    }

    public void CreateNewCoin()
    {
        GameObject coinObject = Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity);
        currentCoin = coinObject.GetComponent<Coin>();
    }

    public void IncreaseScore(int multiplier)
    {
        score += 1 * multiplier;

        int coins = Coins.GetCoins();
        coins += 1 * multiplier;
        Coins.SaveCoins(coins);

        if (activatedChildCount < coinsContainer.childCount)
        {
            Transform child = coinsContainer.GetChild(activatedChildCount);
            if (child != null)
            {
                child.gameObject.SetActive(true);
                activatedChildCount++;
            }
        }

        if (score >= scoreToWin && selectedLevel != 0)
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.completeSound);
            score = scoreToWin;
            Win();
        }
        else
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.scoreSound);
        }

        UpdateUI();
    }

    public void SuperGameWin()
    {
        menuPanel.SetActive(true);
        menuImage.sprite = winSprite;
        menuText.text = "YOU WIN!";
        //levelNumberText.text = $"Super Game";
        menuScoreText.text = score.ToString();
        menuButton.onClick.RemoveAllListeners();
        menuButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
        TextMeshProUGUI buttonText = menuButton.GetComponent<TextMeshProUGUI>();
        buttonText.text = "Try Again";
    }
    private void Win()
    {
        menuPanel.SetActive(true);
        menuImage.sprite = winSprite;
        menuText.text = "YOU WIN!";
        //levelNumberText.text = $"Level {selectedLevel}";
        menuScoreText.text = score.ToString();

        List<LevelData> levelDataList = LevelDataManager.LoadLevelsData();
        if (levelDataList.Count != 0)
        {
            foreach (LevelData data in levelDataList)
            {
                if (data.levelNumber == selectedLevel)
                {
                    data.isCompleted = true;
                    break;
                }
            }
            LevelDataManager.SaveLevelsData(levelDataList);
        }

        int nextLevel = selectedLevel + 1;
        bool isNextLevelUnlocked = false;

        if (nextLevel > levelDataList.Count)
        {
            nextLevel = 1;
            isNextLevelUnlocked = true;
        }
        else
        {
            foreach (LevelData data in levelDataList)
            {
                if (data.levelNumber == nextLevel)
                {
                    isNextLevelUnlocked = data.isUnlocked;
                    break;
                }
            }
        }
        Debug.Log($"Next Level {nextLevel}");
        Debug.Log($"Uncloked {isNextLevelUnlocked}");

        PlayerPrefs.SetInt("SelectedLevel", nextLevel);
        PlayerPrefs.Save();

        menuButton.onClick.RemoveAllListeners();

        if (isNextLevelUnlocked)
        {
            menuButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
            TextMeshProUGUI buttonText = menuButton.GetComponent<TextMeshProUGUI>();
            buttonText.text = "Next Level";
        }
        else
        {
            menuButton.onClick.AddListener(() => SceneManager.LoadScene("Main"));
            TextMeshProUGUI buttonText = menuButton.GetComponent<TextMeshProUGUI>();
            buttonText.text = "Back to Menu";
        }
    }

    public void GameOver()
    {
        menuPanel.SetActive(true);
        menuImage.sprite = loseSprite;
        menuText.text = "YOU LOSE!";
        //levelNumberText.text = $"Level {PlayerPrefs.GetInt("SelectedLevel", 1)}";
        menuScoreText.text = score.ToString();
        menuButton.onClick.RemoveAllListeners();
        menuButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
        TextMeshProUGUI buttonText = menuButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = "Try Again";
    }

    private void CreateEffect(Vector3 position, Sprite effectSprite)
    {
        GameObject effectObject = Instantiate(effectPrefab, position, Quaternion.identity);
        Effect effect = effectObject.GetComponent<Effect>();
        effect.effectSpriteRenderer.sprite = effectSprite;
    }

    public void CreatePlusCoinEffect() 
    {
        CreateEffect(plusCoinSpawnPoint.position, plusCoinSprite);
    }

    public void CreateTakeDamageEffect(float xPos)
    {
        CreateEffect(new Vector3(xPos, yTakeDamagePos, 0), takeDamageSprite);
    }
}
