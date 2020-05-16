using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = Vector3.up * 5;
    [SerializeField] [Range(0, 1)] float movementFactor;
    [SerializeField] float frequency = 0.3f;

    Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float phase = 2 * Mathf.PI * frequency * Time.time;
        movementFactor = Mathf.Sin(phase);

        transform.position = startingPos + movementVector * movementFactor;
    }
}
