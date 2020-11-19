using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MainCharacterController : MonoBehaviour
{
    [Header("Setup")]
    public CharacterController m_CharacterController;
    public Transform m_Camera;
    public CinemachineVirtualCameraBase m_MainCamera;
    public CinemachineVirtualCameraBase m_BoatCamera;
    public Animator m_Animator;
    public float m_Speed = 3f;

    [Header("Floor Raycast")]
    public float m_FloorRaycastDistanceFromOwner = 1f;
    public float m_FloorRaycastLength = 3f;
    public LayerMask m_WaterLayerMask;
    public LayerMask m_FloorRaycastLayerMask;

    private Vector2 m_LastInputMoveDirection;
    private Vector3 m_DesiredVelocity;
    private Vector3 m_GroundVelocity;

    private bool m_IsInBoat = false;
    private BoatController m_BoatController;

    public Action OnInteractCallback;

    private void Awake()
    {
        LockMouse();

#if UNITY_EDITOR
        InputAction backquoteAction = new InputAction(binding: "<Keyboard>/backquote");
        backquoteAction.performed += (callbackContext) => ToggleInputControlAutoSwitch();
        backquoteAction.Enable();
        PlayerContainer.Instance.m_PlayerInput.neverAutoSwitchControlSchemes = true;
#endif
    }

    private void Update()
    {
        Move();
        LookAt();

        UpdateSpeed(m_GroundVelocity.magnitude);
    }

    private void LockMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

#if UNITY_EDITOR
    private void ToggleInputControlAutoSwitch()
    {
        if(PlayerContainer.Instance.m_PlayerInput.currentControlScheme == "Gamepad")
        {
            PlayerContainer.Instance.m_PlayerInput.SwitchCurrentControlScheme("Keyboard&Mouse");
        }
        else
        {
            PlayerContainer.Instance.m_PlayerInput.SwitchCurrentControlScheme("Gamepad");
        }
    }
#endif

    private void Move()
    {
        Vector3 desiredMoveDirection = GetDesiredMoveDirection();
        m_DesiredVelocity.x = desiredMoveDirection.x;
        m_DesiredVelocity.z = desiredMoveDirection.z;

        if (CanMoveTo(desiredMoveDirection))
        {
            m_CharacterController.Move(m_DesiredVelocity * Time.deltaTime * m_Speed);
        }
        m_GroundVelocity = new Vector3(m_CharacterController.velocity.x, 0f, m_CharacterController.velocity.z);

        m_DesiredVelocity.y += Physics.gravity.y;
        if (m_CharacterController.isGrounded && m_DesiredVelocity.y < 0f)
            m_DesiredVelocity.y = 0f;
    }

    private bool CanMoveTo(Vector3 _desiredMoveDirection)
    {
        if (_desiredMoveDirection != Vector3.zero)
        {
            Vector3 normalizedMoveDirection = _desiredMoveDirection.normalized;

            Ray ray = new Ray(transform.position + (normalizedMoveDirection * m_FloorRaycastDistanceFromOwner) + (Vector3.up * (m_FloorRaycastLength / 2f)), Vector3.down);
            bool hasHitSomething = Physics.Raycast(ray, out RaycastHit hitInfo, m_FloorRaycastLength, m_FloorRaycastLayerMask, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(ray.origin, ray.origin + Vector3.down * m_FloorRaycastLength, hasHitSomething ? Color.green : Color.red);

            return hasHitSomething && !m_WaterLayerMask.Contains(hitInfo.transform.gameObject.layer);
        }

        return true;
    }

    private void LookAt()
    {
        if (m_GroundVelocity != Vector3.zero)
            transform.forward = m_GroundVelocity;
    }

    public void UpdateSpeed(float speed)
    {
        m_Animator.SetFloat("Speed", speed);
    }

    public Vector3 GetDesiredMoveDirection()
    {
        Vector3 forward = m_Camera.forward;
        Vector3 right = m_Camera.right;

        //project forward and right vectors on the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        //this is the direction in the world space we want to move:
        return forward * m_LastInputMoveDirection.y + right * m_LastInputMoveDirection.x;
    }

    public bool IsInBoat()
    {
        return m_IsInBoat;
    }

    public BoatController GetCurrentBoat()
    {
        return m_BoatController;
    }

    public void SetInBoat(BoatController _boatController)
    {
        m_IsInBoat = _boatController != null;
        enabled = !m_IsInBoat;
        m_CharacterController.enabled = !m_IsInBoat;
        if (m_IsInBoat)
        {
            m_MainCamera.Priority = 0;
            m_BoatCamera.Priority = 1;
            _boatController.ProxyPlayer(this);
        }
        else
        {
            m_MainCamera.Priority = 1;
            m_BoatCamera.Priority = 0;
            m_BoatController.UnproxyPlayer();
        }
        m_BoatController = _boatController;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        m_LastInputMoveDirection = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnInteractCallback?.Invoke();
    }
}
