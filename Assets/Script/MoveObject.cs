using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;


public class MoveObject : MonoBehaviourPun
{
    public GameObject m_Pointer;
    //public SteamVR_Action_Boolean m_TeleportAction;
    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

    private SteamVR_Behaviour_Pose m_pose = null;
    private bool m_HasPosition = false;


    private RaycastHit hit;
    private GameObject ob;

    public Material initialColor ;
    public Material selectedColor ;


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
        if (ob != null) // follow the mouvement
        {
            float x, y, z;
            x = m_Pointer.transform.position.x / 10;
            y = (m_Pointer.transform.position.y - 1) / 2;
            z = -0.02f;
            ob.transform.localPosition = new Vector3(x, y, z);
        }
        if (interactWithUI.GetStateUp(m_pose.inputSource))
            Move();
    }

    private void Move()
    {
      Debug.Log("Move");

        float x, y, z;
        x = m_Pointer.transform.position.x / 10;
        y = (m_Pointer.transform.position.y - 1) / 2;
        z = -0.02f;


        if (!m_HasPosition)
            return;
        
        else if(hit.transform.tag == "Card" &&  ob == null){
            hit.transform.gameObject.GetComponent<PhotonView>().RequestOwnership();
            //hit.transform.localPosition = new Vector3(m_Pointer.transform.position.x / 10, (m_Pointer.transform.position.y -1) / 2, -0.02f);
            ob = hit.transform.gameObject;
           // ob.GetComponent<Renderer>().material = selectedColor;
         
        }
        else if(hit.transform.tag == "Wall" || hit.transform.tag == "Card")
        {
            
            if(ob != null){
                ob.transform.localPosition = new Vector3(x, y, z); // ici pb
                //ob.GetComponent<Renderer>().material = initialColor;
                ob = null;
          }
        }

        else if (ob != null) // follow the mouvement
        {
            ob.transform.localPosition = new Vector3(x, y, z);
        }
    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Card" || hit.transform.tag == "Wall")
            {
                hit.transform.gameObject.GetComponent<PhotonView>().RequestOwnership();
                m_Pointer.transform.position = hit.point;
                return true;
            }
        }
        return false;
    }
}
