# Ludo Analytics: Generic Analytics Service for Unity

A flexible and extensible analytics service for Unity, designed to simplify the integration of multiple third-party and custom analytics providers. It allows for simultaneous event tracking to various backends and introduces "Analytics Contexts" to automatically enrich events with common data, reducing boilerplate and improving data consistency.

## Core Features

* **Multi-Provider Support:** Integrate and send data to multiple analytics providers (e.g., Firebase, AppsFlyer, custom HTTP endpoint) at the same time.
* **Provider Agnostic API:** A single, clean API to track events, abstracting away provider-specific SDKs.
* **Analytics Contexts:** Define reusable data contexts (e.g., `UserContext`, `SessionContext`, `PlacementContext`) that are automatically merged with your event data. Update contexts once, and all subsequent events are enriched.
* **Extensible Design:** Easily add support for new analytics providers by implementing a simple interface (`IAnalyticsProvider`). Define custom context classes as needed.
* **Configuration via ScriptableObjects:** Configure provider-specific settings (API keys, flags, etc.) directly in the Unity Editor.
* **Offline Queuing:** (Conceptual - providers are responsible for their queuing) Robust handling of events during network unavailability.
* **Consent Management Hooks:** Designed to integrate with user consent mechanisms.
* **Built with SOLID & Clean Code:** Aims for a maintainable, testable, and understandable codebase.

## Installation

### Using Unity Package Manager (UPM)

1. Open the Package Manager window in Unity (Window > Package Manager)
2. Click the "+" button in the top-left corner
3. Select "Add package from git URL..."
4. Enter: `https://github.com/sponticelli/Ludo-Analytics.git`
5. Click "Add"

### Manual Installation

1. Clone this repository
2. Copy the contents to your Unity project's Assets folder

## Quick Start

### 1. Create Provider Configuration

Create a configuration for the Debug provider:

```csharp
// Create via code
var debugConfig = ScriptableObject.CreateInstance<DebugProviderConfig>();
debugConfig.VerboseLogging = true;

// Or create via menu: Create > Ludo > Analytics > Debug Provider Config
// Then assign it in the Inspector
```

### 2. Initialize the Analytics Service

```csharp
// Create the service
IAnalyticsService analyticsService = new AnalyticsService();

// Create and register the debug provider
IAnalyticsProvider debugProvider = new DebugAnalyticsProvider();
analyticsService.RegisterProvider(debugProvider, debugConfig);

// Initialize the service
analyticsService.Initialize();

// Set user consent (required in many regions)
analyticsService.SetConsent(true, null);
```

### 3. Add Global Contexts

```csharp
// Create and add a player context
var playerContext = new PlayerContext
{
    PlayerId = "player123",
    Level = 5,
    Experience = 1250
};
analyticsService.UpdateGlobalContext(playerContext);
```

### 4. Track Events

```csharp
// Simple event
analyticsService.TrackEvent("game_started");

// Event with parameters
analyticsService.TrackEvent("level_completed", new Dictionary<string, object>
{
    { "level_id", "level_1" },
    { "score", 1250 },
    { "time_spent", 120 }
});
```

### 5. Set User Properties

```csharp
analyticsService.SetUserProperty("favorite_character", "wizard");
```

### 6. Flush Events on Exit

```csharp
private void OnApplicationQuit()
{
    analyticsService.FlushAllProviders();
}
```

## Using the Factory

For convenience, you can use the `AnalyticsFactory` to create and set up the service:

```csharp
// Create a service with a debug provider
IAnalyticsService service = AnalyticsFactory.CreateServiceWithDebugProvider();
```