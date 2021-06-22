using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;

public class Network_Player : MonoBehaviour
{
    // empty
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    public Transform torse;
    public Transform ray;
    public Transform pos;

    //avatar
    public Transform headSphere;
    public Transform leftHandSphere;
    public Transform rightHandSphere;
    public Transform palette;
    public Transform rayCast;
    public Transform circle;

    //Tag color
    public Material blue;
    public Material Green;
    public Material white;
    public Material red;
    public Material none;

    //camera tracker
    private GameObject headset;
    private GameObject right;
    private GameObject left;

    //room + wall
    private GameObject salle;

    private PhotonView photonView;

    private RaycastHit hit;
    private string nameR="";
    private string nameT="";
    private SteamVR_Behaviour_Pose m_pose = null;
    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

    private bool synctag = true;


    void Start()
    {
        photonView = GetComponent<PhotonView>();

        //room + wall + camera
        headset = GameObject.Find("Camera (eye)");
        right = GameObject.Find("/[CameraRig]/Controller (right)");
        left = GameObject.Find("/[CameraRig]/Controller (left)");

        salle = GameObject.Find("Salle");

        m_pose = right.GetComponent<SteamVR_Behaviour_Pose>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            //don't show my avatar
            leftHand.gameObject.SetActive(false);
           // rightHand.gameObject.SetActive(false);
            head.gameObject.SetActive(false);
            torse.gameObject.SetActive(false);

            rightHandSphere.GetComponent<Renderer>().material = red;

            //but send the position and rotation over the network
            MapPosition();
        }
        leftHand.gameObject.SetActive(false);
        Ray ray = new Ray(right.transform.position, right.transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
       
            //change tag color of the ray cast
            if (interactWithUI.GetStateDown(m_pose.inputSource) && hit.transform.tag == "tag")
            {
                //nameR = hit.transform.GetComponent<Renderer>().material.name;
                //photonView.RPC("ChangeRayColour", Photon.Pun.RpcTarget.All, nameR);
                //right.GetComponent<PhotonView>().RPC("RayColour", Photon.Pun.RpcTarget.All, nameR);

                if (!synctag && photonView.IsMine)
                {
                    // rayCast.GetComponent<Renderer>().material = hit.transform.GetComponent<Renderer>().material;
                    nameR = hit.transform.GetComponent<Renderer>().material.name;
                    ChangeRayColour(nameR);
                    Debug.Log("tag not sync");
                }
                else
                {
                    nameR = hit.transform.GetComponent<Renderer>().material.name;
                    photonView.RPC("ChangeRayColour", Photon.Pun.RpcTarget.All, nameR);
                    right.GetComponent<PhotonView>().RPC("RayColour", Photon.Pun.RpcTarget.All, nameR);
                    Debug.Log("tag sync");
                }
            }
           
            //teleport the card tag with the color of the ray cast
            if (interactWithUI.GetStateDown(m_pose.inputSource) && hit.transform.tag == "Wall")
            {
                nameT = rayCast.GetComponent<Renderer>().material.name;
               // salle.transform.GetComponent<PhotonView>().RPC("TeleportCard", Photon.Pun.RpcTarget.All, nameT , hit.transform.name);
            }
        }
    }

    //void MapPosition(Transform target, XRNode node)
    void MapPosition()
    {
        // left hand 
        palette.position = left.transform.position;
        palette.rotation = left.transform.rotation;

        //leftHand.position = left.transform.position;
        //leftHand.rotation = left.transform.rotation;
        //left.gameObject.SetActive(false);

        // right hand
        rightHand.position = right.transform.position;
        rightHand.rotation = right.transform.rotation;

        ray.position = right.transform.position;
        ray.rotation = right.transform.rotation;

        // head
        head.position = headset.transform.position;
        head.rotation = headset.transform.rotation;

        // circle
        circle.position = new Vector3(headset.transform.position.x, 0 , headset.transform.position.z);

        // body
        torse.position = headset.transform.position;
    }

    [PunRPC]
    void ChangeRayColour(string nameR)
    {
       // change the ray color 
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
    void ChangeTag( int OB)
    {
        {
            // change the tag color of a picture

            nameT = rayCast.GetComponent<Renderer>().material.name;
            
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

    [PunRPC]
    void tagMode(bool tag)
    {
        Debug.Log("Change tag mode");
        if (tag)
        {
            synctag = false;
            //teleporationMode = "Not syncro";
        }
        else
        {
            synctag = true;
            //teleporationMode = "Syncro";
        }
        Debug.Log(synctag);
    }
}
