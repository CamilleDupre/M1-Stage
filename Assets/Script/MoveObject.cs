using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;


public class MoveObject : MonoBehaviourPun
{
    public GameObject m_Pointer;
    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

    private SteamVR_Behaviour_Pose m_pose = null;
    private bool m_HasPosition = false;

    private RaycastHit hit;
    private GameObject ob;

    public Material initialColor ;
    public Material selectedColor ;

    public Transform MurB;
    public Transform MurL;
    public Transform MurR;
    private string nameM = "";
    public Texture tex;

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

        if (ob != null) // follow the mouvement
        {
            float x, y, z;
            Vector3 v = MurR.localScale;
            Vector3 p = MurR.position;
            x = m_Pointer.transform.position.x / v.x;
            y = (m_Pointer.transform.position.y- p.y) / v.y;
            z = -0.02f;

            if (ob.transform.parent.name == "MUR L")
            {
                ob.transform.localPosition = new Vector3(m_Pointer.transform.position.z / v.x, y, z);  // /10
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
                    photonView.RPC("ChangeMur", Photon.Pun.RpcTarget.All, nameM, ob.GetComponent<PhotonView>().ViewID);
                }
                ob.transform.localPosition = new Vector3(-m_Pointer.transform.position.z / v.x, y, z);
            }
        }
        if (interactWithUI.GetStateUp(m_pose.inputSource))
        {
            Move();
        }   
    }

    private void Move()
    {
        // Debug.Log("Move");

        float x, y, z;
        Vector3 v = MurR.localScale;
        Vector3 p = MurR.position;
        x = m_Pointer.transform.position.x / v.x;
        y = (m_Pointer.transform.position.y - p.y) / v.y;
        z = -0.02f;

        if (!m_HasPosition)
            return;
        
        else if(hit.transform.tag == "Card" &&  ob == null){
            hit.transform.gameObject.GetComponent<PhotonView>().RequestOwnership();
            ob = hit.transform.gameObject;
        }
        else if(hit.transform.tag == "Wall" || hit.transform.tag == "Card")
        {
            
            if(ob != null){
                if(ob.transform.parent.name == "MUR L")
                {
                    ob.transform.localPosition = new Vector3(m_Pointer.transform.position.z / v.x, y, z);
                }
       
                else if (ob.transform.parent.name == "MUR B")
                {
                    ob.transform.localPosition = new Vector3(x, y, z);
                }

                else if (ob.transform.parent.name == "MUR R")
                {
                    ob.transform.localPosition = new Vector3(-m_Pointer.transform.position.z / v.x, y, z);
                }
     
                    ob = null;
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
