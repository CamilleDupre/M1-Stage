using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;


public class MoveObject : MonoBehaviourPun
{
    public GameObject m_Pointer;
    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

    private SteamVR_Behaviour_Pose m_pose = null;
    private bool m_HasPosition = false;

    private RaycastHit hit;
    private GameObject ob;

    public Material initialColor ;
    public Material selectedColor ;

    public Transform MurB;
    public Transform MurL;
    public Transform MurR;

    // public GameObject rayCast;
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
       // m_Pointer.SetActive(m_HasPosition);
        if (ob != null) // follow the mouvement
        {
            float x, y, z;
            x = m_Pointer.transform.position.x / 10;
            y = (m_Pointer.transform.position.y - 1) / 2;
            z = -0.02f;
            //ob.transform.localPosition = new Vector3(x, y, z);
            //ob.transform.localPosition = new Vector3(0, 0, 0);
            if (ob.transform.parent.name == "MUR L")
            {

                ob.transform.localPosition = new Vector3(m_Pointer.transform.position.z / 10, y, z);
                if (hit.transform.name == "MUR B")
                {
                    Debug.Log("changement de mur B ");
                    ob.transform.parent = MurB;
                    ob.transform.rotation = MurB.rotation;
                    ob.transform.localScale = new Vector3(0.04165002f, 0.3106501f, 1.01f);
                }


            }

            else if (ob.transform.parent.name == "MUR B")
            {
                if (hit.transform.name == "MUR L")
                {
                    Debug.Log("changement de mur L ");
                    ob.transform.parent = MurL;
                    ob.transform.rotation = MurL.rotation;
                    ob.transform.localScale = new Vector3(0.04165002f, 0.3106501f, 1.01f);
                }

                if (hit.transform.name == "MUR R")
                {
                    Debug.Log("changement de mur R ");
                    ob.transform.parent = MurR;
                    ob.transform.rotation = MurR.rotation;
                    ob.transform.localScale = new Vector3(0.04165002f, 0.3106501f, 1.01f);
                }
                ob.transform.localPosition = new Vector3(x, y, z);
            }

            else if (ob.transform.parent.name == "MUR R")
            {
                if (hit.transform.name == "MUR B")
                {
                    Debug.Log("changement de mur B ");
                    ob.transform.parent = MurB;
                    ob.transform.rotation = MurB.rotation;
                    ob.transform.localScale = new Vector3(0.04165002f, 0.3106501f, 1.01f);
                }
                ob.transform.localPosition = new Vector3(-m_Pointer.transform.position.z / 10, y, z);
            }
        }
        if (interactWithUI.GetStateUp(m_pose.inputSource))
            Move();


        //Debug.Log("x : " + m_Pointer.transform.position.x  + "/ y : " + m_Pointer.transform.position.y  + " / z : " + m_Pointer.transform.position.z);
    }

    private void Move()
    {
     // Debug.Log("Move");

        float x, y, z;
        x = m_Pointer.transform.position.x / 10;
        y = (m_Pointer.transform.position.y - 1) / 2;
        z = -0.02f;

        if (!m_HasPosition)
            return;
        
        else if(hit.transform.tag == "Card" &&  ob == null){
            hit.transform.gameObject.GetComponent<PhotonView>().RequestOwnership();
            ob = hit.transform.gameObject;
            // ob.GetComponent<Renderer>().material = selectedColor;
            //rayCast.GetComponent<Renderer>().material = selectedColor;

        }
        else if(hit.transform.tag == "Wall" || hit.transform.tag == "Card")
        {
            
            if(ob != null){
                if(ob.transform.parent.name == "MUR L")
                {
                    ob.transform.localPosition = new Vector3(m_Pointer.transform.position.z / 10, y, z);
                }
          

                else if (ob.transform.parent.name == "MUR B")
                {
                    ob.transform.localPosition = new Vector3(x, y, z);
                }

                else if (ob.transform.parent.name == "MUR R")
                {
                    ob.transform.localPosition = new Vector3(-m_Pointer.transform.position.z / 10, y, z);
                }
                    //ob.GetComponent<Renderer>().material = initialColor;
                    ob = null;
          }
        }

        else if (ob != null) // follow the mouvement
        {
            // ob.transform.localPosition = new Vector3(x, y, z);
            // ob.transform.localPosition = new Vector3(z, y, x);
            
        }
    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Card" || hit.transform.tag == "Wall")
            {
                m_Pointer.transform.position = hit.point;
                m_Pointer.gameObject.SetActive(false);
                return true;
            }
        }
        return false;
    }
}
