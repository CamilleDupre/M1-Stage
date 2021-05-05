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

        //Teleport
        if (m_TeleportAction.GetStateUp(m_pose.inputSource))
            tryTeleport();
    }

    private void tryTeleport()
    {
        if (!m_HasPosition || m_IsTeleportoting)
            return;

        Vector3 headPosition = SteamVR_Render.Top().head.position;
        Transform cameraRig = SteamVR_Render.Top().origin;

        Vector3 groundPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);
        Vector3 translateVector = m_Pointer.transform.position - groundPosition;

        MoveO(cameraRig);
    }

    private void MoveO(Transform cameraRig)
    {
        m_IsTeleportoting = true;

        
        
           

        m_IsTeleportoting = false;

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
