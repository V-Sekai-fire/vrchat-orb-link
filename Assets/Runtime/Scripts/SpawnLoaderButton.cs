using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SpawnLoaderButton : UdonSharpBehaviour
{
    [SerializeField] private MultiLoaderManager multiLoaderManager;
    private Button button;
    
    public void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnSpawnClick);
        }
    }
    
    public void OnSpawnClick()
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
