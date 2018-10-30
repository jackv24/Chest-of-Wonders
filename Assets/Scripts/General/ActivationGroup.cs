using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationGroup : MonoBehaviour
{
    [SerializeField]
    private GameObject[] children;

    private void OnEnable()
    {
        foreach(var child in children)
            child?.SetActive(true);
    }

    private void OnDisable()
    {
        foreach (var child in children)
            child?.SetActive(false);
    }
}
