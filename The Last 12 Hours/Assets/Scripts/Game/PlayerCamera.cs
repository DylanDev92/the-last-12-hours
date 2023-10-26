using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Player") is GameObject player && player != null)
        {
            virtualCamera.Follow = player.transform;
        }
    }
}
