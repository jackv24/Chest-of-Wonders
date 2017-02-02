using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Space()]
    public float followSpeed = 10f;
    public float heightOffset = 1f;

    [Space()]
    public float lookAhead = 5f;
    public float lookAheadSpeed = 2f;
    private float aheadDistance;
    private bool lookRight = true;

    private Vector3 targetPos;
    private LevelBounds bounds;

    private float minX, maxX, minY, maxY;

    private void Start()
    {
        //If no target has been assigned, attempt to find and set the player as the target
        if(!target)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player)
                target = player.transform;
        }

        bounds = FindObjectOfType<LevelBounds>();

        CalculateBounds();

        if (target)
        {
            CharacterMove move = target.GetComponent<CharacterMove>();

            if (move)
            {
                move.OnChangedDirection += (float direction) =>
                {
                    if (direction >= 0)
                        lookRight = true;
                    else
                        lookRight = false;
                };
            }
        }
    }

    private void LateUpdate()
    {
        if (target)
        {
            aheadDistance = Mathf.Lerp(aheadDistance, lookAhead * (lookRight ? 1 : -1), lookAheadSpeed * Time.deltaTime);

            targetPos = target.position;
            targetPos.y += heightOffset;
            targetPos.x += aheadDistance;

            targetPos.z = transform.position.z;

            if (bounds)
            {
                //Keep camera inside of level (or centred on x if level does not exceed camera width)
                if (Camera.main.orthographicSize * 2 * (Screen.width / Screen.height) > bounds.width)
                    targetPos.x = 0;
                else
                    targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);

                //Keep camera inside of level (or centred on y if level does not exceed camera height)
                if (Camera.main.orthographicSize * 2 > bounds.height)
                    targetPos.y = 0;
                else
                    targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
            }

            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        }
    }

    public void CalculateBounds()
    {
        if (bounds)
        {
            float vertExtent = Camera.main.orthographicSize;
            float horzExtent = vertExtent * Screen.width / Screen.height;

            //Calculate area in which camera can move inside the level
            minX = horzExtent - bounds.width / 2.0f + bounds.centre.x;
            maxX = bounds.width / 2.0f - horzExtent + bounds.centre.x;
            minY = vertExtent - bounds.height / 2.0f + bounds.centre.y;
            maxY = bounds.height / 2.0f - vertExtent + bounds.centre.y;
        }
    }
}
