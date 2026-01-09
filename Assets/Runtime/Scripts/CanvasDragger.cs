using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class CanvasDragger : UdonSharpBehaviour
{
    private Transform canvasTransform;
    private bool isGrabbed = false;
    private Vector3 grabOffset;
    private VRCPlayerApi localPlayer;

    private void Start()
    {
        canvasTransform = GetComponent<RectTransform>() as Transform;
        localPlayer = Networking.LocalPlayer;
    }

    public override void Interact()
    {
        isGrabbed = !isGrabbed;
        
        if (isGrabbed && localPlayer.IsUserInVR())
        {
            // Store offset between canvas and hand position
            Vector3 handPos = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
            grabOffset = canvasTransform.position - handPos;
        }
    }

    private void Update()
    {
        if (!isGrabbed || !localPlayer.IsUserInVR()) return;

        // Get current hand position and move canvas with it
        Vector3 handPos = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
        canvasTransform.position = handPos + grabOffset;
    }
}
