using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;

public class DragDrop : MonoBehaviourPun
{
    // intersecion raycast and object
    public GameObject m_Pointer;
    private bool m_HasPosition = false;
    private RaycastHit hit;

    //trigger
    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

    //Pose
    private SteamVR_Behaviour_Pose m_pose = null;
    
    // State machine
    private bool isMoving = false;
    private bool wait = false;
    private bool longclic = false;
    private float timer = 0;
    public Vector3 coordClic;
    public Vector3 forwardClic;

   //card to move
    private GameObject ob;

    //Room
    public Transform MurB;
    public Transform MurL;
    public Transform MurR;
    private GameObject salle;


    //texture card
    public Texture tex;
   
    private string nameM = "";
    private string nameR = "";
    //player
    public GameObject player;

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
        //m_Pointer.SetActive(m_HasPosition);

        if (interactWithUI.GetStateUp(m_pose.inputSource))
        {
            if (wait && ob!=null)
            {
                //just a clic -> tag
                player = GameObject.Find("Network Player(Clone)");
                player.GetComponent<PhotonView>().RPC("ChangeTag", Photon.Pun.RpcTarget.AllBuffered, hit.transform.gameObject.GetComponent<PhotonView>().ViewID);
            }
            isMoving = false;
            ob = null;
            wait = false;
            longclic = false;
            Debug.Log("reset");
            timer = 0;
        }

        if (interactWithUI.GetStateDown(m_pose.inputSource))
        {
            if (hit.transform.tag == "Card") {
                //request multi user
                hit.transform.gameObject.GetComponent<PhotonView>().RequestOwnership();
                //card
                ob = hit.transform.gameObject;
            }
           

            //where is the clic
            coordClic = hit.transform.position;
            forwardClic = transform.forward;
            //Debug.Log(coordClic);

            //start waiting
            wait = true;
            timer = Time.time;

        }

        if (ob != null && wait && Vector3.Angle(forwardClic, transform.forward) > 2) //move more than 2* -> moving
        {
            isMoving = true;
            wait = false;
        }

        if (wait)
        {
            if (Time.time - timer > 2) //  after 2s it is long clic
            {
               longclic = true;
               wait = false;
               Debug.Log("long clic");
            }
        }
        if (longclic && hit.transform.tag == "Wall")
        {
            //maybe teleport card on the wall or menuhit.transform.tag == "Wall")
            {
                salle = GameObject.Find("Salle");
                salle.GetComponent<PhotonView>().RPC("TeleportCard", Photon.Pun.RpcTarget.All, nameR, hit.transform.name);
            }
        }
            Move();
    }

    private void Move()
    {
        float x, y, z;
        Vector3 v = MurR.localScale;
        Vector3 p = MurR.position;

        x = m_Pointer.transform.position.x / v.x;
        y = (m_Pointer.transform.position.y - p.y) / v.y;
        z = -0.02f;

        if (!m_HasPosition) { return; }

        if (isMoving)
        {
            //check the wall and if the card from one wall to another
            if (ob.transform.parent.name == "MUR L")
            {
                //change
                if (hit.transform.name == "MUR B")  {   nameM = hit.transform.name; }
                //move on L
                ob.transform.localPosition = new Vector3(m_Pointer.transform.position.z / v.x, y, z);  // /10
            }

            else if (ob.transform.parent.name == "MUR B")
            {
                //change
                if (hit.transform.name == "MUR L")  {   nameM = hit.transform.name; }

                if (hit.transform.name == "MUR R")  {   nameM = hit.transform.name; }
                // move on B
                ob.transform.localPosition = new Vector3(x, y, z);
            }

            else if (ob.transform.parent.name == "MUR R")
            {
                //change
                if (hit.transform.name == "MUR B")  {   nameM = hit.transform.name; }
                //move on R
                ob.transform.localPosition = new Vector3( -m_Pointer.transform.position.z / v.x, y, z);
            }

            //if change then rpc change wall
            if (nameM != "")
            {
                photonView.RPC("ChangeMur", Photon.Pun.RpcTarget.All, nameM, ob.GetComponent<PhotonView>().ViewID);
                nameM = "";
            }
        }
    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        //check if there is a hit
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

    [PunRPC]
    void ChangeMur(string nameT, int OB)
    {
        float w, h;
        float div = 2 * 1000f;
        Transform Mur;


        //what wall
        if (nameT == "MUR L")
        {
            Mur = MurL;
        }
        else if (nameT == "MUR B")
        {
            Mur = MurB;
        }
        else// if (nameT == "MUR R")
        {
            Mur = MurR;

        }

        //teleport the card
        Vector3 v = Mur.localScale;
        h = tex.height / div;
        w = tex.width / div;

        w = w * (v.y / v.x);
        Debug.Log("Changement de mur ");
        PhotonView.Find(OB).gameObject.transform.parent = Mur;
        PhotonView.Find(OB).gameObject.transform.rotation = Mur.rotation;

        PhotonView.Find(OB).gameObject.transform.localScale = new Vector3(w, h, 1.0f);
    }

    [PunRPC]
    void RayColour(string name)
    {
        nameR = name;
    }

}
