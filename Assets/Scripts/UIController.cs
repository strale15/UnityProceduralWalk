using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public LaserPointer laserPointerScript;
    public Label isPasued;
    public Label mode;
    public Label nextFootRight;
    public Label nextFootLeft;
    
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        isPasued = root.Q<Label>("is-paused");
        mode = root.Q<Label>("active-mode");
        nextFootRight = root.Q<Label>("next-foot-r");
        nextFootLeft = root.Q<Label>("next-foot-l");
    }

    
    void Update()
    {
        isPasued.visible = laserPointerScript.isPaused;

        if(laserPointerScript.leftFootDrawTurn)
        {
            nextFootRight.style.display = DisplayStyle.None;
            nextFootLeft.style.display = DisplayStyle.Flex;
        } 
        else
        {
            nextFootRight.style.display = DisplayStyle.Flex;
            nextFootLeft.style.display = DisplayStyle.None;
        }

        if(laserPointerScript.mode == Mode.StepGiver)
        {
            mode.text = "Step Giver";
        } 
        else if(laserPointerScript.mode == Mode.MovePlayer)
        {
            mode.text = "Move";
            nextFootRight.style.display = DisplayStyle.None;
            nextFootLeft.style.display = DisplayStyle.None;
        } 
        else
        {
            mode.text = "Unknown";
        }

    }
}
