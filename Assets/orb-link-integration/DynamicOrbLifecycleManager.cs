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
    /// Manages lifecycle, respawning, and cleanup of dynamically-loaded orbs.
    /// Handles model cache expiration and memory management.
    /// </summary>
    public class DynamicOrbLifecycleManager : UdonSharpBehaviour
    {
        [SerializeField] private Transform orbParent;
        [SerializeField] private float cacheExpirationTime = 3600f; // 1 hour
        [SerializeField] private float orbRespawnTime = 300f; // 5 minutes of inactivity
        
        private float lastCleanupTime = 0f;
        private float cleanupInterval = 60f; // Check every minute

        void Start()
        {
            if (orbParent == null)
            {
                orbParent = transform;
            }
            lastCleanupTime = Time.time;
        }

        void Update()
        {
            // Periodically clean up cache and inactive orbs
            if (Time.time - lastCleanupTime > cleanupInterval)
            {
                PerformMaintenance();
                lastCleanupTime = Time.time;
            }
        }

        private void PerformMaintenance()
        {
            // Find all DynamicOrb components and check their status
            DynamicOrb[] orbs = orbParent.GetComponentsInChildren<DynamicOrb>();
            
            foreach (DynamicOrb orb in orbs)
            {
                Attachable attachable = orb.GetComponent<Attachable>();
                if (attachable == null) continue;

                // Check if orb has been inactive (not tracking and not held)
                bool isTracking = IsOrbTracking(attachable);
                bool isHeld = attachable.IsPickedUp;

                if (!isTracking && !isHeld)
                {
                    // Could implement respawn or cleanup here based on settings
                    // For now, just log the state
                }
            }
        }

        private bool IsOrbTracking(Attachable attachable)
        {
            // This is a placeholder - actual tracking state would need to be queried
            // from the AttachablesGlobalTracking system
            return false;
        }

        /// <summary>
        /// Clears all dynamically-loaded orbs from the world
        /// </summary>
        public void ClearAllDynamicOrbs()
        {
            DynamicOrb[] orbs = orbParent.GetComponentsInChildren<DynamicOrb>();
            
            foreach (DynamicOrb orb in orbs)
            {
                Destroy(orb.gameObject);
            }
            
            Debug.Log("[DynamicOrbLifecycleManager] Cleared all dynamic orbs");
        }

        /// <summary>
        /// Sets the respawn time (0 = disabled)
        /// </summary>
        public void SetRespawnTime(float seconds)
        {
            orbRespawnTime = seconds;
        }

        /// <summary>
        /// Sets the cache expiration time
        /// </summary>
        public void SetCacheExpiration(float seconds)
        {
            cacheExpirationTime = seconds;
        }
    }
}
