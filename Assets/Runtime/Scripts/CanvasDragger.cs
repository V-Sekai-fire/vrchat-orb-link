using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;

public class CanvasDragger : UdonSharpBehaviour
{
    private Transform canvasTransform;
    private Rigidbody rb;
    private bool isGrabbed = false;
    private Vector3 grabOffset;
    private Quaternion grabRotationOffset;
    private VRCPlayerApi localPlayer;

    private void Start()
    {
        canvasTransform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        localPlayer = Networking.LocalPlayer;
    }

    public override void OnPickup()
    {
        isGrabbed = true;
        
        if (localPlayer != null && localPlayer.IsUserInVR())
        {
            // Store offset between canvas and hand
            Vector3 handPos = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
            grabOffset = canvasTransform.position - handPos;
            grabRotationOffset = Quaternion.Inverse(localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).rotation) * canvasTransform.rotation;
        }
    }

    public override void OnDrop()
    {
        isGrabbed = false;
    }

    private void Update()
    {
        if (!isGrabbed || localPlayer == null) return;

        if (localPlayer.IsUserInVR())
        {
            // Update canvas position to follow hand
            Vector3 handPos = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
            canvasTransform.position = handPos + grabOffset;
            
            // Update rotation
            Quaternion handRot = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).rotation;
            canvasTransform.rotation = handRot * grabRotationOffset;
        }
    }
}
