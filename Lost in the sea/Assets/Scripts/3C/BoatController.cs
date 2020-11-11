using UnityEngine;

public class BoatController : MonoBehaviour
{
    [Header("Setup")]
    public Rigidbody m_Rigidbody;
    public Transform m_AnchorPoint;

    [Header("Navigation")]
    public float m_Acceleration;
    public float m_MaxVelocity = 4f;
    public AnimationCurve m_VelocityDragOverTime;
    public float m_MinLookAtAngleToMove = 60f;
    public AnimationCurve m_AngularAccelerationFromAngle;

    [Header("Buoyancy")]
    public BuoyantObject m_BuoyantObject;
    public Vector3 m_Gravity = Physics.gravity;

    //[Header("Floor Raycast")]
    //public float m_FloorRaycastDistanceFromOwner = 1f;
    //public float m_FloorRaycastLength = 3f;
    //public LayerMask m_WaterLayerMask;
    //public LayerMask m_FloorRaycastLayerMask;

    private MainCharacterController m_PlayerOnBoard;
    private float m_InitialVelocityDrag;
    private Vector3 m_InitialGravity;


    private Vector3 m_DesiredMoveDirection;
    private float m_TimeToMove = 0f;

    private float m_LastLookAtAngle = 0f;

    private void Awake()
    {
        enabled = false;
        m_InitialVelocityDrag = m_BuoyantObject.velocityDrag;
        m_InitialGravity = m_BuoyantObject.m_Gravity;
    }

    public void ProxyPlayer(MainCharacterController _mainCharacterController)
    {
        m_PlayerOnBoard = _mainCharacterController;

        m_PlayerOnBoard.transform.SetParent(transform);
        m_PlayerOnBoard.transform.localPosition = Vector3.zero;
        m_PlayerOnBoard.transform.localRotation = Quaternion.identity;

        m_BuoyantObject.m_Gravity = m_Gravity;

        enabled = true;
    }

    public void UnproxyPlayer()
    {
        m_PlayerOnBoard.transform.SetParent(null);
        m_PlayerOnBoard = null;
        enabled = false;
        m_BuoyantObject.velocityDrag = m_InitialVelocityDrag;
        m_BuoyantObject.m_Gravity = m_InitialGravity;
    }

    private void FixedUpdate()
    {
        m_DesiredMoveDirection = m_PlayerOnBoard.GetDesiredMoveDirection();
        Move();
        LookAt();
    }

    private void Move()
    {
        if (m_DesiredMoveDirection != Vector3.zero && Mathf.Abs(m_LastLookAtAngle) < m_MinLookAtAngleToMove)
        {
            m_Rigidbody.AddForce(m_DesiredMoveDirection * m_Acceleration, ForceMode.Acceleration);
            m_Rigidbody.velocity = Vector3.ClampMagnitude(m_Rigidbody.velocity, m_MaxVelocity);
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
            m_Rigidbody.AddTorque(Vector3.up * Mathf.Sign(m_LastLookAtAngle) * m_AngularAccelerationFromAngle.Evaluate(m_LastLookAtAngle), ForceMode.Acceleration);
        }
    }

    private void OnGUI()
    {
        Rect pos = new Rect(10, 10, 300, 20);
        DrawLabel(ref pos, "Gravity :" + m_BuoyantObject.m_Gravity);
        DrawLabel(ref pos, "");
        DrawLabel(ref pos, "Acceleration: " + m_Acceleration);
        DrawLabel(ref pos, "Velocity: " + m_Rigidbody.velocity + " => " + m_Rigidbody.velocity.magnitude + " / " + m_MaxVelocity);
        DrawLabel(ref pos, "Velocity drag: " + m_BuoyantObject.velocityDrag);
        DrawLabel(ref pos, "");
        DrawLabel(ref pos, "Look at angle: " + m_LastLookAtAngle);
        DrawLabel(ref pos, "Angular acceleration: " + m_AngularAccelerationFromAngle.Evaluate(m_LastLookAtAngle));
        DrawLabel(ref pos, "Angular velocity: " + m_Rigidbody.angularVelocity + " => " + m_Rigidbody.angularVelocity.magnitude + " / " + m_Rigidbody.maxAngularVelocity);
        DrawLabel(ref pos, "Angular drag: " + m_BuoyantObject.angularDrag);
    }

    private void DrawLabel(ref Rect pos, string msg)
    {
        GUI.Label(pos, msg);
        pos.y += pos.height;
    }
}
