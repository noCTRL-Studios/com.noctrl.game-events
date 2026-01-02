using System;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEngine;

namespace NoCtrl.GameEvents.Editor
{
    public class EventMetricRow
    {
        public string TimeStamp;
        public int UniqueListeners;
    }

    public class PlaySessionInfo 
    { 
        public string FolderName { get; }
        public string FullPath { get; }

        public PlaySessionInfo(string fullPath)
        {
            FullPath = fullPath; 
            FolderName = Path.GetFileName(fullPath);
        } 
        public override string ToString() => FolderName; 
    }

    public class EventMetricInfo
    {
        public string FileName { get; }
        public string FullPath { get; }

        public EventMetricInfo(string fullPath)
        {
            FullPath = fullPath;
            FileName = Path.GetFileNameWithoutExtension(fullPath);
        }


        public List<GameEvent.RaiseMetric> GetMetrics()
        {

            if (string.IsNullOrEmpty(FullPath) || !File.Exists(FullPath)) 
                return new List<GameEvent.RaiseMetric>(); 
            
            string json = File.ReadAllText(FullPath); 

            if (string.IsNullOrEmpty(json)) 
                return new List<GameEvent.RaiseMetric>();

            var collection = JsonUtility.FromJson<GameEvent.RaiseMetricCollection>(json);
            return collection?.metrics ?? new List<GameEvent.RaiseMetric>();
        }

        public override string ToString() => FileName;
    }
}
