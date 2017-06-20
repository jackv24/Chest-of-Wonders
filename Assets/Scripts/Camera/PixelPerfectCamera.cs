using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelPerfectCamera : MonoBehaviour
{
    public int spriteSize = 32;
    private float scale = 1f;

    [System.Serializable]
    public class BreakPoint
    {
        public int screenHeight = 720;
        public int scale = 2;
    }

    public List<BreakPoint> breakPoints = new List<BreakPoint>();

    [Space()]
    public CanvasScaler canvasScaler;

    private float orthographicSize;

    private void Start()
    {
        StartCoroutine("CheckChangeSize");
    }

    IEnumerator CheckChangeSize()
    {
        int lastHeight = 0, lastWidth = 0;

        CameraFollow follow = GetComponent<CameraFollow>();

        while (true)
        {
            //If the screen size has changed
            if (Screen.height != lastHeight || Screen.width != lastWidth)
            {
                lastHeight = Screen.height;
                lastWidth = Screen.width;

                //Get scale from breakpoints
                for (int i = 0; i < breakPoints.Count; i++)
                {
                    if (Screen.height >= breakPoints[i].screenHeight)
                        scale = breakPoints[i].scale;
                }

                //Calculate and set orthographic size
                orthographicSize = Screen.height / (spriteSize * scale) / 2f;
                Camera.main.orthographicSize = orthographicSize;

                if (canvasScaler)
                    canvasScaler.scaleFactor = scale;

                //If there is a follow script, recalculate the bounds
                if (follow)
                    follow.CalculateBounds();
            }

            //Should only need to check every second
            yield return new WaitForSeconds(1);
        }
    }
}
