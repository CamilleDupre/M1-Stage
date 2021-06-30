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
        if (textures == null) // load one time the texture
        {
            bool card1 = GameObject.Find("/[CameraRig]/Controller (right)").GetComponent<Teleporter>().card1;
            if (card1)
            {
                textures = Resources.LoadAll("dixit_part1/", typeof(Texture2D));
            }
            else
            {
                textures = Resources.LoadAll("dixit_part2/", typeof(Texture2D));
            }
        }

        // wall + card
        Transform mur = PhotonView.Find(p).transform;
        GameObject goCard = PhotonView.Find(OB).gameObject;


        //set the texture
        Texture2D tex = (Texture2D)textures[t];
        goCard.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);

        //height and width depending on the size of te wall
        float w, h;
        float div = 2 * 1000f; //GetDiv();
        Vector3 v = mur.localScale;
        h = tex.height / div;
        w = tex.width / div;
        w = w * (v.y / v.x);

        //set parent, rotation , name , local scale
        goCard.transform.parent = mur;
        goCard.transform.rotation = mur.rotation;
        goCard.name = "Card " + t;
       
        goCard.transform.localScale = new Vector3(w, h, 1.0f);
        if (i < 10) //10 card per ligne
        {
            goCard.transform.localPosition = new Vector3(-0.35f + w + 1.5f * w * i, -1 * h, -0.001f);
        }
        else
        {
            i = i - 10;
            goCard.transform.localPosition = new Vector3(-0.35f + w + 1.5f * w * i, 1 * h, -0.001f);
        }
    }
}
