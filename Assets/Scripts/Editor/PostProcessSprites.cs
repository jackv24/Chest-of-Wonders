using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class PostProcessSprites : AssetPostprocessor
{
	[System.Serializable]
	class AnimData
	{
		public string packingTag = "";
		public Vector2 pivot = new Vector2(0.5f, 0);
	}

	void OnPostprocessTexture(Texture2D texture)
	{
		TextureImporter importer = (TextureImporter)assetImporter;

		importer.spriteImportMode = SpriteImportMode.Single;
		importer.spritePixelsPerUnit = 32;

		importer.filterMode = FilterMode.Point;

		importer.textureCompression = TextureImporterCompression.CompressedHQ;

		string path = Application.dataPath.TrimEnd(("Assets").ToCharArray()) + Path.GetDirectoryName(assetPath) + "/info.json";

		bool info = false;

		if (File.Exists(path))
		{
			info = true;

			string json = File.ReadAllText(path);

			AnimData data = JsonUtility.FromJson<AnimData>(json);

			importer.spritePackingTag = data.packingTag;


			TextureImporterSettings settings = new TextureImporterSettings();
			importer.ReadTextureSettings(settings);

			settings.spriteAlignment = (int)SpriteAlignment.Custom;
			settings.spritePivot = data.pivot;

			importer.SetTextureSettings(settings);
		}

		Debug.Log("Processing " + assetPath + (info ? " with info file" : "<b>without</b> info file"));
	}
}
