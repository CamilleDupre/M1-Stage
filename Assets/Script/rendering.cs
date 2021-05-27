using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class rendering : MonoBehaviour
{
    public GameObject pfCard;
    public Transform MurB;
    public Transform MurL;
    public Transform MurR;

    public class MyCard
    {
        public GameObject goCard = null;
        public string tag = "";

        public MyCard(Texture2D tex, Transform mur , int i )
        {

            GameObject goCard = PhotonNetwork.InstantiateRoomObject("Quad (23)", mur.position, mur.rotation, 0, null);
            goCard.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);

            goCard.transform.parent = mur;
            goCard.transform.rotation = mur.rotation;

           // goCard.AddComponent<PhotonView> ();

            //Debug.Log("TEXTURES: " + tex.width + " " + tex.height);

            float w, h;
            Vector3 v = mur.localScale;

            float div = GetDiv();

            h = tex.height / div;
            w = tex.width / div;

            //Debug.Log("scale: " + v);
            // h = h/mur.
            w = w * (v.y / v.x);

            // goCard.transform.localScale = new Vector3(w, h, 1.0f); // new Vector3(0.04165002f, 0.3106501f, 1.01f);
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
           
           
        }
    }

    public static float GetDiv() { return 2 * 1000f; }

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(waiter());

    }

    IEnumerator waiter()
    {
        //Wait for 4 seconds
        yield return new WaitForSeconds(3); // ici wait pour le spawn du playeur 
       // object[] textures = Resources.LoadAll("dixit_part1/", typeof(Texture2D));
        object[] textures = Resources.LoadAll("dixit_part2/", typeof(Texture2D));

        // Debug.Log("TEXTURES: " + textures.Length);

        int nbcard = textures.Length;
        for (int i = 0 ; i < nbcard ; i++)
        {

           // MyCard c = new MyCard((Texture2D)textures[i], MurB, nbcard , i);

            if (i < nbcard / 3)
            {
              MyCard c = new MyCard((Texture2D)textures[i], MurL, i);
            }
            else if (i < 2* nbcard / 3)
            {
               MyCard c = new MyCard((Texture2D)textures[i], MurB, i - nbcard / 3);
            }
            else 
            {
              MyCard c = new MyCard((Texture2D)textures[i], MurR, i - 2 * nbcard / 3);
            }
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
