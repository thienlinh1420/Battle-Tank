﻿using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor;  
    public GameObject m_TankPrefab;
    [HideInInspector] public int m_PlayerNumber;             
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;          
    [HideInInspector] public int m_Wins;                     


    private TankMovement m_Movement;       
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject;


    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();

        if (m_Instance.name == "Rocket Tank")
        {
            m_Shooting = m_Instance.GetComponent<RocketTankShooting>();
        }
        else
        {
            m_Shooting = m_Instance.GetComponent<TankShooting>();
        }
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        MeshRenderer[] renderers = m_Instance.transform.FindChild("Renderers").GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }


    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset(Transform spawPoint)
    {
        m_Instance.transform.position = spawPoint.position;
        m_Instance.transform.rotation = spawPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
