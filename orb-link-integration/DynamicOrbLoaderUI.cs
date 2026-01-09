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
using UnityEngine.UI;
using TMPro;

namespace net.fushizen.attachable
{
    /// <summary>
    /// UI Panel for dynamic GLB/VRM model loading.
    /// Provides toggles for World/Orb scope, URL input, cooldown selection, and status display.
    /// </summary>
    public class DynamicOrbLoaderUI : UdonSharpBehaviour
    {
        [SerializeField] private DynamicOrbLoader loader;
        
        // UI Elements
        [SerializeField] private Toggle worldOrbToggle;
        [SerializeField] private TMP_InputField urlInputField;
        [SerializeField] private Button loadButton;
        [SerializeField] private TextMeshProUGUI statusText;
        
        // Cooldown toggle buttons
        [SerializeField] private Button[] cooldownButtons = new Button[4]; // Off, 5s, 10s, 30s
        [SerializeField] private Image[] cooldownButtonImages = new Image[4];
        [SerializeField] private Color activeButtonColor = Color.white;
        [SerializeField] private Color inactiveButtonColor = Color.gray;
        
        // Cooldown display
        [SerializeField] private Image[] cooldownNotches = new Image[3];
        [SerializeField] private Color filledNotchColor = Color.white;
        [SerializeField] private Color emptyNotchColor = Color.gray;
        
        private int selectedCooldown = 0; // 0=Off, 1=5s, 2=10s, 3=30s
        private bool isWorldScope = true;

        void Start()
        {
            if (loader == null)
            {
                loader = GetComponent<DynamicOrbLoader>();
            }

            // Setup UI callbacks
            if (worldOrbToggle != null)
            {
                worldOrbToggle.onValueChanged.AddListener(OnWorldOrbToggleChanged);
            }

            if (urlInputField != null)
            {
                urlInputField.onValueChanged.AddListener(OnUrlInputChanged);
            }

            if (loadButton != null)
            {
                loadButton.onClick.AddListener(OnLoadButtonClicked);
            }

            // Setup cooldown buttons
            for (int i = 0; i < cooldownButtons.Length; i++)
            {
                int index = i;
                if (cooldownButtons[i] != null)
                {
                    cooldownButtons[i].onClick.AddListener(() => OnCooldownButtonClicked(index));
                }
            }

            // Set initial state
            UpdateCooldownDisplay();
            UpdateStatusText();
        }

        void Update()
        {
            // Update cooldown notches every frame if cooldown is active
            if (loader != null && loader.IsCooldownActive())
            {
                UpdateCooldownDisplay();
                UpdateStatusText();
            }
        }

        private void OnWorldOrbToggleChanged(bool isOn)
        {
            isWorldScope = isOn;
            loader.SetWorldScope(isOn);
            UpdateStatusText();
        }

        private void OnUrlInputChanged(string newUrl)
        {
            if (loader != null)
            {
                loader.SetUrl(newUrl);
            }
        }

        private void OnLoadButtonClicked()
        {
            if (loader != null)
            {
                loader.LoadModel();
            }
        }

        private void OnCooldownButtonClicked(int optionIndex)
        {
            selectedCooldown = optionIndex;
            if (loader != null)
            {
                loader.SetCooldownOption(optionIndex);
            }
            UpdateCooldownDisplay();
        }

        private void UpdateCooldownDisplay()
        {
            // Update button colors
            for (int i = 0; i < cooldownButtonImages.Length; i++)
            {
                if (cooldownButtonImages[i] != null)
                {
                    cooldownButtonImages[i].color = (i == selectedCooldown) ? activeButtonColor : inactiveButtonColor;
                }
            }

            // Update notches based on cooldown progress
            if (loader != null && selectedCooldown > 0)
            {
                float progress = loader.GetCooldownProgress();
                int filledNotches = Mathf.RoundToInt(progress * 3f);

                for (int i = 0; i < cooldownNotches.Length; i++)
                {
                    if (cooldownNotches[i] != null)
                    {
                        cooldownNotches[i].color = (i < filledNotches) ? filledNotchColor : emptyNotchColor;
                    }
                }
            }
            else
            {
                // All notches empty if Off selected
                for (int i = 0; i < cooldownNotches.Length; i++)
                {
                    if (cooldownNotches[i] != null)
                    {
                        cooldownNotches[i].color = emptyNotchColor;
                    }
                }
            }
        }

        private void UpdateStatusText()
        {
            if (statusText == null) return;

            string scope = isWorldScope ? "World (no scale limit)" : "Orb (fit within boundary)";
            string status = "Ready";

            if (loader != null && loader.IsCooldownActive())
            {
                status = "Cooling down...";
            }

            statusText.text = $"[{scope}] {status}";
        }
    }
}
