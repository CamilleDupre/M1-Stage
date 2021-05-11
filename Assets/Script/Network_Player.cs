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

    private PhotonView photonView;

    public Transform Right;
    public Transform Left;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            Vector3 headPosition = SteamVR_Render.Top().head.position;
            Transform cameraRig = SteamVR_Render.Top().origin;

          
            //leftHand.gameObject.SetActive(false);
           // rightHand.gameObject.SetActive(false);
            //head.gameObject.SetActive(false);

            MapPosition(head, cameraRig);
            MapPosition(rightHand, Right);
            MapPosition(leftHand, Left);

        }
       
    }

    void MapPosition(Transform target, Transform cameraRig)
    {
        target.position = cameraRig.position;
        target.rotation = cameraRig.rotation;
    }
}
