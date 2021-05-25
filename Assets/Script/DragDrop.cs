using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;

public class DragDrop : MonoBehaviourPun
{
    public GameObject m_Pointer;
    //public SteamVR_Action_Boolean m_TeleportAction;
    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

    private SteamVR_Behaviour_Pose m_pose = null;
    private bool m_HasPosition = false;
    private bool isMoving = false;


    private RaycastHit hit;
    private GameObject ob;

    public Material initialColor ;
    public Material selectedColor ;

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
        m_Pointer.SetActive(m_HasPosition);

        if (interactWithUI.GetStateUp(m_pose.inputSource))
        {
            isMoving = false;
            ob = null;
        }
          

        if (interactWithUI.GetStateDown(m_pose.inputSource) && hit.transform.tag == "Card")
        {
            hit.transform.gameObject.GetComponent<PhotonView>().RequestOwnership();
            //Debug.Log(hit.transform.gameObject.GetComponent<PhotonView>());
            isMoving = true;
            ob = hit.transform.gameObject;
        }
        Move();
    }

    private void Move()
    {
        //Debug.Log("Move");

        float x, y, z;
        x = m_Pointer.transform.position.x / 10;
        y = (m_Pointer.transform.position.y - 1) / 2;
        z = -0.02f;


        if (!m_HasPosition) return;

        if (isMoving)
        {
            if (ob.transform.parent.name == "MUR L")
            {

                ob.transform.localPosition = new Vector3(m_Pointer.transform.position.z / 10, y, z);
                if (hit.transform.name == "MUR B")
                {
                    nameM = hit.transform.name;
                  //  Debug.Log("changement de mur B ");
                  // ob.transform.parent = MurB;
                  // ob.transform.rotation = MurB.rotation;
                  // ob.transform.localScale = new Vector3(0.04165002f, 0.3106501f, 1.01f);
                    photonView.RPC("ChangeMur", Photon.Pun.RpcTarget.All, nameM);
                }

            }

            else if (ob.transform.parent.name == "MUR B")
            {
                if (hit.transform.name == "MUR L")
                {
                    // Debug.Log("changement de mur L ");
                    //ob.transform.parent = MurL;
                    //ob.transform.rotation = MurL.rotation;
                    //ob.transform.localScale = new Vector3(0.04165002f, 0.3106501f, 1.01f);
                    nameM = hit.transform.name;
                    photonView.RPC("ChangeMur", Photon.Pun.RpcTarget.All, nameM);
                }

                if (hit.transform.name == "MUR R")
                {
                    //Debug.Log("changement de mur R ");
                    //ob.transform.parent = MurR;
                    //ob.transform.rotation = MurR.rotation;
                    //ob.transform.localScale = new Vector3(0.04165002f, 0.3106501f, 1.01f);
                    nameM = hit.transform.name;
                    photonView.RPC("ChangeMur", Photon.Pun.RpcTarget.All, nameM);
                }
                ob.transform.localPosition = new Vector3(x, y, z);
            }

            else if (ob.transform.parent.name == "MUR R")
            {
                if (hit.transform.name == "MUR B")
                {
                    //Debug.Log("changement de mur B ");
                    //ob.transform.parent = MurB;
                    //ob.transform.rotation = MurB.rotation;
                    //ob.transform.localScale = new Vector3(0.04165002f, 0.3106501f, 1.01f);
                    nameM = hit.transform.name;
                    
                    photonView.RPC("ChangeMur", Photon.Pun.RpcTarget.All, nameM);
                }
                ob.transform.localPosition = new Vector3( -m_Pointer.transform.position.z / 10, y, z);
                Debug.Log("Photon.Pun.RpcTarget.All :" + Photon.Pun.RpcTarget.All);
            }
        }
    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Card" || hit.transform.tag == "Wall")
            {
               // Debug.Log("test");
                m_Pointer.transform.position = hit.point;
                return true;
            }
        }
        return false;
    }

    [PunRPC]
    void ChangeMur(string nameT)
    {
        Debug.Log("nameT :  " + nameT);


        if (nameT == "MUR L")
        {
             Debug.Log("changement de mur L ");
             ob.transform.parent = MurL;
             ob.transform.rotation = MurL.rotation;
             ob.transform.localScale = new Vector3(0.04165002f, 0.3106501f, 1.01f);
        }
        else if (nameT == "MUR B")
        {
             Debug.Log("changement de mur B ");
             ob.transform.parent = MurB;
             ob.transform.rotation = MurB.rotation;
             ob.transform.localScale = new Vector3(0.04165002f, 0.3106501f, 1.01f);
        }
        else if (nameT == "MUR R")
        {
            Debug.Log("changement de mur R ");
            ob.transform.parent = MurR;
            ob.transform.rotation = MurR.rotation;
            ob.transform.localScale = new Vector3(0.04165002f, 0.3106501f, 1.01f);

        }
    }

    }
