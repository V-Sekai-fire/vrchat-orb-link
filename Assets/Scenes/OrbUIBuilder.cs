using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UdonSharp;
using VRC.SDKBase;

/// <summary>
/// Simple GLB loader with URL input.
/// Attach to a GameObject and assign the UI elements in the inspector.
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class OrbUIBuilder : UdonSharpBehaviour
{
    public TMP_InputField urlInputField;
    public Button loadButton;
    public TextMeshProUGUI statusText;
    public UdonSharpBehaviour glbLoader;
    public GameObject glbLoaderPrefab;

    void Start()
    {
        // Ensure GLB Loader is instantiated
        GameObject loaderGO = GameObject.Find("GLBLoader");
        if (loaderGO == null && glbLoaderPrefab != null)
        {
            loaderGO = Instantiate(glbLoaderPrefab);
            loaderGO.name = "GLBLoader";
            Debug.Log("[OrbUIBuilder] Instantiated GLB loader in Start");
        }
        if (loaderGO != null)
        {
            glbLoader = loaderGO.GetComponent<UdonSharpBehaviour>();
        }

        // Setup UI if assigned
        if (loadButton != null)
        {
            loadButton.onClick.AddListener(LoadModel);
        }
    }

    public void LoadModel()
    {
        if (glbLoader == null)
        {
            GameObject loaderGO = GameObject.Find("GLBLoader");
            if (loaderGO != null)
            {
                glbLoader = loaderGO.GetComponent<UdonSharpBehaviour>();
            }
        }

        if (glbLoader != null && urlInputField != null)
        {
            VRCUrl url = new VRCUrl(urlInputField.text);
            glbLoader.SetProgramVariable("userURL", url);
            glbLoader.SendCustomEvent("UserURLUpdated");
            if (statusText != null)
            {
                statusText.text = "Loading...";
                statusText.color = Color.yellow;
            }
            Debug.Log($"[OrbUIBuilder] Loading model from: {url}");
        }
        else
        {
            Debug.LogError("[OrbUIBuilder] GLBLoader or URL input not assigned!");
            if (statusText != null)
            {
                statusText.text = "Error: Loader not found";
                statusText.color = Color.red;
            }
        }
    }
}
