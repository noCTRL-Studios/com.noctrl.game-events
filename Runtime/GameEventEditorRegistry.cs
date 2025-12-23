using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NoCtrl.GameEvents.Editor
{
    // [CreateAssetMenu(fileName = "GameEventEditorRegistry", menuName = "Scriptable Objects/GameEventEditorRegistry")]
    public class GameEventEditorRegistry : ScriptableObject
    {
        // list of all events registered in the editor
        public List<GameEvent> registeredEvents = new List<GameEvent>();  

        private static GameEventEditorRegistry _instance;

        public static GameEventEditorRegistry Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = LoadOrCreate();
                }

                return _instance;
            }
        }

        private static GameEventEditorRegistry LoadOrCreate()
        {
            const string path = "Assets/Runtime/GameEventEditorRegistry.asset";
            var asset = AssetDatabase.LoadAssetAtPath<GameEventEditorRegistry>(path);
            if (asset == null)
            {
                asset = CreateInstance<GameEventEditorRegistry>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }
    }
}
