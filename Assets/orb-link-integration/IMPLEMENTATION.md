# Implementation Reference

## Scripts (6 files, ~830 LOC)

### DynamicOrb.cs

Wrapper for loaded models. Handles:

- VRM Spring Bone detection
- Scope-based scaling (World vs Orb mode)
- Model URL storage

**Key Methods**:

- `Initialize(url, model, worldScope)` - Setup with scope
- `FitModelToOrbBoundary()` - Auto-scale to orb size
- `SetOrbBoundaryRadius(radius)` - Configure boundary

### DynamicOrbLoader.cs

Main loader component. Handles:

- URL input and validation
- Model caching
- Cooldown management (Off/5s/10s/30s)
- Orb instantiation
- Attachable setup

**Key Methods**:

- `LoadModel()` - Trigger load with cooldown check
- `SetUrl(url)` - Set model URL
- `SetWorldScope(bool)` / `SetOrbScope(bool)` - Toggle scope
- `GetCooldownProgress()` - Return 0-1 cooldown state
- `IsCooldownActive()` - Check if cooling down

### DynamicOrbLoaderUI.cs

Canvas UI panel. Provides:

- World/Orb toggle
- URL input field
- Load button
- Cooldown buttons (Off/5s/10s/30s)
- Three-notch progress display
- Status text with scope info

**Key Methods**:

- `UpdateCooldownDisplay()` - Update notch visuals
- `UpdateStatusText()` - Update status label

### DynamicOrbNetworkSync.cs

Network synchronization. Handles:

- Broadcasting loaded URLs
- Late-joiner model loading
- World state management (max 50 URLs)

**Key Methods**:

- `BroadcastLoadedUrl(url)` - Send URL to all players
- `OnDeserialization()` - Auto-load synced models
- `ClearAllUrls()` - Reset URL list

### DynamicOrbLifecycleManager.cs

Lifecycle and cleanup. Manages:

- Cache expiration
- Inactive orb respawning
- Periodic maintenance
- Bulk orb clearing

**Key Methods**:

- `ClearAllDynamicOrbs()` - Remove all dynamic orbs
- `SetRespawnTime(seconds)` - Configure respawn
- `SetCacheExpiration(seconds)` - Configure cache

### AttachableRegistration.cs

Registers orbs with tracking system.

**Key Methods**:

- `RegisterOrb(orb)` - Register with AttachablesGlobalTracking

## File Locations

```
Assets/bd_/AttachToMe/Scripts/
├── DynamicOrb.cs
├── DynamicOrbLoader.cs
├── DynamicOrbLoaderUI.cs
├── DynamicOrbNetworkSync.cs
├── DynamicOrbLifecycleManager.cs
├── AttachableRegistration.cs

thirdparty/
└── vrchat-glb-loader/
    └── Runtime/Scripts/GLBLoader.cs (mesh loading)
```

## Dependencies

- VRCSDK3-Worlds
- UdonSharp 0.20.1+
- Attach-To-Me plugin
- VRChat GLB Loader (thirdparty/)

## Integration

**AttachablesGlobalTracking**:

- Orbs register via `_a_EnableTracking(attachable)`
- Bone tracking happens automatically
- Pickup interaction enabled via VRC_Pickup

**VRChat API**:

- `Networking.LocalPlayer` - Player queries
- `Networking.SetOwner()` - Ownership
- `RequestSerialization()` - Network sync

**GLB Loader**:

- Called via `glbLoaderProxy.SendCustomEvent("UserURLUpdated")`
- Handles mesh instantiation and materials
- Returns loaded GameObject via callback

## Scope-Based Scaling

**World Scope** (`isWorldScope = true`):

- Model loads at original scale
- No constraints applied
- Full-size models supported

**Orb Scope** (`isWorldScope = false`):

- Auto-scales to fit boundaries
- Uses `orbBoundaryRadius * 1.8f` as max size
- Calculates scale factor from model bounds
- Applied during `FitModelToOrbBoundary()`

## Network Synchronization

- **Sync Mode**: Manual (RequestSerialization)
- **Synced Data**: URL array + count
- **Max URLs**: 50 (configurable)
- **Master Only**: URL broadcast restricted to world master
- **Late-Joiners**: Auto-load on deserialization

## UdonSharp Compatibility

All scripts:

- Use UdonSharp syntax
- Compatible with VRChat Udon VM
- No external C# libraries
- Proper error handling and logging

## Performance Notes

- No per-frame overhead when idle
- Caching prevents re-downloads
- Cooldowns reduce spam loading
- Spring bone physics adds CPU cost
- Network sync is manual (efficient)

See [AGENTS.md](AGENTS.md) for architecture.
See [QUICKSTART.md](QUICKSTART.md) for setup guide.
