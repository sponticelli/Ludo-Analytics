using System.Collections.Generic;

namespace Ludo.Core.Analytics
{
    public interface IAnalyticsProvider
    {
        string ProviderName { get; }
        bool IsInitialized { get; }
        bool IsEnabled { get; }

        void Initialize(ProviderConfig config);
        // The AnalyticsEvent received here will have its Parameters dictionary
        // pre-filled with data from global contexts and event-specific data.
        void TrackEvent(AnalyticsEvent analyticsEvent);
        void SetUserProperty(string propertyName, object propertyValue); // Can still be used for provider-specific user models
        void FlushEvents();
        void Enable(bool enable);
        void SetConsent(bool hasConsentedToProvider, Dictionary<string, object> consentDetails);
    }
}