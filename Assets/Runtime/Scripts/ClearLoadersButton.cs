using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ClearLoadersButton : UdonSharpBehaviour
{
    [SerializeField] private MultiLoaderManager multiLoaderManager;
    
    public override void Interact()
    {
        if (multiLoaderManager == null)
        {
            Debug.LogError("MultiLoaderManager not assigned to ClearLoadersButton");
            return;
        }
        
        multiLoaderManager.ClearAllLoaders();
    }
}
