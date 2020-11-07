using System.Collections.Generic;
using TMPro;

public class InteractorDisplayer : Singleton<InteractorDisplayer>
{
    private List<BaseInteractor> m_InteractorsToDisplay = new List<BaseInteractor>();
    private BaseInteractor m_DisplayedInteractor = null;

    private TextMeshProUGUI m_InteractText;

    private void Start()
    {
        m_InteractText = PlayerContainer.Instance.m_InteractText;
    }

    public void RegisterDisplayer(BaseInteractor baseInteractor)
    {
        if(!m_InteractorsToDisplay.Contains(baseInteractor))
        {
            m_InteractorsToDisplay.Add(baseInteractor);
            RefreshDisplayedInteractor();
        }
    }

    public void UnregisterDisplayer(BaseInteractor baseInteractor)
    {
        m_InteractorsToDisplay.Remove(baseInteractor);
        RefreshDisplayedInteractor();
    }

    private void RefreshDisplayedInteractor()
    {
        int interactorToDisplayCount = m_InteractorsToDisplay.Count;
        if(interactorToDisplayCount > 0)
        {
            m_DisplayedInteractor = m_InteractorsToDisplay[interactorToDisplayCount - 1];
            for(int i = 0; i < interactorToDisplayCount; i++)
            {
                m_InteractorsToDisplay[i].SetInteractTextDisplayed(m_InteractorsToDisplay[i] == m_DisplayedInteractor);
            }
        }
        else
        {
            if(m_DisplayedInteractor != null)
            {
                m_DisplayedInteractor.SetInteractTextDisplayed(false);
            }
            m_DisplayedInteractor = null;
        }

        if(m_DisplayedInteractor != null)
        {
            m_InteractText.gameObject.SetActive(true);
            m_InteractText.text = m_DisplayedInteractor.m_InteractText;
        }
        else
        {
            m_InteractText.gameObject.SetActive(false);
            m_InteractText.text = "";
        }
    }
}
