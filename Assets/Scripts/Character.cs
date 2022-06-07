using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Character : MonoBehaviour
{
    SkeletonAnimation skeletonAnimation;
    Spine.Bone bone;
    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        bone = skeletonAnimation.Skeleton.FindBone("crosshair");
        skeletonAnimation.state.SetAnimation(0, "aim", true);
    }

    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bone.SetLocalPosition(mouse - transform.position);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            skeletonAnimation.state.SetAnimation(1, "shoot", false);
        }
    }
}
