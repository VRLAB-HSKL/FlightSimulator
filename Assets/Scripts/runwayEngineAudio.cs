using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class runwayEngineAudio : MonoBehaviour
{

    # region Variables
    private AudioManager am;
    # endregion
    

# region Lifecycle
// Start is called before the first frame update
    // can be a comment
    /*void Start()
    {
    }
    */

    // Update is called once per frame
    void Update()
    {
     am.Play("RunwayEngineSound");
     
    }
# endregion
}