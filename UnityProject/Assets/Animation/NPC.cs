using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    // Points in the scene
    public Transform stopPoint;     // first stop (NPC looks at painting here)
    public Transform afterPoint;    // second point (NPC walks away)
    public Transform lookTarget;    // object NPC should face

    // Movement values
    public float moveSpeed = 1.5f;
    public float rotateSpeed = 360f;
    public float reachDistance = 0.05f; // how close is "arrived"

    // Animator references
    public Animator animator;
    public string speedParam = "Speed";   
    public string idleState = "Idle";
    public string lookRightState = "LookRight";
    public string lookLeftState = "LookLeft";

    // Animation blending
    public float crossFadeTime = 0.15f;
    public float speedDampTime = 0.15f;

    // Timing values
    public float lookIdleTime = 2.5f;     // standing and watching
    public float restartDelay = 1.5f;     // pause before reset

    // Slower animation playback for idle/look
    public float lookAnimSpeed = 0.6f;

    // Material color setup
    [Header("Color Customization")]
    public Renderer characterRenderer;     // main renderer with materials
    public int hairMaterialIndex = 0;
    public int trousersMaterialIndex = 1;
    public int shirtMaterialIndex = 2;
    public int scalpMaterialIndex = 6;     // same color as hair

    public Color[] hairColors;
    public Color[] shirtColors;
    public Color[] trousersColors;

    // Saved start transform
    Vector3 startPos;
    Quaternion startRot;

    void Awake()
    {
        // Get animator if not set
        if (animator == null)
            animator = GetComponent<Animator>();

        // Movement is handled by script, not animation
        animator.applyRootMotion = false;

        // Save start position for reset
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void Start()
    {
        // Start NPC behaviour loop
        StartCoroutine(LoopSequence());
    }

    IEnumerator LoopSequence()
    {
        while (true)
        {
            // Walk to first point
            yield return MoveToPoint(stopPoint.position);

            // Turn towards painting
            if (lookTarget != null)
                yield return RotateTowards(lookTarget.position);

            // Stand and idle
            yield return PlayIdleSlow(lookIdleTime);

            // Look right
            yield return PlayLookState(lookRightState);

            // Short idle
            yield return PlayIdleSlow(0.3f);

            // Look left
            yield return PlayLookState(lookLeftState);

            // Short idle again
            yield return PlayIdleSlow(0.5f);

            // Walk away
            yield return MoveToPoint(afterPoint.position);


            // Reset NPC and repeat
            ResetNPC();
            yield return null;
        }
    }

    IEnumerator PlayLookState(string stateName)
    {
        // Slow down animation
        animator.speed = lookAnimSpeed;

        // Play look animation
        CrossFadeTo(stateName, crossFadeTime);

        // Wait until animation finishes
        yield return WaitForStateFinished(stateName);

        // Restore normal speed
        animator.speed = 1f;
    }

    IEnumerator PlayIdleSlow(float duration)
    {
        // Slow idle animation
        animator.speed = lookAnimSpeed;

        CrossFadeTo(idleState, crossFadeTime);

        yield return new WaitForSeconds(duration);

        animator.speed = 1f;
    }

    void ResetNPC()
    {
        // Teleport NPC back to start
        transform.position = startPos;
        transform.rotation = startRot;

        // Reset animation state
        animator.speed = 1f;
        animator.Play(idleState, 0, 0f);
        animator.SetFloat(speedParam, 0f);

        // Change outfit colors
        RandomizeOutfitColors();
    }

    void RandomizeOutfitColors()
    {
        if (characterRenderer == null)
            return;

        Material[] mats = characterRenderer.materials;

        // Hair AND scalp use the same color
        if (hairColors != null && hairColors.Length > 0)
        {
            Color hairColor = hairColors[Random.Range(0, hairColors.Length)];

            if (hairMaterialIndex < mats.Length)
                mats[hairMaterialIndex].SetColor("_BaseColor", hairColor);

            if (scalpMaterialIndex < mats.Length)
                mats[scalpMaterialIndex].SetColor("_BaseColor", hairColor);
        }

        // Shirt color
        if (shirtColors != null && shirtColors.Length > 0 && shirtMaterialIndex < mats.Length)
            mats[shirtMaterialIndex].SetColor("_BaseColor",
                shirtColors[Random.Range(0, shirtColors.Length)]);

        // Trousers color
        if (trousersColors != null && trousersColors.Length > 0 && trousersMaterialIndex < mats.Length)
            mats[trousersMaterialIndex].SetColor("_BaseColor",
                trousersColors[Random.Range(0, trousersColors.Length)]);

        characterRenderer.materials = mats;
    }

    IEnumerator MoveToPoint(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > reachDistance)
        {
            // Update walk animation
            animator.SetFloat(speedParam, moveSpeed, speedDampTime, Time.deltaTime);

            Vector3 dir = target - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.0001f)
            {
                dir.Normalize();

                // Manual movement
                transform.position += dir * moveSpeed * Time.deltaTime;

                // Rotate towards movement direction
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, rot, rotateSpeed * Time.deltaTime);
            }

            yield return null;
        }

        // Stop walking
        animator.SetFloat(speedParam, 0f);
        CrossFadeTo(idleState, crossFadeTime);
    }

    IEnumerator RotateTowards(Vector3 worldPos)
    {
        // Rotate only on Y axis
        Vector3 dir = worldPos - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            yield break;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);

        while (Quaternion.Angle(transform.rotation, targetRot) > 1f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void CrossFadeTo(string stateName, float fadeTime)
    {
        // Smoothly switch animator states
        animator.CrossFadeInFixedTime(stateName, fadeTime, 0);
    }

    IEnumerator WaitForStateFinished(string stateName)
    {
        // Wait until state starts
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        // Wait until animation almost ends
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.98f)
            yield return null;
    }
}
