using System.Collections.Generic;

namespace Ludo.Core.Analytics
{
    public interface IAnalyticsService
    {
        void Initialize();
        void RegisterProvider(IAnalyticsProvider provider, ProviderConfig config);
        void UnregisterProvider(IAnalyticsProvider provider);

        // Context Management
        void UpdateGlobalContext<T>(T contextData) where T : class; // Add or update a global context
        void RemoveGlobalContext<T>() where T : class;              // Remove a global context
        T GetGlobalContext<T>() where T : class;                    // Retrieve a global context (e.g., for UI)

        // Event Tracking (eventData can be minimal, contexts provide the rest)
        void TrackEvent(string eventName);
        void TrackEvent(string eventName, Dictionary<string, object> eventData);

        void SetUserProperty(string propertyName, object propertyValue); // For direct user properties if needed outside contexts
        void SetConsent(bool hasConsented, Dictionary<string, bool> providerSpecificConsent);
        void FlushAllProviders();
    }
}