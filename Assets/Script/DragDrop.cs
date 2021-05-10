using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DragDrop : MonoBehaviour
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
            Move();
    }

    private void Move()
    {
      Debug.Log("Move");
        if (!m_HasPosition)
            return;
        else if(hit.transform.tag == "Card" &&  isMoving == false){
          isMoving = true;
          hit.transform.localPosition = new Vector3(m_Pointer.transform.position.x / 10, (m_Pointer.transform.position.y -1) / 2, -0.02f);
        }

        else if(hit.transform.tag == "Card" &&  isMoving == true){
          isMoving = false;

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
                return true;
            }
        }
        return false;
    }
}
