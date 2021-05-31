using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
    }

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
                c.pv.RPC("LoadCard", Photon.Pun.RpcTarget.All, c.pv.ViewID, c.p.GetComponent<PhotonView>().ViewID , i);//GetComponent<PhotonView>().ViewID);
            }
            else if (i < 2* nbcard / 3)
            {
               MyCard c = new MyCard((Texture2D)textures[i], MurB, i - nbcard / 3);
                c.pv.RPC("LoadCard", Photon.Pun.RpcTarget.All, c.pv.ViewID, c.p.GetComponent<PhotonView>().ViewID, i - nbcard / 3);
            }
            else 
            {
              MyCard c = new MyCard((Texture2D)textures[i], MurR, i - 2 * nbcard / 3);
                c.pv.RPC("LoadCard", Photon.Pun.RpcTarget.All, c.pv.ViewID, c.p.GetComponent<PhotonView>().ViewID, i - 2 * nbcard / 3);
            }
        
        
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
