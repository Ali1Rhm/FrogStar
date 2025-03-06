using System.Collections;
using FrogStar;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private int gameTime;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Button checkButton;
    [SerializeField] private Image[] hearts;
    [SerializeField] private GameObject wonPanel;
    [SerializeField] private GameObject failedPanel;
    [SerializeField] private MeteoriteBehaviour[] meteorites;
    [SerializeField] private PlanetBehaviour[] planets;
    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(this);
    }

    public (bool, PlanetBehaviour) CheckIntersection(float size, float radius, PlanetBehaviour self)
    {
        bool isIntersection = false;
        PlanetBehaviour intersect = null;

        foreach (var planet in planets)
        {
            if (planet == self) continue;

            if (!planet.DoIntersect(size, radius, self))
                continue;
            else
            {
                isIntersection = true;
                intersect = planet;
            }
        }

        return (isIntersection, intersect);
    }

    private void Start()
    {
        timerText.text = gameTime.ToString();
        StartCoroutine(TimerCoroutine());

        checkButton.onClick.AddListener(() =>
        {
            if(gameTime != 0)
            {
                bool heartsEnabled = true;

                foreach (var heart in hearts)
                {
                    if (heart.enabled == false)
                        heartsEnabled = false;
                }

                if (heartsEnabled)
                    ShowWin();
            }
        });
    }

    IEnumerator TimerCoroutine()
    {
        while (gameTime > 0)
        {
            yield return new WaitForSeconds(1f);
            gameTime -= 1;
            timerText.text = gameTime.ToString();
        }

        StartCoroutine(ShowFailed());
    }

    private void ShowWin()
    {
        if(PlayerPrefs.GetInt("Level") < UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex && PlayerPrefs.GetInt("Level") < 4)
            PlayerPrefs.SetInt("Level", UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

        Time.timeScale = 0;
        AudioManager.Main.PlayWin();
        wonPanel.SetActive(true);
    }

    private IEnumerator ShowFailed()
    {
        foreach (var meteorite in meteorites)
        {
            meteorite.HitThePlanet();
        }

        yield return new WaitForSeconds(1f);

        Time.timeScale = 0;
        failedPanel.SetActive(true);
    }

    public void GoToLevel(int levelIndex)
    {
        Time.timeScale = 1;
        int index = levelIndex == -1 ? UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex : levelIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }
}
