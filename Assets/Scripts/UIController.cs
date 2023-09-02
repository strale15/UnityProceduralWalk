using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public LaserPointer laserPointerScript;
    public Label isPasued;
    public Label mode;
    
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        isPasued = root.Q<Label>("is-paused");
        mode = root.Q<Label>("active-mode");
    }

    
    void Update()
    {
        isPasued.visible = laserPointerScript.isPaused;
        if(laserPointerScript.mode == Mode.StepGiver)
        {
            mode.text = "Step Giver";
        } else if(laserPointerScript.mode == Mode.MovePlayer)
        {
            mode.text = "Move";
        } else
        {
            mode.text = "Unknown";
        }
    }
}
