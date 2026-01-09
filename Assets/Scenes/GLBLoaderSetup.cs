using UnityEngine;
using UdonSharp;

/// <summary>
/// Simple script to instantiate the GLB loader and URL input canvas as-is.
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class GLBLoaderSetup : UdonSharpBehaviour
{
    public GameObject glbLoaderPrefab;
    public GameObject urlInputCanvasPrefab;

    void Start()
    {
        // Instantiate GLB Loader
        if (glbLoaderPrefab != null)
        {
            GameObject loader = Instantiate(glbLoaderPrefab);
            loader.name = "GLBLoader";
            Debug.Log("[GLBLoaderSetup] Instantiated GLB loader");
        }

        // Instantiate URL Input Canvas
        if (urlInputCanvasPrefab != null)
        {
            GameObject canvas = Instantiate(urlInputCanvasPrefab);
            canvas.name = "URLInputCanvas";
            Debug.Log("[GLBLoaderSetup] Instantiated URL input canvas");
        }
    }
}