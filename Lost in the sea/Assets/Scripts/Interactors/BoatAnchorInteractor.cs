using UnityEngine;

public class BoatAnchorInteractor : BaseInteractor
{
    public BoatController m_AnchoredBoatController;
    public Transform m_AnchorPoint;

    protected override void OnPlayerInteract_Internal()
    {
        base.OnPlayerInteract_Internal();
        m_MainCharacterController.SetInBoat(m_AnchoredBoatController);
        m_AnchoredBoatController = null;
    }

    protected override bool ShouldDisplayInteractText()
    {
        return base.ShouldDisplayInteractText() && m_AnchoredBoatController != null;
    }
}
