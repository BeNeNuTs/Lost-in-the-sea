using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerContainer : Singleton<PlayerContainer>
{
    public Animator m_Animator;
    public MainCharacterController m_MainCharacterController;
    public PlayerInput m_PlayerInput;
    public CapsuleCollider m_ClothCollider;
    public TextMeshProUGUI m_InteractText;
}
