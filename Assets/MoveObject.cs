using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class MoveObject : MonoBehaviour
{
    public GameObject m_Pointer;
    //public SteamVR_Action_Boolean m_TeleportAction;
    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

    private SteamVR_Behaviour_Pose m_pose = null;
    private bool m_HasPosition = false;
    private bool m_IsMoving = false;
    public SteamVR_Action_Boolean grabAction;
    //public SteamVR_Action_Boolean m_TeleportAction;
    private RaycastHit hit;
    private RaycastHit hit2;
    private GameObject ob;

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
      //  interactWithUI.GetStateUp(pose.inputSource)
            tryMove();
    }

    private void tryMove()
    {
      Debug.Log("Move");
        if (!m_HasPosition)
            return;
        if(hit.transform.tag == "Card"){
            ob = hit.transform.gameObject;
            MoveCard();
        }
        if(hit.transform.tag == "Wall"){
        float x;
        float y;
        float z;
        x = m_Pointer.transform.position.x;
        y = 0.02f;
        z = m_Pointer.transform.position.z;
            if(ob != null){
            ob.transform.localPosition = new Vector3(-x, y, 0); // ici pb Z
            ob = null;
          }
        }
    }

    private void MoveCard ()
    {
        if(ob == null){
      //  hit2.transform.localPosition = new Vector3(0, 0, 0);
        //Debug.Log(m_Pointer.transform.position);
        //hit.transform.localPosition = new Vector3(0, m_Pointer.transform.position.y, m_Pointer.transform.position.z);
      //  hit.transform.SetParent(m_Pointer.transform);
        //transform.localPosition = new Vector3(0, 1, 0);
          m_IsMoving = true;
      }
      else
      m_IsMoving = false;

    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position, transform.forward);


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
