﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
	public static CameraFollow Instance;

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

	private List<CameraLockArea> cameraLockAreas = new List<CameraLockArea>();
	private bool usingCameraLock = false;
	private Rect camLockRect;

    private Vector2 minCameraWorld;
    private Vector2 maxCameraWorld;

    private new PixelCamera2D camera;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
    {
        //If no target has been assigned, attempt to find and set the player as the target
        if (!target)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player)
                target = player.transform;
        }

        camera = FindObjectOfType<PixelCamera2D>();

        if (camera)
            camera.OnResize += CalculateBounds;

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
        if (target && target.gameObject.activeSelf)
        {
            aheadDistance = Mathf.Lerp(aheadDistance, lookAhead * (lookRight ? 1 : -1), lookAheadSpeed * Time.deltaTime);

            targetPos = target.position;
            targetPos.y += heightOffset;
            targetPos.x += aheadDistance;

            targetPos.z = transform.position.z;

			KeepInBounds();

			if (usingCameraLock)
				KeepInCameraLock();

			transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

            //Calculate camera world bounds
            minCameraWorld = Camera.main.ViewportToWorldPoint(new Vector3(0, 0));
            maxCameraWorld = Camera.main.ViewportToWorldPoint(new Vector3(1, 1));
        }
    }

    void KeepInBounds()
    {
        if (bounds && camera)
        {
			//Keep camera inside of level (or centred on x if level does not exceed camera width)
			if (Camera.main.orthographicSize * 2 * ((float)camera.Width / camera.Height) > bounds.width)
				targetPos.x = bounds.centre.x;
			else
				targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);

			//Keep camera inside of level (or centred on y if level does not exceed camera height)
			if (Camera.main.orthographicSize * 2 > bounds.height)
				targetPos.y = bounds.centre.y;
			else
				targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
        }
    }

    public void CalculateBounds()
    {
        if (bounds && camera)
        {
            float vertExtent = Camera.main.orthographicSize;
            float horzExtent = vertExtent * ((float)camera.Width / camera.Height);

            //Calculate area in which camera can move inside the level
            minX = horzExtent - bounds.width / 2.0f + bounds.centre.x;
            maxX = bounds.width / 2.0f - horzExtent + bounds.centre.x;
            minY = vertExtent - bounds.height / 2.0f + bounds.centre.y;
            maxY = bounds.height / 2.0f - vertExtent + bounds.centre.y;
        }
    }

    public void SetBounds(LevelBounds bounds)
    {
        this.bounds = bounds;

        CalculateBounds();

        //Set initial position to prevent weird lerping
        if (target)
        {
            targetPos = target.position;
            targetPos.y += heightOffset;
            targetPos.x += lookAhead * (lookRight ? 1 : -1);

            targetPos.z = transform.position.z;
        }

        KeepInBounds();

        if(target)
            transform.position = targetPos;
    }

    public bool IsInView(Vector2 worldPos)
    {
        if (worldPos.x > minCameraWorld.x && worldPos.y > minCameraWorld.y &&
            worldPos.x < maxCameraWorld.x && worldPos.y < maxCameraWorld.y)
        {
            return true;
        }
        else
            return false;
    }

	public void AddCameraLock(CameraLockArea camLock)
	{
		if(!cameraLockAreas.Contains(camLock))
		{
			cameraLockAreas.Add(camLock);

			SetCameraLock(camLock);
		}
	}

	public void RemoveCameraLock(CameraLockArea camLock)
	{
		if (cameraLockAreas.Contains(camLock))
		{
			cameraLockAreas.Remove(camLock);

			//If there are still cam locks in the list, use the last one
			if (cameraLockAreas.Count > 0)
				SetCameraLock(cameraLockAreas[cameraLockAreas.Count - 1]);
			else
			{
				usingCameraLock = false;
			}
		}
	}

	void SetCameraLock(CameraLockArea camLock)
	{
		if (camLock && camera)
		{
			//Calculate area in which camera can move inside the level
			camLockRect = camLock.Rect;

			usingCameraLock = true;
		}
	}

	void KeepInCameraLock()
	{
		if (camera)
		{
			float vertExtent = Camera.main.orthographicSize;
			float horzExtent = vertExtent * ((float)camera.Width / camera.Height);

			Rect newRect = camLockRect;
			newRect.width -= horzExtent * 2;
			newRect.height -= vertExtent * 2;
			newRect.x += horzExtent;
			newRect.y += vertExtent;

			if (newRect.width <= 0)
				targetPos.x = camLockRect.center.x;
			else
				targetPos.x = Mathf.Clamp(targetPos.x, newRect.xMin, newRect.xMax);

			if (newRect.height <= 0)
				targetPos.y = camLockRect.center.y;
			else
				targetPos.y = Mathf.Clamp(targetPos.y, newRect.yMin, newRect.yMax);
		}
	}
}
