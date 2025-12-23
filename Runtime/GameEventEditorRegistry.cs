#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace NoCtrl.GameEvents
{
    public class GameEventEditorRegistry : ScriptableObject
    {
        private static GameEventEditorRegistry _instance;

        public static GameEventEditorRegistry Instance
        {
            get
            {
                if (_instance == null)
                    _instance = LoadOrCreate();
                return _instance;
            }
        }

        public List<GameEvent> registeredEvents = new List<GameEvent>();

        private static GameEventEditorRegistry LoadOrCreate()
        {
            const string path = "Assets/Editor/GameEventEditorRegistry.asset";

            // Ensure folder exists
            string folder = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            var asset = AssetDatabase.LoadAssetAtPath<GameEventEditorRegistry>(path);
            if (asset == null)
            {
                asset = CreateInstance<GameEventEditorRegistry>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }

        public void Register(GameEvent evt)
        {
            if (!registeredEvents.Contains(evt))
                registeredEvents.Add(evt);
        }
    }
}
#endif