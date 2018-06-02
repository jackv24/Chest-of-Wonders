using UnityEngine;
using System.Collections;
using UnityEditor;

public class CustomSpriteImporter : AssetPostprocessor
{
	void OnPreprocessTexture()
	{
		//Only preprocess textures in the sprites folder
		if (assetPath.StartsWith("Assets/Sprites"))
		{
			Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));

			//Only process new assets
			if (!asset)
			{
				TextureImporter textureImporter = (TextureImporter)assetImporter;
				textureImporter.spritePixelsPerUnit = 32;
				textureImporter.filterMode = FilterMode.Point;

				Debug.Log($"Preprocessing Texture: {assetPath}");
			}
			else
				Debug.Log($"Skipping preprocess of texture: {assetPath}");
		}
	}
}
