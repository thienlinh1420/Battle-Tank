using UnityEngine;
using System.Collections;

public class HealthRestoreController : MonoBehaviour {

    public float m_HealthRestore = 100f;
    public float m_LifeTime = 15f;

    public void Start()
    {
        Destroy(gameObject, m_LifeTime);
    }

    public void OnTriggerEnter (Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<TankHealth>().RestoreHealth(m_HealthRestore);
            Destroy(gameObject);
        }
    }
}
