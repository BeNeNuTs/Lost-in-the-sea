using UnityEngine;

public class BoatAnchorInteractor : BaseInteractor
{
    private static readonly float K_ROPE_LENGTH = 3f;
    private static readonly float K_ROPE_SCALE_MULT = 1.5f;

    [SerializeField] private BoatController m_AnchoredBoatController;
    public GameObject m_AnchorRope;
    public Transform m_RopeBegin;
    public Transform m_RopeEnd;

    protected override void OnStart_Internal()
    {
        base.OnStart_Internal();
        RefreshAnchorRope();
    }

    protected override void OnUpdate_Internal()
    {
        base.OnUpdate_Internal();
        if(m_AnchoredBoatController != null)
        {
            m_RopeEnd.position = m_AnchoredBoatController.m_AnchorPoint.position;
        }
    }

    protected override void OnPlayerInteract_Internal()
    {
        base.OnPlayerInteract_Internal();
        m_MainCharacterController.SetInBoat(m_AnchoredBoatController);
        m_AnchoredBoatController = null;
        RefreshAnchorRope();
    }

    protected override bool ShouldDisplayInteractText()
    {
        return base.ShouldDisplayInteractText() && m_AnchoredBoatController != null;
    }

    private void RefreshAnchorRope()
    {
        if (m_AnchoredBoatController != null)
        {
            float distanceToBoatAnchorPoint = Vector3.Distance(m_RopeBegin.position, m_AnchoredBoatController.m_AnchorPoint.position);
            float scaleToApplyToRope = (distanceToBoatAnchorPoint / K_ROPE_LENGTH) * K_ROPE_SCALE_MULT;
            m_AnchorRope.transform.localScale = new Vector3(1f, scaleToApplyToRope, 1f);
            m_AnchorRope.SetActive(true);
        }
        else
        {
            m_AnchorRope.SetActive(false);
        }
    }

    public void SetAnchoredBoat(BoatController boat)
    {
        m_AnchoredBoatController = boat;
        RefreshAnchorRope();
    }
}
