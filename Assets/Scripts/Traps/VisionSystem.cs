using System.Collections;
using System.Collections.Generic;
using CharacterMovement;
using UnityEngine;

public class VisionSystem : MonoBehaviour
{
    [SerializeField] public Vector3 _range { get; set; } = new Vector3(5f, 0.1f, 5f); //vision range of the traps
    [SerializeField] private float _FOV = 360f; //field of vision around the traps
    [SerializeField] private Transform _head; //head location of the target
    [SerializeField] private LayerMask _occlusionMask; // mask that represents walls/obstructions between the trap and players
    [SerializeField] private LayerMask _visibilityMask; // mask that represents what is currently observable

    public Vector3 HeadPosition => _head.position;
    public Vector3 HeadDirection => _head.forward;

    public bool TestVisibility(Vector3 point)
    {
        Vector3 vector = point - HeadPosition;
        float distance = vector.magnitude;
        if(distance > _range.magnitude) return false; // point is far away

        // Vector3 direction = vector.normalized;
        // float angle = Vector3.Angle(HeadDirection, direction);
        // if(angle > _FOV * 0.5f) return false; // point is out of the fov
        // Debug.Log("Player is in fov");

        if(Physics.Linecast(HeadPosition, point, _occlusionMask)) return false; //vision is blocked if the linecast hits something

        return true;
    }

    public List<CustomCharacterMovement> GetVisibleTargets()
    {
        List<CustomCharacterMovement> targets = new List<CustomCharacterMovement>();
        Collider[] hits = Physics.OverlapBox(HeadPosition, _range, Quaternion.identity, _visibilityMask);
        foreach(Collider hit in hits)
        {
            if(hit.gameObject == gameObject) continue;
            if(!hit.TryGetComponent(out CustomCharacterMovement targetPlayer)) continue;
            if(!TestVisibility(targetPlayer.transform.position)) continue;

            targets.Add(targetPlayer);
        }

        return targets;
    }

    private void OnDrawGizmosSelected() {
        if(_head == null) return; //stops if it doesnt find the head component

        Gizmos.DrawWireCube(HeadPosition, _range);
        float halfFOV = _FOV * 0.5f;
        Quaternion rotationRight = Quaternion.Euler(0f, halfFOV, 0f) * _head.rotation;
        Quaternion rotationLeft = Quaternion.Euler(0f, -halfFOV, 0f) * _head.rotation;
        Vector3 rayRight = rotationRight * Vector3.forward * _range.magnitude;
        Vector3 rayLeft = rotationLeft * Vector3.forward * _range.magnitude;
        Gizmos.DrawRay(HeadPosition, rayRight);
        Gizmos.DrawRay(HeadPosition, rayLeft);
    }
}
