using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VoyageVoyage;

namespace net.fushizen.attachable
{
    /// <summary>
    /// Integration wrapper for VRChat GLB Loader to load models for the orb system.
    /// Extracts the loaded model and notifies the callback when loading is complete.
    /// </summary>
    public class OrbGLBIntegration : UdonSharpBehaviour
    {
        [SerializeField] private VoyageVoyage.GLBLoader glbLoader;
        [SerializeField] private UdonSharpBehaviour callback; // Our DynamicOrbLoader

        private string currentUrl;

        void Start()
        {
            if (glbLoader != null)
            {
                // Add ourselves to the state receivers so we get notified of loading events
                var receivers = new UdonSharpBehaviour[glbLoader.stateReceivers.Length + 1];
                glbLoader.stateReceivers.CopyTo(receivers, 0);
                receivers[receivers.Length - 1] = this;
                glbLoader.stateReceivers = receivers;
            }
        }

        public void LoadModel(string url)
        {
            if (glbLoader == null)
            {
                Debug.LogError("[OrbGLBIntegration] GLB Loader not assigned");
                return;
            }

            currentUrl = url;
            VRCUrl vrcUrl = new VRCUrl(url);
            glbLoader.userURL = vrcUrl;
            glbLoader.UserURLUpdated();

            Debug.Log($"[OrbGLBIntegration] Initiated loading from: {url}");
        }

        // Called by GLBLoader when scene is loaded
        public void SceneLoaded()
        {
            if (glbLoader == null || callback == null) return;

            // Get the loaded model - assume it's the last child of scenesInfoRoot
            Transform scenesRoot = glbLoader.scenesInfoRoot;
            if (scenesRoot != null && scenesRoot.childCount > 0)
            {
                GameObject loadedModel = scenesRoot.GetChild(scenesRoot.childCount - 1).gameObject;
                // Set the loaded model on the callback and trigger the event
                callback.SetProgramVariable("loadedModel", loadedModel);
                callback.SendCustomEvent("OnGLBLoadComplete");
                Debug.Log($"[OrbGLBIntegration] Loaded model from {currentUrl}: {loadedModel.name}");
            }
            else
            {
                Debug.LogError("[OrbGLBIntegration] No loaded model found in scenes root");
            }
        }

        // Handle other states if needed
        public void SceneLoading()
        {
            // Optional: handle loading start
        }

        public void ParseError()
        {
            Debug.LogError($"[OrbGLBIntegration] Parse error for {currentUrl}");
        }

        public void SceneCleared()
        {
            // Optional: handle clear
        }
    }
}