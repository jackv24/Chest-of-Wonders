using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public static CameraManager Instance;

	public Camera gameCamera;
	public Camera uiCamera;

	public static Camera GameCamera => Instance ? Instance.gameCamera : null;
	public static Camera UICamera => Instance ? Instance.uiCamera : null;

	private void Awake()
	{
		Instance = this;
	}
}
