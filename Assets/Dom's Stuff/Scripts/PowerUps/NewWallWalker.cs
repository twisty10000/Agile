﻿using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class NewWallWalker : MonoBehaviour
{
    private Transform Cam;

    [Header("Movement")]
    [Tooltip("How fast can the player move forward and backward")] [SerializeField] private float MovementSpeed = 10;
    //[Tooltip("How fast can the player move side to side")] [SerializeField] private float StrafeSpeed = 8;
    [Tooltip("How smooth the player rotates when latching to a surface")] [SerializeField] private float LerpSpeed = 5;
    [SerializeField] private float JumpHeight = 350;
    public bool CanWallWalk = false;
    public bool IsJumping = false;

    [SerializeField] private float TurnSpeed = 100;

    [Header("Gravity")]
    [SerializeField] private float Gravity = 10;
    private bool IsGrounded;
    private float DeltaGround = .2f;

    [Header("Ground")]
    [Tooltip("How close the player's feet have to be to the surface to lock onto it")] [SerializeField] private float GravityLock = 2;
    private Vector3 SurfaceNormal;
    private Vector3 MyNormal;
    private float GroundDistance;

    [Header("Components")]
    private Rigidbody RB;
    private CapsuleCollider CC;

    [Header("Player Step")]
    [SerializeField] private Transform LowerStepRay;
    [SerializeField] private Transform UpperStepRay;
    [SerializeField][Tooltip("How height can the player step")] private float StepHeight = .3f;
    [SerializeField][Tooltip("Smooth out transition from stepping")] private float StepSmooth = .1f;

    private void Start()
    {
        gameObject.tag = "Player";

        // Get main camera
        Cam = Camera.main.transform;

        // Get rigibody and collider
        RB = GetComponent<Rigidbody>();
        CC = GetComponent<CapsuleCollider>();


        MyNormal = transform.up;
        RB.freezeRotation = true;
        RB.useGravity = false;
        GroundDistance = CC.bounds.extents.y - CC.center.y;

        // Set step height transform to step height
        UpperStepRay.position = new Vector3(UpperStepRay.position.x, StepHeight, UpperStepRay.position.z);
    }

    private void FixedUpdate()
    {
        RB.AddForce(-Gravity * RB.mass * MyNormal);

        // Movement
        float VelocityZ = Input.GetAxis("Vertical") * MovementSpeed * Time.deltaTime;
        float VelocityX = Input.GetAxis("Horizontal") * MovementSpeed * Time.deltaTime;
        Vector3 MovementDir = new Vector3(VelocityX, 0, VelocityZ);
        MovementDir = Cam.forward * MovementDir.z + Cam.right * MovementDir.x;

        RB.AddForce(MovementDir, ForceMode.VelocityChange);

        //Step
        ClimbStep();
    }

    private void ClimbStep()
    {
        if(Physics.Raycast(LowerStepRay.position, LowerStepRay.forward, out RaycastHit HitLower, 0.1f))
        {
            if (!Physics.Raycast(UpperStepRay.position, LowerStepRay.forward, out RaycastHit HitUpper, 0.2f))
            {
                RB.position -= new Vector3(0f, -StepSmooth, 0f);
            }
        }

        if (Physics.Raycast(LowerStepRay.position, transform.TransformDirection(-1.5f, 0f, 1f), out RaycastHit HitLowerMinus45, 0.1f))
        {
            if (!Physics.Raycast(UpperStepRay.position, transform.TransformDirection(-1.5f, 0f, 1f), out RaycastHit HitUpperMinus45, 0.2f))
            {
                RB.position -= new Vector3(0f, -StepSmooth, 0f);
            }
        }

        if (Physics.Raycast(LowerStepRay.position, transform.TransformDirection(1.5f, 0f, 1f), out RaycastHit HitLower45, 0.1f))
        {
            if (!Physics.Raycast(UpperStepRay.position, transform.TransformDirection(1.5f, 0f, 1f), out RaycastHit HitUpper45, 0.2f))
            {
                RB.position -= new Vector3(0f, -StepSmooth, 0f);
            }
        }
    }

    private void Update()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && IsJumping == false)
        {
            IsJumping = true;
            RB.AddForce(transform.up * JumpHeight, ForceMode.Force);
        }

        // Gravity
        Ray ray;
        RaycastHit Hit;

        ray = new Ray(transform.position, -MyNormal);
        if (Physics.Raycast(ray, out Hit, GravityLock))
        {
            if (CanWallWalk)
            {
                if(Hit.transform.gameObject.layer == 18)
                {
                    IsGrounded = Hit.distance <= GroundDistance + DeltaGround;
                    SurfaceNormal = Hit.normal;
                }
            }
        }
        else
        {
            IsGrounded = false;
            SurfaceNormal = Vector3.up;
        }

        MyNormal = Vector3.Lerp(MyNormal, SurfaceNormal, LerpSpeed * Time.deltaTime);
        Vector3 MyForward = Vector3.Cross(transform.right, MyNormal);
        Quaternion TargetRotation = Quaternion.LookRotation(MyForward, MyNormal);
        transform.rotation = Quaternion.Lerp(transform.rotation, TargetRotation, LerpSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 11 || collision.gameObject.layer == 18)
        {
            IsJumping = false;
        }
    }
}