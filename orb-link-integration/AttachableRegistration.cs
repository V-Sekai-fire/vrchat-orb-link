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
    /// Registers dynamically-created orbs with the global tracking system.
    /// Ensures they participate in bone tracking and attachment system.
    /// </summary>
    public class AttachableRegistration : UdonSharpBehaviour
    {
        [SerializeField] private AttachablesGlobalTracking globalTracking;

        void Start()
        {
            // Find AttachablesGlobalTracking if not assigned
            if (globalTracking == null)
            {
                globalTracking = FindObjectOfType<AttachablesGlobalTracking>();
            }
        }

        public void RegisterOrb(GameObject orb)
        {
            if (globalTracking == null)
            {
                Debug.LogWarning("[AttachableRegistration] AttachablesGlobalTracking not found");
                return;
            }

            // Get the Attachable component
            Attachable attachable = orb.GetComponent<Attachable>();
            if (attachable == null)
            {
                Debug.LogError($"[AttachableRegistration] Orb {orb.name} has no Attachable component");
                return;
            }

            // Register the attachable with the global tracking system
            globalTracking._a_EnableTracking(attachable);
            
            Debug.Log($"[AttachableRegistration] Registered orb {orb.name} with global tracking");
        }
    }
}
