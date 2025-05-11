# Ludo Analytics Documentation

This directory contains documentation for the Ludo Analytics package.

## Contents

- [Dependency Injection](DependencyInjection.md) - How to use Ludo Analytics with dependency injection
- [Creating Custom Providers](CustomProviders.md) - Guide to implementing your own analytics providers

## Core Concepts

### Analytics Service

The `IAnalyticsService` is the main interface for interacting with the analytics system. It provides methods for:

- Tracking events
- Managing global contexts
- Setting user properties
- Handling user consent
- Managing analytics providers

### Analytics Providers

Providers are implementations of the `IAnalyticsProvider` interface that handle sending analytics data to specific backends. The package includes:

- `DebugAnalyticsProvider` - Outputs events to the Unity console for debugging

### Global Contexts

Global contexts are objects that contain data that should be included with all analytics events. For example:

- Player information (ID, level, etc.)
- Device information
- Session information

### Events

Events represent actions or occurrences that you want to track. Each event has:

- A name
- Optional parameters
- A timestamp

## Basic Usage

```csharp
// With dependency injection
[Inject] private IAnalyticsService _analyticsService;

// Track a simple event
_analyticsService.TrackEvent("button_clicked");

// Track an event with parameters
_analyticsService.TrackEvent("item_purchased", new Dictionary<string, object>
{
    { "item_id", "sword_01" },
    { "price", 100 },
    { "currency", "gold" }
});
```

## Configuration

Each provider has its own configuration class that extends `ProviderConfig`. For example, the `DebugProviderConfig` allows you to configure:

- Whether the provider is enabled on start
- Verbose logging
- Log format strings

## Best Practices

1. **Use meaningful event names** - Follow a consistent naming convention (e.g., noun_verb)
2. **Structure parameters consistently** - Use the same parameter names across similar events
3. **Use global contexts** - Avoid repeating common data in every event
4. **Handle consent properly** - Always respect user privacy preferences
5. **Flush events on application quit** - Ensure all events are sent before the application closes
