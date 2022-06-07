using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Character : MonoBehaviour
{
    SkeletonAnimation skeletonAnimation;
    Spine.Bone bone;
    [SerializeField] Transform center;
    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        StartCoroutine(MoveCenter());
    }

    void Update()
    {

    }
    void AimStart()
    {
        bone = skeletonAnimation.Skeleton.FindBone("crosshair");
        skeletonAnimation.state.SetAnimation(0, "aim", true);
    }
    void AimUpdate()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bone.SetLocalPosition(mouse - transform.position);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            skeletonAnimation.state.SetAnimation(1, "shoot", false);
        }
    }
    IEnumerator MoveCenter()
    {
        yield return new WaitForSeconds(1);
        skeletonAnimation.state.SetAnimation(0, "walk", true);
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, center.position, 10 * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
            if(transform.position == center.position)
            {
                break;
            }
        }
        skeletonAnimation.state.SetAnimation(0, "idle", true);
    }
}
