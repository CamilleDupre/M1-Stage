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
            Debug.Log("test 2");
            isMoving = false;
            ob = null;
        }
        if (interactWithUI.GetStateDown(m_pose.inputSource)) Debug.Log("test 23");
      


        if (interactWithUI.GetStateDown(m_pose.inputSource) && hit.transform.tag == "Card")
        {
            hit.transform.gameObject.GetComponent<PhotonView>().RequestOwnership();
            isMoving = true;
            ob = hit.transform.gameObject;
            Debug.Log("Move");
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

        if(isMoving)
         ob.transform.localPosition = new Vector3(x, y, z);
    
    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Card" || hit.transform.tag == "Wall")
            {
                Debug.Log("test");
                m_Pointer.transform.position = hit.point;
                return true;
            }
            else if (hit.transform.tag == "baton" ){ Debug.Log("baton"); }
            else { Debug.Log("rien : " + hit.transform.name); }
        }
        return false;
    }
}
