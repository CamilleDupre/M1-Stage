using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;

public class rendering : MonoBehaviourPunCallbacks //, MonoBehaviourPun
{
    //Prefab card
    public GameObject pfCard;

    //trash
    public GameObject trash1;
    public GameObject trash2;
    public GameObject trash3;
    public GameObject trash4;

    //walls
    public Transform MurB;
    public Transform MurL;
    public Transform MurR;

    public Transform room;

    //Card list
    public List<GameObject> cardList;
    public List<GameObject> cardListToTeleport;

    //List of textures
    public object[] textures;
    public bool card1 = true;

    public GameObject m_Pointer;

    private bool trialEnCours = false ;

    public class MyCard
    {
        // Creation of the card 
        public GameObject goCard = null;
        public string tag = "";
        public PhotonView pv;
        public Transform parent;
       

        public MyCard(Texture2D tex, Transform mur , int i)
        {
            GameObject goCard = PhotonNetwork.InstantiateRoomObject("Card", mur.position, mur.rotation, 0, null);
            goCard.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
            parent = mur;
            pv = goCard.GetPhotonView();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && trialEnCours == false)
        {
            print("space key was pressed");
            CardCreation();
            trialEnCours = true;
            photonView.RPC("startExpe", Photon.Pun.RpcTarget.AllBuffered);

        }


        if (Input.GetKeyDown(KeyCode.E) && trialEnCours == true)
        {
            print("End");
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
       card1 = GameObject.Find("/[CameraRig]/Controller (right)").GetComponent<Teleporter>().card1;
       bool training = GameObject.Find("/[CameraRig]/Controller (right)").GetComponent<Teleporter>().training;
        if (training)
        {
            textures = Resources.LoadAll("dixit_training/", typeof(Texture2D));
        }
        else if(card1)
        {
            textures = Resources.LoadAll("dixit_part1/", typeof(Texture2D));
        }
        else
        {
            textures = Resources.LoadAll("dixit_part2/", typeof(Texture2D));
        }

        trash1.SetActive(false);
        trash2.SetActive(false);
        trash3.SetActive(false);
        trash4.SetActive(false);
    }

    public void CardCreation() { 
        
        Debug.Log("Creation carte " + PhotonNetwork.IsMasterClient);

        int nbcard = textures.Length;

        Transform mur;
        int pos;
        for (int i = 0 ; i < nbcard ; i++)
        {
            // Slit the cqrd over the 3 walls
            if (i < nbcard / 3)
            {
                mur = MurL;
                pos = i ;
            }
            else if (i < 2* nbcard / 3)
            {
                mur = MurB;
                pos = i - nbcard / 3;
            }
            else 
            {
                mur = MurR;
                pos = i - 2 * nbcard / 3;
            }
            MyCard c = new MyCard((Texture2D)textures[i], mur, i);
            photonView.RPC("addListCard", Photon.Pun.RpcTarget.AllBuffered, c.pv.ViewID);
            c.pv.RPC("LoadCard", Photon.Pun.RpcTarget.AllBuffered, c.pv.ViewID, mur.GetComponent<PhotonView>().ViewID, pos, i);
        }
    }


    [PunRPC]
    //Add card to the list of card
    void addListCard(int OB)
    {
        cardList.Add(PhotonView.Find(OB).gameObject);
    }

    [PunRPC]
    //Add card to the list of card
    void startExpe()
    {
        expe expe = new expe("01");
    }

    [PunRPC]
    //Add card to the list of card
    void DestroyCard(int OB, int nbTrashs)
    {
        // Undo.DestroyObjectImmediate(PhotonView.Find(OB).gameObject);
        PhotonView.Find(OB).gameObject.SetActive(false);
        if (nbTrashs >= 1)
        {
            trash1.SetActive(true);
        }
        if (nbTrashs >= 2)
        {
            trash2.SetActive(true);
        }
        if (nbTrashs >= 3)
        {
            trash3.SetActive(true);
        }
        if (nbTrashs >= 4)
        {
            trash4.SetActive(true);
        }
    }
    
    [PunRPC]
    //Add card to the list of card
    void UndoCard(int OB, int nbTrashs)
    {
        // Undo.DestroyObjectImmediate(PhotonView.Find(OB).gameObject);
        PhotonView.Find(OB).gameObject.SetActive(true);

        if (nbTrashs <= 1)
        {
            trash1.SetActive(false);
        }
        if (nbTrashs <= 2)
        {
            trash2.SetActive(false);
        }
        if (nbTrashs <= 3)
        {
            trash3.SetActive(false);
        }
        if (nbTrashs <=4)
        {
            trash4.SetActive(false);
        }
    }
    
}
