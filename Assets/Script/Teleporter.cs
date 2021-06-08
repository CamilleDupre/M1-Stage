using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Teleporter : MonoBehaviour
{
    // intersecion raycast and object
    public GameObject m_Pointer;
    private bool m_HasPosition = false;
    RaycastHit hit;

    //clic touchpad
    public SteamVR_Action_Boolean m_TeleportAction;

    //Pose
    private SteamVR_Behaviour_Pose m_pose = null;

    //Teleportation parameters 
    private bool m_IsTeleportoting = false;
    private float m_FadeTime = 0.5f;
    
    // State machine
    private bool wait = false;
    private bool isMoving = false;
    private Vector3 coordClic;
    private Vector3 coordPrev;
    public Vector3 forwardClic;
    public int timer = 0;
 

    // Start is called before the first frame update
    void Awake()
    {
        m_pose = GetComponent<SteamVR_Behaviour_Pose>();
    }

    // Update is called once per frame
    void Update()
    {
        //Pointer
        m_HasPosition = UpdatePointer();
        //m_Pointer.SetActive(m_HasPosition);

        //Teleport

        if (m_TeleportAction.GetStateDown(m_pose.inputSource))
        {
            coordClic = coordPrev = m_Pointer.transform.position; //hit.transform.position;
            forwardClic = transform.forward;
           // Debug.Log("coordClic : " + coordClic);
            wait = true;

        }


        if (m_TeleportAction.GetStateUp(m_pose.inputSource))
        {
            if (wait)
            { 
                //just q clic -> normal teleportation
                tryTeleport();
            }
           //Debug.Log("reset");
            wait = false;
            isMoving = false;
            timer = 0;

        }

        // check the angle to detect a mouvement
        if (wait && Vector3.Angle(forwardClic, transform.forward) > 2)
        {
            isMoving = true;
            wait = false;
        }

        if (isMoving)
        {
            //dragTeleport(coordPrev, m_Pointer.transform.position);
            coordPrev = m_Pointer.transform.position;
        }

    }
     
    /* try drag wall
    private void dragTeleport(Vector3 prev, Vector3 curr)
    {
        if (!m_HasPosition || m_IsTeleportoting)
            return;

        Vector3 headPosition = SteamVR_Render.Top().head.position;
        Transform cameraRig = SteamVR_Render.Top().origin;

        Vector3 delta = new Vector3();
        delta = curr - prev;

        if (hit.transform.tag == "Wall" || hit.transform.tag == "Card")
        {
            if (hit.transform.name == "MUR B" || hit.transform.parent.name == "MUR B")
            {

                Vector3 translation = new Vector3(-delta.x, 0, 0);
                cameraRig.position = cameraRig.position + translation;
                Debug.Log("drag mur b " + translation);

            }
            else if (hit.transform.name == "MUR R" || hit.transform.parent.name == "MUR R")
            {
                Vector3 translation = new Vector3(0, 0, -delta.z);
                cameraRig.position = cameraRig.position + translation;
                Debug.Log("drag mur r " + translation);

            }
            else if (hit.transform.name == "MUR L" || hit.transform.parent.name == "MUR L")
            {
                Vector3 translation = new Vector3(0, 0, -delta.z);
                cameraRig.position = cameraRig.position + translation;
                Debug.Log("drag mur l " + translation);

            }
        }
    }
    */

    private void tryTeleport()
    {
        //if no hit stop the fonction
        if (!m_HasPosition || m_IsTeleportoting)
            return;

        // head position + camera rig
        Vector3 headPosition = SteamVR_Render.Top().head.position;
        Transform cameraRig = SteamVR_Render.Top().origin;
        
        //player possition
        Vector3 groundPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);

        Vector3 translateVector;
        if (hit.transform.tag == "Tp" )
        {
            translateVector = m_Pointer.transform.position - groundPosition;
            StartCoroutine(MoveRig(cameraRig, translateVector));
        }

        else if (hit.transform.tag == "Wall" || hit.transform.tag == "Card")
        {
            //check the wall
            if (hit.transform.name == "MUR B" || hit.transform.parent.name == "MUR B")
            {
                 translateVector = new Vector3(m_Pointer.transform.position.x - groundPosition.x, 0, 0);
            }
            else if (hit.transform.name == "MUR R" || hit.transform.parent.name == "MUR R")
            { 
                 translateVector = new Vector3(0, 0, m_Pointer.transform.position.z - groundPosition.z);
            }
            else //(hit.transform.name == "MUR L" || hit.transform.parent.name == "MUR L")
            {
                 translateVector = new Vector3(0, 0, m_Pointer.transform.position.z - groundPosition.z); 
            }
            //then teleport
            StartCoroutine(MoveRig(cameraRig, translateVector));
        }
    }

    private IEnumerator MoveRig(Transform cameraRig , Vector3 translation)
    {
        m_IsTeleportoting = true;

        SteamVR_Fade.Start(Color.black, m_FadeTime, true); // black screen

        yield return new WaitForSeconds( m_FadeTime); // fade time
        
        cameraRig.position += translation; // teleportation

        SteamVR_Fade.Start(Color.clear, m_FadeTime, true); // normal screen

        m_IsTeleportoting = false;

    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position, transform.forward);
       
        //check if there is a hit
        if(Physics.Raycast(ray , out hit) )
        {
            if (hit.transform.tag == "Tp" || hit.transform.tag == "Card" || hit.transform.tag == "Wall")
            {
                m_Pointer.transform.position = hit.point;
                return true;
                
            }
        }
        return false;
    }
}
