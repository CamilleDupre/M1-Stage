using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player_Spawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerPrefab;
    // Start is called before the first frame update
    public override void OnJoinedRoom()
    {
        //spawn the player with the prefab
        base.OnJoinedRoom();
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
    }

    public override void OnLeftRoom()
    {
        //destroy the player prefab
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
