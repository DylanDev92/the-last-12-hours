using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SceneInteractable : Interactable
{
    [SerializeField] private bool interactToNextScene;
    [SerializeField] private int nextLevelId;

    private bool alreadyInteracted;
    private Volume postProcessingVolume;

    /// Awake is called when the script instance is being loaded. It initializes the post-processing volume and calls the base class Awake method.
    protected new void Awake()
    {
        postProcessingVolume = GameObject.Find("Post-Processing").GetComponentInChildren<Volume>();
        base.Awake();
    }

    // Interact is called to handle interaction logic. It checks conditions and initiates the transition to the next scene if applicable.
    public override void Interact()
    {
        if (!interactToNextScene || alreadyInteracted) return;

        NextScene();
        alreadyInteracted = true;
    }

    // NextScene handles the logic for transitioning to the next scene. It starts a coroutine for the transition effect or directly loads the next level.
    private void NextScene()
    {
        if (postProcessingVolume != null)
            StartCoroutine(NextSceneTransition());
        else
            GameManager.LoadLevelScene(nextLevelId);
    }

    // NextSceneTransition is a coroutine that gradually changes the post-exposure of the scene before loading the next level.
    private IEnumerator NextSceneTransition()
    {
        if (postProcessingVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            for (float i = colorAdjustments.postExposure.value; i > -5f; i -= 0.1f)
            {
                colorAdjustments.postExposure.value = i;
                yield return new WaitForSeconds(0.025f);
            }
        }
        GameManager.LoadLevelScene(nextLevelId);
    }

    // ShakeCamera initiates the camera shake effect.
    private void ShakeCamera()
    {
        PlayerCamera.Instance.StartShakeCamera(1.0f);
    }

    // StopShakeCamera stops the camera shake effect.
    private void StopShakeCamera()
    {
        PlayerCamera.Instance.StopShakeCamera();
    }
}
