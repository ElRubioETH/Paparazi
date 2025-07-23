using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class FixGame : MonoBehaviour
{
    [SerializeField] Transform topPivot;
    [SerializeField] Transform bottomPivot;

    [SerializeField] Transform Lightning;
    float lightningPosition;
    float lightningDestination;

    float lightningTimer;
    [SerializeField] float timerMultiplicator = 3f;

    float lightningSpeed;
    [SerializeField] float smoothMotion = 1f;

    [SerializeField] Transform hook;
    float hookPosition;
    [SerializeField] float hookSize = 0.1f;
    [SerializeField] float hookPower = 0.5f;
    float hookProgess;
    float hookPullVelocity;
    [SerializeField] float hookPullPower= 0.01f;
    [SerializeField] float hookGravityPower=0.005f;
    [SerializeField] float hookProgressDegradationPower = 0.1f;
    [SerializeField] SpriteRenderer hookSpriteRender;
    [SerializeField] Transform progressBarContainer;
    bool pause =false;

    [SerializeField] float failTimer = 10f;

    private void Start()
    {
        Resize();
    }

    private void Resize()
    {
        Bounds b = hookSpriteRender.bounds;
        float ySize = b.size.y;
        Vector3 Is = hook.localScale;
        float distance = Vector3.Distance(topPivot.position, bottomPivot.position);
        Is.y = (distance / ySize * hookSize);
        hook.localScale= Is;
    }

    private void Update()
    {
        if (pause) return;
        Lightnings();
        Hook();
        ProgressCheck();
    }
    private void ProgressCheck()
    {
        Vector3 Is = progressBarContainer.localScale;
        Is.y = hookProgess;
        progressBarContainer.localScale= Is;

        float min = hookPosition - hookSize / 2;
        float max = hookPosition + hookSize / 2;

        if (min < lightningPosition && lightningPosition < max)
        {
            hookProgess += hookPower * Time.deltaTime;
        }else
        {
            hookProgess -= hookProgressDegradationPower * Time.deltaTime;
            failTimer -= Time.deltaTime;
            if (failTimer < 0)
            {
                Lose();
            }
        }
        if(hookProgess >1f )
        {
            Win();
        }
        hookProgess = Mathf.Clamp(hookProgess, 0f, 1f);
    }
    private void Lose()
    {
        pause = true;
        Debug.Log("U Lose");
    }
    private void Win()
    {
         pause  = true;
        Debug.Log("U WIN");
    }
    void Hook()
    {
        if (Input.GetMouseButton(0))
        {
            hookPullVelocity += hookPullPower * Time.deltaTime;
        }
        hookPullVelocity -= hookGravityPower * Time.deltaTime;

        hookPosition += hookPullVelocity;

        if(hookPosition - hookSize /2 <= 0f && hookPullVelocity <0f )
        {
            hookPullVelocity = 0f;
        }
        if (hookPosition + hookSize / 2 >= 1f && hookPullVelocity > 0f)
        {
            hookPullVelocity = 0f;
        }
        hookPosition += Mathf.Clamp(hookPosition, hookSize/2 , 1 - hookSize/2);
        hook.position = Vector3.Lerp(bottomPivot.position, topPivot.position, hookPosition);
    }
    void Lightnings()
    {
        lightningTimer -= Time.deltaTime;
        if (lightningTimer < 0)
        {
            lightningTimer = UnityEngine.Random.value * timerMultiplicator;

            lightningDestination = UnityEngine.Random.value;
        }

        lightningPosition = Mathf.SmoothDamp(lightningPosition, lightningDestination, ref lightningSpeed, smoothMotion);
        Lightning.position = Vector3.Lerp(bottomPivot.position, topPivot.position, lightningPosition);

    }
}
