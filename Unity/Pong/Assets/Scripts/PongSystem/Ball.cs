using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    public GameManager gm;
    private Rigidbody2D ballRb;
    public float rotationSpeed = 1;
    public GameObject bar;
    public float maxSpeed = 0;  // 0 = infinity

    public float currentMagnitude;
    public float currentAngle;
    public float desiredMagnitude;
    public GameObject spawnPoint;

    public AudioSource audioSource;

    public ParticleSystem barHit;
    // Start is called before the first frame update
    void Start()
    {
        ballRb = GetComponent<Rigidbody2D>();
    }
    
    /// <summary>
    /// Launches the ball in a random direction (always 45 deg)
    /// </summary>
    public void LaunchBall(float speed = 2)
    {
        desiredMagnitude = speed;
        transform.position = new Vector3(spawnPoint.transform.position.x,spawnPoint.transform.position.y +0.5f, 47f);

        ballRb.velocity = new Vector2(speed * (int)(Random.Range(0, 2) * 2 - 1),  -Math.Abs(speed * (int)(Random.Range(0, 2) * 2 - 1)) );
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Ball: OnCollisionEnter2D Event");
        //Instantiate(barHit, new Vector3(transform.position.x,transform.position.y-0.3f,transform.position.z), transform.rotation,bar.transform);
        gm.BallTrigger(col);
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        currentMagnitude = ballRb.velocity.magnitude;
        //transform.Rotate(0,0,rotationSpeed);
    }

    /// <summary>
    /// Makes the ball faster (or slower with negative value)
    /// </summary>
    public void addSpeed(float amount)
    {
        desiredMagnitude += amount;
        if (maxSpeed > 0 && desiredMagnitude > maxSpeed)
            desiredMagnitude = maxSpeed;
    }

    /// <summary>
    /// Ensures the ball always has the correct velocity (speed)
    /// </summary>
    public void validateSpeed()
    {
        ballRb.velocity = ClampMagnitude(ballRb.velocity, desiredMagnitude + 0.01f, desiredMagnitude - 0.01f);
    }
    
    /// <summary>
    /// Ensures the ball always has an angle != 0
    /// </summary>
    public void validateAngle()
    {
        const float minY = 0.5f;
        const float minX = 0.1f;
        Vector2 norm = ballRb.velocity.normalized;
        if (norm.y is > -minY and < minY)
        {
            if (norm.y > 0) norm.y = minY;
            else norm.y = -minY;
        }
        if (norm.x is > -minX and < minX)
        {
            if (norm.x > 0) norm.x = minX;
            else norm.x = -minX;
        }
        ballRb.velocity = ClampMagnitude(norm, desiredMagnitude + 0.01f, desiredMagnitude - 0.01f);
    }

    /// <summary>
    /// Ball gets redirected when hitting moving playbar
    /// </summary>
    public void addSideForce(float amount)
    {
        const int amountLimit = 90;
        const float maxDegree = 30;
        
        const float amountMultiplier = maxDegree / amountLimit;
        
        if (amount < -amountLimit)
            amount = -amountLimit;
        else if (amount > amountLimit)
            amount = amountLimit;
        amount *= amountMultiplier;

        float rad = (90-amount) * Mathf.Deg2Rad;

        Vector2 newV = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        Vector2 velo = ballRb.velocity;
        Vector2 newDir = velo.normalized + newV;
        ballRb.velocity = newDir.normalized * velo.magnitude;
    }

    /// <summary>
    /// Helper function for validating Speed
    /// </summary>
    private static Vector3 ClampMagnitude(Vector3 v, float max, float min)
    {
        double sm = v.sqrMagnitude;
        if (sm > (double)max * (double)max) return v.normalized * max;
        if (sm < (double)min * (double)min) return v.normalized * min;
        return v;
    }
}
