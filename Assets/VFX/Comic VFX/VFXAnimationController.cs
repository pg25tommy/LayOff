using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXAnimationController : MonoBehaviour
{
    [BoxGroup("Animation Settings")] public float vfxLifetime = 3f;
    [BoxGroup("Animation Settings"), Tooltip("Go to Assets/VFX/Characters/Comic VFX/Animations to find the animation clips and copy their names.")] public string animationClipName;
    [BoxGroup("Refs")] public Animator animator;
    [BoxGroup("Refs")] public GameObject objectToDestroy;
    [BoxGroup("Refs")] public PhotonView photonView;

    private void Awake()
    {
        if (animator != null)
        {
            animator.Play(animationClipName);
        }
        if (objectToDestroy is not null)
        {
            StartCoroutine(DestroyVFX());
        }
    }

    private IEnumerator DestroyVFX()
    {
        yield return new WaitForSeconds(vfxLifetime);

        if (photonView is not null)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(objectToDestroy.gameObject);
            }
        }
        else if (photonView is null)
        {
            Destroy(objectToDestroy.gameObject);
            //Debug.LogError("Missing photonView reference in: " + this.name);
        }
    }
}
