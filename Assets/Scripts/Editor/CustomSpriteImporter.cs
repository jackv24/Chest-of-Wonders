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

			//Don't process this asset if it is labeled with the skip label
			string[] assetLabels = AssetDatabase.GetLabels(asset);
			foreach(string label in assetLabels)
			{
				if(label == "SkipPrep")
					return;
			}

			Debug.Log($"Preprocessing Texture: {assetPath}", asset);

			TextureImporter textureImporter = (TextureImporter)assetImporter;
			textureImporter.spritePixelsPerUnit = 32;
			textureImporter.filterMode = FilterMode.Point;
		}
	}
}
