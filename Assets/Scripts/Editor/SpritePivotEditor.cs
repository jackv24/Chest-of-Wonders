using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpritePivotEditor : EditorWindow
{
	static void SetSpritePivot(SpriteAlignment alignment)
	{
		Texture2D[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

		foreach(Texture2D texture in textures)
		{
			Debug.Log($"Setting sprite pivot to {alignment} on {texture}");

			string path = AssetDatabase.GetAssetPath(texture);
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

			TextureImporterSettings settings = new TextureImporterSettings();
			textureImporter.ReadTextureSettings(settings);

			settings.spriteAlignment = (int)alignment;

			textureImporter.SetTextureSettings(settings);
			AssetDatabase.ImportAsset(path);
		}
	}

	[MenuItem("Overgrowth Tools/Sprite Pivots/Set Bottom")]
	static void SetSpritePivotBottom()
	{
		SetSpritePivot(SpriteAlignment.BottomCenter);
	}

	[MenuItem("Overgrowth Tools/Sprite Pivots/Set Center")]
	static void SetSpritePivotCenter()
	{
		SetSpritePivot(SpriteAlignment.Center);
	}
}
