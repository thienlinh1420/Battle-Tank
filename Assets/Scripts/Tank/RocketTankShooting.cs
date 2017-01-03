using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RocketTankShooting : TankShooting
{
    public float m_TimeReloading = 50f;
    private int m_numberRocket = 8;
    private Transform m_listRocket;
    private bool m_isReload = false;

    private const int MAX_ROCKET = 8;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_listRocket = transform.Find("Rockets");
        DoReload();
    }

    protected override void Fire ()
    {
        // Set the fired flag so only Fire is only called once.
        m_Fired = true;

        // Create an instance of the shell and store a reference to it's rigidbody.
        GameObject shell = null;
        if (!m_isReload)
        {
            shell = m_listRocket.Find("Rocket (" + m_numberRocket + ")").gameObject;
        

            if (shell != null)
            {
                GameObject shellInstance =
                    Instantiate(shell, shell.transform.position, shell.transform.rotation) as GameObject;

                shellInstance.AddComponent<Rigidbody>();
                shellInstance.GetComponent<Collider>().enabled = true;
                shellInstance.GetComponent<RocketExplosion>().enabled = true;
                shellInstance.transform.Find("Body").Find("Trails").GetComponent<ParticleSystem>().Play();
                shellInstance.transform.Find("Tail").gameObject.SetActive(true);
                shellInstance.GetComponent<RocketExplosion>().setPlayerNumber(m_PlayerNumber);

                shellInstance.GetComponent<Rigidbody>().velocity = m_CurrentLaunchForce * shellInstance.transform.forward;
                shell.SetActive(false);

                m_Fire.transform.position = shellInstance.transform.position;
            }

            // Set the shell's velocity to the launch force in the fire position's forward direction.


            if (m_ShootingAudio.isPlaying)
            {
                m_ShootingAudio.Stop();
            }
            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();

            // Reset the launch force.  This is a precaution in case of missing button events.
            m_CurrentLaunchForce = m_MinLaunchForce;

            m_numberRocket--;

            if (m_numberRocket == 0)
            {
                m_isReload = true;   
                Reload();
            }

            EnableSpark();
        }
    }

    private void Reload()
    {
        StartCoroutine(WaitReload());
    }

    private IEnumerator WaitReload()
    {
        yield return new WaitForSeconds(m_TimeReloading);
        DoReload();
       
    }

    private void DoReload()
    {
        for (int i = 1; i <= MAX_ROCKET; i++)
        {
            m_listRocket.Find("Rocket (" + i + ")").gameObject.SetActive(true);
        }
        m_numberRocket = MAX_ROCKET;
        m_isReload = false;
    }
}