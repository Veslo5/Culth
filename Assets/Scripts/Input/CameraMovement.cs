using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class CameraMovement : MonoBehaviour
{
    private PlayerInput input;

    private InputAction zoomAction;
    private InputAction clickAction;
    private void Awake()
    {
        input = GetComponent<PlayerInput>();

        zoomAction = input.actions["ZOOM"];
        zoomAction.performed += Zoomed;

        clickAction = input.actions["CLICK"];
        clickAction.started += ClickStart;
        clickAction.canceled += ClickReleased;

    }

    private void ClickReleased(InputAction.CallbackContext context)
    {
        drag = false;
    }

    private bool drag = false;
    Vector3 difference;
    Vector3 origin;
    private void ClickStart(InputAction.CallbackContext context)
    {
        drag = true;
        origin = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    private void Zoomed(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();
        // bug from unity input system
        value = Mathf.Abs(value) > 1 ? value / 120f : value;


        Camera.main.orthographicSize += value;

        if (Camera.main.orthographicSize < 2f)
        {
            Camera.main.orthographicSize = 2f;
            return;
        }

        if (value < 0f)
        {
            Camera.main.transform.position = Vector2.Lerp(Camera.main.transform.position, Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), 1f / Camera.main.orthographicSize);
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (drag == true)
        {
            difference = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - Camera.main.transform.position;
            Camera.main.transform.position = origin - difference;
        }
    }
}
