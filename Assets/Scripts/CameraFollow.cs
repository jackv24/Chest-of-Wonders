using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Space()]
    public float followSpeed = 10f;

    public Vector2 offset = Vector2.up;

    private Vector3 targetPos;

    private void Start()
    {
        //If no target has been assigned, attempt to find and set the player as the target
        if(!target)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player)
                target = player.transform;
        }
    }

    private void LateUpdate()
    {
        if (target)
        {
            targetPos = target.position + (Vector3)offset;
            targetPos.z = transform.position.z;

            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        }
    }
}
