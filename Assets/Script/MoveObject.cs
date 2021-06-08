using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;


public class MoveObject : MonoBehaviourPun
{
    // intersecion raycast and object
    public GameObject m_Pointer;
    private bool m_HasPosition = false;
    private RaycastHit hit;

    // laterals buttons
    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

    //pose
    private SteamVR_Behaviour_Pose m_pose = null;

    //card to move
    private GameObject ob;

    //Room
    public Transform MurB;
    public Transform MurL;
    public Transform MurR;

    private string nameM = "";
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

        if (ob != null) // follow the mouvement
        {
            float x, y, z;
            Vector3 v = MurR.localScale;
            Vector3 p = MurR.position;

            x = m_Pointer.transform.position.x / v.x;
            y = (m_Pointer.transform.position.y - p.y) / v.y;
            z = -0.02f;


            if (ob.transform.parent.name == "MUR L")
            {
                //change
                if (hit.transform.name == "MUR B") { nameM = hit.transform.name; }
                //move on L
                ob.transform.localPosition = new Vector3(m_Pointer.transform.position.z / v.x, y, z);  // /10
            }

            else if (ob.transform.parent.name == "MUR B")
            {
                //change
                if (hit.transform.name == "MUR L") { nameM = hit.transform.name; }

                if (hit.transform.name == "MUR R") { nameM = hit.transform.name; }
                // move on B
                ob.transform.localPosition = new Vector3(x, y, z);
            }

            else if (ob.transform.parent.name == "MUR R")
            {
                //change
                if (hit.transform.name == "MUR B") { nameM = hit.transform.name; }
                //move on R
                ob.transform.localPosition = new Vector3(-m_Pointer.transform.position.z / v.x, y, z);
            }

            //if change then rpc change wall
            if (nameM != "")
            {
                photonView.RPC("ChangeMur", Photon.Pun.RpcTarget.All, nameM, ob.GetComponent<PhotonView>().ViewID);
                nameM = "";
            }

        }
        if (interactWithUI.GetStateUp(m_pose.inputSource))
        {
            Move();
        }   
    }

    private void Move()
    {
        // Debug.Log("Move");

        float x, y, z;
        Vector3 v = MurR.localScale;
        Vector3 p = MurR.position;

        x = m_Pointer.transform.position.x / v.x;
        y = (m_Pointer.transform.position.y - p.y) / v.y;
        z = -0.02f;


        if (!m_HasPosition)
            return;
        
        else if(hit.transform.tag == "Card" &&  ob == null){
            hit.transform.gameObject.GetComponent<PhotonView>().RequestOwnership();
            ob = hit.transform.gameObject;
        }
        else if(hit.transform.tag == "Wall" || hit.transform.tag == "Card")
        {
            
            if(ob != null){
                if(ob.transform.parent.name == "MUR L")
                {
                    ob.transform.localPosition = new Vector3(m_Pointer.transform.position.z / v.x, y, z);
                }
       
                else if (ob.transform.parent.name == "MUR B")
                {
                    ob.transform.localPosition = new Vector3(x, y, z);
                }

                else if (ob.transform.parent.name == "MUR R")
                {
                    ob.transform.localPosition = new Vector3(-m_Pointer.transform.position.z / v.x, y, z);
                }
     
                    ob = null;
          }
        }
    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        //check if there is a hit
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Card" || hit.transform.tag == "Wall")
            {
                m_Pointer.transform.position = hit.point;
                return true;
            }
        }
        return false;
    }
}
