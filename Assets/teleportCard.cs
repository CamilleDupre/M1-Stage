using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class teleportCard : MonoBehaviour
{

    private RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Wall")
            {
                // Debug.Log("test");
               //photonView.RPC("ChangeMur", Photon.Pun.RpcTarget.All, nameM, ob.GetComponent<PhotonView>().ViewID);
            }

        }
    }
}
