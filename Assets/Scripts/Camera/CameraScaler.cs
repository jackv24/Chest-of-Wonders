using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public Camera gameCamera;
    public PixelPerfectCamera gameCameraPixel;

    public GameObject screenQuad;

    void Start()
    {
        if(gameCameraPixel)
        {
            gameCameraPixel.OnChangeSize += UpdateScale;
            UpdateScale();
        }
    }

    void UpdateScale()
    {
        int resWidth = Mathf.RoundToInt(gameCameraPixel.nativeAssetResolution.x);
        int resHeight = Mathf.RoundToInt(gameCameraPixel.nativeAssetResolution.y);

        RenderTexture texture = new RenderTexture(resWidth, resHeight, 0, RenderTextureFormat.ARGB32);
        texture.filterMode = FilterMode.Point;

        gameCamera.targetTexture = texture;

        screenQuad.GetComponent<Renderer>().material.mainTexture = texture;

        float ratio = (float)resWidth / resHeight;

        Vector3 s = screenQuad.transform.localScale;
        s.x = s.y * ratio;
        screenQuad.transform.localScale = s;
    }
}
