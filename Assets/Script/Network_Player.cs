using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;

public class Network_Player : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Transform headSphere;
    public Transform leftHandSphere;
    public Transform rightHandSphere;
    public Transform ray;
    public Transform palette;

    public Transform rayCast;

    public Material blue;
    public Material Green;
    public Material white;
    public Material red;
    public Material none;

    private GameObject headset;
    private GameObject right;
    private GameObject left;

    private PhotonView photonView;

    private RaycastHit hit;
    private string nameR="";
    private string nameT="";
    private SteamVR_Behaviour_Pose m_pose = null;
    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        headset = GameObject.Find("Camera (eye)");
        right = GameObject.Find("/[CameraRig]/Controller (right)");
        left = GameObject.Find("/[CameraRig]/Controller (left)");
        m_pose = right.GetComponent<SteamVR_Behaviour_Pose>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            leftHand.gameObject.SetActive(false);
            rightHand.gameObject.SetActive(false);
            head.gameObject.SetActive(false);
          //  headSphere.GetComponent<Renderer>().material = blue;
          //  leftHandSphere.GetComponent<Renderer>().material = blue;
          //  rightHandSphere.GetComponent<Renderer>().material = blue;
            MapPosition();
        }
        Ray ray = new Ray(right.transform.position, right.transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
           // Debug.Log(" test 1 " + hit.transform.tag);
            if (interactWithUI.GetStateDown(m_pose.inputSource) &&  hit.transform.tag == "tag")
            {
                nameR = hit.transform.GetComponent<Renderer>().material.name;
              photonView.RPC("ChangeRayColour", Photon.Pun.RpcTarget.All, nameR);
            }
            if (interactWithUI.GetStateDown(m_pose.inputSource) &&  hit.transform.tag == "Card")
            {
                nameT = rayCast.GetComponent<Renderer>().material.name;
               photonView.RPC("ChangeTag", Photon.Pun.RpcTarget.All, nameT, hit.transform.gameObject.GetComponent<PhotonView>().ViewID);
            }
        }

    }

    //void MapPosition(Transform target, XRNode node)
    void MapPosition()
    {
        palette.position = left.transform.position;
        palette.rotation = left.transform.rotation;

        leftHand.position = left.transform.position;
        leftHand.rotation = left.transform.rotation;

        rightHand.position = right.transform.position;
        rightHand.rotation = right.transform.rotation;

        ray.position = right.transform.position;
        ray.rotation = right.transform.rotation;


        head.position = headset.transform.position;
        head.rotation = headset.transform.rotation;
    }

    [PunRPC]
    void ChangeRayColour(string nameR)
    {
        //Debug.Log("ChangeRayColour /" + name + "/");
        if (nameR == "blue (Instance)")
        {
            rayCast.GetComponent<Renderer>().material = blue;
        }
        else if(nameR == "Green (Instance)")
        {
            rayCast.GetComponent<Renderer>().material = Green;
        }
        else if(nameR == "Red (Instance)")
        {
            rayCast.GetComponent<Renderer>().material = red;
        }
        else if (nameR == "white (Instance)")
        {
            rayCast.GetComponent<Renderer>().material = white;
        }
        else 
        {
            rayCast.GetComponent<Renderer>().material = none;
        }
    }

    [PunRPC]
    void ChangeTag(string nameT , int OB)
    {
        {
           // nameT = rayCast.GetComponent<Renderer>().material.name;
            // Debug.Log("ChangeRayColour /" + nameT + "/");
            if (nameT == "blue (Instance)")
              {
                PhotonView.Find(OB).gameObject.transform.GetChild(0).GetComponent<Renderer>().material = blue;
              }
              else if (nameT == "Green (Instance)")
              {
                PhotonView.Find(OB).gameObject.transform.GetChild(0).GetComponent<Renderer>().material = Green;
              }
              else if (nameT == "Red (Instance)")
              {
                PhotonView.Find(OB).gameObject.transform.GetChild(0).GetComponent<Renderer>().material = red;
              }
              else if (nameT == "white (Instance)")
              {
                PhotonView.Find(OB).gameObject.transform.GetChild(0).GetComponent<Renderer>().material = white;
              }
             else
              {
                PhotonView.Find(OB).gameObject.transform.GetChild(0).GetComponent<Renderer>().material = none;
              }
        }

    }
}
