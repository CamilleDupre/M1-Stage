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
      //  interactWithUI.GetStateUp(pose.inputSource)
            tryMove();
    }

    private void tryMove()
    {
      Debug.Log("Move");
        if (!m_HasPosition)
            return;
        else if(hit.transform.tag == "Card"){
            ob = hit.transform.gameObject;
            ob.GetComponent<Renderer>().material = selectedColor;
            //MoveCard();
        }
        else if(hit.transform.tag == "Wall"){
        float x,y,z;
        x =  m_Pointer.transform.position.x / 10;
        y =  (m_Pointer.transform.position.y -1) / 2  ; //devant le mur
        //5.21f/2 +
        z = -0.02f;
            if(ob != null){
           ob.transform.localPosition = new Vector3(x, y, z); // ici pb
        //  ob.transform.localPosition = new Vector3(0, 0, 0);
            ob.GetComponent<Renderer>().material = initialColor;
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
