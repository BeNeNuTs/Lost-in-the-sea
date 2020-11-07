using UnityEngine;

public class BoatController : MonoBehaviour
{
    public Rigidbody m_Rigidbody;
    public float m_Speed;

    private MainCharacterController m_PlayerOnBoard;

    private void Awake()
    {
        enabled = false;
    }

    public void ProxyPlayer(MainCharacterController _mainCharacterController)
    {
        m_PlayerOnBoard = _mainCharacterController;

        m_PlayerOnBoard.transform.SetParent(transform);
        m_PlayerOnBoard.transform.localPosition = Vector3.zero;
        m_PlayerOnBoard.transform.localRotation = Quaternion.identity;

        enabled = true;
    }

    public void UnproxyPlayer()
    {
        m_PlayerOnBoard.transform.SetParent(null);
        m_PlayerOnBoard = null;
        enabled = false;
    }

    private void FixedUpdate()
    {
        Move();
        LookAt();
    }

    private void Move()
    {
        Vector3 desiredMoveDirection = m_PlayerOnBoard.GetDesiredMoveDirection();
        if (CanMoveTo(desiredMoveDirection))
        {
            m_Rigidbody.MovePosition(m_Rigidbody.position + desiredMoveDirection * m_Speed * Time.fixedDeltaTime);
        }
    }

    private bool CanMoveTo(Vector3 _desiredMoveDirection)
    {
        /*if (_desiredMoveDirection != Vector3.zero)
        {
            Vector3 normalizedMoveDirection = _desiredMoveDirection.normalized;

            Ray ray = new Ray(transform.position + (normalizedMoveDirection * m_FloorRaycastDistanceFromOwner) + (Vector3.up * (m_FloorRaycastLength / 2f)), Vector3.down);
            bool hasHitSomething = Physics.Raycast(ray, out RaycastHit hitInfo, m_FloorRaycastLength, m_FloorRaycastLayerMask, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(ray.origin, ray.origin + Vector3.down * m_FloorRaycastLength, hasHitSomething ? Color.green : Color.red);

            return hasHitSomething && !m_WaterLayerMask.Contains(hitInfo.transform.gameObject.layer);
        }*/

        return true;
    }

    private void LookAt()
    {
        /*if (m_GroundVelocity != Vector3.zero)
            transform.forward = m_GroundVelocity;*/
    }
}
