# Ludo Analytics: Generic Analytics Service for Unity 6

A flexible and extensible analytics service for Unity 6, designed to simplify the integration of multiple third-party and custom analytics providers. It allows for simultaneous event tracking to various backends and introduces "Analytics Contexts" to automatically enrich events with common data, reducing boilerplate and improving data consistency.

## Core Features

* **Multi-Provider Support:** Integrate and send data to multiple analytics providers (e.g., Firebase, AppsFlyer, custom HTTP endpoint) at the same time.
* **Provider Agnostic API:** A single, clean API to track events, abstracting away provider-specific SDKs.
* **Analytics Contexts:** Define reusable data contexts (e.g., `UserContext`, `SessionContext`, `PlacementContext`) that are automatically merged with your event data. Update contexts once, and all subsequent events are enriched.
* **Extensible Design:** Easily add support for new analytics providers by implementing a simple interface (`IAnalyticsProvider`). Define custom context classes as needed.
* **Configuration via ScriptableObjects:** Configure provider-specific settings (API keys, flags, etc.) directly in the Unity Editor.
* **Offline Queuing:** (Conceptual - providers are responsible for their queuing) Robust handling of events during network unavailability.
* **Consent Management Hooks:** Designed to integrate with user consent mechanisms.
* **Built with SOLID & Clean Code:** Aims for a maintainable, testable, and understandable codebase.