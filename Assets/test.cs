using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void LoadCard(int OB, int p , int i)
    {
        object[] textures = Resources.LoadAll("dixit_part2/", typeof(Texture2D));
        Transform mur = PhotonView.Find(p).transform;
        GameObject goCard = PhotonView.Find(OB).gameObject;

        goCard.transform.parent = mur;
        goCard.transform.rotation = mur.rotation;
        float w, h;
        Vector3 v = mur.localScale;

        float div = 2 * 1000f; //GetDiv();
        Texture2D tex = (Texture2D)textures[i];
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

        Debug.Log("rpc: ");
    }
}
