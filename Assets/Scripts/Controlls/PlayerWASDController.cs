﻿using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerWASDController : MonoBehaviour
{

    public float moveSpeed;
    public float jumpForce;
    public CharacterController character;

    public float gravityScale;
    private Vector3 moveDirection;
    public GameObject playerModel;
    public Transform pivot;
    public float rotateSpeed;
    public Animator animator;

    public delegate void OnFocusChanged(Interactable newFocus);
    public OnFocusChanged onFocusChanged;
    Interactable focus;

    private Camera camera;

    void Start()
    {
        character = GetComponent<CharacterController>();
        camera = Camera.main;
    }

    void Update()
    {
        Movement();
        HandleInput();
    }

    void Movement()
    {
        //moveDirection = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, moveDirection.y, Input.GetAxis("Vertical") * moveSpeed);
        float yStore = moveDirection.y;

        moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        moveDirection = moveDirection.normalized * moveSpeed;
        moveDirection.y = yStore;

        if (character.isGrounded)
        {
            moveDirection.y = 0f;
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpForce;
            }
        }


        moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);
        character.Move(moveDirection * Time.deltaTime);

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }

        animator.SetFloat("speed", (Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Horizontal"))));
        animator.SetBool("isGrounded", character.isGrounded);
    }

    void HandleInput()
    {
        if (character.isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                /*Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100f))
                {
                    Interactable interactable = hit.collider.GetComponent<Interactable>();
                    if (interactable != null)
                    {
                        //TODO: Automatic rotation to target 
                        SetFocus(interactable);
                    }
                }*/

                StartCoroutine(AttackRoutine());
            }
        }
    }


    void SetFocus(Interactable newFocus)
    {
        if (onFocusChanged != null)
        {
            onFocusChanged.Invoke(newFocus);
        }

        if (focus != newFocus && focus != null)
        {
            focus.OnUnfocused();
        }

        focus = newFocus;
        if (focus != null)
        {
            focus.OnFocused(transform);
        }

    }

    IEnumerator AttackRoutine()
    {
        animator.SetBool("punch", true);
        yield return new WaitForSeconds(0.15f);
        animator.SetBool("punch", false);

    }
}