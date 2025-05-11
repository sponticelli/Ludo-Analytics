using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ludo.Core.Analytics
{
    /// <summary>
    /// Concrete implementation of the IAnalyticsService interface.
    /// Manages multiple analytics providers and handles event tracking, context management, and user properties.
    /// </summary>
    public class AnalyticsService : IAnalyticsService
    {
        private readonly Dictionary<IAnalyticsProvider, ProviderConfig> _providers = new();
        private readonly Dictionary<Type, object> _globalContexts = new();
        private bool _isInitialized;

        /// <summary>
        /// Initializes the analytics service and all registered providers.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("AnalyticsService is already initialized.");
                return;
            }

            foreach (var providerEntry in _providers)
            {
                try
                {
                    var provider = providerEntry.Key;
                    var config = providerEntry.Value;
                    
                    provider.Initialize(config);
                    provider.Enable(config.IsEnabledOnStart);
                    
                    Debug.Log($"Initialized analytics provider: {provider.ProviderName}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to initialize analytics provider: {ex.Message}");
                }
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Registers a new analytics provider with its configuration.
        /// </summary>
        /// <param name="provider">The analytics provider to register</param>
        /// <param name="config">The configuration for the provider</param>
        public void RegisterProvider(IAnalyticsProvider provider, ProviderConfig config)
        {
            if (provider == null)
            {
                Debug.LogError("Cannot register null analytics provider");
                return;
            }

            if (config == null)
            {
                Debug.LogError($"Cannot register provider {provider.ProviderName} with null config");
                return;
            }

            if (_providers.ContainsKey(provider))
            {
                Debug.LogWarning($"Provider {provider.ProviderName} is already registered. Updating config.");
                _providers[provider] = config;
                return;
            }

            _providers.Add(provider, config);
            
            // Initialize the provider if the service is already initialized
            if (_isInitialized)
            {
                provider.Initialize(config);
                provider.Enable(config.IsEnabledOnStart);
            }
        }

        /// <summary>
        /// Unregisters an analytics provider.
        /// </summary>
        /// <param name="provider">The provider to unregister</param>
        public void UnregisterProvider(IAnalyticsProvider provider)
        {
            if (provider == null || !_providers.ContainsKey(provider))
            {
                return;
            }

            _providers.Remove(provider);
        }

        /// <summary>
        /// Updates or adds a global context object that will be included with all events.
        /// </summary>
        /// <typeparam name="T">The type of context data</typeparam>
        /// <param name="contextData">The context data object</param>
        public void UpdateGlobalContext<T>(T contextData) where T : class
        {
            if (contextData == null)
            {
                RemoveGlobalContext<T>();
                return;
            }

            _globalContexts[typeof(T)] = contextData;
        }

        /// <summary>
        /// Removes a global context by type.
        /// </summary>
        /// <typeparam name="T">The type of context to remove</typeparam>
        public void RemoveGlobalContext<T>() where T : class
        {
            _globalContexts.Remove(typeof(T));
        }

        /// <summary>
        /// Gets a global context by type.
        /// </summary>
        /// <typeparam name="T">The type of context to retrieve</typeparam>
        /// <returns>The context object or null if not found</returns>
        public T GetGlobalContext<T>() where T : class
        {
            return _globalContexts.TryGetValue(typeof(T), out var context) ? context as T : null;
        }

        /// <summary>
        /// Tracks an event with no additional parameters.
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        public void TrackEvent(string eventName)
        {
            TrackEvent(eventName, null);
        }

        /// <summary>
        /// Tracks an event with additional parameters.
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        /// <param name="eventData">Additional event parameters</param>
        public void TrackEvent(string eventName, Dictionary<string, object> eventData)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("Cannot track event with null or empty name");
                return;
            }

            var analyticsEvent = new AnalyticsEvent(eventName, eventData);
            TrackEvent(analyticsEvent);
        }

        /// <summary>
        /// Tracks a pre-constructed analytics event.
        /// </summary>
        /// <param name="analyticsEvent">The event to track</param>
        public void TrackEvent(AnalyticsEvent analyticsEvent)
        {
            if (analyticsEvent == null)
            {
                Debug.LogError("Cannot track null analytics event");
                return;
            }

            // Enrich the event with global contexts
            EnrichEventWithGlobalContexts(analyticsEvent);

            // Send to all enabled providers
            foreach (var provider in _providers.Keys.Where(p => p.IsInitialized && p.IsEnabled))
            {
                try
                {
                    provider.TrackEvent(analyticsEvent);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error tracking event with provider {provider.ProviderName}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Sets a user property across all providers.
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="propertyValue">The value of the property</param>
        public void SetUserProperty(string propertyName, object propertyValue)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                Debug.LogError("Cannot set user property with null or empty name");
                return;
            }

            foreach (var provider in _providers.Keys.Where(p => p.IsInitialized && p.IsEnabled))
            {
                try
                {
                    provider.SetUserProperty(propertyName, propertyValue);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error setting user property with provider {provider.ProviderName}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Sets consent status for analytics tracking.
        /// </summary>
        /// <param name="hasConsented">Whether the user has consented to analytics</param>
        /// <param name="providerSpecificConsent">Provider-specific consent settings</param>
        public void SetConsent(bool hasConsented, Dictionary<string, bool> providerSpecificConsent)
        {
            foreach (var providerEntry in _providers)
            {
                var provider = providerEntry.Key;
                var config = providerEntry.Value;
                
                if (!provider.IsInitialized)
                {
                    continue;
                }

                bool providerConsent = hasConsented;
                
                // Check for provider-specific consent
                if (providerSpecificConsent != null && 
                    providerSpecificConsent.TryGetValue(config.ProviderIdentifier, out bool specificConsent))
                {
                    providerConsent = specificConsent;
                }

                // Create provider-specific consent details
                var consentDetails = new Dictionary<string, object>
                {
                    { "hasConsented", providerConsent }
                };

                try
                {
                    provider.SetConsent(providerConsent, consentDetails);
                    provider.Enable(providerConsent); // Enable/disable based on consent
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error setting consent for provider {provider.ProviderName}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Flushes pending events for all providers.
        /// </summary>
        public void FlushAllProviders()
        {
            foreach (var provider in _providers.Keys.Where(p => p.IsInitialized))
            {
                try
                {
                    provider.FlushEvents();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error flushing events for provider {provider.ProviderName}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Enriches an event with data from global contexts.
        /// </summary>
        /// <param name="analyticsEvent">The event to enrich</param>
        private void EnrichEventWithGlobalContexts(AnalyticsEvent analyticsEvent)
        {
            foreach (var contextEntry in _globalContexts)
            {
                var contextType = contextEntry.Key;
                var contextObject = contextEntry.Value;

                // Add context type name as prefix to avoid parameter name collisions
                var prefix = contextType.Name;

                // Use reflection to get all public properties and add them to the event parameters
                var properties = contextType.GetProperties();
                foreach (var property in properties)
                {
                    if (property.CanRead)
                    {
                        try
                        {
                            var value = property.GetValue(contextObject);
                            var paramName = $"{prefix}.{property.Name}";
                            
                            // Don't overwrite existing parameters
                            if (!analyticsEvent.Parameters.ContainsKey(paramName))
                            {
                                analyticsEvent.Parameters[paramName] = value;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"Error getting property {property.Name} from context {contextType.Name}: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}
