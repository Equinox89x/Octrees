using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : BasicCharacter
{
    const string MOVEMENT_VERTICAL = "Vertical";
    const string MOVEMENT_HORIZONTAL = "Horizontal";

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
    }

    void HandleMovementInput()
    {
        if (_movementBehaviour == null) return;

        float horizontalMovement = Input.GetAxis(MOVEMENT_HORIZONTAL);
        float verticalMovement = Input.GetAxis(MOVEMENT_VERTICAL);

        Vector3 targetDirection = new Vector3(horizontalMovement, 0f, verticalMovement);
        targetDirection = Camera.main.transform.TransformDirection(targetDirection);
        targetDirection.y = 0.0f;

        _movementBehaviour.DesiredMovementDirection = targetDirection;
    }
}
