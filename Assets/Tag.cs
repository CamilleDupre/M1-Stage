using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;


public class Tag : MonoBehaviour
{

    public GameObject m_Pointer;
    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

    private SteamVR_Behaviour_Pose m_pose = null;
    private bool m_HasPosition = false;
    private RaycastHit hit;
    private PhotonView photonView;

    private string nameR = "";
    // Start is called before the first frame update
    void Start()
    {

       // GameObject.Find("Network Player(Clone)");
        //photonView = GameObject.Find("Network Player(Clone)").GetComponent<PhotonView>();
    }
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
        if(photonView == null)
        {
            photonView = GameObject.Find("Network Player(Clone)").GetComponent<PhotonView>();
        }
       
            if (interactWithUI.GetStateDown(m_pose.inputSource) && hit.transform.tag == "Card")
        {
            //nameT = rayCast.GetComponent<Renderer>().material.name;
            photonView.RPC("ChangeTag", Photon.Pun.RpcTarget.All, hit.transform.gameObject.GetComponent<PhotonView>().ViewID);
        }

        if (interactWithUI.GetStateDown(m_pose.inputSource) && hit.transform.tag == "tag")
        {
            nameR = hit.transform.GetComponent<Renderer>().material.name;
            photonView.RPC("ChangeRayColour", Photon.Pun.RpcTarget.All, nameR);
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
            m_Pointer.gameObject.SetActive(false);
        }
        return false;
    }
}
