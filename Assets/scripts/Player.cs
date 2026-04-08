using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float smoothMoveTime = 0.1f;

    public float lookSensitivity = 2f;
    public float lookSmoothTime = 0.05f;

    private Vector2 currentMouseDelta;
    private Vector2 currentMouseDeltaVelocity;
    private Vector3 currentMoveVelocity;
    private Vector3 smoothMoveVelocity;
    private Vector2 rotation;

    private float fixedY;


    bool canMove = true;

    private void Start()
    {
        fixedY = transform.position.y;
        Manager.OnStateChanged += OnStateChangedListner;
    }

    private void OnStateChangedListner(VirtualCam state)
    {
        if (state == VirtualCam.PlayerCam)
        {
            canMove = true;
            //Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            canMove = false;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        if (!canMove)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.visible = false;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(h, 0f, v).normalized;
        Vector3 targetVelocity = inputDir * moveSpeed;

        currentMoveVelocity = Vector3.SmoothDamp(
            currentMoveVelocity,
            targetVelocity,
            ref smoothMoveVelocity,
            smoothMoveTime
        );

        transform.Translate(currentMoveVelocity * Time.deltaTime, Space.Self);

        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * lookSensitivity;

        Vector2 targetMouseDelta = new Vector2(mouseX, mouseY);

        currentMouseDelta = Vector2.SmoothDamp(
            currentMouseDelta,
            targetMouseDelta,
            ref currentMouseDeltaVelocity,
            lookSmoothTime
        );

        if (targetMouseDelta.sqrMagnitude > 0.0001f)
        {
            rotation.y += currentMouseDelta.x;
            rotation.x -= currentMouseDelta.y;
            rotation.x = Mathf.Clamp(rotation.x, -20f, 45f);
        }

        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);
    }

    private void OnDisable()
    {
        Manager.OnStateChanged -= OnStateChangedListner;
    }
}
