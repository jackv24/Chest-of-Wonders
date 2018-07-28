using UnityEditor;
using UnityEngine;

public static class SceneCameraSettings
{
    private const string SortMenuPath = "Overgrowth Tools/Camera/Sort Mode/";
    
    [MenuItem(SortMenuPath + "Orthographic #o")]
    public static void SetCameraSortModeOrthographic()
    {
        SetCameraSortMode(TransparencySortMode.Orthographic);
    }
    
    [MenuItem(SortMenuPath + "Perspective #p")]
    public static void SetCameraSortModePerspective()
    {
        SetCameraSortMode(TransparencySortMode.Perspective);
    }

    private static void SetCameraSortMode(TransparencySortMode sortMode)
    {
        Camera camera = SceneView.lastActiveSceneView.camera;

        if (!camera)
        {
            Debug.LogError("Couldn't get last active scene view camera.");
            return;
        }

        camera.transparencySortMode = sortMode;
        
        Debug.Log($"Set scene camera sort mode to {sortMode.ToString()}");
    }
}
