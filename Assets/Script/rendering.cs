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

    public class MyCard
    {
        public GameObject goCard = null;
        public string tag = "";
        public PhotonView pv;
        public Transform p;

        public MyCard(Texture2D tex, Transform mur , int i )
        {
            GameObject goCard = PhotonNetwork.InstantiateRoomObject("Quad (23)", mur.position, mur.rotation, 0, null);
            //goCard.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
            //goCard.transform.parent = mur;
            p = mur;
            //goCard.transform.rotation = mur.rotation;
            pv = goCard.GetPhotonView();
        }
    }

   // public static float GetDiv() { return 2 * 1000f; }

    // Start is called before the first frame update
    void Awake()
    {
        //StartCoroutine(waiter());
    }

    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
       // Debug.Log("new player");
        //base.OnPlayerEnteredRoom(newPlayer);
    //}

    public override void OnCreatedRoom() { 
        
        
       // object[] textures = Resources.LoadAll("dixit_part1/", typeof(Texture2D));
        object[] textures = Resources.LoadAll("dixit_part2/", typeof(Texture2D));

        // Debug.Log("TEXTURES: " + textures.Length);

        Debug.Log("Creation carte " + PhotonNetwork.IsMasterClient);

        int nbcard = textures.Length;
        for (int i = 0 ; i < nbcard ; i++)
        {
            if (i < nbcard / 3)
            {
              MyCard c = new MyCard((Texture2D)textures[i], MurL, i);
                c.pv.RPC("LoadCard", Photon.Pun.RpcTarget.AllBuffered, c.pv.ViewID, MurL.GetComponent<PhotonView>().ViewID , i);//GetComponent<PhotonView>().ViewID);
            }
            else if (i < 2* nbcard / 3)
            {
               MyCard c = new MyCard((Texture2D)textures[i], MurB, i - nbcard / 3);
                c.pv.RPC("LoadCard", Photon.Pun.RpcTarget.AllBuffered, c.pv.ViewID, MurB.GetComponent<PhotonView>().ViewID, i - nbcard / 3);
            }
            else 
            {
              MyCard c = new MyCard((Texture2D)textures[i], MurR, i - 2 * nbcard / 3);
                c.pv.RPC("LoadCard", Photon.Pun.RpcTarget.AllBuffered, c.pv.ViewID, MurR.GetComponent<PhotonView>().ViewID, i - 2 * nbcard / 3);
            }
        
        
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
