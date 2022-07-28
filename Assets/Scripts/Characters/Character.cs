using System.Collections;
using System.Threading;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Characters
{
    public class Character : MonoBehaviour
    {
        public bool IsLeftSide { get; private set; }
        public bool IsDead { get; private set; } = false;
        
        private int _health = 100;
        private int _runningSpeed = 20;
        private Vector3 _attackPosition;
        private Text _textHealth;
        private SkeletonAnimation _skeletonAnimation;
        public Spine.Bone AimTarget { get; private set; }
        private Spine.Bone _boneAim;
        private MeshRenderer _meshRenderer;
        private RectTransform _canvasText;

        private Vector3 _startPosition;

        public Character Setup(bool leftSide, Vector3 center)
        {
            IsLeftSide = leftSide;
            _attackPosition = center;

            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _textHealth = GetComponentInChildren<Text>();

            _canvasText = transform.GetChild(0).GetComponent<RectTransform>();
            AimTarget = _skeletonAnimation.Skeleton.FindBone("head");
            _boneAim = _skeletonAnimation.Skeleton.FindBone("crosshair");
            _textHealth.text = _health.ToString();
            _startPosition = transform.position;
            
            _skeletonAnimation.state.SetAnimation(0, "idle", true);
            if (leftSide == false) _canvasText.rotation = Quaternion.identity;

            return this;
        }

        public int GetHit()
        {
            _health -= Random.Range(30, 101);
            //health -= 100;
            _textHealth.text = _health >= 0 ? _health.ToString() : "0";
            if (_health > 0) return _health;

            _canvasText.gameObject.SetActive(false);
            IsDead = true;
            _skeletonAnimation.state.SetAnimation(0, "death", false);

            return _health;
        }

        public IEnumerator Shoot(CancellationTokenSource cts)
        {
            yield return new WaitForSeconds(0.5f);
            _skeletonAnimation.state.SetAnimation(2, "shoot", false);
            cts.Cancel();
            yield return new WaitForSeconds(0.5f);
            _skeletonAnimation.ClearState();
            _skeletonAnimation.state.SetAnimation(0, "idle", true);
        }

        public IEnumerator Aim(Spine.Bone boneHead, CancellationToken token, Transform target)
        {
            SetLayerAttack();
            _skeletonAnimation.state.SetAnimation(1, "aim", false);
            while (true)
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    Vector2 positionHead = new Vector2(boneHead.WorldX, boneHead.WorldY) + (Vector2)target.position;
                    _boneAim.SetLocalPosition(transform.InverseTransformPoint(positionHead));
                }
                catch
                {
                    yield break;
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        public IEnumerator Hit()
        {
            _skeletonAnimation.state.SetAnimation(0, "idle-turn", false);
            yield return new WaitForSeconds(0.2f);
            _skeletonAnimation.state.SetAnimation(0, "idle", true);
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

            _skeletonAnimation.state.SetAnimation(0, "run", true);
            while (true)
            {
                transform.position = Vector3.MoveTowards(transform.position, _attackPosition, _runningSpeed * Time.deltaTime);
                yield return new WaitForSeconds(0.1f * Time.deltaTime);
                if (transform.position == _attackPosition)
                {
                    break;
                }
            }

            _skeletonAnimation.state.SetAnimation(0, "idle", true);
        }

        public IEnumerator RunBack()
        {
            yield return new WaitForSeconds(1);
            transform.rotation = IsLeftSide ? 
                Quaternion.Euler(0, -180, 0) : 
                Quaternion.Euler(0, 0, 0);
            
            _canvasText.rotation = Quaternion.identity;
            _skeletonAnimation.ClearState();
            _skeletonAnimation.state.SetAnimation(0, "run", true);

            while (true)
            {
                transform.position = Vector3.MoveTowards(transform.position, _startPosition, _runningSpeed * Time.deltaTime);
                yield return new WaitForSeconds(0.1f * Time.deltaTime);
                if (transform.position == _startPosition)
                {
                    break;
                }
            }

            transform.rotation = IsLeftSide ? 
                Quaternion.Euler(0, 0, 0) : 
                Quaternion.Euler(0, -180, 0);

            _canvasText.rotation = Quaternion.identity;
            SetLayerBack();
            _skeletonAnimation.state.SetAnimation(0, "idle", true);
        }

        public void Reset()
        {
            transform.position = _startPosition;
            _skeletonAnimation.ClearState();
            _skeletonAnimation.state.SetAnimation(0, "idle", true);
            _health = 100;
            _textHealth.text = _health.ToString();
            IsDead = false;
            transform.rotation = IsLeftSide ? 
                Quaternion.Euler(0, 0, 0) : 
                Quaternion.Euler(0, -180, 0);

            _canvasText.rotation = Quaternion.identity;
            _canvasText.gameObject.SetActive(true);
        }

        private void SetLayerAttack()
        {
            _meshRenderer.sortingOrder = 4;
        }

        private void SetLayerTop()
        {
            _meshRenderer.sortingOrder = 3;
        }

        public void SetLayerBack()
        {
            _meshRenderer.sortingOrder = 2;
        }
    }
}