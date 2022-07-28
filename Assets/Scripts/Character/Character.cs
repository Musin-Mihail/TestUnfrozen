using System.Collections;
using System.Threading;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    public class Character
    {
        private int _health = 100;
        private Text _textHealth;
        private bool _leftSide;
        private SkeletonAnimation _skeletonAnimation;
        private bool _death = false;
        private Transform _body;
        private Vector3 _center;
        private int _runningSpeed = 20;
        private Vector3 _place;
        private Spine.Bone _boneHead;
        private Spine.Bone _boneAim;
        private MeshRenderer _meshRenderer;
        private RectTransform _canvasText;
        public Character(GameObject go, bool leftSide, Vector3 center)
        {
            _leftSide = leftSide;
            _skeletonAnimation = go.GetComponent<SkeletonAnimation>();
            _skeletonAnimation.state.SetAnimation(0, "idle", true);
            _body = go.transform;
            _center = center;
            _place = go.transform.position;
            _boneHead = _skeletonAnimation.Skeleton.FindBone("head");
            _boneAim = _skeletonAnimation.Skeleton.FindBone("crosshair");
            _meshRenderer = go.GetComponent<MeshRenderer>();
            _textHealth = go.GetComponentInChildren<Text>();
            _textHealth.text = _health.ToString();
            _canvasText = go.transform.GetChild(0).GetComponent<RectTransform>();
            if (leftSide == false)
            {
                _canvasText.rotation = Quaternion.identity;
            }
        }
        public bool GetSide()
        {
            return _leftSide;
        }
        public int TakeAwayHealth()
        {
            _health -= Random.Range(30, 101);
            //health -= 100;
            if (_health >= 0)
            {
                _textHealth.text = _health.ToString();
            }
            else
            {
                _textHealth.text = "0";
            }
            if (_health <= 0)
            {
                _canvasText.gameObject.SetActive(false);
                _death = true;
                _skeletonAnimation.state.SetAnimation(0, "death", false);
            }
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
                    _boneAim.SetLocalPosition(_body.InverseTransformPoint(positionHead));
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
            return _body;
        }
        public IEnumerator Hit()
        {
            _skeletonAnimation.state.SetAnimation(0, "idle-turn", false);
            yield return new WaitForSeconds(0.2f);
            _skeletonAnimation.state.SetAnimation(0, "idle", true);
        }
        public bool GetDeath()
        {
            return _death;
        }
        public Spine.Bone GetBoneHead()
        {
            return _boneHead;
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
                _body.position = Vector3.MoveTowards(_body.position, _center, _runningSpeed * Time.deltaTime);
                yield return new WaitForSeconds(0.1f * Time.deltaTime);
                if (_body.position == _center)
                {
                    break;
                }
            }
            _skeletonAnimation.state.SetAnimation(0, "idle", true);
        }
        public IEnumerator RunBack()
        {
            yield return new WaitForSeconds(1);
            if (_leftSide == true)
            {
                _body.transform.rotation = Quaternion.Euler(0, -180, 0);
            }
            else
            {
                _body.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            _canvasText.rotation = Quaternion.identity;
            _skeletonAnimation.ClearState();
            _skeletonAnimation.state.SetAnimation(0, "run", true);

            while (true)
            {
                _body.position = Vector3.MoveTowards(_body.position, _place, _runningSpeed * Time.deltaTime);
                yield return new WaitForSeconds(0.1f * Time.deltaTime);
                if (_body.position == _place)
                {
                    break;
                }
            }
            if (_leftSide == true)
            {
                _body.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                _body.transform.rotation = Quaternion.Euler(0, -180, 0);
            }
            _canvasText.rotation = Quaternion.identity;
            SetLayerBack();
            _skeletonAnimation.state.SetAnimation(0, "idle", true);
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
        public void Reset(Vector3 newPosition, bool leftSide, Vector3 center)
        {
            _body.position = newPosition;
            _place = newPosition;
            _skeletonAnimation.ClearState();
            _skeletonAnimation.state.SetAnimation(0, "idle", true);
            _health = 100;
            _textHealth.text = _health.ToString();
            _leftSide = leftSide;
            _death = false;
            _center = center;
            if (leftSide == true)
            {
                _body.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                _body.rotation = Quaternion.Euler(0, -180, 0);
            }
            _canvasText.rotation = Quaternion.identity;
            _canvasText.gameObject.SetActive(true);
        }
    }
}