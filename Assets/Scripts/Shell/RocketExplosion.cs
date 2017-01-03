using UnityEngine;
using System;

public class RocketExplosion : ShellExplosion
{
    public float m_MaxFindDistance = 10f;
    public float m_LaunchForce = 50f;
    public float m_TimeTail = 2f;

    private int playerNumber = -1;

    public void setPlayerNumber(int num)
    {
        this.playerNumber = num;
    }

    protected override void Update()
    {
        base.Update();

        FindTarget();
    }

    protected override void OnTriggerEnter (Collider other)
    {
        try
        {

        GameObject tail = gameObject.transform.Find("Tail").gameObject;

        if (tail != null)
        {
            tail.transform.parent = null;

            Destroy(tail, m_TimeTail);
        }
        }
        catch (NullReferenceException e)
        {
            
        }
        finally
        {
            base.OnTriggerEnter(other);
        }

    }

    private void FindTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = -1;
        Vector3 target = Vector3.zero;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].GetComponent<TankShooting>().m_PlayerNumber != playerNumber)
            {
                float distance = Vector3.Distance(targets[i].transform.position, transform.position);

                if (minDistance == -1)
                {
                    minDistance = distance;
                    target = targets[i].transform.position;
                }
                else
                {
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        target = targets[i].transform.position;
                    }
                }
            }
        }

        if (minDistance != -1 && minDistance <= m_MaxFindDistance)
        {
           
//            Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
//            transform.rotation = Quaternion.Slerp(Quaternion.Euler(transform.forward), targetRotation,  Time.deltaTime * 100f);
            transform.LookAt(target);
            GetComponent<Rigidbody>().velocity = m_LaunchForce * transform.forward;
        }
    }
}