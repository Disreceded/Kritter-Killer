using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /// <summary>
    /// A class dedicated to providing an animation event function at any time during an animation.
    /// </summary>

public class DestroyAnimationEventFunction : MonoBehaviour
{
    public void Destroy() { Destroy(gameObject); }
}
