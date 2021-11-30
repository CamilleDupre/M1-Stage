using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;
using UnityEditor;

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
    public List<GameObject> obUndo;

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

    GameObject emptyToMoveCard;
    private bool cardSeletedForGroupMove = false;
    private Vector3 loalscaleEmpty;

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

            if (emptyToMoveCard != null)
            {               
                int children = emptyToMoveCard.transform.childCount;

                for (int i = 0; i<children; i++)
                {
                    if (emptyToMoveCard.GetComponent<PhotonView>().IsMine)
                    {
                        photonView.RPC("ChangeMur", Photon.Pun.RpcTarget.All, emptyToMoveCard.transform.parent.name, emptyToMoveCard.transform.GetChild(0).GetComponent<PhotonView>().ViewID);
                        //  emptyToMoveCard.transform.GetChild(0).transform.parent = emptyToMoveCard.transform.parent;
                    }

                }
                photonView.RPC("Desroyempty", Photon.Pun.RpcTarget.All, emptyToMoveCard.GetComponent<PhotonView>().ViewID);

               // Destroy(emptyToMoveCard);
                cardSeletedForGroupMove = false;
            }

            isMoving = false;
            ob = null;
            wait = false;
            longclic = false;
            //Debug.Log("reset");
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
           
            coordClic = hit.transform.position;
            forwardClic = transform.forward;
            //start waiting
            wait = true;
            timer = Time.time;

        }

        if (ob != null && wait && Vector3.Angle(forwardClic, transform.forward) > 2) //move more than 2* -> moving
        {
            isMoving = true;
            wait = false;
        }

        //destroy a card a remove the tag
        if (ob != null && UpdatePointer()  &&  hit.transform.tag == "trash")
        {
            //Debug.Log("destroy");
            photonView.GetComponent<PhotonView>().RPC("AddobUndo", Photon.Pun.RpcTarget.All, ob.GetComponent<PhotonView>().ViewID);
            player = GameObject.Find("Network Player(Clone)");
            player.GetComponent<PhotonView>().RPC("removeTag", Photon.Pun.RpcTarget.AllBuffered, ob.GetComponent<PhotonView>().ViewID);

            salle = GameObject.Find("Salle");
            salle.GetComponent<PhotonView>().RPC("DestroyCard", Photon.Pun.RpcTarget.All, ob.GetComponent<PhotonView>().ViewID, obUndo.Count);
            
            ob = null;

        }
    
        //undo the last destroy action
        if (obUndo != null && UpdatePointer() && hit.transform.tag == "trash" && interactWithUI.GetStateDown(m_pose.inputSource))
        {
            Debug.Log("undo");
            GameObject temp = obUndo[obUndo.Count-1];
            salle = GameObject.Find("Salle");
            salle.GetComponent<PhotonView>().RPC("UndoCard", Photon.Pun.RpcTarget.All, temp.GetComponent<PhotonView>().ViewID , obUndo.Count);
            photonView.GetComponent<PhotonView>().RPC("RemoveobUndo", Photon.Pun.RpcTarget.All, temp.GetComponent<PhotonView>().ViewID);
        }
      

        if (wait)
        {
            if (Time.time - timer > 1.5) //  after 1.5s it is long clic
            {
               longclic = true;
               wait = false;
              // Debug.Log("long clic");
            }
        }

        //long clic -> move cards with tag 
        if (longclic && UpdatePointer() && (hit.transform.tag == "Wall" || hit.transform.tag == "Card"))
        {
            string namewall = "";
            if (hit.transform.tag == "Card")
            {
                if (hit.transform.parent.tag != "Wall")
                {
                    namewall = hit.transform.parent.parent.name; // we want the wall and not the empty
                }
                else
                {
                    namewall = hit.transform.parent.name;
                }
            }
            else
            {
                namewall = hit.transform.name;
            }
            salle = GameObject.Find("Salle");
            player = GameObject.Find("Network Player(Clone)");

            if (emptyToMoveCard == null){
                emptyToMoveCard = PhotonNetwork.Instantiate("emptyToMoveCard", transform.position, transform.rotation);
                //emptyToMoveCard = new GameObject("TempEmptyToMove");
                photonView.RPC("Initempty", Photon.Pun.RpcTarget.All, player.GetComponent<Network_Player>().nameR, namewall, emptyToMoveCard.GetComponent<PhotonView>().ViewID);

            }
           TeleportCard(player.GetComponent<Network_Player>().nameR, namewall);
            // photonView.RPC("TeleportCard", Photon.Pun.RpcTarget.All, player.GetComponent<Network_Player>().nameR, namewall);
           

        }
     
        Move();
    }

    [PunRPC]
    void Desroyempty(int OB)
    {
        Destroy(PhotonView.Find(OB).gameObject);
    }

    [PunRPC]
    void Initempty(string nameR, string murName, int OB)
    {
        Transform mur;
        if (murName == "MUR B") { mur = MurB; }
        else if (murName == "MUR L") { mur = MurL; }
        else { mur = MurR; }


        PhotonView.Find(OB).transform.parent = mur;
        PhotonView.Find(OB).transform.rotation = mur.rotation;
        PhotonView.Find(OB).transform.localPosition = new Vector3(0, 0, 0);
        PhotonView.Find(OB).transform.localScale = new Vector3(1, 1, 1);

        Vector3 v = MurB.localScale;
        float w;
        float div = 2 * 1000f;
        w = tex.width / div;
        w = w * (v.y / v.x);

        salle = GameObject.Find("Salle");
        List<GameObject> cardList = salle.GetComponent<rendering>().cardList;
        int nbCardToTeleport = 0;
        for (int i = 0; i < cardList.Count; i++)
        {

            // check the material to know if the card must be teleported
            if (cardList[i].transform.GetChild(0).GetComponent<Renderer>().material.name == nameR)
            {
                nbCardToTeleport++;
                if (cardSeletedForGroupMove == false)
                {
                    cardList[i].GetComponent<PhotonView>().RequestOwnership();
                }

                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).gameObject.transform.parent = PhotonView.Find(OB).transform;
                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).gameObject.transform.rotation = PhotonView.Find(OB).transform.rotation;
                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.localPosition =
                    new Vector3(w * nbCardToTeleport,
                    0,
                    -0.02f);

            }

        }
    }

    [PunRPC]
    void TeleportCard(string nameR, string murName)
    {
        //Debug.Log("name wall " + murName);
        if (nameR == "transparent (Instance)") { return; }
        salle = GameObject.Find("Salle");
        List<GameObject> cardList = salle.GetComponent<rendering>().cardList;

        

        float w, h;
        float div = 2 * 1000f;

        Transform mur;

        //Check the walls
        if (murName == "MUR B") { mur = MurB; }
        else if (murName == "MUR L") { mur = MurL; }
        else { mur = MurR; }

     

        Vector3 v = MurB.localScale;
        h = tex.height / div;
        w = tex.width / div;
        w = w * (v.y / v.x);
        Vector3 p = mur.position;
       
      
        cardSeletedForGroupMove = true;
        if (murName != emptyToMoveCard.transform.parent.name)
        {
            photonView.RPC("ChangeMur2", Photon.Pun.RpcTarget.All, murName , emptyToMoveCard.GetComponent<PhotonView>().ViewID);
        }
       
        for (int i = 0; i < cardList.Count; i++)
        {
            
            // check the material to know if the card must be teleported
            if (cardList[i].transform.GetChild(0).GetComponent<Renderer>().material.name == nameR)
            {
        
                cardList[i].transform.localScale = new Vector3(w, h, 1.0f);

                // width heigth depending on the scale of the wall

                //photonView.RPC("ChangeMur", Photon.Pun.RpcTarget.All, murName, cardList[i].GetComponent<PhotonView>().View

                //PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.transform.localPosition = new Vector3(-w * (nbCardToTeleport / 4) + x + 1f * w * (j / 2), y, -0.02f); //+-0.35f + w
                //emptyToMoveCard.transform.localPosition = new Vector3( x , y, 0); //+-0.35f + w
                float x = mur.InverseTransformPoint(m_Pointer.transform.position).x;
                float q = mur.InverseTransformPoint(m_Pointer.transform.position).y;

                photonView.RPC("MoveEmpty", Photon.Pun.RpcTarget.All, emptyToMoveCard.GetComponent<PhotonView>().ViewID , x , q);
            }
        }

    }

    [PunRPC]
    void MoveEmpty(int OB, float x, float q)
    {
        PhotonView.Find(OB).gameObject.transform.localPosition = new Vector3(x, q, 0);
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

        if (isMoving && ob != null)
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
            if (hit.transform.tag == "Card" || hit.transform.tag == "Wall" || hit.transform.tag == "trash")
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
    void ChangeMur2(string nameT, int OB)
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
        //Debug.Log("Changement de mur " + nameT);
        PhotonView.Find(OB).gameObject.transform.parent = Mur;
        PhotonView.Find(OB).gameObject.transform.localScale = new Vector3(1, 1, 1);
        PhotonView.Find(OB).gameObject.transform.rotation = Mur.rotation;

       
    }

    [PunRPC]
    void RayColour(string name)
    {
        //nameR = name;
        //Debug.Log("nameR" + nameR);
    }

    [PunRPC]
    void AddobUndo(int OB)
    {
        obUndo.Add(PhotonView.Find(OB).gameObject);
    }

    [PunRPC]
    void RemoveobUndo(int OB)
    {
        obUndo.Remove(PhotonView.Find(OB).gameObject);
    }
}
