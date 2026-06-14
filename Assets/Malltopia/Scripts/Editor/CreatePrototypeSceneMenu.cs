#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Malltopia.Editor
{
    public static class CreatePrototypeSceneMenu
    {
        private const string SceneDirectory = "Assets/Malltopia/Scenes";
        private const string ScenePath = "Assets/Malltopia/Scenes/Game.unity";

        [MenuItem("Malltopia/Prototype/Create Game Scene")]
        public static void CreateGameScene()
        {
            if (!Directory.Exists(SceneDirectory))
            {
                Directory.CreateDirectory(SceneDirectory);
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject bootstrapObject = new GameObject("StagePrototypeBootstrap");
            bootstrapObject.AddComponent<global::Malltopia.StagePrototypeBootstrap>();

            EditorSceneManager.SaveScene(scene, ScenePath);
            Selection.activeGameObject = bootstrapObject;
            EditorGUIUtility.PingObject(bootstrapObject);

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
        }
    }
}
#endif
