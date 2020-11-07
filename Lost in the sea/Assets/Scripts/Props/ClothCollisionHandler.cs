using UnityEngine;

public class ClothCollisionHandler : MonoBehaviour
{
    public Cloth[] m_Clothes;

    private void Start()
    {
        CapsuleCollider playerClothCollider = PlayerContainer.Instance.m_ClothCollider;
        for(int i = 0; i < m_Clothes.Length; i++)
        {
            m_Clothes[i].capsuleColliders = new CapsuleCollider[1] { playerClothCollider };
        }
    }
}
