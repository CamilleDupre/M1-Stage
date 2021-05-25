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

    private GameObject headset;
    private GameObject right;
    private GameObject left;

    //public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");
    private PhotonView photonView;
   // private SteamVR_Behaviour_Pose m_pose = null;
    private RaycastHit hit;
    private string nameR="";
    private string nameT="";
    private GameObject ob;




    void Start()
    {
        photonView = GetComponent<PhotonView>();

        headset = GameObject.Find("Camera (eye)");
        right = GameObject.Find("/[CameraRig]/Controller (right)");
        left = GameObject.Find("/[CameraRig]/Controller (left)");
    }

    // Update is called once per frame
    void Update()
    {
       

        if (photonView.IsMine)
        {
            leftHand.gameObject.SetActive(false);
            rightHand.gameObject.SetActive(false);
            head.gameObject.SetActive(false);

            headSphere.GetComponent<Renderer>().material = blue;
            leftHandSphere.GetComponent<Renderer>().material = blue;
            rightHandSphere.GetComponent<Renderer>().material = blue;
            //rayCast.GetComponent<Renderer>().material = blue;
            //pointer.GetComponent<Renderer>().material = blue;
            MapPosition();
        }
        Ray ray = new Ray(right.transform.position, right.transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
           // Debug.Log(" test 1 " + hit.transform.tag);
            if (hit.transform.tag == "tag")
            {
                nameR = hit.transform.GetComponent<Renderer>().material.name;
                photonView.RPC("ChangeRayColour", Photon.Pun.RpcTarget.All, nameR);
                //rayCast.GetComponent<Renderer>().material = hit.transform.GetComponent<Renderer>().material;

            }
            if (hit.transform.tag == "Card")
            {
                //hit.transform.GetChild(0).gameObject.GetComponent<PhotonView>().RequestOwnership();
                hit.transform.gameObject.GetComponent<PhotonView>().RequestOwnership();
                // photonView.RPC("ChangeTag", Photon.Pun.RpcTarget.All, blue);

                //hit.transform.GetChild(0).GetComponent<Renderer>().material = rayCast.GetComponent<Renderer>().material;
                ob = hit.transform.gameObject;
                nameT = rayCast.GetComponent<Renderer>().material.name;
                photonView.RPC("ChangeTag", Photon.Pun.RpcTarget.All, nameT);


                //rayCast.GetComponent<Renderer>().material = red;
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
        else
        {
            rayCast.GetComponent<Renderer>().material = white;
        }
    }

    [PunRPC]
    void ChangeTag(string nameT)
    {
        //Debug.Log("ChangeTag ");

        //if (ob.transform.tag == "Card")
        //{

            /*  if (nameT == "blue (Instance)")
              {
                  ob.transform.GetChild(0).GetComponent<Renderer>().material = blue;
              }
              else if (nameT == "Green (Instance)")
              {
                  ob.transform.GetChild(0).GetComponent<Renderer>().material = Green;
              }
              else if (nameT == "Red (Instance)")
              {
                  ob.transform.GetChild(0).GetComponent<Renderer>().material = red;
              }
              else
              {
                  ob.transform.GetChild(0).GetComponent<Renderer>().material = white;
              }
              */
            ob.transform.GetChild(0).GetComponent<Renderer>().material = red;
            ob = null;
        //}
    }
}
