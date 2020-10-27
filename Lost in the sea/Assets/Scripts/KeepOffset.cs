using UnityEngine;

[ExecuteInEditMode]
public class KeepOffset : MonoBehaviour
{
    public Transform m_Target;

    void Update()
    {
        transform.position = new Vector3(m_Target.position.x, transform.position.y, m_Target.position.z);
    }
}
