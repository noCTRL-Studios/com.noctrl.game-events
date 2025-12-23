using System.IO;
using UnityEngine;

namespace NoCtrl.GameEvents
{
    public static class GameEventsDefinitions
    {
        public static string MetricsDirectory
        {
            get
            {
#if UNITY_EDITOR
                string path = Path.Combine(Application.dataPath, "../Library/GameEventMetrics");
                return Path.GetFullPath(path);
#else

                return string.Empty;

#endif
            }
        }
    }
}
        
