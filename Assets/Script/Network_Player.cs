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

    private GameObject headset;
    private GameObject right;
    private GameObject left;

    //public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");
    private PhotonView photonView;
   // private SteamVR_Behaviour_Pose m_pose = null;
    private RaycastHit hit;


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
                photonView.RPC("ChangeRayColour", Photon.Pun.RpcTarget.All, hit.transform.GetComponent<Renderer>().material);
               
            }
            if (hit.transform.tag == "Card")
            {
                hit.transform.gameObject.GetComponent<PhotonView>().RequestOwnership();
                photonView.RPC("ChangeTag", Photon.Pun.RpcTarget.All, rayCast.GetComponent<Renderer>().material);
                
               // hit.transform.GetChild(0).GetComponent<Renderer>().material = rayCast.GetComponent<Renderer>().material;
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

      void ChangeRayColour(Material m)
    {
        rayCast.GetComponent<Renderer>().material = m ;
    }

    void ChangeTag(Material m)
    {
        hit.transform.GetChild(0).GetComponent<Renderer>().material = m;
    }
}
