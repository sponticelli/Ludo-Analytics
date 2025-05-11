using UnityEngine;

namespace Ludo.Core.Analytics
{
    public abstract class ProviderConfig : ScriptableObject
    {
        public bool IsEnabledOnStart = true;
        public abstract string ProviderIdentifier { get; } // e.g., "Firebase", "Appsflyer"
    }
}