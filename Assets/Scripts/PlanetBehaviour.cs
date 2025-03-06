using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using FrogStar;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class PlanetBehaviour : MonoBehaviour
{
    [SerializeField] private Transform star;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float planetSize;
    [SerializeField] private float minRadius;
    [SerializeField] private float maxRadius;
    [SerializeField] private bool clockwise = true;
    [SerializeField] private float duration = 2f;
    [SerializeField] private Slider orbitSlider;
    [SerializeField] private Image heartImage;
    [SerializeField] private MeteoriteBehaviour meteorite;
    [Space]
    [SerializeField] private float lifeSunTemp;
    [SerializeField] private float lifeRadius;
    [Space]
    [SerializeField] private bool displayInfo;
    [SerializeField] private string planetName;
    [SerializeField] private string coreElement;
    [SerializeField] private ElementPercent[] elements;
    [Space]
    [SerializeField] private Sprite[] planetStatesSprites;

    private SpriteRenderer spriteRenderer;
    private LineRenderer lineRenderer;
    private int segments = 40;
    private bool inIntersection = false;
    private List<PlanetBehaviour> intersectedPlanets = new();
    private Tweener tweener;
    int prevDir = -2;
    private Tween orbitTween;
    private float lastAngle = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        transform.localScale = Vector3.one * planetSize;

        InitLineRenderer();
        InitSlider();
        meteorite.InitMovement(star, this.transform, duration, clockwise);

        RotateAroundStar();

        UpdateOrbit();
        UpdateTemperature();
    }

    private void ChangeRadius(float newRadius)
    {
        /*if (Mathf.Abs(newRadius - radius) < 0.05f)
            return;*/

        radius = newRadius;

        UpdateOrbit();
        UpdateTemperature();
        RotateAroundStar();

        (bool intersecting, PlanetBehaviour intersect) = GameManager.Instance.CheckIntersection(planetSize, radius, this);

        if(intersecting && !intersectedPlanets.Contains(intersect))
            intersectedPlanets.Add(intersect);
        else if(!intersecting)
            intersectedPlanets.Clear();

        ChangeIntersectionColor(intersectedPlanets.Count != 0);
    }

    public void UpdateTemperature()
    {
        float lifeSunDist = SunBehaviour.Sun.TempScale - lifeSunTemp;

        float radiusNorm = (radius - minRadius) / (maxRadius - minRadius);
        float lifeRadiusNorm = (lifeRadius - minRadius) / (maxRadius - minRadius);
        float lifeRadiusDist = lifeRadiusNorm - radiusNorm;

        float finalLife = (lifeRadiusDist + 0.5f + lifeSunDist + 0.5f) / 2;

        if (finalLife < 0.4f)
            spriteRenderer.sprite = planetStatesSprites[0];
        else if (finalLife >= 0.4f && finalLife <= 0.6f)
            spriteRenderer.sprite = planetStatesSprites[1];
        else if (finalLife > 0.6f)
            spriteRenderer.sprite = planetStatesSprites[2];

        if (inIntersection)
        {
            if (heartImage == null || !heartImage.enabled) return;
            heartImage.transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => heartImage.enabled = false);

            return;
        }

        int changeDir = Mathf.Abs(finalLife - 0.5f) <= 0.1f ? 0 : MathF.Sign(finalLife - 0.5f);

        if (changeDir == 0)
        {
            if (heartImage == null || heartImage.enabled)
                return;

            if(changeDir != prevDir)
            {
                AudioManager.Main.PlayComfort();
                prevDir = changeDir;
            }

            tweener.Kill();

            heartImage.transform.localScale = Vector3.one;
            heartImage.enabled = true;
            tweener = heartImage.transform.DOPunchScale(Vector3.one, 0.25f);
        }
        else
        {
            if (heartImage == null || !heartImage.enabled) return;


            if (changeDir != prevDir)
            {
                if(changeDir == -1)
                    AudioManager.Main.PlayerShiver();
                else
                    AudioManager.Main.PlayBurning();

                prevDir = changeDir;
            }
                

            tweener.Kill();

            tweener = heartImage.transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => heartImage.enabled = false);
        }
    }

    private void UpdateOrbit()
    {
        lineRenderer.positionCount = segments + 1;
        Vector3[] path = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * 2 * Mathf.PI / segments;
            Vector3 point = star.position + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            lineRenderer.SetPosition(i, point);
            path[i] = point;
        }
    }

    private void RotateAroundStar()
    {
        float direction = clockwise ? 1f : -1f;

        float dynamicDuration = duration * 2 * ((radius - minRadius) / (maxRadius - minRadius) + 0.1f);

        if (orbitTween != null) orbitTween.Kill();

        orbitTween = DOTween.To(() => lastAngle, angle =>
        {
            lastAngle = angle;
            float rad = direction * lastAngle * Mathf.Deg2Rad;
            transform.position = star.position + new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0);
        }, lastAngle + 360f, dynamicDuration)
        .SetLoops(-1, LoopType.Restart)
        .SetEase(Ease.Linear);
    }

    private void InitLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }

    private void InitSlider()
    {
        if (orbitSlider != null)
        {
            orbitSlider.minValue = minRadius;
            orbitSlider.maxValue = maxRadius;
            orbitSlider.value = radius;
            orbitSlider.onValueChanged.AddListener(ChangeRadius);
        }
    }

    private void OnMouseDown()
    {
        if (!displayInfo)
            return;

        UIManager.Instance.DisplayInfo(planetName, coreElement, elements);
    }

    public bool DoIntersect(float size, float radius, PlanetBehaviour planet)
    {
        bool doIntersect = MathF.Abs(this.radius - radius) <= this.planetSize/2 + size/2;

        if (doIntersect)
        {
            if (!intersectedPlanets.Contains(planet))
                intersectedPlanets.Add(planet);
        }
        else
        {
            if (intersectedPlanets.Contains(planet))
            {
                intersectedPlanets.Remove(planet);
            }
        }

        ChangeIntersectionColor(intersectedPlanets.Count != 0);
        return doIntersect;
    }

    private void ChangeIntersectionColor(bool doIntersect)
    {
        if (doIntersect)
        {
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;

            inIntersection = true;
        }
        else
        {
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;

            inIntersection = false;
        }

        UpdateTemperature();
    }
}

[Serializable]
public struct ElementPercent
{
    public string ElementName;
    public int Percent;
}
