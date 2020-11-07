using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MainCharacterController : MonoBehaviour
{
    public CharacterController m_CharacterController;
    public Transform m_Camera;
    public Animator m_Animator;
    public float m_Speed = 3f;

    private Vector2 m_LastInputMoveDirection;
    private Vector3 m_CurrentVelocity;
    private Vector3 m_GroundVelocity;

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
        if(Keyboard.current.escapeKey.isPressed)
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
        m_CurrentVelocity.x = desiredMoveDirection.x;
        m_CurrentVelocity.z = desiredMoveDirection.z;

        m_CharacterController.Move(m_CurrentVelocity * Time.deltaTime * m_Speed);
        m_GroundVelocity = new Vector3(m_CharacterController.velocity.x, 0f, m_CharacterController.velocity.z);

        m_CurrentVelocity.y += Physics.gravity.y;
        if (m_CharacterController.isGrounded && m_CurrentVelocity.y < 0f)
            m_CurrentVelocity.y = 0f;
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

    private Vector3 GetDesiredMoveDirection()
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

    public void OnMove(InputAction.CallbackContext context)
    {
        m_LastInputMoveDirection = context.ReadValue<Vector2>();
    }
}
