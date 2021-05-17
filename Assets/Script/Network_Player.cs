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

    public Transform pointer;

    public Material blue;

    private GameObject headset;
    private GameObject right;
    private GameObject left;

    private PhotonView photonView;
    public SteamVR_Input_Sources inputSource;
    public SteamVR_Input_Sources inputSource2;
    public SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");


    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        headset = GameObject.Find("Camera (eye)");
       // headset = GameObject.Find("/[CameraRig]/Camera");
        right = GameObject.Find("/[CameraRig]/Controller (right)");
        left = GameObject.Find("/[CameraRig]/Controller (left)");
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            //Vector3 headPosition = SteamVR_Render.Top().head.position;
            // Transform cameraRig = SteamVR_Render.Top().origin;


            //leftHand.gameObject.SetActive(false);
            // rightHand.gameObject.SetActive(false);
            //head.gameObject.SetActive(false);

            /*
            MapPosition(head, XRNode.Head);
            MapPosition(rightHand, XRNode.RightHand);
            MapPosition(leftHand, XRNode.LeftHand);
            */
            headSphere.GetComponent<Renderer>().material = blue;
            leftHandSphere.GetComponent<Renderer>().material = blue;
            rightHandSphere.GetComponent<Renderer>().material = blue;
            pointer.GetComponent<Renderer>().material = blue;
            MapPosition();
        }
       
    }

    //void MapPosition(Transform target, XRNode node)
    void MapPosition()
    {
        /*  InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
          InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);
          target.position = position;
          target.rotation = rotation;
        */

        // m_pose.inputSource
        // head.position = SteamVR_Render.Top().origin.position;
        // head.rotation = SteamVR_Render.Top().origin.rotation;
        //rightHand.position = poseAction[inputSource].localPosition;
        //rightHand.rotation = poseAction[inputSource].localRotation;

        leftHand.position = left.transform.position;
        leftHand.rotation = left.transform.rotation;

        rightHand.position = right.transform.position;
        rightHand.rotation = right.transform.rotation;

        
        head.position = headset.transform.position;
        head.rotation = headset.transform.rotation;
    }
}
