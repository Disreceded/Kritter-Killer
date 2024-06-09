using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PseudoparentType {
    None,
    Position,
    Rotation,
    Scale,
    PositionRotation,
    PositionScale,
}
public class Pseudoparent : MonoBehaviour
{
    public GameObject parent;
    public PseudoparentType type = PseudoparentType.None;

    public Vector3 posOffset;
    private void FixedUpdate() {
        switch(type) {
            case PseudoparentType.Position:
                gameObject.transform.position = parent.transform.position + posOffset;
                break;
            case PseudoparentType.Rotation:
                gameObject.transform.rotation = parent.transform.rotation;
                break;
            case PseudoparentType.Scale:
                gameObject.transform.localScale = parent.transform.localScale;
                break;
            case PseudoparentType.PositionRotation:
                gameObject.transform.position = parent.transform.position;
                gameObject.transform.rotation = parent.transform.rotation;
                break;
            case PseudoparentType.PositionScale:
                gameObject.transform.position = parent.transform.position + posOffset;
                gameObject.transform.localScale = parent.transform.localScale;
                break;
        }
    }
}
