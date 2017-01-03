using UnityEngine;
using System.Collections;

public class TreeDestroy : MonoBehaviour {

    public GameObject[] stumps;
    public GameObject m_Fire;
    public float m_Health = 500f;
    public float m_SmokeTime = 180f;

    private float m_CurrentHealth;
    private GameObject m_FireInstance;

    private void Start()
    {
        m_CurrentHealth = m_Health;
        m_FireInstance = 
            Instantiate(m_Fire, gameObject.transform) as GameObject;
        m_FireInstance.transform.position = gameObject.transform.position;
        m_FireInstance.transform.Translate(0f, 2f, 0f);
    }
	
    public void TakeDamage(float damage)
    {
        if (m_CurrentHealth <= 0)
        {
            return;
        }

        m_CurrentHealth -= damage;

        if (m_CurrentHealth < m_Health * 0.8f)
        {
            ParticleSystem Smoke = m_FireInstance.transform.Find("SmokeLit").GetComponent<ParticleSystem>();
            if (!Smoke.isPlaying)
            {
                Smoke.Play();
            }
        }

        if (m_CurrentHealth < m_Health * 0.6f)
        {
            ParticleSystem Glow = m_FireInstance.transform.Find("Glow").GetComponent<ParticleSystem>();
            if (!Glow.isPlaying)
            {
                Glow.Play();
            }
        }

        if (m_CurrentHealth < m_Health * 0.4f)
        {
            ParticleSystem Fire = m_FireInstance.GetComponent<ParticleSystem>();
            if (!Fire.isPlaying)
            {
                m_FireInstance.transform.Find("SmokeLit").GetComponent<ParticleSystem>().Stop();
                m_FireInstance.transform.Find("Glow").GetComponent<ParticleSystem>().Stop();

                Fire.Play();
                m_FireInstance.transform.Find("Light").gameObject.SetActive(true);

            }

        }

        if (m_CurrentHealth <= 0f)
        {
            GameObject stumpClone = 
                Instantiate(stumps[Random.Range(0, stumps.Length)], 
                    transform.position, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up),
                    gameObject.transform.parent) as GameObject;

            Instantiate(m_FireInstance.transform.Find("SmokeLit"), stumpClone.transform);
            stumpClone.transform.GetChild(0).Translate(0f, -1f, 0f);

            stumpClone.transform.GetChild(0).GetComponent<ParticleSystem>().Play();

            Destroy(stumpClone.transform.GetChild(0).gameObject, m_SmokeTime);

            Destroy(gameObject);
        }
       
    }
	
}
