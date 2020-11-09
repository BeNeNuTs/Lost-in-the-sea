using UnityEngine;

public class BoatController : MonoBehaviour
{
    public Rigidbody m_Rigidbody;
    public BuoyantObject m_BuoyantObject;
    public float m_Speed;
    public AnimationCurve m_VelocityDragOverTime;
    public float m_MinLookAtAngleToMove = 60f;

    public float m_AngularSpeed;
    public AnimationCurve m_AngularDragOverTime;

    //[Header("Floor Raycast")]
    //public float m_FloorRaycastDistanceFromOwner = 1f;
    //public float m_FloorRaycastLength = 3f;
    //public LayerMask m_WaterLayerMask;
    //public LayerMask m_FloorRaycastLayerMask;

    private MainCharacterController m_PlayerOnBoard;
    private float m_InitialVelocityDrag;
    private float m_InitialAngularDrag;


    private Vector3 m_DesiredMoveDirection;
    private float m_TimeToMove = 0f;

    private float m_LastLookAtAngle = 0f;
    private float m_TimeToLookAt = 0f;

    private void Awake()
    {
        enabled = false;
        m_InitialVelocityDrag = m_BuoyantObject.velocityDrag;
        m_InitialAngularDrag = m_BuoyantObject.angularDrag;
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
        m_BuoyantObject.velocityDrag = m_InitialVelocityDrag;
        m_BuoyantObject.angularDrag = m_InitialAngularDrag;
    }

    private void FixedUpdate()
    {
        m_DesiredMoveDirection = m_PlayerOnBoard.GetDesiredMoveDirection();
        Move();
        LookAt();
    }

    private void Move()
    {
        /*bool canMoveTo = CanMoveTo(m_DesiredMoveDirection);
        if (m_DesiredMoveDirection != Vector3.zero && canMoveTo && Mathf.Abs(m_LastLookAtAngle) < m_MinLookAtAngleToMove)
        {
            m_TimeToMove += Time.fixedDeltaTime;
            m_TimeToMove = Mathf.Clamp(m_TimeToMove, m_MinSpeed, m_MaxSpeed);
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_DesiredMoveDirection * m_SpeedOverTime.Evaluate(m_TimeToMove) * Time.fixedDeltaTime);
            //m_Rigidbody.AddForce(m_DesiredMoveDirection * m_SpeedOverTime.Evaluate(m_TimeToMove), ForceMode.Acceleration);
        }
        else if(CanMoveTo(m_Rigidbody.transform.forward))
        {
            m_TimeToMove -= Time.fixedDeltaTime;
            m_TimeToMove = Mathf.Clamp(m_TimeToMove, m_MinSpeed, m_MaxSpeed);

            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Rigidbody.transform.forward * m_SpeedOverTime.Evaluate(m_TimeToMove) * Time.fixedDeltaTime);
            //m_Rigidbody.AddForce(m_Rigidbody.velocity * m_SpeedOverTime.Evaluate(m_TimeToMove) * Time.fixedDeltaTime, ForceMode.Acceleration);
        }*/

        if(m_DesiredMoveDirection != Vector3.zero && Mathf.Abs(m_LastLookAtAngle) < m_MinLookAtAngleToMove)
        {
            m_Rigidbody.AddForce(m_DesiredMoveDirection * m_Speed, ForceMode.Acceleration);
            m_TimeToMove += Time.fixedDeltaTime;
        }
        else
        {
            m_TimeToMove = 0f;
        }

        m_TimeToMove = Mathf.Clamp(m_TimeToMove, m_VelocityDragOverTime[0].time, m_VelocityDragOverTime[m_VelocityDragOverTime.length - 1].time);
        m_BuoyantObject.velocityDrag = m_VelocityDragOverTime.Evaluate(m_TimeToMove);
        

        //m_PlayerOnBoard.UpdateSpeed(m_Rigidbody.velocity.magnitude);
    }

    private bool CanMoveTo(Vector3 _desiredMoveDirection)
    {
        /*Vector3 normalizedMoveDirection = _desiredMoveDirection.normalized;

        Ray ray = new Ray(transform.position + (normalizedMoveDirection * m_FloorRaycastDistanceFromOwner) + (Vector3.up * (m_FloorRaycastLength / 2f)), Vector3.down);
        bool hasHitSomething = Physics.Raycast(ray, out RaycastHit hitInfo, m_FloorRaycastLength, m_FloorRaycastLayerMask, QueryTriggerInteraction.Ignore);
        Debug.DrawLine(ray.origin, ray.origin + Vector3.down * m_FloorRaycastLength, hasHitSomething && m_WaterLayerMask.Contains(hitInfo.transform.gameObject.layer) ? Color.green : Color.red);

        return hasHitSomething && m_WaterLayerMask.Contains(hitInfo.transform.gameObject.layer);*/
        return true;
    }

    private void LookAt()
    {
        if (m_DesiredMoveDirection != Vector3.zero)
        {
            Vector3 flatLookAt = new Vector3(transform.forward.x, 0f, transform.forward.z);

            Debug.DrawLine(transform.position, transform.position + m_DesiredMoveDirection, Color.green);
            Debug.DrawLine(transform.position, transform.position + flatLookAt, Color.red);

            m_LastLookAtAngle = Vector3.SignedAngle(flatLookAt, m_DesiredMoveDirection, Vector3.up);
            m_Rigidbody.AddTorque(Vector3.up * Mathf.Sign(m_LastLookAtAngle) * m_AngularSpeed, ForceMode.Acceleration);

            if(Mathf.Abs(m_LastLookAtAngle) >= m_MinLookAtAngleToMove)
            {
                m_TimeToLookAt += Time.fixedDeltaTime;
            }
            else
            {
                m_TimeToLookAt = 0f;
            }
        }
        else
        {
            m_TimeToLookAt = 0f;
        }

        m_TimeToLookAt = Mathf.Clamp(m_TimeToLookAt, m_AngularDragOverTime[0].time, m_AngularDragOverTime[m_AngularDragOverTime.length - 1].time);
        m_BuoyantObject.angularDrag = m_AngularDragOverTime.Evaluate(m_TimeToLookAt);
    }
}
