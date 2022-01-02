using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCharacter : MonoBehaviour
{
    //protected ShootingBehaviour _shootingBehaviour;
    protected MovementBehaviour _movementBehaviour;
    // Start is called before the first frame update
    private void Awake()
    {
        //_shootingBehaviour = GetComponent<ShootingBehaviour>();
        _movementBehaviour = GetComponent<MovementBehaviour>();
    }
}
