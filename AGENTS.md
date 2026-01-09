# GLB/glTF2 Asset Loading

## Project Overview

Basic VRChat world setup for loading 3D models (GLB, glTF2) using the Voyage GLB loader package as-is.

**Status**: ✅ Basic Setup

## Architecture

```
┌─────────────────────────────┐
│     GLBLoaderSetup Script   │
│  - Instantiates GLB Loader  │
│  - Instantiates URL Canvas  │
└─────────────┬───────────────┘
              │
┌─────────────▼───────────────┐
│   Voyage GLB Loader         │
│  - Loads GLB/glTF from URL  │
│  - Spawns in scene          │
└─────────────────────────────┘
```

## Design Decisions

1. **Use As-Is**: No modifications to the original Voyage GLB loader
2. **Basic Setup**: Simple instantiation of existing prefabs
3. **Minimal Code**: Just setup script to instantiate components

## Core Components

| File                            | Purpose                                           | Lines |
| ------------------------------- | ------------------------------------------------- | ----- |
| `Scenes/GLBLoaderSetup.cs`      | Setup script to instantiate loader and UI         | 25    |

## Implementation Status

✅ Basic setup script implemented
✅ GLB loader prefab instantiation
✅ URL input canvas instantiation

## Key Features

- **Original GLB Loader**: Uses Voyage GLB loader without modifications
- **URL Input**: Built-in VRCUrlInputField for entering model URLs
- **Basic Functionality**: Load GLB, glTF2 models from web URLs

## Integration Points

- **GLB Loader**: `thirdparty/vrchat-glb-loader` package
- **VRChat API**: VRCUrl for web loading
- **Udon System**: UdonSharpBehaviour for VRChat compatibility
