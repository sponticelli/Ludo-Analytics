# Creating Custom Analytics Providers

This guide explains how to create custom analytics providers for the Ludo Analytics system.

## Overview

The Ludo Analytics system is designed to be extensible, allowing you to create custom providers for any analytics service. This is done by implementing the `IAnalyticsProvider` interface and creating a corresponding `ProviderConfig` class.

## Step 1: Create a Provider Configuration

First, create a configuration class for your provider by extending the `ProviderConfig` abstract class:

```csharp
using UnityEngine;
using Ludo.Core.Analytics;

[CreateAssetMenu(fileName = "MyProviderConfig", menuName = "Ludo/Analytics/My Provider Config")]
public class MyProviderConfig : ProviderConfig
{
    [SerializeField] private string _providerIdentifier = "MyProvider";
    [SerializeField] private string _apiKey = "";
    [SerializeField] private bool _enableCrashReporting = true;
    
    public override string ProviderIdentifier => _providerIdentifier;
    
    // Add properties for your provider-specific settings
    public string ApiKey => _apiKey;
    public bool EnableCrashReporting => _enableCrashReporting;
}
```

## Step 2: Implement the IAnalyticsProvider Interface

Next, create a class that implements the `IAnalyticsProvider` interface:

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using Ludo.Core.Analytics;

public class MyAnalyticsProvider : IAnalyticsProvider
{
    private MyProviderConfig _config;
    private bool _isInitialized;
    private bool _isEnabled;
    
    // Properties required by the interface
    public string ProviderName => "My Analytics Provider";
    public bool IsInitialized => _isInitialized;
    public bool IsEnabled => _isEnabled && _isInitialized;
    
    // Constructor with dependency injection
    [Inject]
    public MyAnalyticsProvider()
    {
        // Constructor logic
    }
    
    // Initialize the provider with the given configuration
    public void Initialize(ProviderConfig config)
    {
        if (config is not MyProviderConfig myConfig)
        {
            Debug.LogError($"{ProviderName} requires a MyProviderConfig, but received {config.GetType().Name}");
            return;
        }
        
        _config = myConfig;
        
        try
        {
            // Initialize your analytics SDK here
            // Example:
            // MyAnalyticsSDK.Initialize(_config.ApiKey);
            // MyAnalyticsSDK.SetCrashReportingEnabled(_config.EnableCrashReporting);
            
            _isInitialized = true;
            _isEnabled = _config.IsEnabledOnStart;
            
            Debug.Log($"{ProviderName} initialized successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize {ProviderName}: {ex.Message}");
        }
    }
    
    // Track an analytics event
    public void TrackEvent(AnalyticsEvent analyticsEvent)
    {
        if (!IsEnabled || analyticsEvent == null)
        {
            return;
        }
        
        try
        {
            // Convert the Ludo Analytics event to your provider's format
            // Example:
            // var myEvent = new MyAnalyticsSDK.Event(analyticsEvent.EventName);
            // foreach (var param in analyticsEvent.Parameters)
            // {
            //     myEvent.AddParameter(param.Key, param.Value);
            // }
            // MyAnalyticsSDK.LogEvent(myEvent);
            
            Debug.Log($"{ProviderName} tracked event: {analyticsEvent.EventName}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking event with {ProviderName}: {ex.Message}");
        }
    }
    
    // Set a user property
    public void SetUserProperty(string propertyName, object propertyValue)
    {
        if (!IsEnabled)
        {
            return;
        }
        
        try
        {
            // Set the user property in your analytics SDK
            // Example:
            // MyAnalyticsSDK.SetUserProperty(propertyName, propertyValue);
            
            Debug.Log($"{ProviderName} set user property: {propertyName} = {propertyValue}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error setting user property with {ProviderName}: {ex.Message}");
        }
    }
    
    // Flush pending events
    public void FlushEvents()
    {
        if (!IsEnabled)
        {
            return;
        }
        
        try
        {
            // Flush events in your analytics SDK
            // Example:
            // MyAnalyticsSDK.Flush();
            
            Debug.Log($"{ProviderName} flushed events");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error flushing events with {ProviderName}: {ex.Message}");
        }
    }
    
    // Enable or disable the provider
    public void Enable(bool enable)
    {
        if (!_isInitialized)
        {
            return;
        }
        
        _isEnabled = enable;
        
        try
        {
            // Enable or disable your analytics SDK
            // Example:
            // MyAnalyticsSDK.SetEnabled(enable);
            
            Debug.Log($"{ProviderName} {(enable ? "enabled" : "disabled")}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error {(enable ? "enabling" : "disabling")} {ProviderName}: {ex.Message}");
        }
    }
    
    // Set consent status
    public void SetConsent(bool hasConsentedToProvider, Dictionary<string, object> consentDetails)
    {
        try
        {
            // Set consent in your analytics SDK
            // Example:
            // MyAnalyticsSDK.SetConsent(hasConsentedToProvider);
            
            // Apply provider-specific consent details if available
            if (consentDetails != null)
            {
                // Example:
                // if (consentDetails.TryGetValue("advertising", out var advertisingConsent))
                // {
                //     MyAnalyticsSDK.SetAdvertisingConsent((bool)advertisingConsent);
                // }
            }
            
            Debug.Log($"{ProviderName} consent set to: {hasConsentedToProvider}");
            
            // Update enabled state based on consent
            Enable(hasConsentedToProvider);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error setting consent for {ProviderName}: {ex.Message}");
        }
    }
}
```

## Step 3: Register Your Provider with the Analytics Service

Register your provider with the analytics service using dependency injection:

```csharp
// In your installer class
public class AnalyticsInstaller : MonoInstaller
{
    [SerializeField] private MyProviderConfig _myProviderConfig;

    public override void InstallBindings()
    {
        // Bind IAnalyticsService to AnalyticsService as a singleton
        Container.Bind<IAnalyticsService>().To<AnalyticsService>().AsSingle();
        
        // Register your provider
        Container.BindInterfacesAndSelfTo<MyAnalyticsProvider>().AsSingle();
        
        // Bind the config
        Container.Bind<MyProviderConfig>().FromInstance(_myProviderConfig);
        
        // Initialize the service after all providers are registered
        Container.BindExecutionOrder<AnalyticsInitializer>().After<MyAnalyticsProvider>();
    }
}

// In your initializer
public class AnalyticsInitializer : IInitializable
{
    private readonly IAnalyticsService _analyticsService;
    private readonly MyAnalyticsProvider _myProvider;
    private readonly MyProviderConfig _myConfig;

    [Inject]
    public AnalyticsInitializer(
        IAnalyticsService analyticsService,
        MyAnalyticsProvider myProvider,
        MyProviderConfig myConfig)
    {
        _analyticsService = analyticsService;
        _myProvider = myProvider;
        _myConfig = myConfig;
    }

    public void Initialize()
    {
        // Register your provider
        _analyticsService.RegisterProvider(_myProvider, _myConfig);
        
        // Initialize the service
        _analyticsService.Initialize();
    }
}
```

## Best Practices for Custom Providers

1. **Error Handling**: Always wrap SDK calls in try-catch blocks to prevent crashes.

2. **Respect Consent**: Make sure your provider respects the user's consent settings.

3. **Parameter Conversion**: Handle parameter type conversion appropriately for your analytics backend.

4. **Batching**: Consider implementing batching for events to reduce network usage.

5. **Offline Support**: Implement offline caching if your analytics backend doesn't support it.

6. **Documentation**: Document any specific requirements or limitations of your provider.

## Example: Firebase Analytics Provider

Here's a simplified example of a Firebase Analytics provider:

```csharp
public class FirebaseAnalyticsProvider : IAnalyticsProvider
{
    private FirebaseProviderConfig _config;
    private bool _isInitialized;
    private bool _isEnabled;
    
    public string ProviderName => "Firebase Analytics";
    public bool IsInitialized => _isInitialized;
    public bool IsEnabled => _isEnabled && _isInitialized;
    
    public void Initialize(ProviderConfig config)
    {
        if (config is not FirebaseProviderConfig firebaseConfig)
        {
            Debug.LogError($"{ProviderName} requires a FirebaseProviderConfig");
            return;
        }
        
        _config = firebaseConfig;
        
        try
        {
            // Firebase is initialized elsewhere, just check if it's available
            if (Firebase.FirebaseApp.DefaultInstance != null)
            {
                _isInitialized = true;
                _isEnabled = _config.IsEnabledOnStart;
                Debug.Log($"{ProviderName} initialized successfully");
            }
            else
            {
                Debug.LogError($"{ProviderName} initialization failed: Firebase not initialized");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize {ProviderName}: {ex.Message}");
        }
    }
    
    public void TrackEvent(AnalyticsEvent analyticsEvent)
    {
        if (!IsEnabled || analyticsEvent == null)
        {
            return;
        }
        
        try
        {
            // Convert parameters to Firebase format
            var parameters = new List<Firebase.Analytics.Parameter>();
            
            foreach (var param in analyticsEvent.Parameters)
            {
                // Handle different parameter types
                if (param.Value is string stringValue)
                {
                    parameters.Add(new Firebase.Analytics.Parameter(param.Key, stringValue));
                }
                else if (param.Value is int intValue)
                {
                    parameters.Add(new Firebase.Analytics.Parameter(param.Key, intValue));
                }
                else if (param.Value is long longValue)
                {
                    parameters.Add(new Firebase.Analytics.Parameter(param.Key, longValue));
                }
                else if (param.Value is double doubleValue)
                {
                    parameters.Add(new Firebase.Analytics.Parameter(param.Key, doubleValue));
                }
                else
                {
                    // Convert other types to string
                    parameters.Add(new Firebase.Analytics.Parameter(param.Key, param.Value.ToString()));
                }
            }
            
            // Log the event to Firebase
            Firebase.Analytics.FirebaseAnalytics.LogEvent(
                analyticsEvent.EventName,
                parameters.ToArray());
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking event with {ProviderName}: {ex.Message}");
        }
    }
    
    // Implement other interface methods...
}
```

## Conclusion

By following this guide, you can create custom analytics providers that integrate with any analytics service while maintaining a consistent API through the Ludo Analytics system.
