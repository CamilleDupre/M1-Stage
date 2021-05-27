using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Touchpad : MonoBehaviour
{

    public GameObject m_Pointer;
    public SteamVR_Action_Boolean touchPos;
    private SteamVR_Behaviour_Pose m_pose = null;
    private bool m_HasPosition = false;
    private bool m_IsTeleportoting = false;
    private float m_FadeTime = 0.5f;
    RaycastHit hit;

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
        //m_Pointer.SetActive(m_HasPosition);

        //Teleport
        if (touchPos.GetStateUp(m_pose.inputSource))
            tryTeleport();
    }

    private void tryTeleport()
    {
        if (!m_HasPosition || m_IsTeleportoting)
            return;

        Vector3 headPosition = SteamVR_Render.Top().head.position;
        Transform cameraRig = SteamVR_Render.Top().origin;

        if (hit.transform.tag == "Tp")
        {
            Vector3 groundPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);
            Vector3 translateVector = m_Pointer.transform.position - groundPosition;

            StartCoroutine(MoveRig(cameraRig, translateVector));
        }

    }

    private IEnumerator MoveRig(Transform cameraRig, Vector3 translation)
    {
        m_IsTeleportoting = true;

        SteamVR_Fade.Start(Color.black, m_FadeTime, true);

        yield return new WaitForSeconds(m_FadeTime);
        cameraRig.position += translation;

        SteamVR_Fade.Start(Color.clear, m_FadeTime, true);

        m_IsTeleportoting = false;

    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Tp" || hit.transform.tag == "Card" || hit.transform.tag == "Wall")
            {
                m_Pointer.transform.position = hit.point;
                // m_Pointer.gameObject.SetActive(false);
                return true;

            }
        }
        return false;
    }
}
