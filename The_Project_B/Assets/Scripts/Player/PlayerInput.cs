using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    [Tooltip("Sensibilité de la souris")]
    public float sensibiliteSouris = 1f;

    private const string axisHorizontal = "Horizontal";
    private const string axisVertical = "Vertical";

    private const string axisSourisHorizontal = "Mouse X";
    private const string axisSourisVertical = "Mouse Y";

    private const string jumpInput = "Jump";
    private const string crouchInput = "Crouch";

    public Vector3 getMoveInput()
    {

        Vector3 move = new Vector3(Input.GetAxisRaw(axisHorizontal), 0, Input.GetAxisRaw(axisVertical));

        move = Vector3.ClampMagnitude(move, 1);

        return move;
    }

    public float getLookInputHorizontal()
    {
        return GetMouseOrStickLookAxis(axisSourisHorizontal);
    }

    public float getLookInputVertical()
    {
        return GetMouseOrStickLookAxis(axisSourisVertical);
    }

    public bool getInputJump()
    {
        return Input.GetButtonDown(jumpInput);
    }

    public bool getInputCrouchDown()
    {
        return Input.GetButtonDown(crouchInput);
    }

    public bool getInputCrouchUp()
    {
        return Input.GetButtonUp(crouchInput);
    }

    float GetMouseOrStickLookAxis(string mouseInputName)
    {

        float i = Input.GetAxisRaw(mouseInputName);

        i *= sensibiliteSouris;

        i *= 0.01f;

        return i;

    }

}
