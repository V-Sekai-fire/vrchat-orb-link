using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SpawnLoaderButton : UdonSharpBehaviour
{
    [SerializeField] private MultiLoaderManager multiLoaderManager;
    
    public override void Interact()
    {
        if (multiLoaderManager == null)
        {
            Debug.LogError("MultiLoaderManager not assigned to SpawnLoaderButton");
            return;
        }
        
        // Spawn the loader with the default URL
        multiLoaderManager.SpawnLoader(new VRCUrl("https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/main/2.0/Duck/glTF-Binary/Duck.glb"));
    }
}
