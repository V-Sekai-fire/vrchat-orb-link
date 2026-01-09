using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MultiLoaderManager : UdonSharpBehaviour
{
    [SerializeField] private GameObject loaderPrefab;
    [SerializeField] private Transform loaderContainer;
    
    [UdonSynced] public int activeLoaderCount = 0;
    [UdonSynced] public VRCUrl[] loaderURLs = new VRCUrl[10];
    
    private GameObject[] loaderInstances = new GameObject[10];
    private const float LOADER_SPACING = 3f;
    private const int MAX_LOADERS = 10;
    
    public void SpawnLoader(VRCUrl url)
    {
        if (activeLoaderCount >= MAX_LOADERS)
        {
            Debug.LogWarning("Maximum loader limit reached (10 loaders)");
            return;
        }
        
        if (!Networking.IsOwner(gameObject))
        {
            Debug.LogWarning("Only the owner can spawn loaders");
            return;
        }
        
        // Calculate spawn position (grid pattern)
        int index = activeLoaderCount;
        Vector3 spawnPos = new Vector3(index * LOADER_SPACING, 0f, 0f);
        
        // Instantiate loader
        GameObject newLoader = Instantiate(loaderPrefab);
        newLoader.transform.SetParent(loaderContainer);
        newLoader.transform.localPosition = spawnPos;
        newLoader.transform.localRotation = Quaternion.identity;
        
        // Store reference
        loaderInstances[index] = newLoader;
        loaderURLs[index] = url;
        activeLoaderCount++;
        
        // Get the GLBLoader component and set URL
        VoyageVoyage.LoaderProxy loaderProxy = newLoader.GetComponent<VoyageVoyage.LoaderProxy>();
        if (loaderProxy != null)
        {
            loaderProxy.userURL = url;
            loaderProxy.SendCustomEvent("UserURLUpdated");
        }
        
        // Sync state to other players
        RequestSerialization();
    }
    
    public void ClearAllLoaders()
    {
        if (!Networking.IsOwner(gameObject))
        {
            Debug.LogWarning("Only the owner can clear loaders");
            return;
        }
        
        // Destroy all loader instances
        for (int i = 0; i < activeLoaderCount; i++)
        {
            if (loaderInstances[i] != null)
            {
                Destroy(loaderInstances[i]);
            }
        }
        
        // Reset state
        activeLoaderCount = 0;
        for (int i = 0; i < loaderURLs.Length; i++)
        {
            loaderURLs[i] = new VRCUrl("");
        }
        
        RequestSerialization();
    }
    
    public override void OnDeserialization()
    {
        // Called when network state is synced from owner
        // Update UI to reflect active loader count
        SendCustomEventDelayedSeconds("UpdateLoaderCountDisplay", 0.1f);
    }
    
    public void UpdateLoaderCountDisplay()
    {
        // This event can be received by UI to update count display
    }
}
