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
// this class provides a way to pseudoparent (basically set some transform properties to the parent properties without the object ACTUALLY being a child of the parent object (hope that made sense)) something to something else
// this class is unused as of june 9th 2024.
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
