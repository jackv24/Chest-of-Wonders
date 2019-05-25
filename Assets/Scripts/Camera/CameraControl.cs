using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraControl : MonoBehaviour
{
    public class FocusTarget
    {
        public Transform target;
        public Vector2 offset;

        public Vector2 Position { get { return target ? (Vector2)target.position + offset : Vector2.zero; } }

        public float weight;

        public FocusTarget(Transform target, Vector2 offset, float weight = 1.0f)
        {
            if (!target)
                Debug.LogError("Camera Focus Target cannot be null!", target);

            this.target = target;
            this.offset = offset;
            this.weight = weight;
        }
    }

    public static CameraControl Instance;

    public Transform target;

	private List<FocusTarget> focusTargets = new List<FocusTarget>();

	public float focusTargetLerpSpeed = 2.0f;

	[Space()]
    public float followSpeed = 10f;
    public float heightOffset = 1f;

    [Space()]
    public float lookAhead = 5f;
    public float lookAheadSpeed = 2f;
    private float aheadDistance;
    private bool lookRight = true;

	private bool skipLerpAhead = false;
	private float skippedAheadDistance;

	[Space()]
	public float cameraMinMove = 0.1f;

    private Vector3 targetPos;
    private LevelBounds bounds;

    private float minX, maxX, minY, maxY;

	[Space()]
	public float cameraLockLerpSpeed = 2.0f;

	private List<CameraLockArea> cameraLockAreas = new List<CameraLockArea>();
	private bool usingCameraLock = false;
	private bool didJustEnterLevel = false;
	private Bounds targetLockBounds;
	private Bounds currentLockBounds;

    private Vector2 minCameraWorld;
    private Vector2 maxCameraWorld;

	private Vector3 worldPos;

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

		worldPos = transform.position;
    }

    private void LateUpdate()
    {
        if (target && target.gameObject.activeSelf)
        {
			float lerpSpeed = 1.0f;

			//Only follow player if there are no other focus targets
			if (focusTargets.Count <= 0)
			{
				//Don't lerp if flag is set (used when coming out of focus targets)
				if (skipLerpAhead)
				{
					aheadDistance = skippedAheadDistance;

					skipLerpAhead = false;
				}
				else
				{
					//aheadDistance = Mathf.Lerp(aheadDistance, lookAhead * (lookRight ? 1 : -1), lookAheadSpeed * Time.deltaTime);
					aheadDistance = Helper.LerpClamped(aheadDistance, lookAhead * (lookRight ? 1 : -1), lookAheadSpeed * Time.deltaTime, cameraMinMove);
				}

				targetPos = target.position;
				targetPos.y += heightOffset;
				targetPos.x += aheadDistance;

				targetPos.z = transform.position.z;

				lerpSpeed = followSpeed;
			}
			else //Get average position (weighted) between all follow targets
			{
				float division = 0;
				Vector2 sum = Vector2.zero;

				foreach(FocusTarget focusTarget in focusTargets)
				{
					//Multiply position by weight and include weight in division for a weighted average
					sum += focusTarget.Position * focusTarget.weight;
					division += focusTarget.weight;
				}

				Vector2 pos = sum / division;

				targetPos = new Vector3(pos.x, pos.y, transform.position.z);

				lerpSpeed = focusTargetLerpSpeed;
			}

			KeepInCameraLock();

			KeepInBounds();

			if (didJustEnterLevel)
				worldPos = targetPos;
			else
				worldPos = Vector3.Lerp(worldPos, targetPos, lerpSpeed * Time.deltaTime);

			transform.position = worldPos.SnapToGrid();

            //Calculate camera world bounds
            minCameraWorld = Camera.main.ViewportToWorldPoint(new Vector3(0, 0));
            maxCameraWorld = Camera.main.ViewportToWorldPoint(new Vector3(1, 1));
        }

		//Set did just enter level flag at end of frame
		didJustEnterLevel = false;
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

		//If new level bounds are set then new level must have been entered
		// clearing camlocks from previous error removes missing references
		ClearCameraLocks();
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

	#region Camera Lock Methods
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
				//Create new cam lock bounds from level bounds, for smooth lerping when exiting all CameraLockAreas
				targetLockBounds = new Bounds(bounds.centre, new Vector3(bounds.width, bounds.height));

				usingCameraLock = false;
			}
		}
	}

	void ClearCameraLocks()
	{
		cameraLockAreas.Clear();

		currentLockBounds.size = Vector3.zero;

		usingCameraLock = false;
		didJustEnterLevel = true;
	}

	void SetCameraLock(CameraLockArea camLock)
	{
		if (camLock && camera)
		{
			//Calculate area in which camera can move inside the level
			targetLockBounds = camLock.Bounds;

			if (didJustEnterLevel)
			{
				currentLockBounds = targetLockBounds;
			}
			else
			{
				//If not coming from a previous camera lock, use screen bounds for lerping
				if (!usingCameraLock)
				{
					currentLockBounds = GetCameraBounds();
				}
			}

			usingCameraLock = true;
		}
	}

	void KeepInCameraLock()
	{
		//If bounds are zero then there are no bounds set
		if (currentLockBounds.size == Vector3.zero)
			return;

		currentLockBounds = Helper.Lerp(currentLockBounds, targetLockBounds, cameraLockLerpSpeed * Time.deltaTime, cameraMinMove);

		if (camera)
		{
			float vertExtent = Camera.main.orthographicSize;
			float horzExtent = vertExtent * ((float)camera.Width / camera.Height);

			Vector3 screenExtents = new Vector3(horzExtent, vertExtent, 0);

			Bounds bounds = new Bounds();
			bounds.min = currentLockBounds.min + screenExtents;
			bounds.max = currentLockBounds.max - screenExtents;

			if (bounds.size.x <= 0)
				targetPos.x = currentLockBounds.center.x;
			else
				targetPos.x = Mathf.Clamp(targetPos.x, bounds.min.x, bounds.max.x);

			if (bounds.size.y <= 0)
				targetPos.y = currentLockBounds.center.y;
			else
				targetPos.y = Mathf.Clamp(targetPos.y, bounds.min.y, bounds.max.y);
		}
	}

	Bounds GetCameraBounds()
	{
		Bounds bounds = new Bounds();

		//Rect starts at top left, and positive Y is down
		bounds.min = minCameraWorld;
		bounds.max = maxCameraWorld;

		return bounds;
	}
	#endregion

	#region Camera Target Methods
	public void AddFocusTarget(FocusTarget focusTarget)
	{
		if (!focusTargets.Contains(focusTarget))
		{
			focusTargets.Add(focusTarget);

			//Clear flag in case targets were cleared previously in this frame
			skipLerpAhead = false;
		}
	}

	public void RemoveFocusTarget(FocusTarget focusTarget)
	{
		if (focusTargets.Contains(focusTarget))
			focusTargets.Remove(focusTarget);

		if (focusTargets.Count <= 0)
		{
			EndFocusTargets(focusTarget);
		}
	}

	public void ClearFocusTargets()
	{
		if (focusTargets.Count > 0)
		{
			EndFocusTargets(focusTargets[focusTargets.Count - 1]);

			focusTargets.Clear();
		}
	}

	private void EndFocusTargets(FocusTarget lastTarget)
	{
		skipLerpAhead = true;

		//Prevent look ahead from looking weird when coming out of focusing on targets
		skippedAheadDistance = Mathf.Abs(target.transform.position.x - lastTarget.Position.x) * (lookRight ? 1 : -1);
	}

	public FocusTarget GetFocusTargetByTransform(Transform target)
	{
		foreach(FocusTarget focusTarget in focusTargets)
		{
			if (focusTarget.target == target)
				return focusTarget;
		}

		return null;
	}
	#endregion

	private void OnDrawGizmos()
	{
		if (currentLockBounds.size != Vector3.zero)
			Gizmos.DrawWireCube(currentLockBounds.center, currentLockBounds.size);
	}
}
