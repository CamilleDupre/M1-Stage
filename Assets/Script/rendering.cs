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

    //Card list
    public List<GameObject> cardList;
    public List<GameObject> cardListToTeleport;

    //List of textures
    public object[] textures;
    public bool card1 = true;

    public GameObject m_Pointer;

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

   // public static float GetDiv() { return 2 * 1000f; }

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

    public override void OnCreatedRoom() { 
        
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

    // Update is called once per frame
    void Update()
    {
    
    }

    [PunRPC]
    //Add card to the list of card
    void addListCard(int OB)
    {
        cardList.Add(PhotonView.Find(OB).gameObject);
    }

    [PunRPC]
    // Teleport card tag
    void TeleportCard(string nameR, string murName)
    {
        float w, h;
        float div = 2 * 1000f;
        Texture tex= (Texture2D)textures[0];
        Transform mur;

        //Check the walls
        if (murName == "MUR B") { mur = MurB; }
        else if (murName == "MUR L") { mur = MurL; }
        else { mur = MurR; }

        int j = 0; // number of card teleported
        int nbCardToTeleport = 0;
        for (int i = 0; i < cardList.Count; i++)
        {

            // check the material to know if the card must be teleported
            if (cardList[i].transform.GetChild(0).GetComponent<Renderer>().material.name == nameR)
            {
                nbCardToTeleport++;
            }
        }
        for (int i = 0; i < cardList.Count; i++)
        {

            // check the material to know if the card must be teleported
            if (cardList[i].transform.GetChild(0).GetComponent<Renderer>().material.name == nameR)
            {               
                float y = 0;
                // width heigth depending on the scale of the wall
                Vector3 v = MurB.localScale;
                h = tex.height / div;
                w = tex.width / div;
                w = w * (v.y / v.x);
                Vector3 p = mur.position;

                //Set parent, rotation and localscale
                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.transform.parent = mur;
                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.transform.rotation = mur.rotation;
                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.transform.localScale = new Vector3(w, h, 1.0f);

                //Set position depending on how many card teleported
                // PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.transform.localPosition = new Vector3(-0.35f + w + 1.5f * w * j, 0, -0.02f);

                if (nbCardToTeleport > 1)
                {
                    if (j % 2 == 0) //1st line
                    {
                        y = 0.15f;
                    }
                    else // 2nd line
                    {
                        y = -0.15f;
                    }
                }
                //
               // m_Pointer = GameObject.Find("/[CameraRig]/Controller (right)").GetComponent<DragDrop>().m_Pointer;
                Debug.Log("m_Pointer " + m_Pointer);

                float x = 0;

                if (mur.name == "MUR L")
                {
                    x = m_Pointer.transform.position.z / v.x;
                    y += (m_Pointer.transform.position.y - p.y) / v.y;
                }

                else if (mur.name == "MUR B")
                {
                    x = m_Pointer.transform.position.x / v.x;
                    y += (m_Pointer.transform.position.y - p.y) / v.y;
                }

                else if (mur.name == "MUR R")
                {
                    x = -m_Pointer.transform.position.z / v.x;
                    y += (m_Pointer.transform.position.y - p.y) / v.y;
                }

                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.transform.localPosition = new Vector3(-w*(nbCardToTeleport/4) + x + 1f * w * (j / 2), y, -0.02f); //+-0.35f + w
                
                j++; // 1 card more teleported
                
                
            }
        }

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
