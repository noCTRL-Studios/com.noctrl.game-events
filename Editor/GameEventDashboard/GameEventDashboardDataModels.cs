using System.IO;
using UnityEngine;

namespace NoCtrl.GameEvents.Editor
{
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

        public override string ToString() => FileName;
    }
}
