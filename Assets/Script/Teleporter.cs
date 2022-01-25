using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;
using UnityEngine.UI;

public class Teleporter : MonoBehaviour
{
    public GameObject Menu;

    public GameObject tpsync;
    public GameObject tpNotsync;
    public GameObject tagsync;
    public GameObject tagNotsync;

    public GameObject Cube;
    public GameObject CubePlayer;

    // intersecion raycast and object
    public GameObject m_Pointer;
    private bool m_HasPosition = false;
    RaycastHit hit;
    RaycastHit[] objectHit;
 

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
    private bool doubleclick = false;
    private Vector3 coordClic;
    private Vector3 coordPrev;
    private Vector3 forwardClic;
    private float timer = 0;

    public bool syncTeleportation = false;
    private string teleporationMode = "Not syncro";
    float desiredDistance = 1;


    private bool n = false;
    private bool s = false;
    private bool e = false;
    private bool w = false;
    
    public bool synctag = true;
   

    int nbClick = 0;

    public Transform character;

    Vector2 position;


    private PhotonView photonView;
    //player
    private GameObject player;
    public Vector3 cameraRigPos;
    Transform cameraRig;


    expe expe;


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
        if (expe == null)
        {
            expe = GameObject.Find("/Salle").GetComponent<rendering>().expe;
        }
        cameraRig = SteamVR_Render.Top().origin;
        cameraRigPos = cameraRig.position;
        //Pointer
        m_HasPosition = UpdatePointer();

        if (syncTeleportation == true)
        {
            tpsync.SetActive(false);
            tpNotsync.SetActive(true);
           // Cube.SetActive(true);
        }
        if (syncTeleportation == false)
        {
            tpNotsync.SetActive(false);
            tpsync.SetActive(true);
            Cube.SetActive(false);

        }
        if (synctag == true)
        {
            tagsync.SetActive(false);
            tagNotsync.SetActive(true);
        }
        if (synctag == false)
        {
            tagNotsync.SetActive(false);
            tagsync.SetActive(true);
        }

        //Teleport
        position = SteamVR_Actions.default_Pos.GetAxis(SteamVR_Input_Sources.Any);


        if (m_TeleportAction.GetStateDown(m_pose.inputSource))
        {
            if (position.x < -0.5)
            {
                Debug.Log("W");
                w = true;
                tryTeleport();
            }
            /*
            else if(position.y > 0.5)
            {
                Debug.Log("N");
                n = true;
                tryTeleport();
            }
            else if (position.y < -0.5)
            {
                Debug.Log("S");
                s = true;
                tryTeleport();
            }
            */
            else if (position.x > 0.5)
            {
                Debug.Log("E");
                e = true;
                tryTeleport();
            }
            
            else
            {
                nbClick++;
                //Debug.Log("C");
                coordClic = coordPrev = m_Pointer.transform.position; //hit.transform.position;
                forwardClic = transform.forward;
                wait = true;
                timer = Time.time;
            }
        }


        if (wait)
        {
            if (Time.time - timer > 0.7) //  after 0.7s it is long clic
            {
                longclic = true;
                wait = false;
                // Debug.Log("long clic");
            }
        }
        if (longclic)
        {
            syncTeleportation = true;
            tryTeleport();
            expe.curentTrial.incNbSyncTp();
            //syncTeleportation = false;
            longclic = false;
        }
       
        if (m_TeleportAction.GetStateUp(m_pose.inputSource))
        {
          
            //Debug.Log("reset");

            isMoving = false;
            longclic = false;
            n = false;
            s = false;
            e = false;
            w = false;
            
            if (wait)
            { 
              tryTeleport();
            }
            wait = false;
            longclic = false;
        }

        if (isMoving)
        {
            //dragTeleport(coordPrev, m_Pointer.transform.position);
            coordPrev = m_Pointer.transform.position;
        }


        // MENU //
        if (UpdatePointer() == true && hit.transform.name == "syncro")
        {
            // Debug.Log("Syncro");
            Menu.SetActive(false);
            photonView.RPC("teleportationMode", Photon.Pun.RpcTarget.All, syncTeleportation);
        }

        if (UpdatePointer() == true && hit.transform.name == "not syncro")
        {
            // Debug.Log("Not Syncro");
            Menu.SetActive(false);
            photonView.RPC("teleportationMode", Photon.Pun.RpcTarget.All, syncTeleportation);
        }

        if (UpdatePointer() == true && hit.transform.name == "syncro tag")
        {
            // Debug.Log("Syncro");
            Menu.SetActive(false);
            player = GameObject.Find("Network Player(Clone)");
            synctag = true;
            photonView.RPC("tagMode", Photon.Pun.RpcTarget.All, synctag);
        }

        if (UpdatePointer() == true && hit.transform.name == "not syncro tag")
        {
            // Debug.Log("Not Syncro");
            Menu.SetActive(false);
            player = GameObject.Find("Network Player(Clone)");
            synctag = false;
            photonView.RPC("tagMode", Photon.Pun.RpcTarget.All, synctag);
        }

        if (UpdatePointer() == true && hit.transform.name == "cancel")
        {
            // Debug.Log("Cancel");
            Menu.SetActive(false);
        }

    }

    private void tryTeleport()
    {

        //if no hit stop the fonction
        if ( m_IsTeleportoting) //!m_HasPosition ||
            return;

        // head position + camera rig
        Vector3 headPosition = SteamVR_Render.Top().head.position;
        //Transform cameraRig = SteamVR_Render.Top().origin;
      
        //player possition
        Vector3 groundPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);
        if (e)
        {
            Transform cam = cameraRig.Find("Camera (eye)");
            if (!syncTeleportation)
            {
                cameraRig.RotateAround(cam.transform.position, Vector3.up, 90);
                CubePlayer.transform.RotateAround(CubePlayer.transform.position, Vector3.up, 90);
            }
            else
            {
                photonView.RPC("RotationRigRPC", Photon.Pun.RpcTarget.All, "e");
            }
            return;
        }
        else if (w)
        {
            Transform cam = cameraRig.Find("Camera (eye)");

            if (!syncTeleportation)
            {
                cameraRig.RotateAround(cam.transform.position, Vector3.up, -90);
                CubePlayer.transform.RotateAround(CubePlayer.transform.position, Vector3.up, -90);
            }
            else
            {
                photonView.RPC("RotationRigRPC", Photon.Pun.RpcTarget.All, "w");
            }
            return;
        }
        Vector3 translateVector;
        /*
        if (n)
        {
            //translateVector =  character.transform.forward * desiredDistance; // y not fix
            translateVector = new Vector3(character.transform.forward.x * desiredDistance, cameraRig.position.y, character.transform.forward.z * desiredDistance);  //  y fix
                                                                                                                                                                    // StartCoroutine(MoveRig(cameraRig, translateVector));
            if (!syncTeleportation)
            {
                StartCoroutine(MoveRig(cameraRig, translateVector));
            }
            else
            {
                photonView.RPC("MoveRigRPC", Photon.Pun.RpcTarget.All, cameraRig.gameObject.GetComponent<PhotonView>().ViewID, translateVector);
            }
        }
        else if (s)
        {
            //translateVector =  - character.transform.forward * desiredDistance;
            translateVector = new Vector3(-character.transform.forward.x * desiredDistance, cameraRig.position.y, -character.transform.forward.z * desiredDistance);  //  y fix
                                                                                                                                                                      //StartCoroutine(MoveRig(cameraRig, translateVector));
            if (!syncTeleportation)
            {
                StartCoroutine(MoveRig(cameraRig, translateVector));
            }
            else
            {
                photonView.RPC("MoveRigRPC", Photon.Pun.RpcTarget.All, cameraRig.gameObject.GetComponent<PhotonView>().ViewID, translateVector);
            }
        }
        */
        if (!m_HasPosition) // ||
            return;


        
        
        else if (hit.transform.tag == "Tp" )
        {
            Vector3 posPointer = m_Pointer.transform.position;
            if (posPointer.x < -3.5) { posPointer.x = -3.5f; }
            if (posPointer.x >  3.5) { posPointer.x =  3.5f; }
            if (posPointer.z < -3.5) { posPointer.z = -3.5f; }
            if (posPointer.z >  3.5) { posPointer.z =  3.5f; }
            translateVector = posPointer - groundPosition;

            if (!syncTeleportation)
            {
                expe.curentTrial.incNbAsyncTPGround(translateVector);
                StartCoroutine(MoveRig(cameraRig, translateVector));
            }
            else {
                StartCoroutine(MoveRig(cameraRig, translateVector));
                expe.curentTrial.incNbSyncTpGround(translateVector);
                }
            
        }

        else if (hit.transform.tag == "Wall" || hit.transform.tag == "Card")
        {
            translateVector = new Vector3(0, 0, 0);
            //check the wall
            if (hit.transform.name == "MUR B" || hit.transform.parent.name == "MUR B")
            {
                    objectHit = Physics.RaycastAll(cameraRig.transform.position, cameraRig.transform.forward, 100.0F);
                    float x = -cameraRig.transform.position.x;
                    float z = -cameraRig.transform.position.z;
                    Transform cam = cameraRig.Find("Camera (eye)");

                    for (int i = 0; i < objectHit.Length; i++)
                    {
                        //Debug.Log("objHit : " + objectHit[i].transform.name);
                        if (objectHit[i].transform.name == "MUR B" || objectHit[i].transform.parent.name == "MUR B")
                        {
                            translateVector = new Vector3(m_Pointer.transform.position.x - groundPosition.x, 0, 0);
                        }
                        else if (objectHit[i].transform.name == "MUR L" || objectHit[i].transform.parent.name == "MUR L")
                        {
                            //Debug.Log("need to rotate w 1 time");
                            cameraRig.RotateAround(cam.transform.position, Vector3.up, 90);
                            translateVector = new Vector3(m_Pointer.transform.position.x - groundPosition.x, 0, z + Mathf.Abs(x));
                        }
                        else if (objectHit[i].transform.name == "MUR R" || objectHit[i].transform.parent.name == "MUR R")
                        {
                           // Debug.Log("need to rotate w 1 times");
                            cameraRig.RotateAround(cam.transform.position, Vector3.up, -90);
                            translateVector = new Vector3(m_Pointer.transform.position.x - groundPosition.x, 0, z + Mathf.Abs(x));
                        }
                    }
                    objectHit = Physics.RaycastAll(cameraRig.transform.position, -cameraRig.transform.forward, 100.0F);
                    for (int i = 0; i < objectHit.Length; i++)
                    {
                        if (objectHit[i].transform.name == "MUR B" || objectHit[i].transform.parent.name == "MUR B")
                        {
                            cameraRig.RotateAround(cam.transform.position, Vector3.up, -180);
                        }
                    }
                
                
            }
            else if (hit.transform.name == "MUR R" || hit.transform.parent.name == "MUR R")
            {
                
                    objectHit = Physics.RaycastAll(cameraRig.transform.position, cameraRig.transform.forward, 100.0F);
                    Transform cam = cameraRig.Find("Camera (eye)");
                    float x = -cameraRig.transform.position.x;
                    float z = -cameraRig.transform.position.z;

                    for (int i = 0; i < objectHit.Length; i++)
                    { 
                        //Debug.Log("objHit : " + objectHit[i].transform.name);
                   
                        if (objectHit[i].transform.name == "MUR R" || objectHit[i].transform.parent.name == "MUR R")
                        {
                            translateVector = new Vector3(0, 0, m_Pointer.transform.position.z- groundPosition.z);
                        }

                        if (objectHit[i].transform.name == "MUR B" || objectHit[i].transform.parent.name == "MUR B")
                        {
                            //Debug.Log("need to rotate e 1 time");
                            cameraRig.RotateAround(cam.transform.position, Vector3.up, 90);
                            translateVector = new Vector3(x + Mathf.Abs(z), 0, m_Pointer.transform.position.z - groundPosition.z);
                        }
                        else if (objectHit[i].transform.name == "MUR L" || objectHit[i].transform.parent.name == "MUR L")
                        {
                            //Debug.Log("need to rotate w 2 times");
                            cameraRig.RotateAround(cam.transform.position, Vector3.up, 180);
                            translateVector = new Vector3(-2 * groundPosition.x, 0, m_Pointer.transform.position.z - groundPosition.z);
                        }
                    }
                    objectHit = Physics.RaycastAll(cameraRig.transform.position, -cameraRig.transform.forward, 100.0F);
                    for (int i = 0; i < objectHit.Length; i++)
                    {
                        if (objectHit[i].transform.name == "MUR B" || objectHit[i].transform.parent.name == "MUR B")
                        {
                            cameraRig.RotateAround(cam.transform.position, Vector3.up, -90);
                        }
                    }
                
            }
            else //(hit.transform.name == "MUR L" || hit.transform.parent.name == "MUR L")
            {
                
               
                    objectHit = Physics.RaycastAll(cameraRig.transform.position, cameraRig.transform.forward, 100.0F);
                    float x = -cameraRig.transform.position.x;
                    float z = -cameraRig.transform.position.z;
                    Transform cam = cameraRig.Find("Camera (eye)");

                    for (int i = 0; i < objectHit.Length; i++)
                    {
                        //cameraRig.rotation = new Quaternion(0.0f, -0.7f, 0.0f, 0.7f);
                        //Debug.Log("objHit : " + objectHit[i].transform.name);

                        if (objectHit[i].transform.name == "MUR L" || objectHit[i].transform.parent.name == "MUR L")
                        {
                            translateVector = new Vector3(0, 0, m_Pointer.transform.position.z - groundPosition.z);
                        }

                        if (objectHit[i].transform.name == "MUR B" || objectHit[i].transform.parent.name == "MUR B")
                        {
                            //Debug.Log("need to rotate w 1 time");
                            
                            cameraRig.RotateAround(cam.transform.position, Vector3.up, -90);
                            translateVector = new Vector3(x - Mathf.Abs(z), 0, m_Pointer.transform.position.z - groundPosition.z);
                        }
                        else if (objectHit[i].transform.name == "MUR R" || objectHit[i].transform.parent.name == "MUR R")
                        {
                            //Debug.Log("need to rotate w 2 times");
                            cameraRig.RotateAround(cam.transform.position, Vector3.up, 180);
                            translateVector = new Vector3(-2 * groundPosition.x, 0, m_Pointer.transform.position.z - groundPosition.z);
                        }
                    }
                    objectHit = Physics.RaycastAll(cameraRig.transform.position, -cameraRig.transform.forward, 100.0F);
                    for (int i = 0; i < objectHit.Length; i++)
                    {
                        if (objectHit[i].transform.name == "MUR B" || objectHit[i].transform.parent.name == "MUR B")
                        {
                            cameraRig.RotateAround(cam.transform.position, Vector3.up, 90);
                        }
                    }

            }

            //then teleport
            if (!syncTeleportation)
            {
                expe.curentTrial.incNbAsyncTPWall(translateVector);
                StartCoroutine(MoveRig(cameraRig, translateVector));
            }
            else
            {
                StartCoroutine(MoveRig(cameraRig, translateVector));
                expe.curentTrial.incNbSyncTpWall(translateVector);
                }
        }
    }

    [PunRPC]
    void MoveRigRPC(int cameraRig, Vector3 pos, Quaternion rotat)
    {
        // StartCoroutine(MoveRig(PhotonView.Find(cameraRig).transform, translation));

        StartCoroutine(MoveRigForSyncTP(PhotonView.Find(cameraRig).transform, pos , rotat));
        
    }

    [PunRPC]
    void RotationRigRPC(string s)
    {
        Transform cameraRig2 = SteamVR_Render.Top().origin;

        Transform cam = cameraRig2.Find("Camera (eye)");
        Debug.Log("test ");
        if (s == "e")
        {
            Cube.transform.RotateAround(Cube.transform.position, Vector3.up, 90);
            cameraRig2.RotateAround(Cube.transform.position, Vector3.up, 90);

        }
        else if (s == "w")
        {
            Cube.transform.RotateAround(Cube.transform.position, Vector3.up, -90);
            cameraRig2.RotateAround(Cube.transform.position, Vector3.up, -90);
        }
        
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
    }

    private IEnumerator MoveRig(Transform cameraRig , Vector3 translation)
    {
        m_IsTeleportoting = true;

        SteamVR_Fade.Start(Color.black, m_FadeTime, true); // black screen

        yield return new WaitForSeconds( m_FadeTime); // fade time
        
        cameraRig.position += translation; // teleportation
        Debug.Log("camera rig pos tp :" +cameraRig.position);
        if (syncTeleportation)
        {
            Cube.transform.position += translation; // teleportation
        }

        SteamVR_Fade.Start(Color.clear, m_FadeTime, true); // normal screen

        if (syncTeleportation)
        {
            Quaternion rotat = SteamVR_Render.Top().origin.rotation;
            Debug.Log("rotation" +rotat);
            Vector3 headPosition = SteamVR_Render.Top().head.position;
            Vector3 playerPos = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);
            Debug.Log(playerPos);
            //R
            if (rotat.y == 90)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    playerPos.x += 1;
                }
                else
                {
                    playerPos.x -= 1;
                }

            }
            //B
            else if (rotat.y== 0 && rotat.x==0 && rotat.z == 0)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    playerPos.z += 1;
                }
                else
                {
                    playerPos.z -= 1;
                }

            }
            //L
            else if (rotat.y == -90)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    playerPos.x -= 1;
                }
                else
                {
                    playerPos.x += 1;
                }

            }
            //no wall
            else //if (Physics.RaycastAll(player.transform.position, player.transform.forward, 100.0F)[0].transform.name == "MUR R")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    playerPos.z -= 1;
                }
                else
                {
                    playerPos.z += 1;
                }

            }
            Debug.Log(playerPos);
            photonView.RPC("MoveRigRPC", Photon.Pun.RpcTarget.Others, cameraRig.gameObject.GetComponent<PhotonView>().ViewID, cameraRig.position, rotat);
            syncTeleportation = false;
        }

        m_IsTeleportoting = false;

    }

    private IEnumerator MoveRigForSyncTP(Transform cameraRig, Vector3 pos, Quaternion rotat)
    {
        m_IsTeleportoting = true;

        SteamVR_Fade.Start(Color.black, m_FadeTime, true); // black screen
        // Rotation
        {
            cameraRig.rotation = rotat;
        }
        yield return new WaitForSeconds(m_FadeTime); // fade time

        cameraRig.position = pos; // teleportation
        if (syncTeleportation)
        {
            Cube.transform.position = pos; // teleportation
        }

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
