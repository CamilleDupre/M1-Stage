using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class test : MonoBehaviour
{
    public Vector2 axis { get; }
    public Vector2 delta { get; }

    public SteamVR_Action_Vector2 Pos;
    private SteamVR_Behaviour_Pose m_pose = null;
    private bool m_IsTeleportoting = false;
    private float m_FadeTime = 0.5f;
    RaycastHit hit;
    Vector2 initialPos;
    Vector2 position;
    private bool wait = false;

    // Start is called before the first frame update
    void Awake()
    {
        m_pose = GetComponent<SteamVR_Behaviour_Pose>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    { 
        Transform cameraRig = SteamVR_Render.Top().origin;
        
        if (SteamVR_Actions.default_touchPos.GetStateDown(m_pose.inputSource))
        {
            initialPos = SteamVR_Actions.default_Pos.GetAxis(SteamVR_Input_Sources.Any);
            Debug.Log("initialPos " + initialPos); 
            wait = true;
        }

        if (SteamVR_Actions.default_touchPos.GetStateUp(m_pose.inputSource))
        {
            //initialPos = SteamVR_Actions.default_Pos.GetAxis(SteamVR_Input_Sources.Any);
            // Debug.Log("initialPos " + initialPos);
           // bool wait = false;
            //position = new Vector2();
        }

        position = SteamVR_Actions.default_Pos.GetAxis(SteamVR_Input_Sources.Any);

        if (position.y - initialPos.y > 1 && wait)
        {
            Debug.Log("Move up");
            cameraRig.Translate(Vector3.forward);//translation;
            wait = false;
        }

        else if (position.y - initialPos.y < 1 && wait)
        {
            Debug.Log("Move down");
            cameraRig.Translate(-Vector3.forward);
            wait = false;
        }
       // Debug.Log("wait " + wait);

    }
}

