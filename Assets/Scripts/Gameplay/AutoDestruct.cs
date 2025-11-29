using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour
{
    [SerializeField]
    private float destructTime = 2.0f;

    // Simply destroy the gameobject after the given destructTime duration
    private void Start()
    {
        Destroy(gameObject, destructTime);
    }
}
