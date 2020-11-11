//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━																												
// Copyright 2020, Alexander Ameye, All rights reserved.
// https://alexander-ameye.gitbook.io/stylized-water/
//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━	

using UnityEngine;
using StylizedWater;

[RequireComponent(typeof(Rigidbody))]
public class BuoyantObject : MonoBehaviour
{
    private Color red = new Color(0.92f, 0.25f, 0.2f);
    private Color green = new Color(0.2f, 0.92f, 0.51f);
    private Color blue = new Color(0.2f, 0.67f, 0.92f);
    private Color orange = new Color(0.97f, 0.79f, 0.26f);

    [Header("Water Object")]
    public StylizedWater.StylizedWaterURP water;

    [Header("Buoyancy")]
    public Vector3 m_Gravity = Physics.gravity;

    [Range(1, 5)] public float strength = 1f;
    [Range(0.2f, 5)] public float objectDepth = 1f;

    public float velocityDrag = 0.99f;
    public float angularDrag = 0.5f;

    [Header("Effectors")]
    public Transform[] effectors;

    private Rigidbody rb;
    private Vector3[] effectorProjections;

    void Awake()
    {
        // Get rigidbody
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        effectorProjections = new Vector3[effectors.Length];
        for (int i = 0; i < effectors.Length; i++) effectorProjections[i] = effectors[i].position;
    }

    void OnDisable()
    {
        rb.useGravity = true;
    }
    
    void FixedUpdate()
    {
        int effectorAmount = effectors.Length;

        for (int i = 0; i < effectorAmount; i++)
        {
            Vector3 effectorPosition = effectors[i].position;

            effectorProjections[i] = effectorPosition;
            effectorProjections[i].y = water.transform.position.y + GerstnerWaveDisplacement.GetWaveDisplacement(effectorPosition, water.GetWaveSteepness(), water.GetWaveLength(), water.GetWaveSpeed(), water.GetWaveDirections()).y;

            // gravity
            rb.AddForceAtPosition(m_Gravity / effectorAmount, effectorPosition, ForceMode.Acceleration);

            float waveHeight = effectorProjections[i].y;
            float effectorHeight = effectorPosition.y;

            if (effectorHeight < waveHeight) // submerged
            {
                float submersion = Mathf.Clamp01(waveHeight - effectorHeight) / objectDepth;
                float buoyancy = Mathf.Abs(m_Gravity.y) * submersion * strength;

                // buoyancy
                rb.AddForceAtPosition(Vector3.up * buoyancy, effectorPosition, ForceMode.Acceleration);

                // drag
                rb.AddForce(-rb.velocity * velocityDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);

                // torque
                rb.AddTorque(-rb.angularVelocity * angularDrag * Time.fixedDeltaTime, ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (effectors == null) return;

        for (int i = 0; i < effectors.Length; i++)
        {
            if (!Application.isPlaying && effectors[i] != null)
            {
                Gizmos.color = green;
                Gizmos.DrawSphere(effectors[i].position, 0.06f);
            }

            else
            {
                if (effectors[i] == null) return;

                if (effectors[i].position.y < effectorProjections[i].y) Gizmos.color = red; //submerged
                else Gizmos.color = green;

                Gizmos.DrawSphere(effectors[i].position, 0.06f);

                Gizmos.color = orange;
                Gizmos.DrawSphere(effectorProjections[i], 0.06f);

                Gizmos.color = blue;
                Gizmos.DrawLine(effectors[i].position, effectorProjections[i]);
            }
        }
    }
}