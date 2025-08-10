using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public CharacterController characterController;
    public InputActionAsset InputActions;
    public float mouseSensitivity = 150.0f;
    public Transform playerBody;
    private Vector3 position;

    private InputAction m_moveAction;
    private InputAction m_jumpAction;
    private InputAction m_dashAction;
    private InputAction m_mouseAction;
    private InputAction m_rewindAction;
    private InputAction m_fireAction;
    private Vector2 m_moveAmt;
    private float xRotation;

    private float yBuffer;

    public Transform groundCheck;
    public float groundDistance = 0.01f;
    public LayerMask groundMask;

    public float speed = 12f;
    public float gravity = -30.0f;
    public float jumpHeight = 3.0f;
    public Vector3 fullDashVel;
    public float DASH_TIME_MAX = 1.0f;

    private int health;
    private const int MAX_HEALTH = 50;

    public int timelineUpdateFreq = 5;
    public int timelineLength = 5;
    private int timelineCount;
    private int timelineLimit;
    private float timelineCountdown;
    private TimeCell buffer;
    private float fps;

    private Vector3 velocity;
    private Vector3 dashVel = Vector3.zero;
    private float dashTimer;
    bool grounded = false;
    bool doubleJump = true;

    public TimerScript timerRef;
    public Health healthRef;
    private Gun gunRef;

    LinkedList<TimeCell> timeline = new LinkedList<TimeCell>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        timelineLimit = timelineUpdateFreq * timelineLength;
        timelineCountdown = (float)timelineUpdateFreq / 100;
        gunRef = GetComponentInChildren<Gun>();
    }

    public void spawn(Vector3 t_position) 
    {
        health = MAX_HEALTH;
        characterController.enabled = false;
        characterController.transform.position = t_position;
        transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        characterController.enabled = true;
        xRotation = 0.0f;
        timeline.Clear();
    }

    private void Update()
    {
        healthRef.setValue(health);

        if (m_fireAction != null)
        {
            m_fireAction.performed += ctx => gunRef.fire();
            m_fireAction.Enable();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (grounded && velocity.y < 0)
        {
            velocity = Vector3.zero;
            velocity.y = -2.0f;
            doubleJump = true;
        }

        m_moveAmt = m_moveAction.ReadValue<Vector2>();
        var mouseInput = m_mouseAction.ReadValue<Vector2>();

        xRotation -= mouseInput.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);

        if (!m_rewindAction.IsPressed())
        {
            playerBody.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);
            transform.Rotate(0, mouseInput.x * mouseSensitivity, 0);
        }

        yBuffer = velocity.y;

        velocity = transform.right * m_moveAmt.x + transform.forward * m_moveAmt.y;

        velocity.y = yBuffer;
        if (m_dashAction != null)
        {
            //m_dashAction.performed += ctx => dash();
            if (m_dashAction.WasReleasedThisFrame() && dashTimer <= 0.0f)
            {
                dash();
                dashTimer = DASH_TIME_MAX;
            }
            else
            {
                if (dashTimer > 0.0f)
                {
                    dashTimer -= Time.deltaTime;
                }
                else
                {
                    dashVel = Vector3.zero;
                    dashTimer = 0.0f;
                }
            }
        }

        velocity.x *= speed;
        velocity.z *= speed;

        if (m_jumpAction != null)
        {
            m_jumpAction.performed += ctx => jump();
        }

        velocity.y += gravity * Time.deltaTime;
        velocity += dashVel;

        if (dashVel != Vector3.zero)
        {
            dashVel.x -= dashVel.x / 10.0f;
            dashVel.z -= dashVel.z / 10.0f;

            velocity.y += dashVel.y;
            dashVel.y -= 0.05f;
        }

        if (m_rewindAction.IsPressed() && timeline.Count > 0)
        {
            rewind();
        }
        else if (!m_rewindAction.IsPressed())
        {
            updateTime(mouseInput);
            timerRef.stopRewinding();
            characterController.Move(velocity * Time.deltaTime);
        }
    }

    public bool isAlive()
    {
        if (health > 0)
        {
            return true;
        }
        return false;
    }

    public void damage()
    {
        if (health > 0)
        { 
            health -= 10; 
        }
        if (health <= 0)
        {
            die();
        }
    }

    public void die()
    {
        Debug.Log("Player dead");
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }

    private void dash()
    {
        dashVel = Vector3.Scale(velocity, fullDashVel);
        dashVel.y = 1.0f;
    }

    private void jump()
    {
        if (grounded || doubleJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
            if (!grounded && doubleJump)
            {
                doubleJump = false;
            }
        }
    }


    private void updateTime(Vector2 t_mInput)
    {
        if (timelineCountdown > 0.0f)
        {
            timelineCountdown -= Time.deltaTime;
        }
        else
        {
            timelineCountdown = (float)timelineUpdateFreq / 100;

            buffer.position = characterController.transform.position;
            buffer.velocity = velocity;
            buffer.mouseInputX = t_mInput.x;
            buffer.xRotation = xRotation;
            buffer.timer = timerRef.getTime();
            buffer.health = health;

            while (timeline.Count >= timelineLimit)
            {
                timeline.RemoveFirst();
            }

            timeline.AddLast(buffer);
            timelineCount = timeline.Count;
        }
    }

    private void rewind()
    {
        if (timelineCountdown > 0.0f)
        {
            timelineCountdown -= Time.deltaTime;
        }
        else
        {
            timerRef.startRewinding();
            timelineCountdown = ((float)timelineUpdateFreq / 2.0f) / 100;
            characterController.transform.position = timeline.Last.Value.position;
            velocity = timeline.Last.Value.velocity;
            playerBody.localRotation = Quaternion.Euler(timeline.Last.Value.xRotation, 0.0f, 0.0f);
            transform.Rotate(0, -timeline.Last.Value.mouseInputX, 0);
            timerRef.rewindTimer(timeline.Last.Value.timer);
            health = timeline.Last.Value.health;
            healthRef.setValue(health);
            timeline.RemoveLast();
        }
    }


    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }
    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }
    private void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_mouseAction = InputSystem.actions.FindAction("Look");
        m_jumpAction = InputSystem.actions.FindAction("Jump");
        m_dashAction = InputSystem.actions.FindAction("Sprint");
        m_rewindAction = InputSystem.actions.FindAction("Rewind");
        m_fireAction = InputSystem.actions.FindAction("Attack");
    }
}


struct TimeCell
{
    public Vector3 position;
    public Vector3 velocity;
    public float mouseInputX;
    public float xRotation;
    public float timer;
    public int health;
}