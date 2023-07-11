// автор Pravda_Sempai xd
// https://www.youtube.com/channel/UCGHBUHxuZL9v8hEguNZkk7g
using System.Collections;
using UnityEngine;


[System.Serializable]
public class MaterialSound
{
    public PhysicMaterial material;
    public AudioClip[] walkSounds;
    public AudioClip[] runSounds;
    public AudioClip[] crouchSounds;
}

public enum SoundType
{
    Walk,
    Run,
    Crouch
}

public class FpsMove : MonoBehaviour
{
    [Space(20)] public Rigidbody rb;
    public float walkSpeed = 5f;
    public float crouchSpeed = 2f;
    Vector3 playerInput;
    Vector3 velocity;
    Vector3 velocityChange;

    [Space(20)]
    public bool enableJump = true;
    public float jumpPower = 5f;
    public float crouchScale = 0.6f;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Space(20)]
    public AudioClip[] defaultWalkSounds;
    public MaterialSound[] materialSounds;

    public AudioSource audioSource;


    private bool isJumping;
    private bool isGrounded;
    private PhysicMaterial currentMaterial;

    private bool isPlayingSound;
    private float soundDelay = 1.5f;

    private void Update()
    {
        if (enableJump && Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        CheckGround();
    }

    void FixedUpdate()
    {
        float moveSpeed = walkSpeed;

        if (playerInput != Vector3.zero && isGrounded)
        {
            if (Input.GetKey(runKey))
            {
                moveSpeed *= 2f;
                PlayMaterialSound(currentMaterial, SoundType.Run);
            }
            else
            {
                if (Input.GetKey(crouchKey))
                {
                    transform.localScale = new Vector3(1f, crouchScale, 1f);
                    moveSpeed = crouchSpeed;
                    PlayMaterialSound(currentMaterial, SoundType.Crouch);
                }
                else
                {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    PlayMaterialSound(currentMaterial, SoundType.Walk);
                }
            }
        }
        else
        {
            audioSource.Stop();
        }

        playerInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        playerInput = transform.TransformDirection(playerInput) * moveSpeed;

        velocity = rb.velocity;
        velocityChange = (playerInput - velocity);
        velocityChange.y = 0;

        rb.MovePosition(rb.position + velocityChange * Time.fixedDeltaTime);
    }

    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = .75f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            isGrounded = true;
            isJumping = false;

            currentMaterial = hit.collider.sharedMaterial;
        }
        else
        {
            isGrounded = false;
            isJumping = true;
            currentMaterial = null;
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void PlayMaterialSound(PhysicMaterial material, SoundType soundType)
    {
        if (!isPlayingSound)
        {
            AudioClip[] soundClips = GetSoundClips(material, soundType);

            if (soundClips != null && soundClips.Length > 0)
            {
                audioSource.Stop();

                int randomIndex = Random.Range(0, soundClips.Length);
                audioSource.clip = soundClips[randomIndex];
                audioSource.Play();

                isPlayingSound = true;

                StartCoroutine(ResetSoundDelay());
            }
        }
    }

    private IEnumerator ResetSoundDelay()
    {
        yield return new WaitForSeconds(soundDelay);

        isPlayingSound = false;
    }

    private AudioClip[] GetSoundClips(PhysicMaterial material, SoundType soundType)
    {
        foreach (MaterialSound materialSound in materialSounds)
        {
            if (materialSound.material == material)
            {
                switch (soundType)
                {
                    case SoundType.Walk:
                        return materialSound.walkSounds;
                    case SoundType.Run:
                        return materialSound.runSounds;
                    case SoundType.Crouch:
                        return materialSound.crouchSounds;
                }
            }
        }

        return null;
    }

    private void PlayRandomSound(AudioClip[] clips)
    {
        if (!audioSource.isPlaying && clips != null && clips.Length > 0)
        {
            int randomIndex = Random.Range(0, clips.Length);
            audioSource.PlayOneShot(clips[randomIndex]);
        }
    }
}