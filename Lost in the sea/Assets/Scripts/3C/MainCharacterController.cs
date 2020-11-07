using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MainCharacterController : MonoBehaviour
{
    [Header("Setup")]
    public CharacterController m_CharacterController;
    public Transform m_Camera;
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
        ToggleMouseLockState();
    }

    private void Update()
    {
        Move();
        LookAt();

        UpdateSpeed();

#if UNITY_EDITOR
        if (Keyboard.current.escapeKey.isPressed)
        {
            ToggleMouseLockState();
        }
#endif
    }

    private void ToggleMouseLockState()
    {
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

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

    private void UpdateSpeed()
    {
        m_Animator.SetFloat("Speed", m_GroundVelocity.magnitude);
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
            _boatController.ProxyPlayer(this);
        }
        else
        {
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
