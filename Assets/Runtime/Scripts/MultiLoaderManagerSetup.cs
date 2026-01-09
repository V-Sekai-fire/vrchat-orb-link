using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class MultiLoaderManagerSetup : UdonSharpBehaviour
{
    [SerializeField] private MultiLoaderManager multiLoaderManager;
    [SerializeField] private GameObject loaderPrefab;
    [SerializeField] private Transform loaderContainer;
    
    public void Start()
    {
        // Set references on MultiLoaderManager
        if (multiLoaderManager != null)
        {
            multiLoaderManager.SetProgramVariable("loaderPrefab", loaderPrefab);
            multiLoaderManager.SetProgramVariable("loaderContainer", loaderContainer);
        }
    }
}
