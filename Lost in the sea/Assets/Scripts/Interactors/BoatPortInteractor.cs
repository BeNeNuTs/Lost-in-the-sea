using UnityEngine;

public class BoatPortInteractor : BaseInteractor
{
    public BoatAnchorInteractor m_BoatAnchorInteractor;
    public Transform m_ExitPoint;

    protected override void OnPlayerInteract_Internal()
    {
        base.OnPlayerInteract_Internal();

        m_BoatAnchorInteractor.SetAnchoredBoat(m_MainCharacterController.GetCurrentBoat());
        m_MainCharacterController.transform.position = m_ExitPoint.position;
        m_MainCharacterController.SetInBoat(null);
    }

    protected override bool ShouldDisplayInteractText()
    {
        return base.ShouldDisplayInteractText() && m_MainCharacterController.IsInBoat();
    }
}
