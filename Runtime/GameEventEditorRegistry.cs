#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace NoCtrl.GameEvents
{
    [CreateAssetMenu]
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

        public List<GameEvent> m_registeredEvents = new List<GameEvent>();

        public void Register(GameEvent gameEvent)
        {
            if (!m_registeredEvents.Contains(gameEvent))
                m_registeredEvents.Add(gameEvent);
        }

        public void Unregister(GameEvent gameEvent)
        {
            if (m_registeredEvents.Contains(gameEvent))
            {
                m_registeredEvents.Remove(gameEvent);
                CleanNullEntries();
                EditorUtility.SetDirty(this);
            }
        }

        public void BeginSession()
        {
            CleanNullEntries();
            // sessionStartTime = DateTime.UtcNow;

        }

        public void EndSession()
        {

            CleanNullEntries();
            // sessionEndTime = DateTime.UtcNow;

        }

        public void CleanNullEntries()
        {
            m_registeredEvents.RemoveAll(e => e == null);
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            { 
                case
                    PlayModeStateChange.EnteredPlayMode: 
                    BeginSession();
                    break;
                case
                    PlayModeStateChange.ExitingPlayMode: 
                    EndSession();
                    break;

            }
        }

        private static GameEventEditorRegistry LoadOrCreate()
        {
            string path = GameEventsDefinitions.GameEventEditorRegistryPath;

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


    }
}
#endif