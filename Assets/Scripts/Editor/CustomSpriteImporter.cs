using UnityEngine;
using System.Collections;
using UnityEditor;

public class CustomSpriteImporter : AssetPostprocessor
{
	void OnPreprocessTexture()
	{
		Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));

		string[] labels = AssetDatabase.GetLabels(asset);

		foreach(string label in labels)
		{
			//Don't PreProcess asset if it is tagged with the skip tag
			if (label == "SkipPreProcess")
				return;
		}

		//Only preprocess textures in the sprites folder
		if (assetPath.StartsWith("Assets/Sprites"))
		{
			TextureImporter textureImporter = (TextureImporter)assetImporter;
			textureImporter.spritePixelsPerUnit = 32;
			textureImporter.filterMode = FilterMode.Point;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;

			Debug.Log($"Preprocessing Texture: {assetPath}");
		}
	}
}
