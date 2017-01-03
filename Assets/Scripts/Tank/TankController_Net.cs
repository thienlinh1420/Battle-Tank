using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TankController_Net : NetworkBehaviour {

    [Header("Movement Variables")]
    [SerializeField]public float m_MovementSpeed = 12f;            
    [SerializeField]public float m_MinTurnSpeed = 180f;       
    [SerializeField]public float m_MaxTurnSpeed = 360f; 

    [Header("Audio Variables")]
    [SerializeField]public AudioSource m_MovementAudio;    
    [SerializeField]public AudioClip m_EngineIdling;       
    [SerializeField]public AudioClip m_EngineDriving;      
    [SerializeField]public float m_PitchRange = 0.2f;

    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementVerticalValue;    
    private float m_MovementHorizontalValue;        
    private float m_OriginalPitch;   
    private Vector3 m_Move;
    private float m_TurnAmount;
    private float m_ForwardAmount;   

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementVerticalValue = 0f;
        m_MovementHorizontalValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }

	// Use this for initialization
	void Start () {
        if (!isLocalPlayer)
        {
            //Destroy(this);
            return;
        }
	
        m_MovementAxisName = "Vertical";// + m_PlayerNumber;
        m_TurnAxisName = "Horizontal";// + m_PlayerNumber;
        m_OriginalPitch = m_MovementAudio.pitch;
	}
	
    private void FixedUpdate ()
    {
        if (!isLocalPlayer)
            return;
        m_MovementVerticalValue = Input.GetAxis (m_MovementAxisName);
        m_MovementHorizontalValue = Input.GetAxis (m_TurnAxisName);

        Turn();

    }


    private void EngineAudio ()
    {
        // If there is no input (the tank is stationary)...
        if (Mathf.Abs (m_MovementVerticalValue) < 0.1f && Mathf.Abs (m_MovementHorizontalValue) < 0.1f)
        {
            // ... and if the audio source is currently playing the driving clip...
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                // ... change the clip to idling and play it.
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = m_OriginalPitch + m_Move.z;
                m_MovementAudio.Play ();
            }
        }
        else
        {
            // Otherwise if the tank is moving and if the idling clip is currently playing...
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                // ... change the clip to driving and play.
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = m_OriginalPitch + m_Move.z;

                m_MovementAudio.Play();

            }
        }
    }


    private void Move ()
    {
        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        Vector3 movement = transform.forward * m_MovementSpeed * Time.deltaTime;
        // Apply this movement to the rigidbody's position.
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }





    private void Turn()
    {
        m_Move = m_MovementVerticalValue * Vector3.forward + m_MovementHorizontalValue * Vector3.right;

        if (m_Move.magnitude > 1f)
        {
            m_Move.Normalize();
        }
        m_Move = transform.InverseTransformDirection(m_Move);

        m_TurnAmount = Mathf.Atan2(m_Move.x, m_Move.z);
        m_ForwardAmount = m_Move.z;

        float turnSpeed = Mathf.Lerp(m_MinTurnSpeed, m_MaxTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);

        if (Mathf.Abs(m_TurnAmount) > 0 && Mathf.Abs(m_TurnAmount) < 0.5f)
        {
            Move();
        }

    }
}
