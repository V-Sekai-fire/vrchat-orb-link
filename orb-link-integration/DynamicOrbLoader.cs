/*
 * Copyright (c) 2025-present K. S. Ernest (iFire) Lee
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace net.fushizen.attachable
{
    /// <summary>
    /// Handles dynamic loading of GLB/glTF2 and VRM models into attachable orbs.
    /// Uses a single shared VRChat GLB Loader instance for efficiency.
    /// Integrates with Attach-To-Me system.
    /// </summary>
    public class DynamicOrbLoader : UdonSharpBehaviour
    {
        [SerializeField] private GameObject orbPrefab;
        [SerializeField] private Transform orbParent;
        [SerializeField] private net.fushizen.attachable.OrbGLBIntegration glbIntegration;
        [SerializeField] private AttachableRegistration attachableRegistration;
        
        private string currentUrl = "";
        private float cooldownEndTime = 0f;
        private int selectedCooldownOption = 0; // 0=Off, 1=5s, 2=10s, 3=30s
        private bool isWorldScope = true;
        private GameObject selectedOrb = null;
        
        // Cache for loaded models
        private class CacheEntry
        {
            public GameObject model;
            public float timestamp;
        }
        private VRC.SDK3.Data.DataDictionary modelCache = new VRC.SDK3.Data.DataDictionary();
        
        void Start()
        {
            if (orbParent == null)
            {
                orbParent = transform;
            }
            
            if (attachableRegistration == null)
            {
                attachableRegistration = GetComponent<AttachableRegistration>();
            }

            // Ensure single GLB loader instance
            if (glbLoader == null)
            {
                GameObject loaderObj = GameObject.Find("GLBLoader");
                if (loaderObj == null && glbLoaderPrefab != null)
                {
                    loaderObj = Instantiate(glbLoaderPrefab);
                    loaderObj.name = "GLBLoader";
                    Debug.Log("[DynamicOrbLoader] Instantiated shared GLB loader");
                }
                if (loaderObj != null)
                {
                    glbLoader = loaderObj.GetComponent<UdonSharpBehaviour>();
                }
            }
        }

        public void SetUrl(string url)
        {
            currentUrl = url;
        }

        public void SetWorldScope(bool isWorld)
        {
            isWorldScope = isWorld;
            selectedOrb = null;
        }

        public void SetOrbScope(bool isOrb)
        {
            isWorldScope = !isOrb;
        }

        public void SelectOrb(GameObject orb)
        {
            if (!isWorldScope)
            {
                selectedOrb = orb;
            }
        }

        public void SetCooldownOption(int option)
        {
            // option: 0=Off, 1=5s, 2=10s, 3=30s
            selectedCooldownOption = Mathf.Clamp(option, 0, 3);
        }

        public void LoadModel()
        {
            // Check cooldown
            if (selectedCooldownOption > 0 && Time.time < cooldownEndTime)
            {
                Debug.LogWarning("[DynamicOrbLoader] Load is on cooldown. Please wait.");
                return;
            }

            if (string.IsNullOrEmpty(currentUrl))
            {
                Debug.LogError("[DynamicOrbLoader] URL is empty");
                return;
            }

            // Check cache first
            if (modelCache.ContainsKey(currentUrl))
            {
                var cached = modelCache[currentUrl];
                if (cached is GameObject)
                {
                    SpawnOrb((GameObject)cached);
                }
            }
            else if (glbIntegration != null)
            {
                // Delegate to our GLB integration
                glbIntegration.LoadModel(currentUrl);
            }
            else
            {
                Debug.LogError("[DynamicOrbLoader] GLB Integration not assigned");
            }

            // Set cooldown
            if (selectedCooldownOption > 0)
            {
                float cooldownDuration = selectedCooldownOption switch
                {
                    1 => 5f,
                    2 => 10f,
                    3 => 30f,
                    _ => 0f
                };
                cooldownEndTime = Time.time + cooldownDuration;
            }
        }

        private void SpawnOrb(GameObject model)
        {
            if (orbPrefab == null)
            {
                Debug.LogError("[DynamicOrbLoader] Orb prefab not assigned");
                return;
            }

            // Instantiate new orb
            GameObject newOrb = Instantiate(orbPrefab, orbParent);
            newOrb.name = $"DynamicOrb_{System.DateTime.Now.Ticks}";
            
            // Get DynamicOrb component
            DynamicOrb dynamicOrb = newOrb.GetComponent<DynamicOrb>();
            if (dynamicOrb != null)
            {
                // Initialize with scope setting (true = world scope, false = orb scope)
                dynamicOrb.Initialize(currentUrl, model, isWorldScope);
            }

            // Make it attachable by applying the Attachable setup
            ApplyAttachableSetup(newOrb);
            
            // Register with the global tracking system
            if (attachableRegistration != null)
            {
                attachableRegistration.RegisterOrb(newOrb);
            }
            
            string scopeType = isWorldScope ? "World" : "Orb";
            Debug.Log($"[DynamicOrbLoader] Spawned new {scopeType}-scope orb: {newOrb.name}");
        }

        private void ApplyAttachableSetup(GameObject orb)
        {
            // Get or create the Attachable component
            Attachable attachable = orb.GetComponent<Attachable>();
            if (attachable == null)
            {
                attachable = orb.AddComponent<Attachable>();
            }

            // Get or ensure Rigidbody
            Rigidbody rb = orb.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = orb.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            // Ensure Collider exists
            Collider collider = orb.GetComponent<Collider>();
            if (collider == null)
            {
                SphereCollider sphere = orb.AddComponent<SphereCollider>();
                sphere.radius = 0.5f;
            }

            // Create attachment direction marker if it doesn't exist
            Transform directionMarker = orb.transform.Find("AttachmentDirection");
            if (directionMarker == null)
            {
                GameObject dirMarkerObj = new GameObject("AttachmentDirection");
                dirMarkerObj.transform.SetParent(orb.transform);
                dirMarkerObj.transform.localPosition = Vector3.zero;
                dirMarkerObj.transform.localRotation = Quaternion.identity;
                directionMarker = dirMarkerObj.transform;
            }

            // Configure Attachable component
            attachable.pickup = orb.GetComponent<VRC_Pickup>();
            if (attachable.pickup == null)
            {
                attachable.pickup = orb.AddComponent<VRC_Pickup>();
            }
            
            attachable.attachmentDirection = directionMarker;
            
            Debug.Log($"[DynamicOrbLoader] Applied Attachable setup to {orb.name}");
        }

        public void OnGLBLoadComplete(GameObject loadedModel)
        {
            if (loadedModel != null)
            {
                SpawnOrb(loadedModel);
            }
        }

        public float GetCooldownProgress()
        {
            if (selectedCooldownOption == 0) return 1f; // Off
            
            float remaining = Mathf.Max(0f, cooldownEndTime - Time.time);
            float total = selectedCooldownOption switch
            {
                1 => 5f,
                2 => 10f,
                3 => 30f,
                _ => 1f
            };
            
            return 1f - (remaining / total);
        }

        public bool IsCooldownActive()
        {
            return selectedCooldownOption > 0 && Time.time < cooldownEndTime;
        }
    }
}
