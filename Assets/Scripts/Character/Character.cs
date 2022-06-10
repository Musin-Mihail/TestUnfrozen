using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using System.Threading;
public class Character
{
    private int health = 100;
    private Text textHealth;
    private bool leftSide;
    private SkeletonAnimation skeletonAnimation;
    private bool death = false;
    private Transform body;
    private Vector3 center;
    private int runningSpeed = 20;
    private Vector3 place;
    private Spine.Bone boneHead;
    private Spine.Bone boneAim;
    private MeshRenderer meshRenderer;
    private RectTransform canvasText;
    public Character(GameObject GO, bool leftSide, Vector3 center)
    {
        this.leftSide = leftSide;
        skeletonAnimation = GO.GetComponent<SkeletonAnimation>();
        skeletonAnimation.state.SetAnimation(0, "idle", true);
        body = GO.transform;
        this.center = center;
        place = GO.transform.position;
        boneHead = skeletonAnimation.Skeleton.FindBone("head");
        boneAim = skeletonAnimation.Skeleton.FindBone("crosshair");
        meshRenderer = GO.GetComponent<MeshRenderer>();
        textHealth = GO.GetComponentInChildren<Text>();
        textHealth.text = health.ToString();
        canvasText = GO.transform.GetChild(0).GetComponent<RectTransform>();
        if (leftSide == false)
        {
            canvasText.rotation = Quaternion.identity;
        }
    }
    public bool getSide()
    {
        return leftSide;
    }
    public int TakeAwayHealth()
    {
        health -= Random.Range(30, 101);
        //health -= 100;
        if (health >= 0)
        {
            textHealth.text = health.ToString();
        }
        else
        {
            textHealth.text = "0";
        }
        if (health <= 0)
        {
            canvasText.gameObject.SetActive(false);
            death = true;
            skeletonAnimation.state.SetAnimation(0, "death", false);
        }
        return health;
    }
    public IEnumerator Shoot(CancellationTokenSource cts)
    {
        yield return new WaitForSeconds(0.5f);
        skeletonAnimation.state.SetAnimation(2, "shoot", false);
        cts.Cancel();
        yield return new WaitForSeconds(0.5f);
        skeletonAnimation.ClearState();
        skeletonAnimation.state.SetAnimation(0, "idle", true);
    }
    public IEnumerator Aim(Spine.Bone boneHead, CancellationToken token, Transform target)
    {
        SetLayerAttack();
        skeletonAnimation.state.SetAnimation(1, "aim", false);
        while (true)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                Vector2 positionHead = new Vector2(boneHead.WorldX, boneHead.WorldY) + (Vector2)target.position;
                boneAim.SetLocalPosition(body.InverseTransformPoint(positionHead));
            }
            catch
            {
                yield break;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    public Transform GetBody()
    {
        return body;
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
    public Spine.Bone GetBoneHead()
    {
        return boneHead;
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
            yield return new WaitForSeconds(0.1f * Time.deltaTime);
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
        canvasText.rotation = Quaternion.identity;
        skeletonAnimation.ClearState();
        skeletonAnimation.state.SetAnimation(0, "run", true);

        while (true)
        {
            body.position = Vector3.MoveTowards(body.position, place, runningSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.1f * Time.deltaTime);
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
        canvasText.rotation = Quaternion.identity;
        SetLayerBack();
        skeletonAnimation.state.SetAnimation(0, "idle", true);
    }
    private void SetLayerAttack()
    {
        meshRenderer.sortingOrder = 4;
    }
    private void SetLayerTop()
    {
        meshRenderer.sortingOrder = 3;
    }
    public void SetLayerBack()
    {
        meshRenderer.sortingOrder = 2;
    }
    public void Reset(Vector3 newPosition, bool leftSide, Vector3 center)
    {
        body.position = newPosition;
        place = newPosition;
        skeletonAnimation.ClearState();
        skeletonAnimation.state.SetAnimation(0, "idle", true);
        health = 100;
        textHealth.text = health.ToString();
        this.leftSide = leftSide;
        death = false;
        this.center = center;
        if (leftSide == true)
        {
            body.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            body.rotation = Quaternion.Euler(0, -180, 0);
        }
        canvasText.rotation = Quaternion.identity;
        canvasText.gameObject.SetActive(true);
    }
}