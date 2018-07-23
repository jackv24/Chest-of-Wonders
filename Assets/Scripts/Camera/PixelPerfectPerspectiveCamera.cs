﻿using UnityEngine;

[ExecuteInEditMode]
public class PixelPerfectPerspectiveCamera : MonoBehaviour
{
    public const float PixelsPerUnit = 32.0f;

    private const float TargetFrustHeight = 360.0f / PixelsPerUnit;
    
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
        if (!cam)
            cam = GetComponent<Camera>();

        if (!cam) return;
        
        var frustumInnerAngles = (180f - cam.fieldOfView) / 2f * Mathf.PI / 180f;
        var newCamDist = Mathf.Tan(frustumInnerAngles) * (TargetFrustHeight / 2);
        transform.SetLocalPositionZ(-newCamDist);
    }

    private void OnDrawGizmosSelected()
    {
        if (!cam)
            cam = GetComponent<Camera>();

        if (!cam) return;
        
        // Draw box for orthographic size of camera at Z = 0 (2D game plane)
        Vector3 pos = transform.position;
        pos.z = 0;

        float width = TargetFrustHeight * cam.aspect;
        
        Gizmos.DrawWireCube(pos, new Vector3(width, TargetFrustHeight));
    }
}