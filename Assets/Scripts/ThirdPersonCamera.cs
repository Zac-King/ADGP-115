﻿using System.Collections.Generic;
using System.Runtime.InteropServices;
using Interfaces;
using Library;
using Units.Controller;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThirdPersonCamera : MonoBehaviour
{
#if !UNITY_WEBGL
    public struct Point
    {
        public int X;
        public int Y;

        public Point(int a_X, int a_Y)
        {
            X = a_X;
            Y = a_Y;
        }
    }
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out Point a_Point);
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);
#else
    [SerializeField]
    private EventSystem m_EventSystem;
#endif
    private Vector2 m_MouseAnchor;

    private Vector2 m_PrevMousePosition;
    private Vector2 m_DeltaMousePosition;

    [SerializeField]
    private GameObject m_Following;
    [SerializeField]
    private GameObject m_Target;

    [SerializeField]
    private Vector3 m_Offset;

    [System.Serializable]
    private struct Box
    {
        public Vector3 m_Min;
        public Vector3 m_Max;
    }

    [SerializeField]
    private Box m_ScreenBorders;

    private Camera m_Camera;

    public GameObject following
    {
        get { return m_Following; }
        set { m_Following = value; }
    }

    public GameObject target
    {
        get { return m_Target; }
        set { m_Target = value; }
    }

    public bool isTargeting
    {
        get { return m_Target != null; }
    }

    public Vector3 offset
    {
        get { return m_Offset; }
        set { m_Offset = value; }
    }

    private void Start()
    {
#if UNITY_WEBGL
        m_EventSystem = FindObjectOfType(typeof(EventSystem)) as EventSystem;
#endif

        if (m_Camera == null && GetComponentInChildren<Camera>() != null)
            m_Camera = GetComponentInChildren<Camera>();
        else
        {
            Debug.LogWarning(name + " needs a camera to be parented to this object!");
            gameObject.SetActive(false);
        }
    }

    private void OnGUI()
    {
        if (Event.current.type == EventType.ScrollWheel)
        {
            m_Offset += new Vector3(0, 0, -Event.current.delta.y);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_Camera == null || m_Following == null)
            return;

        CheckMouseInput();

        if (m_Target != null)
        {
            Vector3 newPosition = m_Following.transform.position + m_Target.transform.position;
            newPosition /= 2f;

            transform.position = newPosition;

            Vector3 distanceVector = transform.position - m_Following.transform.position;

            float distance = Mathf.Sqrt(
                Mathf.Pow(distanceVector.x, 2)
                + Mathf.Pow(distanceVector.y, 2)
                + Mathf.Pow(distanceVector.z, 2));
            m_Offset = new Vector3(m_Offset.x, m_Offset.y, -10 - (distance * 1.1f));

            m_Camera.transform.localPosition = m_Offset;

            float angle = Mathf.Atan(distanceVector.x / distanceVector.z);
            if ((distanceVector.x < 0.0f && distanceVector.z < 0.0f) ||
                (distanceVector.x > 0.0f && distanceVector.z < 0.0f) ||
                (distanceVector.x == 0.0f && distanceVector.z < 0.0f))
                angle += Mathf.PI;

            transform.eulerAngles = new Vector3(
                35f,
                angle * (180f / Mathf.PI) - 25f,
                transform.eulerAngles.z);
        }
        else
        {
            transform.position = m_Following.transform.position;
            m_Camera.transform.localPosition = m_Offset;
        }

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, m_ScreenBorders.m_Min.x, m_ScreenBorders.m_Max.x),
            Mathf.Clamp(transform.position.y, m_ScreenBorders.m_Min.y, m_ScreenBorders.m_Max.y),
            Mathf.Clamp(transform.position.z, m_ScreenBorders.m_Min.z, m_ScreenBorders.m_Max.z));
    }

    private void CheckMouseInput()
    {
#if !UNITY_WEBGL
        Point currentMousePosition;
        GetCursorPos(out currentMousePosition);

        m_DeltaMousePosition =
            new Vector2(currentMousePosition.X, currentMousePosition.Y) - m_PrevMousePosition;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            m_MouseAnchor = new Vector2(currentMousePosition.X, currentMousePosition.Y);

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            Cursor.visible = false;

            SetCursorPos((int)m_MouseAnchor.x, (int)m_MouseAnchor.y);
            GetCursorPos(out currentMousePosition);

            Vector3 newAngle = transform.eulerAngles;
            newAngle +=
                new Vector3(
                    m_DeltaMousePosition.y / 12f,
                    m_DeltaMousePosition.x / 10f,
                    0);

            newAngle =
                    new Vector3(
                        Mathf.Clamp(newAngle.x, 10, 90),
                        newAngle.y,
                        newAngle.z);

            transform.eulerAngles = newAngle;
        }
        else
            Cursor.visible = true;

        m_PrevMousePosition = new Vector2(currentMousePosition.X, currentMousePosition.Y);
#else
        if (m_EventSystem.currentSelectedGameObject == null)
        {
            Vector2 currentMousePosition = Input.mousePosition;
            m_DeltaMousePosition =
                new Vector2(
                    Input.GetAxis("Mouse X"),
                    -Input.GetAxis("Mouse Y"));

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                m_MouseAnchor = currentMousePosition;
            }
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                Vector3 newAngle = transform.eulerAngles;
                newAngle +=
                    new Vector3(
                        m_DeltaMousePosition.y / 3500f,
                        m_DeltaMousePosition.x / 3000f,
                        0);

                newAngle =
                        new Vector3(
                            Mathf.Clamp(newAngle.x, 10, 90),
                            newAngle.y,
                            newAngle.z);

                transform.eulerAngles = newAngle;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if (currentMousePosition != m_MouseAnchor)
                m_PrevMousePosition = currentMousePosition;
        }
#endif
    }
}
