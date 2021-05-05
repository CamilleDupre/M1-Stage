using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MoveObject : MonoBehaviour
{
    public GameObject m_Pointer;
    public SteamVR_Action_Boolean m_TeleportAction;

    private SteamVR_Behaviour_Pose m_pose = null;
    private bool m_HasPosition = false;
    private bool m_IsTeleportoting = false;

    // Start is called before the first frame update
    void Awake()
    {
        m_pose = GetComponent<SteamVR_Behaviour_Pose>();
    }

    // Update is called once per frame
    void Update()
    {
        //Pointer
        m_HasPosition = UpdatePointer();
        m_Pointer.SetActive(m_HasPosition);

    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Wall")
            {
                m_Pointer.transform.position = hit.point;
                return true;
            }
        }
        return false;
    }
}
