using UnityEngine;

public class PixelCamera2D : MonoBehaviour
{
    public delegate void NormalEvent();
    public event NormalEvent OnResize;

    public int Width { get { return baseWidth; } }
    public int Height { get { return baseHeight; } }

    [SerializeField]
    private int baseWidth = 640;

    [SerializeField]
    private int baseHeight = 360;

    [SerializeField]
    private MeshRenderer fullScreenQuad;

    [SerializeField]
    private MeshRenderer[] gameScreenQuads;

    public int BaseWidth { get { return baseWidth; } }
    public int BaseHeight { get { return baseHeight; } }

    public Camera mainCamera;
    public Camera screenCamera;

    private int previousWidth = 0;
    private int previousHeight = 0;

    private void Update()
    {
        if (Screen.width != previousWidth || Screen.height != previousHeight)
        {
            UpdatePreviousValues();

            UpdateCamera();
        }
    }

    private void UpdateCamera()
    {
        ScaleBehaviour();

        if (OnResize != null)
            OnResize();
    }

    private void ScaleBehaviour()
    {
        float targetAspectRatio = baseWidth / (float)baseHeight;
        float windowAspectRatio = Screen.width / (float)Screen.height;
        float scaleHeight = windowAspectRatio / targetAspectRatio;

        if (scaleHeight < 1f)
            fullScreenQuad.transform.localScale = new Vector3(targetAspectRatio * scaleHeight, scaleHeight, 1f);
        else
            fullScreenQuad.transform.localScale = new Vector3(targetAspectRatio, 1f, 1f);

        foreach (var quad in gameScreenQuads)
            quad.transform.localScale = new Vector3(targetAspectRatio, 1f, 1f);
    }

    private void UpdatePreviousValues()
    {
        previousWidth = Screen.width;
        previousHeight = Screen.height;
    }

    private Camera GetPixelCameraRenderer(Camera cameraToIgnore)
    {
        foreach (Camera possiblePixelCameraRenderer in GetComponentsInChildren<Camera>())
        {
            if (possiblePixelCameraRenderer != cameraToIgnore)
            {
                return possiblePixelCameraRenderer;
            }
        }

        return null;
    }

    private float GetCameraRectOffset(int size)
    {
        return size % 2 == 0 ? 0 : 1f / size;
    }

    public Vector3 ScreenToWorldPosition(Vector3 screenPosition)
    {
        int targetWidth  = Screen.width;
        int targetHeight = Screen.height;

        float xScaleFactor = (float)targetWidth / baseWidth;
        float yScaleFactor = (float)targetHeight / baseHeight;
        float scalefactor = Mathf.Min(xScaleFactor, yScaleFactor);

        targetWidth = (int)(baseWidth * scalefactor);
        targetHeight = (int)(baseHeight * scalefactor);

        Vector3 offset = new Vector3(
            (Screen.width - targetWidth) / 2,
            (Screen.height - targetHeight) / 2,
            0.0f);

        Vector3 correctedPosition = (screenPosition - offset) / scalefactor;

        return mainCamera.ScreenToWorldPoint(correctedPosition);
    }
}
