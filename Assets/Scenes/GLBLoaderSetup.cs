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

            // Add CanvasDragger script to make it grabbable
            CanvasDragger dragger = canvas.AddComponent<CanvasDragger>();

            // Add collider for interaction if not present
            if (canvas.GetComponent<Collider>() == null)
            {
                BoxCollider collider = canvas.AddComponent<BoxCollider>();
                // Set collider size based on canvas - adjust as needed
                RectTransform rectTransform = canvas.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    collider.size = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.1f);
                }
            }

            Debug.Log("[GLBLoaderSetup] Instantiated URL input canvas with dragger");
        }
    }
}
