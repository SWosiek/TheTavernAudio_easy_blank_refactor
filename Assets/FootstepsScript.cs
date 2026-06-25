using UnityEngine;
using FMODUnity;

public class FootstepsScript : MonoBehaviour
{
    [Header("FMOD Events")]
    public EventReference footstepEvent;
    public EventReference jumpEvent;
    public EventReference landEvent;

    [Header("Timing")]
    public float stepRate = 0.5f;

    private float stepTimer;
    private bool isJumping = false;
    private float liftOffTime;

    void Update()
    {
        // 1. Walking (Only allowed to play if NOT mid-jump)
        if (!isJumping && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlaySound(footstepEvent);
                stepTimer = stepRate;
            }
        }

        // 2. Jump Trigger
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            PlaySound(jumpEvent);
            isJumping = true;
            liftOffTime = 0.15f;
        }

        // 3. Landing Logic
        if (isJumping)
        {
            liftOffTime -= Time.deltaTime;

            if (liftOffTime <= 0f)
            {
                if (Physics.Raycast(transform.position, Vector3.down, 1.2f))
                {
                    PlaySound(landEvent);
                    isJumping = false; // Footsteps are unlocked the exact frame you land!
                }
            }
        }
    }

    void PlaySound(EventReference fmodEvent)
    {
        int surfaceValue = 0;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2.0f))
        {
            if (hit.collider.CompareTag("Wood"))
            {
                surfaceValue = 1;
            }
        }

        FMOD.Studio.EventInstance instance = RuntimeManager.CreateInstance(fmodEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        instance.setParameterByName("Surface", surfaceValue);
        instance.start();
        instance.release();
    }
}