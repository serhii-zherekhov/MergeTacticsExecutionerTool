using System;
using System.Collections;
using UnityEngine;

public class LightDawn : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] float speed = 4f;

    private void Start()
    {
        StartCoroutine(Dawn());
    }

    private IEnumerator Dawn()
    {
        float minIntensity = 0f;
        float maxIntensity = 2.5f;
        float intensity = minIntensity;

        do
        {
            intensity += Time.deltaTime * speed;
            _light.intensity = intensity;
            yield return null;
        }
        while (intensity < maxIntensity);

        intensity = maxIntensity;
        _light.intensity = intensity;
    }
}
