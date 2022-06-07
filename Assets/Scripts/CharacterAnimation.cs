using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class CharacterAnimation: MonoBehaviour
{
    SkeletonAnimation skeletonAnimation;
    Spine.Bone bone;
    [SerializeField] Transform center;
    int runningSpeed = 80;
    int walkingSpeed = 10;
    bool aim = false;
    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        //StartCoroutine(WalkCenter());
        //StartCoroutine(RunCenter());
    }

    void Update()
    {

    }
    void AimStart()
    {
        bone = skeletonAnimation.Skeleton.FindBone("crosshair");
        skeletonAnimation.state.SetAnimation(0, "aim", true);
        aim = true;
    }
    void AimUpdate()
    {
        if (aim == true)
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bone.SetLocalPosition(mouse - transform.position);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                skeletonAnimation.state.SetAnimation(1, "shoot", false);
            }
        }
    }
    IEnumerator WalkCenter()
    {
        yield return new WaitForSeconds(1);
        skeletonAnimation.state.SetAnimation(0, "walk", true);
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, center.position, walkingSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
            if (transform.position == center.position)
            {
                break;
            }
        }
        skeletonAnimation.state.SetAnimation(0, "idle", true);
    }
    IEnumerator RunCenter()
    {
        yield return new WaitForSeconds(1);
        skeletonAnimation.state.SetAnimation(0, "run", true);
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, center.position, runningSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
            if (transform.position == center.position)
            {
                break;
            }
        }
        skeletonAnimation.state.SetAnimation(0, "idle", true);
    }
}