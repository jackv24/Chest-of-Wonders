using UnityEngine;
using System.Collections;
using UnityEditor;

public class CustomSpriteImporter : AssetPostprocessor
{
	void OnPreprocessTexture()
	{
		if (assetPath.StartsWith("Assets/Sprites"))
		{
			Debug.Log($"Preprocessing Texture: {assetPath}");

			TextureImporter textureImporter = (TextureImporter)assetImporter;
			textureImporter.spritePixelsPerUnit = 32;
			textureImporter.filterMode = FilterMode.Point;
		}
	}
}
