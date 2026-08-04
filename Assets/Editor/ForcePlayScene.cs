using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class ForcePlayScene
{
    static ForcePlayScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Main Menu.unity");
        }
    }
}