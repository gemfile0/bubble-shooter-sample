using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace BubbleShooterSample.Helper
{
    public class SwitchSceneMenu
    {
        private const string ScenePath = "Assets/BubbleShooterSample/Scenes";

        private const string SceneName_LevelEditor = "LevelEditorScene.unity";
        private const string SceneName_LevelPlayer = "LevelPlayerScene.unity";
        private const string SceneName_Title = "TitleScene.unity";

        [MenuItem("BubbleShooterSample/Switch Scene/Level Editor", false, 1001)]
        public static void SwitchSceneToLevelEditor()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            string path = Path.Combine(ScenePath, SceneName_LevelEditor);

            EditorSceneManager.OpenScene(path);
        }

        [MenuItem("BubbleShooterSample/Switch Scene/Level Player", false, 1002)]
        public static void SwitchSceneToLevelPlayer()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            string path = Path.Combine(ScenePath, SceneName_LevelPlayer);

            EditorSceneManager.OpenScene(path);
        }

        [MenuItem("BubbleShooterSample/Switch Scene/Title", false, 1003)]
        public static void SwitchSceneToTitle()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            string path = Path.Combine(ScenePath, SceneName_Title);

            EditorSceneManager.OpenScene(path);
        }
    }
}