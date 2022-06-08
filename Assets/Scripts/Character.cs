using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.Threading;
using System.Threading.Tasks;
public struct Character
{
    int health;
    bool leftSide;
    SkeletonAnimation skeletonAnimation;
    bool death;
    Transform body;
    Vector3 center;
    int runningSpeed;
    Vector3 place;
    Spine.Bone boneHead;
    Spine.Bone boneAim;


    public Character(GameObject GO, bool leftSide, Transform center)
    {
        health = 100;
        this.leftSide = leftSide;
        skeletonAnimation = GO.GetComponent<SkeletonAnimation>();
        skeletonAnimation.state.SetAnimation(0, "idle", true);
        body = GO.transform;
        death = false;
        this.center = center.position;
        runningSpeed = 80;
        place = GO.transform.position;
        boneHead = skeletonAnimation.Skeleton.FindBone("head");
        boneAim = skeletonAnimation.Skeleton.FindBone("crosshair");
    }
    public int gethealth()
    {
        return health;
    }
    public bool getSide()
    {
        return leftSide;
    }
    public int TakeAwayHealth()
    {
        health -= Random.Range(0, 100);
        if (health <= 0)
        {
            death = true;
            skeletonAnimation.state.SetAnimation(0, "death", false);
            Debug.Log("Death");
        }
        else
        {
            Debug.Log(health);
        }
        return health;
    }
    public IEnumerator Shoot(Vector2 target)
    {
        skeletonAnimation.state.SetAnimation(1, "aim", false);
        Debug.Log(target);
        boneAim.SetLocalPosition(body.InverseTransformPoint(target));
        yield return new WaitForSeconds(0.5f);
        skeletonAnimation.state.SetAnimation(2, "shoot", false);
        yield return new WaitForSeconds(0.5f);
        skeletonAnimation.ClearState();
        skeletonAnimation.state.SetAnimation(0, "idle", false);
    }
    public bool GetDeath()
    {
        return death;
    }
    public Vector2 GetPositionHead()
    {
        Vector2 positionHead = new Vector2(boneHead.WorldX, boneHead.WorldY) + (Vector2)body.position;
        return positionHead;
    }
    public IEnumerator RunCenter()
    {
        yield return new WaitForSeconds(1);
        SetLayerTop();
        skeletonAnimation.state.SetAnimation(0, "run", true);
        while (true)
        {
            body.position = Vector3.MoveTowards(body.position, center, runningSpeed * Time.deltaTime);
            yield return new WaitForSeconds(5f * Time.deltaTime);
            if (body.position == center)
            {
                break;
            }
        }
        skeletonAnimation.state.SetAnimation(0, "idle", true);
    }
    public IEnumerator RunCenterAtack()
    {
        yield return new WaitForSeconds(1);
        SetLayerAtack();
        skeletonAnimation.state.SetAnimation(0, "run", true);
        while (true)
        {
            body.position = Vector3.MoveTowards(body.position, center, runningSpeed * Time.deltaTime);
            yield return new WaitForSeconds(5f * Time.deltaTime);
            if (body.position == center)
            {
                break;
            }
        }
        skeletonAnimation.state.SetAnimation(0, "idle", true);
    }
    public IEnumerator RunBack()
    {
        yield return new WaitForSeconds(1);
        if (leftSide == true)
        {
            body.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            body.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        skeletonAnimation.state.SetAnimation(0, "run", true);
        while (true)
        {
            body.position = Vector3.MoveTowards(body.position, place, runningSpeed * Time.deltaTime);
            yield return new WaitForSeconds(5f * Time.deltaTime);
            if (body.position == place)
            {
                break;
            }
        }
        if (leftSide == true)
        {
            body.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            body.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        SetLayerBack();
        skeletonAnimation.state.SetAnimation(0, "idle", true);
    }
    public void SetLayerAtack()
    {
        skeletonAnimation.gameObject.GetComponent<MeshRenderer>().sortingOrder = 4;
    }
    public void SetLayerTop()
    {
        skeletonAnimation.gameObject.GetComponent<MeshRenderer>().sortingOrder = 3;
    }
    public void SetLayerBack()
    {
        skeletonAnimation.gameObject.GetComponent<MeshRenderer>().sortingOrder = 2;
    }
}