using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class rendering : MonoBehaviourPunCallbacks //, MonoBehaviourPun
{
    public GameObject pfCard;
    public Transform MurB;
    public Transform MurL;
    public Transform MurR;
    public List<GameObject> cardList;
    public object[] textures;

    public class MyCard
    {
        public GameObject goCard = null;
        public string tag = "";
        public PhotonView pv;
        public Transform p;
       

        public MyCard(Texture2D tex, Transform mur , int i )
        {
            GameObject goCard = PhotonNetwork.InstantiateRoomObject("Quad (23)", mur.position, mur.rotation, 0, null);
            goCard.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
            //goCard.transform.parent = mur;
            p = mur;
            //goCard.transform.rotation = mur.rotation;
            pv = goCard.GetPhotonView();

            /*
            //Debug.Log("TEXTURES: " + tex.width + " " + tex.height);

            float w, h;
            Vector3 v = mur.localScale;

            float div =  2 * 1000f; //GetDiv();

            h = tex.height / div;
            w = tex.width / div;

            //Debug.Log("scale: " + v);
            w = w * (v.y / v.x);

            goCard.transform.localScale = new Vector3(w, h, 1.0f);
            if (i < 10)
            {
                goCard.transform.localPosition = new Vector3(-0.35f + w + 1.5f * w * i, -1 * h, -0.001f);
            }
            else
            {
                i = i - 10;
                goCard.transform.localPosition = new Vector3(-0.35f + w + 1.5f * w * i, 1 * h, -0.001f);
            }

            //PhotonView photonView = PhotonView.Get(goCard.GetComponent<PhotonView>());
            //photonView.RPC("ChangeTag", Photon.Pun.RpcTarget.All, nameT, hit.transform.gameObject.GetComponent<PhotonView>().ViewID);
            */
        }
    }

   // public static float GetDiv() { return 2 * 1000f; }

    // Start is called before the first frame update
    void Awake()
    {
        //StartCoroutine(waiter());
        textures = Resources.LoadAll("dixit_part2/", typeof(Texture2D));
    }

    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
       // Debug.Log("new player");
        //base.OnPlayerEnteredRoom(newPlayer);
    //}

    public override void OnCreatedRoom() { 
        
        
       // object[] textures = Resources.LoadAll("dixit_part1/", typeof(Texture2D));
      

        // Debug.Log("TEXTURES: " + textures.Length);

        Debug.Log("Creation carte " + PhotonNetwork.IsMasterClient);

        int nbcard = textures.Length;
        for (int i = 0 ; i < nbcard ; i++)
        {
            if (i < nbcard / 3)
            {
              MyCard c = new MyCard((Texture2D)textures[i], MurL, i);
                // Material m = c.goCard.transform.GetChild(0).GetComponent<Renderer>().material;
                // cardList.Add(PhotonView.Find(c.pv.ViewID).gameObject);
                photonView.RPC("addListCard", Photon.Pun.RpcTarget.AllBuffered, c.pv.ViewID);
                c.pv.RPC("LoadCard", Photon.Pun.RpcTarget.AllBuffered, c.pv.ViewID, MurL.GetComponent<PhotonView>().ViewID , i , i);//GetComponent<PhotonView>().ViewID);
            }
            else if (i < 2* nbcard / 3)
            {
               MyCard c = new MyCard((Texture2D)textures[i], MurB, i - nbcard / 3);
                //cardList.Add(PhotonView.Find(c.pv.ViewID).gameObject);
                photonView.RPC("addListCard", Photon.Pun.RpcTarget.AllBuffered, c.pv.ViewID);
                c.pv.RPC("LoadCard", Photon.Pun.RpcTarget.AllBuffered, c.pv.ViewID, MurB.GetComponent<PhotonView>().ViewID, i - nbcard / 3, i);
            }
            else 
            {
              MyCard c = new MyCard((Texture2D)textures[i], MurR, i - 2 * nbcard / 3);
                cardList.Add(PhotonView.Find(c.pv.ViewID).gameObject);
                photonView.RPC("addListCard", Photon.Pun.RpcTarget.AllBuffered, c.pv.ViewID);
                c.pv.RPC("LoadCard", Photon.Pun.RpcTarget.AllBuffered, c.pv.ViewID, MurR.GetComponent<PhotonView>().ViewID, i - 2 * nbcard / 3, i);
            }
        
        
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]

    void addListCard(int OB)
    {
        cardList.Add(PhotonView.Find(OB).gameObject);
    }


    [PunRPC]
    void TeleportCard(string nameR, string murName)
    {
         Debug.Log(" TeleportCard 1 ");

        Debug.Log(nameR);
        float w, h;
        float div = 2 * 1000f;
        Texture tex= (Texture2D)textures[0];

        Transform mur;
        if (murName == "MUR B")
        { mur = MurB; }
        else if (murName == "MUR L")
        { mur = MurL; }
        else
        { mur = MurR; }

        int j = 0;
        for (int i = 0; i < cardList.Count; i++)
        {
            Debug.Log(cardList[i].transform.GetChild(0).GetComponent<Renderer>().material.name);
            if (cardList[i].transform.GetChild(0).GetComponent<Renderer>().material.name == nameR)
            {
                Debug.Log(" TeleportCard 2 ");
                Vector3 v = MurB.localScale;

                h = tex.height / div;
                w = tex.width / div;
                w = w * (v.y / v.x);

                Debug.Log("changement de mur B ");
                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.transform.parent = mur;
                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.transform.rotation = mur.rotation;

                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.transform.localScale = new Vector3(w, h, 1.0f);
                PhotonView.Find(cardList[i].GetComponent<PhotonView>().ViewID).transform.transform.localPosition = new Vector3(-0.35f + w + 1.5f * w * j, 0, -0.02f);
                j++;
            }
        }
    }
}
