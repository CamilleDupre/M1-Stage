using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class rendering : MonoBehaviourPunCallbacks //, MonoBehaviourPun
{
    //Prefab card
    public GameObject pfCard;

    //walls
    public Transform MurB;
    public Transform MurL;
    public Transform MurR;

    //Card list
    public List<GameObject> cardList;

    //List of textures
    public object[] textures;

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
        textures = Resources.LoadAll("dixit_part2/", typeof(Texture2D));
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
        for (int i = 0; i < cardList.Count; i++)
        {
            // check the material to know if the card must be teleported
            if (cardList[i].transform.GetChild(0).GetComponent<Renderer>().material.name == nameR)
            {
                // width heigth depending on the scale of the wall
                Vector3 v = MurB.localScale;
                h = tex.height / div;
                w = tex.width / div;
                w = w * (v.y / v.x);

                //Set parent, rotation and localscale
                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.transform.parent = mur;
                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.transform.rotation = mur.rotation;
                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.transform.localScale = new Vector3(w, h, 1.0f);

                //Set position depending on how many card teleported
                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.transform.localPosition = new Vector3(-0.35f + w + 1.5f * w * j, 0, -0.02f);
                j++; // 1 card more teleported
            }
        }
    }
}
