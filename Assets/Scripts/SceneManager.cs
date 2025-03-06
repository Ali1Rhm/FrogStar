using UnityEngine;
using UnityEngine.UI;

namespace FrogStar
{
    public class SceneManager : MonoBehaviour
    {
        [SerializeField] private Button[] levelButtons;

        private void Start()
        {
            if (Application.targetFrameRate != 60)
                Application.targetFrameRate = 60;

            if (levelButtons.Length == 0) return;

            int level = 0;
            if (PlayerPrefs.HasKey("Level"))
            {
                level = PlayerPrefs.GetInt("Level");
            }
            else
                PlayerPrefs.SetInt("Level", level);

            for (int i = 0; i <= level; i++)
            {
                levelButtons[i].interactable = true;
            }
        }

        public void LoadLevelByIndex(int index)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(index);
        }
    }
}
