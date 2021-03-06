﻿using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float  paddleSpeed   = default;
    [SerializeField] private string inputAxisName = default;

    private Vector2 initialPosition;
    private Vector2 inputVelocity;
    private Rigidbody2D paddleBody;

    public void Reset()
    {
        inputVelocity       = Vector2.zero;
        paddleBody.velocity = inputVelocity;
        paddleBody.position = initialPosition;
    }
    void Awake()
    {
        paddleBody          = gameObject.transform.GetComponent<Rigidbody2D>();
        initialPosition     = paddleBody.position;
        inputVelocity       = Vector2.zero;
        paddleBody.velocity = inputVelocity;
    }
    void Update()
    {
        inputVelocity = new Vector2(0, paddleSpeed * Input.GetAxisRaw(inputAxisName));
    }
    void FixedUpdate()
    {
        paddleBody.velocity = inputVelocity;
    }
}
