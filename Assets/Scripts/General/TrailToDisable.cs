using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailToDisable : MonoBehaviour
{
    public Transform target;
    public bool shouldMove = false;

    [Space()]
    public float moveSpeed = 10.0f;

    private TrailRenderer trail;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
    }

    void OnEnable()
    {
        shouldMove = false;
    }

    void Update()
    {
        if(shouldMove && target)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.position) < 0.1f)
                StartCoroutine("StartDestroy");
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        shouldMove = true;
    }

    IEnumerator StartDestroy()
    {
        shouldMove = false;

        float time = 0;

        while(time < trail.time)
        {
            yield return new WaitForEndOfFrame();

            time += Time.deltaTime;
        }

        gameObject.SetActive(false);
    }
}
