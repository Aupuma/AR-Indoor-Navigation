using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBgEventHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public void ShowMap()
    {
        CanvasManager.instance.SwitchState(AppState.CHOOSING_DESTINATION);
    }

    public void HideMap()
    {
        CanvasManager.instance.SwitchState(AppState.NAVIGATION);
    }
}
