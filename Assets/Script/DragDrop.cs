using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;

public class DragDrop : MonoBehaviourPun
{
    public GameObject m_Pointer;
    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

    private SteamVR_Behaviour_Pose m_pose = null;
    private bool m_HasPosition = false;
    private bool isMoving = false;
    private bool wait = false;
    private bool longclic = false;

    private RaycastHit hit;
    private GameObject ob;

    public Material initialColor ;
    public Material selectedColor ;

    public Transform MurB;
    public Transform MurL;
    public Transform MurR;
    public Texture tex;
    public Vector3 coordClic;
    public int timer = 0;

    private string nameM = "";

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
            if (wait)
            {
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

        if (interactWithUI.GetStateDown(m_pose.inputSource) && hit.transform.tag == "Card")
        {
            hit.transform.gameObject.GetComponent<PhotonView>().RequestOwnership();
            //isMoving = true;
            ob = hit.transform.gameObject;
            coordClic = hit.transform.position;
            Debug.Log(coordClic);
            wait = true;

        }

        if (wait && Vector3.Distance(coordClic, hit.transform.position) > 0.000001 )
        {
            isMoving = true;
            wait = false;
        }

        if (wait)
        {
            timer++; 
            if (timer > 500)
            {
               longclic = true;
               wait = false;
                Debug.Log("long clic");
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
            if (ob.transform.parent.name == "MUR L")
            {
                ob.transform.localPosition = new Vector3(m_Pointer.transform.position.z / v.x , y, z);  // /10
                if (hit.transform.name == "MUR B")
                {
                    nameM = hit.transform.name;
                    photonView.RPC("ChangeMur", Photon.Pun.RpcTarget.All, nameM, ob.GetComponent<PhotonView>().ViewID);
                }
            }

            else if (ob.transform.parent.name == "MUR B")
            {
                if (hit.transform.name == "MUR L")
                {
                    nameM = hit.transform.name;
                    photonView.RPC("ChangeMur", Photon.Pun.RpcTarget.All, nameM, ob.GetComponent<PhotonView>().ViewID);
                }

                if (hit.transform.name == "MUR R")
                {
                    nameM = hit.transform.name;
                    photonView.RPC("ChangeMur", Photon.Pun.RpcTarget.All, nameM, ob.GetComponent<PhotonView>().ViewID);
                }
                ob.transform.localPosition = new Vector3(x, y, z);
            }

            else if (ob.transform.parent.name == "MUR R")
            {
                if (hit.transform.name == "MUR B")
                {
                    nameM = hit.transform.name;
                    photonView.RPC("ChangeMur", Photon.Pun.RpcTarget.All, nameM , ob.GetComponent<PhotonView>().ViewID);
                }
                ob.transform.localPosition = new Vector3( -m_Pointer.transform.position.z / v.x, y, z);
            }
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

    [PunRPC]
    void ChangeMur(string nameT, int OB)
    {
        Debug.Log("nameT :  " + nameT);

        float w, h;
        float div = 2 * 1000f;
       
        if (nameT == "MUR L")
        {

            Vector3 v = MurL.localScale;
            h = tex.height / div;
            w = tex.width / div;

            w = w * (v.y / v.x);
            Debug.Log("changement de mur L ");
            PhotonView.Find(OB).gameObject.transform.parent = MurL;
            PhotonView.Find(OB).gameObject.transform.rotation = MurL.rotation;

            PhotonView.Find(OB).gameObject.transform.localScale = new Vector3(w, h, 1.0f);
        }
        else if (nameT == "MUR B")
        {
            Vector3 v = MurB.localScale;
           
            h = tex.height / div;
            w = tex.width / div;
            w = w * (v.y / v.x);

            Debug.Log("changement de mur B ");
            PhotonView.Find(OB).transform.parent = MurB;
            PhotonView.Find(OB).transform.rotation = MurB.rotation;

            PhotonView.Find(OB).transform.localScale = new Vector3(w, h, 1.0f);
        }
        else if (nameT == "MUR R")
        {
            Vector3 v = MurR.localScale;
             Debug.Log("R scale: " + v);
            h = tex.height / div;
            w = tex.width / div;
            w = w * (v.y / v.x);
            Debug.Log("changement de mur R ");
            PhotonView.Find(OB).transform.parent = MurR;
            PhotonView.Find(OB).transform.rotation = MurR.rotation;

            PhotonView.Find(OB).transform.localScale = new Vector3(w, h, 1.0f);

        }
    }

    }
