﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Packages;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    Vector3 mousePosition;

    public float time = 0f;

    Rigidbody2D dupedBullet;
    public GameObject duplicateBullet;
    [SerializeField] GameObject bullet;

    [SerializeField] Rigidbody2D bulletRb;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    // gun transform
    [SerializeField] private Transform gun;

    [SerializeField] private float runSpeed = 5f;
    Vector2 moveInput;
    Rigidbody2D playerRigidbody;
    bool enemyHit = false;
    Vector2 playerPosition;

    // Jump
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private float jumpingPower = 6f;


    void Start()
    {
        // hides gun
        //gun.GetComponent<Renderer>().enabled = false;
        vcam = GetComponent<CinemachineVirtualCamera>();

        playerRigidbody = GetComponent<Rigidbody2D>();
        playerRigidbody.freezeRotation = true;
        playerPosition = playerRigidbody.position;
    }

    void Update()
    {
        Move();
        FlipSprite();

        if (enemyHit == true)
        {
            playerRigidbody.position = playerPosition;
        }
    }

    void OnFire()
    {
        duplicateBullet = Instantiate(bullet, gun.position, gun.rotation);
        dupedBullet = duplicateBullet.GetComponent<Rigidbody2D>();

        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (transform.localScale.x == -1f)
        {
            dupedBullet.AddForce((mousePosition * -1) * -100);
        }
        else
        {
            dupedBullet.AddForce((mousePosition * 1) * 100);
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    void Move()
    {

        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, playerRigidbody.velocity.y);

        if (moveInput.y > 0 && IsGrounded())
        {
            playerVelocity = new Vector2(moveInput.x, jumpingPower);
        }

        playerRigidbody.velocity = playerVelocity;

    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void FlipSprite()
    {
        bool playerHasSpeed = Mathf.Abs(playerRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidbody.velocity.x), transform.localScale.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            enemyHit = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            enemyHit = false;
        }
    }
}
