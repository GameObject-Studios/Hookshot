using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    // Fields
    [SerializeField] float speed, maxVelocity, lookSpeed;
    bool canMove = true;
    [SerializeField, Header("Player input values")] Vector2 move;
    [SerializeField] Vector2 look;
    [SerializeField] bool invertY;
    int inversion;
    [SerializeField] bool paused;
    readonly float camXRotLimit = 0.5f;

    // component references
    Rigidbody m_rigidbody;
    PlayerInput m_playerInput;
    InputActions m_actionsInput;
    Camera m_camera;

    // Properties
    Rigidbody rb { get { if (Equals(m_rigidbody, null)) { TryGetComponent(out m_rigidbody); } return m_rigidbody; } }
    PlayerInput playerInput { get { if (Equals(m_playerInput, null)) { TryGetComponent(out m_playerInput); } return m_playerInput; } }
    InputActions inputActions { get { if (Equals(m_actionsInput, null)) { m_actionsInput = new InputActions(); } return m_actionsInput; } }
    Transform cam { get { if (Equals(m_camera, null)) { m_camera = Camera.main; } return m_camera.transform; } }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    // Start is called before the first frame update
    void Start()
    {
        SetInversion();
        LockCursor();
    }
    // Update is called once per frame
    void Update()
    {
        move = canMove ? inputActions.Player.Move.ReadValue<Vector2>() : Vector2.zero;
        look = canMove ? inputActions.Player.Look.ReadValue<Vector2>() : Vector2.zero;
        SetInversion();
        HandlePausing();
    }
    void FixedUpdate()
    {
        MovePlayer();
    }
    void LateUpdate()
    {
        RotatePlayerAndCamera();
    }

    void HandlePausing()
    {
        if (!paused && inputActions.Player.Pause.IsPressed())
        {
            paused = true;
        }
        else if (paused && inputActions.Player.Unpause.IsPressed())
        {
            paused = false;
        }

        UpdateCursorState();
    }

    void LockCursor()
    {
        if (!CursorLockMode.Equals(Cursor.lockState, CursorLockMode.Locked))
            Cursor.lockState = CursorLockMode.Locked;

        if (!bool.Equals(Cursor.visible, false))
            Cursor.visible = false;

        if (!float.Equals(Time.timeScale, 1))
            Time.timeScale = 1;

        if (bool.Equals(canMove, false))
            canMove = true;
    }

    void UnlockCursor()
    {
        if (!CursorLockMode.Equals(Cursor.lockState, CursorLockMode.None))
            Cursor.lockState = CursorLockMode.None;

        if (!bool.Equals(Cursor.visible, true))
            Cursor.visible = true;

        if (Time.timeScale > 0)
            Time.timeScale = 0;

        if (bool.Equals(canMove, true))
            canMove = false;
    }

    void UpdateCursorState()
    {
        if (paused)
            UnlockCursor();
        else
            LockCursor();
    }

    void SetInversion()
    {
        inversion = invertY ? -1 : 1;
    }

    public void SetInversion(bool newInvert)
    {
        invertY = newInvert;
    }

    void MovePlayer()
    {
        var moveVector = (transform.right.normalized * move.x * speed) + (transform.forward.normalized * move.y * speed);

        if (!Vector3.Equals(move, Vector2.zero))
        {
            if (rb.velocity.magnitude < maxVelocity)
                rb.AddForce(moveVector);
        }
        else
        {
            var vec = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), 0.1f);  // TODO: Add check for current vel == (0, y, 0)
            rb.velocity = vec;
        }
    }

    void RotatePlayerAndCamera()
    {
        if (float.Equals(Time.timeScale, 0)) return;

        cam.Rotate(look.y * lookSpeed * inversion, 0, 0);
        transform.Rotate(0, look.x * lookSpeed, 0);

        if (cam.localRotation.x > camXRotLimit)
            cam.localRotation = new Quaternion(camXRotLimit, cam.localRotation.y, cam.localRotation.z, cam.localRotation.w);
        else if (cam.localRotation.x < -camXRotLimit)
            cam.localRotation = new Quaternion(-camXRotLimit, cam.localRotation.y, cam.localRotation.z, cam.localRotation.w);
    }

    // FOR DEBUGGING
#if UNITY_EDITOR
    private void OnGUI()
    {
        var style = new GUIStyle()
        {
            fontSize = 22
        };

        var display = "<color=\"black\"> Veclocity: </color><color=\"cyan\">" + System.Math.Round(rb.velocity.magnitude, 2).ToString() + "</color>";

        GUI.Label(new Rect(10, 40, 100, 200), display, style);
    }
#endif
}
