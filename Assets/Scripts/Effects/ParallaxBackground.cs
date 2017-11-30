using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
	public float depthLimit = 10.0f;

	public float divide = 32.0f;

	private class Background
	{
		public Transform transform;
		public MeshRenderer renderer;
		public Vector2 initialPos;
	}

	private Background[] backgrounds;

	private Transform mainCam;

	private void Start()
	{
		MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

		backgrounds = new Background[renderers.Length];

		for(int i = 0; i < backgrounds.Length; i++)
		{
			backgrounds[i] = new Background();

			backgrounds[i].transform = renderers[i].transform;
			backgrounds[i].renderer = renderers[i];
			backgrounds[i].initialPos = renderers[i].transform.position;
		}
	}

	private void Update()
	{
		if (mainCam)
		{
			Vector3 pos = mainCam.position;
			pos.z = 0;

			transform.position = pos;

			for(int i = 0; i < backgrounds.Length; i++)
			{
				Vector2 offset = (Vector2)backgrounds[i].transform.position - backgrounds[i].initialPos;

				float multiplier = Mathf.Lerp(1, 0, backgrounds[i].transform.position.z / depthLimit);

				backgrounds[i].renderer.material.SetTextureOffset("_MainTex", (offset / divide) * multiplier);
			}
		}
		else if(Camera.main)
			mainCam = Camera.main.transform;
	}
}
