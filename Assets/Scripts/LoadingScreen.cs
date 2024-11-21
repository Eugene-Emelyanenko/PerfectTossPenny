using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Image fillImage;
    public float loadingTime = 5f;
    public string sceneToLoad = "Tutorial";

    private void Start()
    {
        StartCoroutine(AnimateLoading());
    }

    private IEnumerator AnimateLoading()
    {
        float elapsedTime = 0f;

        while (elapsedTime < loadingTime)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / loadingTime);

            fillImage.fillAmount = progress;

            loadingText.text = Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }

        fillImage.fillAmount = 1f;
        loadingText.text = "100%";

        SceneManager.LoadScene(sceneToLoad);
    }
}
