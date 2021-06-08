using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LoadingCard : MonoBehaviour
{
    object[] textures;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void LoadCard( int OB, int p , int i , int t)
    {
        if (textures == null)
        {
            textures = Resources.LoadAll("dixit_part2/", typeof(Texture2D));
        }

        Transform mur = PhotonView.Find(p).transform;
        GameObject goCard = PhotonView.Find(OB).gameObject;
        Texture2D tex = (Texture2D)textures[t];

        //GameObject goCard = PhotonNetwork.InstantiateRoomObject("Quad (23)", mur.position, mur.rotation, 0, null);
        goCard.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);

        goCard.transform.parent = mur;
        goCard.transform.rotation = mur.rotation;
        goCard.name = "Card " + t;
        float w, h;
        Vector3 v = mur.localScale;

        float div = 2 * 1000f; //GetDiv();
       
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

       // Debug.Log("rpc: ");
    }
}
