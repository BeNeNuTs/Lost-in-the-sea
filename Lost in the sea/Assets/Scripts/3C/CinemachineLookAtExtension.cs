using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CinemachineLookAtExtension : CinemachineExtension
{
    public CinemachineFreeLook m_CinemachineFreeLook;

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 lookMovement = context.ReadValue<Vector2>().normalized;
        m_CinemachineFreeLook.m_XAxis.m_InputAxisValue = lookMovement.x;
        m_CinemachineFreeLook.m_YAxis.m_InputAxisValue = lookMovement.y;
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {}
}
