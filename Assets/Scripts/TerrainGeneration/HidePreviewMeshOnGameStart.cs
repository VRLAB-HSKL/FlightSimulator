using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hides the parent gameobject on startup
/// </summary>
public class HidePreviewMeshOnGameStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }
}
