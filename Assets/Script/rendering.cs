using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public MyCard(GameObject go, Texture2D tex, Transform mur)
        {
            goCard = go;
            goCard.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);

            goCard.transform.parent = mur;
            goCard.transform.rotation = mur.rotation;

           // goCard.AddComponent<PhotonView> ();

            Debug.Log("TEXTURES: " + tex.width + " " + tex.height);

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
            goCard.transform.localPosition = new Vector3(0, 0, -0.001f);
        }
    }

    public static float GetDiv() { return 2 * 1000f; }

    // Start is called before the first frame update
    void Start()
    {
        object[] textures = Resources.LoadAll("dixit_part1/", typeof(Texture2D));

        Debug.Log("TEXTURES: " + textures.Length);

        int nbcard = 1;
        for (int i = 0 ; i < nbcard ; i++)
        {
            if (i < nbcard / 3)
            {
                MyCard c = new MyCard(Instantiate(pfCard), (Texture2D)textures[i], MurL);
            }
            else if (i < 2* nbcard / 3)
            {
                MyCard c = new MyCard(Instantiate(pfCard), (Texture2D)textures[i], MurB);
            }
            else 
            {
                MyCard c = new MyCard(Instantiate(pfCard), (Texture2D)textures[i], MurR);
            }
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
