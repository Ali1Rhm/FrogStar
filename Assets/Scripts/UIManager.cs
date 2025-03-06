using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FrogStar
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [SerializeField] private RectTransform infoPanel;
        [SerializeField] private TMP_Text planetNameText;
        [SerializeField] private TMP_Text DominantElementText;
        [SerializeField] private TMP_Text[] elementsNameTexts;
        [SerializeField] private Slider[] elementsSlider;

        private Coroutine routine;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Destroy(this);
        }

        public void DisplayInfo(string planetName, string dominantElement, ElementPercent[] elementPercents)
        {
            if (elementPercents.Length > 3)
                return;

            if (infoPanel.anchoredPosition.y != 200f)
            {
                if (routine != null)
                    StopCoroutine(routine);
                routine = StartCoroutine(UpdatePanel(planetName, dominantElement, elementPercents));
            }
            else
            {
                planetNameText.text = planetName;
                DominantElementText.text = "Dominant Element:\n" + dominantElement;
                for (int i = 0; i < elementPercents.Length; i++)
                {
                    elementsNameTexts[i].text = elementPercents[i].ElementName;
                    elementsSlider[i].value = elementPercents[i].Percent / 100f;
                }
                routine = StartCoroutine(ShowInfoPanel());
            }
        }

        private IEnumerator UpdatePanel(string planetName, string dominantElement, ElementPercent[] elementPercents)
        {
            infoPanel.DOMoveY(-200, 0.25f);
            yield return new WaitForSeconds(0.25f);

            planetNameText.text = planetName;
            DominantElementText.text = "Dominant Element:\n"+dominantElement;
            for (int i = 0; i < elementPercents.Length; i++)
            {
                elementsNameTexts[i].text = elementPercents[i].ElementName;
                elementsSlider[i].value = elementPercents[i].Percent / 100f;
            }

            infoPanel.DOMoveY(140, 0.25f);
            yield return new WaitForSeconds(1.75f);
            infoPanel.DOMoveY(-200, 0.25f);
        }

        private IEnumerator ShowInfoPanel()
        {
            infoPanel.DOMoveY(140, 0.25f);
            yield return new WaitForSeconds(1.75f);
            infoPanel.DOMoveY(-200, 0.25f);
        }

    }
}
