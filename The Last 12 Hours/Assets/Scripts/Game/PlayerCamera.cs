using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;

    private CinemachineVirtualCamera virtualCamera;

    private CinemachineBasicMultiChannelPerlin camMCPerlin;
    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        camMCPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Player") is GameObject player && player != null)
        {
            virtualCamera.Follow = player.transform;
        }
    }

    // Shakes the camera, default values to intesity and seconds, if seconds is more than zero it uses the coroutine.
    public void StartShakeCamera(float intensity = 1, float seconds = 0)
    {
        if (camMCPerlin.m_AmplitudeGain > 0) return;

        camMCPerlin.m_AmplitudeGain = intensity;

        if (seconds > 0)
        {
            StartCoroutine(ShakeCameraDelayStop(seconds));
        }
    }

    // For stopping the shake
    public void StopShakeCamera()
    {
        camMCPerlin.m_AmplitudeGain = 0;
    }

    // Waits for the time and then stops the shaking
    private IEnumerator ShakeCameraDelayStop(float time)
    {
        yield return new WaitForSeconds(time);
        StopShakeCamera();
    }
}