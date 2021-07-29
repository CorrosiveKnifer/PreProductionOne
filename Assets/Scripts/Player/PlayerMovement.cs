﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    private Camera playerCamera;
    private float yVelocity = 0.0f;
    private Vector3 storedMovement;
    private Vector3 lastMovement;

    public bool grounded = true;
    public float movementSpeed = 2.0f;
    public float jumpSpeed = 4.0f;
    public float gravity = 9.81f;

    public float ledgeForgiveDelay = 0.0f;
    private float ledgeForgiveTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // Check if character is touching the ground
        if (characterController.isGrounded)
        {
            ledgeForgiveTimer = 0;
            grounded = true;

            if (yVelocity < 0.0f) // Checking if character is not going upwards
            {
                // Snap character to ground
                yVelocity = 0.0f;
            }
        }
        else
        {
            // If not grounded apply gravity
            yVelocity -= gravity * Time.fixedDeltaTime;

            if (ledgeForgiveTimer < ledgeForgiveDelay)
            {
                ledgeForgiveTimer += Time.fixedDeltaTime;
            }
            else if (grounded)
            {
                grounded = false;
                storedMovement = lastMovement;
            }
        }
    }
    public void Move(Vector2 _move, bool _jump)
    {
        Vector3 normalizedMove = new Vector3(0, 0, 0);

        // Movement
        normalizedMove += _move.y * transform.forward;
        normalizedMove += _move.x * transform.right;

        // Apply movement
        Vector3 movement = normalizedMove.normalized * movementSpeed * Time.fixedDeltaTime;
        lastMovement = movement;

        // Jump
        if (grounded && _jump)
        {
            yVelocity = jumpSpeed;
            grounded = false;
            storedMovement = lastMovement;
        }

        if (!grounded)
        {
            movement = storedMovement;
        }

        // Apply Y velocity
        movement.y += yVelocity * Time.fixedDeltaTime;

        // Apply movement to character controller
        characterController.Move(movement);
    }
}