using DG.Tweening;
using FrogStar;
using UnityEngine;

public class MeteoriteBehaviour : MonoBehaviour
{
    [SerializeField] private float radius = 20f;
    [SerializeField] private float speed = 10f;

    private Transform starPoint;
    private Transform planetTransform;
    private float duration = 2f;
    private bool clockwise = true;
    private float updateRate = 0.1f;

    private void Start()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void InitMovement(Transform starPoint, Transform planetTransform, float duration, bool clockwise)
    {
        this.starPoint = starPoint;
        this.planetTransform = planetTransform;
        this.duration = duration;
        this.clockwise = clockwise;

        RotateWithPlanet();
    }

    private void RotateWithPlanet()
    {
        float direction = clockwise ? 1f : -1f;

        DOTween.To(() => 0f, angle =>
        {
            float rad = direction * angle * Mathf.Deg2Rad;
            transform.position = starPoint.position + new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0);
        }, 360f, duration)
        .SetLoops(-1, LoopType.Restart)
        .SetEase(Ease.Linear);
    }

    public void HitThePlanet()
    {
        GetComponent<SpriteRenderer>().enabled = true;

        InvokeRepeating(nameof(UpdatePath), 0f, updateRate);
    }

    void UpdatePath()
    {
        transform.DOMove(planetTransform.position, speed, false).SetSpeedBased().OnComplete(() => {
            GetComponent<SpriteRenderer>().enabled = false;
            AudioManager.Main.PlayDestroy();
        });
    }
}
