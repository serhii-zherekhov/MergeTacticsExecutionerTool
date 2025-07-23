using UnityEngine;

public class Rogue : MonoBehaviour
{
    [SerializeField] private GameObject[] _crossbows;

    private void Awake()
    { 
        _crossbows[Random.Range(0, 2)].gameObject.SetActive(true);
    }
}
