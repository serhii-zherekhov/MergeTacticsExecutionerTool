using System.Collections;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private GameObject _unit;
    private MeshRenderer _renderer;

    [SerializeField] private AnimationCurve _animationCurveForHalfSpeedIndicationFade;
    private static Color _defaultColor => new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private static Color _darkColor => new Color(0.3f, 0.3f, 0.3f, 1.0f);


    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void Select()
    {
        _renderer.material.color = _darkColor;
    }

    public void Unselect()
    {
        _renderer.material.color = _defaultColor;
    }

    public void IndicateFade()
    {
        StopAllCoroutines();
        StartCoroutine(IndicateFadeCoroutine());
    }

    private IEnumerator IndicateFadeCoroutine()
    {
        float minChannel = 0.3f;
        float maxChannel = 1f;
        float channel = minChannel;

        do
        {
            channel += Time.deltaTime;
            _renderer.material.color = new Color(channel, channel, channel, 1.0f);
            yield return null;
        }
        while (channel < maxChannel);

        channel = maxChannel;
        _renderer.material.color = new Color(channel, channel, channel, 1.0f);
    }

    private void IndicateFadeHalfSpeed()
    {
        StopAllCoroutines();
        StartCoroutine(IndicateFadeHalfSpeedCoroutine());
    }

    private IEnumerator IndicateFadeHalfSpeedCoroutine()
    {
        float halfSpeed = 0.5f;

        float minChannel = 0.3f;
        float maxChannel = 1f;
        float progress = minChannel;
        float channel = minChannel;

        do
        {
            progress += Time.deltaTime * halfSpeed;

            channel = Mathf.Lerp(minChannel, maxChannel, _animationCurveForHalfSpeedIndicationFade.Evaluate(progress));

            _renderer.material.color = new Color(channel, channel, channel, 1.0f);
            yield return null;
        }
        while (progress < maxChannel);

        progress = maxChannel;
        _renderer.material.color = new Color(channel, channel, channel, 1.0f);
    }

    public void SetUnit(GameObject unit)
    {
        _unit = unit;
    }

    public bool HasUnit()
    {
        return (_unit != null);
    }

    public void DeleteUnit()
    {
        Destroy(_unit);
        _unit = null;
    }
}
