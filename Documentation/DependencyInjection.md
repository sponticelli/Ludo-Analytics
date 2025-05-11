# Using Dependency Injection with Ludo Analytics

This guide explains how to use dependency injection with the Ludo Analytics package, specifically focusing on injecting the `IAnalyticsService` into your components.

## Overview

Instead of using the `AnalyticsFactory` to create instances of the analytics service, you can use dependency injection to have the service automatically injected into your components. This approach:

- Reduces coupling between components
- Makes testing easier
- Centralizes the configuration of the analytics service
- Follows the Inversion of Control principle

## Setup with UnityInject

Ludo Analytics supports dependency injection through the UnityInject pattern. Here's how to set it up:

### 1. Register the Analytics Service in Your Installer

First, register the `AnalyticsService` as the implementation for `IAnalyticsService` in your dependency injection installer:

```csharp
// In your installer class
public class AnalyticsInstaller : MonoInstaller
{
    [SerializeField] private DebugProviderConfig _debugProviderConfig;

    public override void InstallBindings()
    {
        // Bind IAnalyticsService to AnalyticsService as a singleton
        Container.Bind<IAnalyticsService>().To<AnalyticsService>().AsSingle();
        
        // Register providers
        Container.BindInterfacesAndSelfTo<DebugAnalyticsProvider>().AsSingle();
        
        // Bind the config
        Container.Bind<DebugProviderConfig>().FromInstance(_debugProviderConfig);
        
        // Initialize the service after all providers are registered
        Container.BindExecutionOrder<AnalyticsInitializer>().After<DebugAnalyticsProvider>();
    }
}

// Create an initializer to set up the service after injection
public class AnalyticsInitializer : IInitializable
{
    private readonly IAnalyticsService _analyticsService;
    private readonly DebugAnalyticsProvider _debugProvider;
    private readonly DebugProviderConfig _debugConfig;

    [Inject]
    public AnalyticsInitializer(
        IAnalyticsService analyticsService,
        DebugAnalyticsProvider debugProvider,
        DebugProviderConfig debugConfig)
    {
        _analyticsService = analyticsService;
        _debugProvider = debugProvider;
        _debugConfig = debugConfig;
    }

    public void Initialize()
    {
        // Register providers
        _analyticsService.RegisterProvider(_debugProvider, _debugConfig);
        
        // Initialize the service
        _analyticsService.Initialize();
        
        // Set default consent
        _analyticsService.SetConsent(true, null);
    }
}
```

### 2. Inject the Service into Your Components

Now you can inject the `IAnalyticsService` into any component that needs it:

```csharp
public class GameManager : MonoBehaviour
{
    [Inject] private IAnalyticsService _analyticsService;

    private void Start()
    {
        // Track game start event
        _analyticsService.TrackEvent("game_started");
    }

    public void LevelCompleted(int levelId, int score)
    {
        // Track level completion with parameters
        _analyticsService.TrackEvent("level_completed", new Dictionary<string, object>
        {
            { "level_id", levelId },
            { "score", score }
        });
    }
}
```

## Using Global Contexts with Dependency Injection

You can still use global contexts with the injected service:

```csharp
public class PlayerManager : MonoBehaviour
{
    [Inject] private IAnalyticsService _analyticsService;
    
    private PlayerData _playerData;

    private void Start()
    {
        _playerData = new PlayerData
        {
            PlayerId = "player123",
            Level = 5,
            Experience = 1250
        };
        
        // Add player data as a global context
        _analyticsService.UpdateGlobalContext(_playerData);
    }
}

public class PlayerData
{
    public string PlayerId { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
}
```

## Implementing Custom Providers with Dependency Injection

When implementing custom providers like the `DebugAnalyticsProvider`, you can also use dependency injection:

```csharp
public class DebugAnalyticsProvider : IAnalyticsProvider
{
    private DebugProviderConfig _config;
    private bool _isEnabled;
    private bool _isInitialized;

    // Constructor injection
    [Inject]
    public DebugAnalyticsProvider(DebugProviderConfig config = null)
    {
        // If no config is provided through DI, create a default one
        _config = config ?? ScriptableObject.CreateInstance<DebugProviderConfig>();
    }

    public string ProviderName => "Debug Analytics Provider";
    public bool IsInitialized => _isInitialized;
    public bool IsEnabled => _isEnabled && _isInitialized;

    // Rest of the implementation...
}
```

## Best Practices

1. **Single Responsibility**: Each component should only track events relevant to its functionality.

2. **Context Management**: Use a dedicated service for managing global contexts.

3. **Error Handling**: Always handle potential errors when tracking events.

4. **Testing**: Create mock implementations of `IAnalyticsService` for testing.

5. **Consent Management**: Handle user consent appropriately in a central location.

## Example: Analytics Manager

Here's a complete example of an analytics manager that uses dependency injection:

```csharp
public class AnalyticsManager : MonoBehaviour
{
    [Inject] private IAnalyticsService _analyticsService;
    
    // Called by the DI framework after injection
    [Inject]
    private void Initialize()
    {
        // Set up global contexts
        SetupGlobalContexts();
        
        // Track application start
        _analyticsService.TrackEvent("application_start", new Dictionary<string, object>
        {
            { "app_version", Application.version },
            { "platform", Application.platform.ToString() }
        });
    }
    
    private void SetupGlobalContexts()
    {
        // Device context
        var deviceContext = new DeviceContext
        {
            DeviceModel = SystemInfo.deviceModel,
            OperatingSystem = SystemInfo.operatingSystem,
            DeviceMemory = SystemInfo.systemMemorySize,
            GraphicsDeviceName = SystemInfo.graphicsDeviceName
        };
        
        _analyticsService.UpdateGlobalContext(deviceContext);
        
        // Session context
        var sessionContext = new SessionContext
        {
            SessionId = System.Guid.NewGuid().ToString(),
            SessionStartTime = DateTime.UtcNow
        };
        
        _analyticsService.UpdateGlobalContext(sessionContext);
    }
    
    private void OnApplicationQuit()
    {
        // Flush events before quitting
        _analyticsService.FlushAllProviders();
    }
}
```

## Conclusion

Using dependency injection with Ludo Analytics provides a clean, maintainable approach to analytics integration in your Unity project. It centralizes the configuration and initialization of the analytics service while making it easily accessible throughout your codebase.
