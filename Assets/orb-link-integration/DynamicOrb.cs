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

namespace net.fushizen.attachable
{
    /// <summary>
    /// Wrapper component for dynamically-loaded GLB models.
    /// Handles initialization, scaling based on scope, and VRM Spring Bone detection/preservation.
    /// </summary>
    public class DynamicOrb : UdonSharpBehaviour
    {
        [SerializeField] private string modelUrl;
        [SerializeField] private GameObject loadedModel;
        [SerializeField] private float orbBoundaryRadius = 0.5f;
        
        private bool isInitialized = false;
        private bool isWorldScope = true;

        public void Initialize(string url, GameObject model, bool worldScope = true)
        {
            modelUrl = url;
            loadedModel = model;
            isWorldScope = worldScope;
            
            // Parent the loaded model to this orb
            if (loadedModel != null)
            {
                loadedModel.transform.SetParent(transform);
                loadedModel.transform.localPosition = Vector3.zero;
                loadedModel.transform.localRotation = Quaternion.identity;
                
                // Scale model based on scope
                if (!isWorldScope)
                {
                    FitModelToOrbBoundary();
                }
                // World scope: keep original scale (do nothing)
            }
            
            // Detect and preserve VRM Spring Bones
            DetectAndPreserveVRMSpringBones();
            
            isInitialized = true;
        }
        
        private void FitModelToOrbBoundary()
        {
            if (loadedModel == null) return;
            
            // Get the bounds of the loaded model
            Renderer[] renderers = loadedModel.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return;
            
            Bounds totalBounds = renderers[0].bounds;
            foreach (Renderer renderer in renderers)
            {
                totalBounds.Encapsulate(renderer.bounds);
            }
            
            // Calculate the maximum dimension of the model
            Vector3 size = totalBounds.size;
            float maxDimension = Mathf.Max(size.x, size.y, size.z);
            
            if (maxDimension > 0)
            {
                // Scale to fit within orb boundary (with some padding)
                float maxAllowedSize = orbBoundaryRadius * 1.8f;
                float scaleFactor = maxAllowedSize / maxDimension;
                loadedModel.transform.localScale *= scaleFactor;
                
                Debug.Log($"[DynamicOrb] Scaled model to fit within orb boundary. Scale factor: {scaleFactor:F3}");
            }
        }
        
        public void SetOrbBoundaryRadius(float radius)
        {
            orbBoundaryRadius = radius;
        }

        private void DetectAndPreserveVRMSpringBones()
        {
            if (loadedModel == null) return;
            
            // Find all VRMSpringBone components in the loaded model
            // We only detect them - preservation is automatic since we don't modify them
            var springBones = loadedModel.GetComponentsInChildren(System.Type.GetType("VRM.VRMSpringBone"));
            
            if (springBones != null && springBones.Length > 0)
            {
                Debug.Log($"[DynamicOrb] Found {springBones.Length} VRM Spring Bone components - preserving existing parameters");
            }
        }

        public string GetModelUrl()
        {
            return modelUrl;
        }

        public bool IsInitialized()
        {
            return isInitialized;
        }
    }
}
