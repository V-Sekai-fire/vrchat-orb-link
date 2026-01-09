# Simple GLB/glTF2 Asset Loading

## Project Overview

A minimal VRChat world system for loading 3D models (GLB, glTF2) from URLs. Features a floating URL input interface that loads models into the scene.

## Architecture

```
┌─────────────────────────────┐
│     UI Elements             │
│  - TMP_InputField (URL)     │
│  - Button (Load)            │
│  - TextMeshProUGUI (Status) │
└─────────────┬───────────────┘
              │
┌─────────────▼───────────────┐
│   OrbUIBuilder Script       │
│  - Instantiates GLB Loader  │
│  - Handles URL input        │
│  - Triggers loading         │
└─────────────┬───────────────┘
              │
┌─────────────▼───────────────┐
│   Voyage GLB Loader         │
│  - Loads GLB/glTF from URL  │
│  - Spawns in scene          │
└─────────────────────────────┘
```

## Design Decisions

1. **Simplicity First**: Minimal viable product - just load GLB from URL
2. **UI Assignment**: UI elements assigned in inspector rather than procedurally generated
3. **GLB Loader Integration**: Uses existing Voyage GLB loader for reliability
4. **No Complex Features**: No caching, cooldowns, or orb spawning - just basic loading

## Core Components

| File                     | Purpose                                       | Lines |
| ------------------------ | --------------------------------------------- | ----- |
| `Scenes/OrbUIBuilder.cs` | Main script - instantiates loader, handles UI | 58    |

## UI Layout

```
┌────────────────────────────────┐
│ URL: [https://example.com...]  │
│                     [LOAD]     │
│ Status: Loading...             │
└────────────────────────────────┘
```

## Key Features

- **Simple Loading**: Load GLB, glTF2 from URLs
- **Floating UI**: URL input field for easy model loading
- **Status Feedback**: Shows loading progress and errors
- **Default Model**: Pre-loaded with GLTF Duck for testing

## Integration Points

- **GLB Loader**: Uses `thirdparty/vrchat-glb-loader` package
- **VRChat API**: VRCUrl for web loading
- **Udon System**: UdonSharpBehaviour for VRChat compatibility
