using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MainCharacterController : MonoBehaviour
{
    public CharacterController m_CharacterController;
    public Transform m_Camera;
    public Animator m_Animator;
    public float m_Speed = 3f;

    private Vector3 m_LastInputMoveDirection;
    private Vector3 m_CurrentVelocity;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Move();
        LookAt();

        UpdateSpeed();
    }

    private void Move()
    {
        Vector3 desiredMoveDirection = GetDesiredMoveDirection();
        m_CurrentVelocity.x = desiredMoveDirection.x;
        m_CurrentVelocity.z = desiredMoveDirection.z;

        m_CharacterController.Move(m_CurrentVelocity * Time.deltaTime * m_Speed);

        m_CurrentVelocity.y += Physics.gravity.y;
        if (m_CharacterController.isGrounded && m_CurrentVelocity.y < 0f)
            m_CurrentVelocity.y = 0f;
    }

    private void LookAt()
    {
        if (m_CurrentVelocity.x != 0f || m_CurrentVelocity.z != 0f)
            transform.forward = new Vector3(m_CurrentVelocity.x, 0f, m_CurrentVelocity.z);
    }

    private void UpdateSpeed()
    {
        Vector3 groundVelocity = new Vector3(m_CharacterController.velocity.x, 0f, m_CharacterController.velocity.z);
        m_Animator.SetFloat("Speed", groundVelocity.magnitude);
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
        return forward * m_LastInputMoveDirection.z + right * m_LastInputMoveDirection.x;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputMove = context.ReadValue<Vector2>();
        m_LastInputMoveDirection = new Vector3(inputMove.x, 0f, inputMove.y);
    }
}
