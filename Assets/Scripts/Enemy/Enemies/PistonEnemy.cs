using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonEnemy : MonoBehaviour
{
    [Header("Waiting times")]
    [Tooltip("Time it takes for the piston to go down")][SerializeField] float waitTimeToGoDown;
    [Tooltip("Time it takes for the piston to go up")][SerializeField] float waitTimeToGoUp;
    [Header("Movement speed")]
    [Tooltip("Speed of the piston when moving up")][SerializeField] float speedUp;
    [Tooltip("Speed of the piston when moving down")][SerializeField] float speedDown;
    [Header("Targets")]
    [Tooltip("Target position when moving down")][SerializeField] Transform targetDown;
    [Tooltip("Target position when moving up")][SerializeField] Transform targetUp;
    private Vector3 startingPosition;
    bool movingDown = false;
    bool movingUp = false;

    void Start()
    {
        // Starts the script by moving the piston towards the "down" position.
        MoveDown();
    }

    void Update()
    {
        if (movingDown)
        {
            // If the bool "movingDown" is true, then this code will move the piston towards the "down" position.
            var step =  speedDown * Time.deltaTime; // This multiplies the speed by deltaTime, it would be a sin if we didn't
            transform.position = Vector3.MoveTowards(transform.position, targetDown.position, step);
            if(Vector2.Distance(transform.position,targetDown.position)<0.1f)
            {
                movingDown = false;
                Invoke("MoveUp", waitTimeToGoUp);
            }
        }
        if (movingUp)
        {
            // If the bool "movingUp" is true, then this code will move the piston towards the "up" position.
            var step =  speedUp * Time.deltaTime; // This multiplies the speed by deltaTime, again, it would be a sin if we didn't
            transform.position = Vector3.MoveTowards(transform.position, targetUp.position, step);
            if(Vector2.Distance(transform.position,targetUp.position)<0.1f)
            {
                movingUp = false;
                Invoke("MoveDown", waitTimeToGoDown);
            }
        }
    }

    void MoveDown()
    {
        movingDown = true;
    }
    void MoveUp()
    {
        movingUp = true;
    }
}
