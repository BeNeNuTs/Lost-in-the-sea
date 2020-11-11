using UnityEngine;

public abstract class BaseInteractor : MonoBehaviour
{
    public string m_InteractText;

    protected MainCharacterController m_MainCharacterController;
    protected bool m_IsPlayerInsideTrigger = false;
    protected bool m_IsInteractTextDisplayed = false;

    private void Start()
    {
        m_MainCharacterController = PlayerContainer.Instance.m_MainCharacterController;
        OnStart_Internal();
    }

    protected virtual void OnStart_Internal() {}

    private void OnDestroy()
    {
        if(InteractorDisplayer.Instance)
            InteractorDisplayer.Instance.UnregisterDisplayer(this);
    }

    private void Update()
    {
        OnUpdate_Internal();
    }

    protected virtual void OnUpdate_Internal() {}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(m_MainCharacterController.gameObject.tag))
        {
            m_IsPlayerInsideTrigger = true;
        }

        if(ShouldDisplayInteractText())
        {
            InteractorDisplayer.Instance.RegisterDisplayer(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(m_MainCharacterController.gameObject.tag))
        {
            m_IsPlayerInsideTrigger = false;
        }

        if(ShouldClearInteractText())
        {
            InteractorDisplayer.Instance.UnregisterDisplayer(this);
        }
    }

    protected virtual bool ShouldDisplayInteractText()
    {
        return m_IsPlayerInsideTrigger;
    }

    protected virtual bool ShouldClearInteractText()
    {
        return !ShouldDisplayInteractText();
    }

    public void SetInteractTextDisplayed(bool isDisplayed)
    {
        m_IsInteractTextDisplayed = isDisplayed;
        if(m_IsInteractTextDisplayed)
        {
            m_MainCharacterController.OnInteractCallback += OnPlayerInteract;
        }
        else
        {
            m_MainCharacterController.OnInteractCallback -= OnPlayerInteract;
        }
    }

    private void OnPlayerInteract()
    {
        if(m_IsInteractTextDisplayed)
        {
            OnPlayerInteract_Internal();
        }
    }

    protected virtual void OnPlayerInteract_Internal()
    {
        InteractorDisplayer.Instance.UnregisterDisplayer(this);
    }
}
