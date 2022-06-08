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
    MeshRenderer meshRenderer;
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
        meshRenderer = GO.GetComponent<MeshRenderer>();
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
        //health -= Random.Range(0, 100);
        health -= 100;
        if (health <= 0)
        {
            death = true;
            skeletonAnimation.state.SetAnimation(0, "death", false);
        }
        return health;
    }
    public IEnumerator Shoot(Vector2 target)
    {
        skeletonAnimation.state.SetAnimation(1, "aim", false);
        boneAim.SetLocalPosition(body.InverseTransformPoint(target));
        yield return new WaitForSeconds(0.5f);
        skeletonAnimation.state.SetAnimation(2, "shoot", false);
        yield return new WaitForSeconds(0.5f);
        skeletonAnimation.ClearState();
        skeletonAnimation.state.SetAnimation(0, "idle", true);
    }
    public IEnumerator Hit()
    {
        skeletonAnimation.state.SetAnimation(0, "idle-turn", false);
        yield return new WaitForSeconds(0.2f);
        skeletonAnimation.state.SetAnimation(0, "idle", true);
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
    public IEnumerator RunCenter(bool attack)
    {
        yield return new WaitForSeconds(1);
        if (attack)
        {
            SetLayerAttack();
        }
        else
        {
            SetLayerTop();
        }
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
        skeletonAnimation.ClearState();
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
    public void SetLayerAttack()
    {
        meshRenderer.sortingOrder = 4;
    }
    public void SetLayerTop()
    {
        meshRenderer.sortingOrder = 3;
    }
    public void SetLayerBack()
    {
        meshRenderer.sortingOrder = 2;
    }
    public void Reset(Vector3 newPosition, bool leftSide, Transform center)
    {
        body.position = newPosition;
        place = newPosition;
        skeletonAnimation.ClearState();
        skeletonAnimation.state.SetAnimation(0, "idle", true);
        health = 100;
        this.leftSide = leftSide;
        death = false;
        this.center = center.position;
        if(leftSide == true)
        {
            body.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            body.rotation = Quaternion.Euler(0, -180, 0);
        }
    }
}