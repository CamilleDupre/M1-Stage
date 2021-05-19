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

    public Material blue;

    private GameObject headset;
    private GameObject right;
    private GameObject left;

    private Ray m_Ray;

    private PhotonView photonView;

    // Start is called before the first frame update
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
           // leftHand.gameObject.SetActive(false);
           // rightHand.gameObject.SetActive(false);
            head.gameObject.SetActive(false);

            headSphere.GetComponent<Renderer>().material = blue;
            leftHandSphere.GetComponent<Renderer>().material = blue;
            rightHandSphere.GetComponent<Renderer>().material = blue;
            //pointer.GetComponent<Renderer>().material = blue;
            MapPosition();
        }
       
    }

    //void MapPosition(Transform target, XRNode node)
    void MapPosition()
    {
        //Ray raycast = new Ray(transform.position, transform.forward);
        //RaycastHit hit;

        m_Ray = new Ray(transform.position, transform.forward);
        // ray.position = right.transform.position;
        //ray.localPosition= new Vector3(ray.position.x +1, ray.position.y, ray.position.z);
        //ray.rotation = right.transform.forward;

        leftHand.position = left.transform.position;
        leftHand.rotation = left.transform.rotation;

        rightHand.position = right.transform.position;
        rightHand.rotation = right.transform.rotation;

        
        head.position = headset.transform.position;
        head.rotation = headset.transform.rotation;
    }
}
