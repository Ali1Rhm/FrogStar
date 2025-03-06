using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SunBehaviour : MonoBehaviour
{
    public static SunBehaviour Sun;
    public float TempScale => tempScale;

    [SerializeField][Range(0f, 1f)] private float tempScale;
    [SerializeField] private Slider tempSlider;
    [SerializeField] private float minTempSize;
    [SerializeField] private float maxTempSize;
    [SerializeField] private PlanetBehaviour[] planets;
    [SerializeField] private Sprite[] sunStateSprites;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (Sun == null)
            Sun = this;
        else
            Destroy(this);

        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (tempSlider != null)
        {
            tempSlider.minValue = 0;
            tempSlider.maxValue = 1;
            tempSlider.value = tempScale;
            tempSlider.onValueChanged.AddListener(ChangeColorAndSize);
        }

        ChangeColorAndSize(tempScale);
    }

    private void ChangeColorAndSize(float newTemp)
    {
        tempScale = newTemp;

        float scaleRange = maxTempSize - minTempSize;
        Vector3 newScale = Vector3.one * (minTempSize + scaleRange * tempScale);
        transform.DOScale(newScale, 0f);

        if (tempScale < 0.2f)
            spriteRenderer.sprite = sunStateSprites[0];
        else if (tempScale >= 0.2f && tempScale < 0.5f)
            spriteRenderer.sprite = sunStateSprites[1];
        else if(tempScale >= 0.5f && tempScale < 0.7f)
            spriteRenderer.sprite = sunStateSprites[2];
        else if (tempScale >= 0.7f)
            spriteRenderer.sprite = sunStateSprites[3];

        foreach (var planet in planets)
            planet.UpdateTemperature();
    }
}
