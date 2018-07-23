using UnityEngine;

[ExecuteInEditMode]
public class PixelPerfectPerspectiveCamera : MonoBehaviour
{
    public const float PixelsPerUnit = 32.0f;

    private Camera cam;

    private void Start()
    {
        UpdatePosition();
    }

    private void Update()
    {
        // We only need to update camera position every frame while in edit mode
        // since it is not likely the camera FOV will change in-game
        if (!Application.isPlaying) UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        if (!cam) return;
        
        const float targetFrustWidth = 360.0f / PixelsPerUnit;
        var frustumInnerAngles = (180f - cam.fieldOfView) / 2f * Mathf.PI / 180f;
        var newCamDist = Mathf.Tan(frustumInnerAngles) * (targetFrustWidth / 2);
        transform.SetLocalPositionZ(-newCamDist);
    }
}