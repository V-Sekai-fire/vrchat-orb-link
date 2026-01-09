using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ButtonSetup : UdonSharpBehaviour
{
    [SerializeField] private MultiLoaderManager multiLoaderManager;
    [SerializeField] private SpawnLoaderButton spawnButton;
    [SerializeField] private ClearLoadersButton clearButton;
    
    public void Start()
    {
        // Wire up the spawn button
        if (spawnButton != null && multiLoaderManager != null)
        {
            spawnButton.SetProgramVariable("multiLoaderManager", multiLoaderManager);
        }
        
        // Wire up the clear button
        if (clearButton != null && multiLoaderManager != null)
        {
            clearButton.SetProgramVariable("multiLoaderManager", multiLoaderManager);
        }
    }
}
