using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureScaler : MonoBehaviour
{
	public RenderTexture templateTexture;

	[Space()]
	public Camera targetCamera;
	public Renderer targetQuad;

	private PixelCamera2D cameraScaler;

	private void Awake()
	{
		cameraScaler = GetComponentInParent<PixelCamera2D>();

		if(cameraScaler)
		{
			cameraScaler.OnResize += UpdateTexture;
		}
	}

	public void UpdateTexture()
	{
		if(!templateTexture)
		{
			Debug.LogError("No template texture assigned to scaler!", this);
			return;
		}

		if(!targetQuad)
		{
			Debug.LogError("No target quad assigned to scaler!", this);
			return;
		}

		if(!targetCamera)
		{
			Debug.LogError("No target camera assigned to scaler!", this);
			return;
		}

		int width = templateTexture.width;
		int height = templateTexture.height;
		FilterMode filterMode = FilterMode.Point;

		//If screen size is not a perfect multiple of base resolution, find closest multiple higher than screen resolution
		if (Screen.width % width != 0 && Screen.height % height != 0)
		{
			int maxIterations = 10;
			int currentIteration = 0;
			while (true)
			{
				//Prevent getting stuck in an infinite loop
				currentIteration++;
				if (currentIteration > maxIterations)
				{
					Debug.LogError("Render Texture Scaler exceeded max iterations!", this);
					return;
				}

				//Increase multiplier until bigger or equal to screen resolution
				width = templateTexture.width * currentIteration;
				height = templateTexture.height * currentIteration;

				if (width >= Screen.width && height >= Screen.height)
					break;
			}

			// Since we're downscaling from a higher reolution use smooth filtering
			// (slightly blurry downscaled is preferrable to half-pixels in point filtering)
			filterMode = FilterMode.Bilinear;
		}

		RenderTexture targetTexture = new RenderTexture(width, height, templateTexture.depth, templateTexture.format);
		targetTexture.filterMode = filterMode;

		targetCamera.targetTexture = targetTexture;

		targetQuad.material.SetTexture("_MainTex", targetTexture);
	}
}
