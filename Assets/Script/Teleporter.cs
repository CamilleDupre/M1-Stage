using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;
using UnityEngine.UI;

public class Teleporter : MonoBehaviour
{
    public GameObject Menu;
    public Text m_text;
    // intersecion raycast and object
    public GameObject m_Pointer;
    private bool m_HasPosition = false;
    RaycastHit hit;

    //clic touchpad
    public SteamVR_Action_Boolean m_TeleportAction;
    //public SteamVR_Action_Boolean m_up;
    //public SteamVR_Action_Boolean m_down;

    //Pose
    private SteamVR_Behaviour_Pose m_pose = null;

    //Teleportation parameters 
    private bool m_IsTeleportoting = false;
    private float m_FadeTime = 0.5f;
    
    // State machine
    private bool wait = false;
    private bool isMoving = false;
    private bool longclic = false;
    private Vector3 coordClic;
    private Vector3 coordPrev;
    public Vector3 forwardClic;
    public float timer = 0;

    private bool syncTeleportation = false;
    private string teleporationMode = "Not syncro";
    float desiredDistance = 1;


    private bool n = false;
    private bool s = false;
    private bool e = false;
    private bool w = false;
    private bool synctag = true;

    public Transform character;

    Vector2 position;


    private PhotonView photonView;
    //player
    public GameObject player;


    // Start is called before the first frame update
    void Awake()
    {
        m_pose = GetComponent<SteamVR_Behaviour_Pose>();
        photonView = GetComponent<PhotonView>();
        Menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Pointer
        m_HasPosition = UpdatePointer();
        //m_text.text = "Teleportation mode : \n" + teleporationMode;

        m_text.text = "tag mode : \n" + synctag;
        //m_Pointer.SetActive(m_HasPosition);

        //Teleport


        /*if (m_up.GetStateDown(m_pose.inputSource))
        {
            //Debug.Log("up ");
        }
        else if (m_down.GetStateDown(m_pose.inputSource))
        {
           // Debug.Log("donw ");
        }
        else*/
        position = SteamVR_Actions.default_Pos.GetAxis(SteamVR_Input_Sources.Any);


        if (m_TeleportAction.GetStateDown(m_pose.inputSource))
        {

            if (position.y > 0.5)
            {
                Debug.Log("N");
                //cameraRig.transform.position += character.transform.forward * desiredDistance;
                n = true;
                tryTeleport();
            }
            else if (position.y < -0.5)
            {
                Debug.Log("S");
                //cameraRig.transform.position -= character.transform.forward * desiredDistance;
                s = true;
                tryTeleport();
            }
            else if (position.x > 0.5)
            {
                Debug.Log("E");
                e = true;
                tryTeleport();
            }
            else if (position.x < -0.5)
            {
                Debug.Log("W");
                w = true;
                tryTeleport();
            }
            else
            {
                Debug.Log("C");
                coordClic = coordPrev = m_Pointer.transform.position; //hit.transform.position;
                forwardClic = transform.forward;
                Debug.Log("coordClic : " + coordClic);
                wait = true;
                timer = Time.time;
            }
        }
       


        if (m_TeleportAction.GetStateUp(m_pose.inputSource))
        {
            if (wait)
            { 
                //just a clic -> normal teleportation
                tryTeleport();
            }
           //Debug.Log("reset");
            wait = false;
            isMoving = false;
            timer = 0;
            longclic = false;
            n = false;
            s = false;
            e = false;
            w = false;


}

        // check the angle to detect a mouvement
        if (wait && Vector3.Angle(forwardClic, transform.forward) > 2)
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
                //syncTeleportation = !syncTeleportation;
                Menu.SetActive(true);
                }
        }

        if (isMoving)
        {
            //dragTeleport(coordPrev, m_Pointer.transform.position);
            coordPrev = m_Pointer.transform.position;
        }

        if (UpdatePointer() == true && hit.transform.name == "syncro")
        {
           // Debug.Log("Syncro");
            Menu.SetActive(false);
            //syncTeleportation = true;
            photonView.RPC("teleportationMode", Photon.Pun.RpcTarget.All, syncTeleportation);
            //teleporationMode = "Syncro";
        }

        if (UpdatePointer() == true && hit.transform.name == "not syncro")
        {
           // Debug.Log("Not Syncro");
            Menu.SetActive(false);
            //syncTeleportation = false;
            photonView.RPC("teleportationMode", Photon.Pun.RpcTarget.All, syncTeleportation);
           // teleporationMode = "Not syncro";
        }

        if (UpdatePointer() == true && hit.transform.name == "syncro tag")
        {
            // Debug.Log("Syncro");
            Menu.SetActive(false);
            player = GameObject.Find("Network Player(Clone)");
            synctag = true;
            player.GetComponent<PhotonView>().RPC("tagMode", Photon.Pun.RpcTarget.AllBuffered, "syncro tag");
            photonView.RPC("tagMode", Photon.Pun.RpcTarget.All, synctag);

        }

        if (UpdatePointer() == true && hit.transform.name == "not syncro tag")
        {
            // Debug.Log("Not Syncro");
            Menu.SetActive(false);
            player = GameObject.Find("Network Player(Clone)");
           synctag = false;
            player.GetComponent<PhotonView>().RPC("tagMode", Photon.Pun.RpcTarget.AllBuffered, "not syncro tag");
           photonView.RPC("tagMode", Photon.Pun.RpcTarget.All, synctag);

        }

        if (UpdatePointer() == true && hit.transform.name == "cancel")
        {
           // Debug.Log("Cancel");
            Menu.SetActive(false);
        }



    }

    public void clic()
    {
        Debug.Log("clic");
    }

    /* try drag wall
    private void dragTeleport(Vector3 prev, Vector3 curr)
    {
        if (!m_HasPosition || m_IsTeleportoting)
            return;

        Vector3 headPosition = SteamVR_Render.Top().head.position;
        Transform cameraRig = SteamVR_Render.Top().origin;

        Vector3 delta = new Vector3();
        delta = curr - prev;

        if (hit.transform.tag == "Wall" || hit.transform.tag == "Card")
        {
            if (hit.transform.name == "MUR B" || hit.transform.parent.name == "MUR B")
            {

                Vector3 translation = new Vector3(-delta.x, 0, 0);
                cameraRig.position = cameraRig.position + translation;
                Debug.Log("drag mur b " + translation);

            }
            else if (hit.transform.name == "MUR R" || hit.transform.parent.name == "MUR R")
            {
                Vector3 translation = new Vector3(0, 0, -delta.z);
                cameraRig.position = cameraRig.position + translation;
                Debug.Log("drag mur r " + translation);

            }
            else if (hit.transform.name == "MUR L" || hit.transform.parent.name == "MUR L")
            {
                Vector3 translation = new Vector3(0, 0, -delta.z);
                cameraRig.position = cameraRig.position + translation;
                Debug.Log("drag mur l " + translation);

            }
        }
    }
    */

    private void tryTeleport()
    {
        //if no hit stop the fonction
        if (!m_HasPosition || m_IsTeleportoting)
            return;

        // head position + camera rig
        Vector3 headPosition = SteamVR_Render.Top().head.position;
        Transform cameraRig = SteamVR_Render.Top().origin;
        
        //player possition
        Vector3 groundPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);

        Vector3 translateVector;
        if (n)
        {
            //translateVector =  character.transform.forward * desiredDistance; // y not fix
            translateVector = new Vector3(character.transform.forward.x * desiredDistance, cameraRig.position.y, character.transform.forward.z * desiredDistance);  //  y fix
            StartCoroutine(MoveRig(cameraRig, translateVector));
        }
        else if (s)
            {
            //translateVector =  - character.transform.forward * desiredDistance;
            translateVector = new Vector3(- character.transform.forward.x * desiredDistance, cameraRig.position.y, - character.transform.forward.z * desiredDistance);  //  y fix
            StartCoroutine(MoveRig(cameraRig, translateVector));
        }
        else if (e)
        {
            //translateVector =  character.transform.forward * desiredDistance;
            //translateVector = new Vector3(character.transform.right.x * desiredDistance, cameraRig.position.y, character.transform.right.z * desiredDistance);  //  y fix
            //StartCoroutine(MoveRig(cameraRig, translateVector));
            cameraRig.Rotate(0.0f, 90.0f, 0.0f, Space.World);
        }
        else if (w)
        {
            //translateVector =  - character.transform.forward * desiredDistance;
            //translateVector = new Vector3(-character.transform.right.x * desiredDistance, cameraRig.position.y, -character.transform.right.z * desiredDistance);  //  y fix
            //StartCoroutine(MoveRig(cameraRig, translateVector));
            cameraRig.Rotate( 0.0f,  -90.0f, 0.0f, Space.World);
        }
        else if (hit.transform.tag == "Tp" )
        {
            translateVector = m_Pointer.transform.position - groundPosition;

            if (!syncTeleportation)
            {
                StartCoroutine(MoveRig(cameraRig, translateVector));
            }
            else {
                photonView.RPC("MoveRig2", Photon.Pun.RpcTarget.All, cameraRig.gameObject.GetComponent<PhotonView>().ViewID, translateVector);
            }
            
        }

        else if (hit.transform.tag == "Wall" || hit.transform.tag == "Card")
        {
            //check the wall
            if (hit.transform.name == "MUR B" || hit.transform.parent.name == "MUR B")
            {
                 translateVector = new Vector3(m_Pointer.transform.position.x - groundPosition.x, 0, 0);
            }
            else if (hit.transform.name == "MUR R" || hit.transform.parent.name == "MUR R")
            { 
                 translateVector = new Vector3(0, 0, m_Pointer.transform.position.z - groundPosition.z);
            }
            else //(hit.transform.name == "MUR L" || hit.transform.parent.name == "MUR L")
            {
                 translateVector = new Vector3(0, 0, m_Pointer.transform.position.z - groundPosition.z); 
            }
            //then teleport
            // 
            Debug.Log("Camera " +cameraRig.GetComponent<PhotonView>());
            if (!syncTeleportation)
            {
                StartCoroutine(MoveRig(cameraRig, translateVector));
            }
            else
            {
                photonView.RPC("MoveRig2", Photon.Pun.RpcTarget.All, cameraRig.gameObject.GetComponent<PhotonView>().ViewID, translateVector);
            }
        }
    }

    [PunRPC]
    void MoveRig2(int cameraRig, Vector3 translation)
    {
        Debug.Log("test");
        StartCoroutine(MoveRig(PhotonView.Find(cameraRig).transform, translation));
    }

    [PunRPC]
    void teleportationMode(bool tp)
    {
        Debug.Log("Change teleportation mode");
        if (tp)
        {
            syncTeleportation = false;
            teleporationMode = "Not syncro";
        }
        else
        {
            syncTeleportation = true;
            teleporationMode = "Syncro";
        }
        
    }
    [PunRPC]
    void tagMode(bool tag)
    {
       // Debug.Log("Change tag mode");
        synctag = tag;
        Debug.Log("teleport tag : "+synctag);
    }

    private IEnumerator MoveRig(Transform cameraRig , Vector3 translation)
    {
        m_IsTeleportoting = true;

        SteamVR_Fade.Start(Color.black, m_FadeTime, true); // black screen

        yield return new WaitForSeconds( m_FadeTime); // fade time
        
        cameraRig.position += translation; // teleportation

        SteamVR_Fade.Start(Color.clear, m_FadeTime, true); // normal screen

        m_IsTeleportoting = false;

    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position, transform.forward);
       
        //check if there is a hit
        if(Physics.Raycast(ray , out hit) )
        {
            if (hit.transform.tag == "Tp" || hit.transform.tag == "Card" || hit.transform.tag == "Wall" || hit.transform.tag == "tag")
            {
                m_Pointer.transform.position = hit.point;
                return true;
                
            }
        }
        return false;
    }
}
