using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    public bool m_UseRelativeRotation = true;  
    public float rotation;

    private Quaternion m_RelativeRotation;     


    private void Start()
    {
        m_RelativeRotation = Quaternion.Euler(new Vector3(90f, 0f, rotation));
    }


    private void Update()
    {
        if (m_UseRelativeRotation)
            transform.rotation = m_RelativeRotation;
    }
}
