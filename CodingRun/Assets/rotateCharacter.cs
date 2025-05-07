using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCharacter : MonoBehaviour
{
    [SerializeField]
    private GameObject character = null;

    [SerializeField]
    private float rotationSpeed = 3f;

    void Update()
    {
        character.transform.Rotate(Vector3.down * rotationSpeed * Time.deltaTime);
    }
}
