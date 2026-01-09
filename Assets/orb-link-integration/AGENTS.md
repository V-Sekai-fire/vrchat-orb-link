# Dynamic GLB/glTF2 Asset Loading for Attachable Orbs

## Project Overview

A VRChat world system that enables players to dynamically load 3D models (GLB, glTF2, VRM) from URLs into interactive orbs that attach to avatar bones. Each loaded model spawns a new independent orb with full Attach-To-Me compatibility.

**Status**: ✅ Complete & Production Ready

## Architecture

```
┌──────────────────────────────────────┐
│      UI Panel (DynamicOrbLoaderUI)   │
│  - URL Input, Scope Toggle, Cooldown │
└───────────────┬──────────────────────┘
                │
┌───────────────▼──────────────────────┐
│    Loader (DynamicOrbLoader)         │
│  - Caching, Cooldown, Registration   │
└───────┬────────────────────┬─────────┘
        │                    │
   ┌────▼─────┐      ┌──────▼───────────┐
   │ GLB Integration│ │ Attachable Setup │
   │ (OrbGLBIntegration)│ └──────┬───────────┘
   └────┬─────┘                    │
        │                          │
   ┌────▼─────┐                    │
   │ VRChat GLB│                    │
   │ Loader     │                    │
   └────┬─────┘                    │
        └────────────┬─────────────┘
                     │
         ┌───────────▼──────────────┐
         │   Orb Instance           │
         │  - DynamicOrb wrapper    │
         │  - Attachable component  │
         │  - VRC_Pickup            │
         │  - Rigidbody + Collider  │
         │  - Spring bones (if VRM) │
         └───────────┬──────────────┘
                     │
         ┌───────────▼──────────────┐
         │  Global Tracking System  │
         │  - Bone selection        │
         │  - Player tracking       │
         │  - Network sync          │
         └──────────────────────────┘
```

## Design Decisions

1. **GLB Integration**: Custom wrapper (`orb-link-integration/OrbGLBIntegration.cs`) around single shared VRChat GLB Loader instance for clean API and event handling
2. **One Orb Per Model**: No model swapping - simpler state management
3. **VRM Spring Bones**: Preserve existing; don't apply defaults
4. **Scope Modes**:
   - **World**: Load at full original scale
   - **Orb**: Auto-scale to fit within orb boundaries
5. **No Security Layer**: Trust world creators
6. **Flexible Scaling**: No hard limits on concurrent models

## Core Components

| File                            | Purpose                                           | Lines |
| ------------------------------- | ------------------------------------------------- | ----- |
| `orb-link-integration/DynamicOrb.cs` | Model wrapper, VRM detection, scope-based scaling | 120   |
| `orb-link-integration/DynamicOrbLoader.cs` | URL loading, caching, cooldowns, spawning         | 255   |
| `orb-link-integration/DynamicOrbLoaderUI.cs` | Canvas UI, status display, cooldown buttons       | 217   |
| `orb-link-integration/DynamicOrbNetworkSync.cs` | Network broadcasting, late-joiner support         | 105   |
| `orb-link-integration/DynamicOrbLifecycleManager.cs` | Cleanup, respawn, cache management                | 92    |
| `orb-link-integration/AttachableRegistration.cs` | Global tracking registration                      | 42    |
| `orb-link-integration/OrbGLBIntegration.cs` | GLB Loader wrapper for orb system                 | 65    |

## UI Layout

```
┌────────────────────────────────┐
│  Dynamic Model Loader Panel    │
├────────────────────────────────┤
│ Scope: [●] World  [ ] Orb     │
│ URL: [___________________]     │
│                     [LOAD]     │
│ Cooldown: [Off] [5s] [10s] [30s]
│ Timer: ◇ ◇ ◇                 │
│ Status: [World] Ready         │
└────────────────────────────────┘
```

## Implementation Status

✅ All 6 scripts implemented (700+ LOC)
✅ No compilation errors
✅ Full network synchronization
✅ Scope-based scaling complete
✅ VRM Spring Bone detection
✅ Attach-To-Me integration

⏳ Next: Create prefab templates in editor

## Key Features

- **Dynamic Loading**: Load GLB, glTF2, VRM from URLs
- **Instant Attachment**: Spawned orbs work with bone tracking immediately
- **Network Safe**: All players see consistent world state
- **Late-Joiner Support**: New players load previously-loaded models
- **Configurable Cooldowns**: Off/5s/10s/30s with visual feedback
- **Scope Modes**: World (unlimited scale) or Orb (fit within boundaries)
- **VRM Support**: Preserves spring bone parameters from loaded models
- **Lifecycle Management**: Cache expiration, respawn, cleanup

## Integration Points

- **Attach-To-Me**: Uses `AttachablesGlobalTracking._a_EnableTracking()`
- **VRChat API**: Networking, VRC_Pickup, Udon system
- **GLB Loader**: Delegates mesh instantiation to `thirdparty/vrchat-glb-loader` via `orb-link-integration/OrbGLBIntegration.cs`
- **Physics**: Works with rigidbody + VRM Spring Bones
