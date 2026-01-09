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
using VRC.Udon.Common.Interfaces;

namespace net.fushizen.attachable
{
    /// <summary>
    /// Synchronizes dynamically-loaded orb model URLs across the network.
    /// Ensures all players load the same models for consistent world state.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DynamicOrbNetworkSync : UdonSharpBehaviour
    {
        [SerializeField] private DynamicOrbLoader orbLoader;
        [SerializeField] private int maxUrlsToSync = 50;
        
        // Synced variables
        [UdonSynced] public string[] loadedModelUrls = new string[0];
        [UdonSynced] public int urlCount = 0;

        void Start()
        {
            if (orbLoader == null)
            {
                orbLoader = GetComponent<DynamicOrbLoader>();
            }
        }

        /// <summary>
        /// Broadcast a newly loaded model URL to all players
        /// </summary>
        public void BroadcastLoadedUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            
            // Only the world master or owner should broadcast
            if (!Networking.LocalPlayer.isMaster && !Networking.LocalPlayer.IsOwner(gameObject))
            {
                return;
            }

            // Add URL to the list if not already present
            bool alreadyExists = false;
            for (int i = 0; i < urlCount; i++)
            {
                if (loadedModelUrls[i] == url)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists && urlCount < maxUrlsToSync)
            {
                System.Array.Resize(ref loadedModelUrls, urlCount + 1);
                loadedModelUrls[urlCount] = url;
                urlCount++;
                RequestSerialization();
            }
        }

        /// <summary>
        /// Called when serialized data is received - late joiners load previously-loaded models
        /// </summary>
        public override void OnDeserialization()
        {
            // Reload all models from the synced URL list
            for (int i = 0; i < urlCount; i++)
            {
                if (!string.IsNullOrEmpty(loadedModelUrls[i]))
                {
                    orbLoader.SetUrl(loadedModelUrls[i]);
                    orbLoader.LoadModel();
                }
            }
        }

        /// <summary>
        /// Clear all synced URLs (typically called on world reset)
        /// </summary>
        public void ClearAllUrls()
        {
            if (!Networking.LocalPlayer.isMaster)
            {
                return;
            }

            loadedModelUrls = new string[0];
            urlCount = 0;
            RequestSerialization();
        }
    }
}
