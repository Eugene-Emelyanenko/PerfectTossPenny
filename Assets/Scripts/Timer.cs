using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText; // Текст для отображения времени
    [SerializeField] private GameManager gameManager;
    private float elapsedTime = 0f; // Прошедшее время
    private bool timerIsRunning = false; // Флаг для контроля запуска/остановки таймера
    private int selectedLevel;
    private int superGameTime;
    private int levelTime = 0;

    void Start()
    {
        selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        List<LevelData> levelDataList = LevelDataManager.LoadLevelsData();
        LevelData selectedLevelData = levelDataList.Find(level => level.levelNumber == selectedLevel);

        if (selectedLevelData != null)
            levelTime = selectedLevelData.time;

        superGameTime = PlayerPrefs.GetInt("CurrentSuperGameTime", 0);

        if (selectedLevel != 0)
        {
            elapsedTime = levelTime; // Начальное время равно времени уровня
            StartTimer();
        }
        else
        {
            StartTimer();
        }

        UpdateTimerText(elapsedTime); // Обновление текста таймера
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (selectedLevel != 0)
            {
                elapsedTime -= Time.deltaTime;
                if (elapsedTime <= 0)
                {
                    elapsedTime = 0;
                    StopTimer();
                    gameManager.GameOver();
                }
            }
            else
            {
                elapsedTime += Time.deltaTime;
                if (superGameTime == Mathf.FloorToInt(elapsedTime))
                {
                    gameManager.SuperGameWin();
                    StopTimer();
                }
            }

            UpdateTimerText(elapsedTime);
        }
    }

    public void StartTimer()
    {
        timerIsRunning = true;
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = levelTime;
        UpdateTimerText(elapsedTime);
    }

    public void PauseTimer()
    {
        timerIsRunning = false;
    }

    void UpdateTimerText(float timeToDisplay)
    {
        // Конвертируем время в минуты и секунды
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        // Отображаем текст на экране в формате MM:SS
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
