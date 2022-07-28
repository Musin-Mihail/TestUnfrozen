using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Gameplay
{
    public class Fight : MonoBehaviour
    {
        private float _delayRun = 2.5f;
        private float _delayShoot = 0.5f;
    
        private bool _boolFight = false;
        private bool _boolWait = false;
        private bool _boolAutoFight = false;
        private int _indexTarget;
        private Game _game;
        private CharactersController _charactersController;
        private List<Characters.Character> _turn;
        private Characters.Character _targetCharacter;
        public IEnumerator StartFight(CharactersController charactersController, List<Characters.Character> turn, Game game)
        {
            _game = game;
            _charactersController = charactersController;
            _turn = turn;
            while (true)
            {
                List<Characters.Character> newTurn = new List<Characters.Character>();
                for (int indexTurn = 0; indexTurn < turn.Count; indexTurn++)
                {
                    if (charactersController.CharacterRight.Count == 0 || charactersController.CharacterLeft.Count == 0)
                    {
                        int count = charactersController.CharacterLeft.Count;
                        for (int i = 0; i < count; i++)
                        {
                            charactersController.PoolCharacters.Add(charactersController.CharacterLeft[0]);
                            charactersController.CharacterLeft.RemoveAt(0);
                        }
                        count = charactersController.CharacterRight.Count;
                        for (int i = 0; i < count; i++)
                        {
                            charactersController.PoolCharacters.Add(charactersController.CharacterRight[0]);
                            charactersController.CharacterRight.RemoveAt(0);
                        }
                        yield return new WaitForSeconds(2.0f);
                        game.RestartGame();
                        yield break;
                    }
                    if (turn[indexTurn].IsDead == false)
                    {
                        bool leftSide = turn[indexTurn].IsLeftSide;
                        if (leftSide == true)
                        {
                            if (_boolAutoFight == false)
                            {
                                game.BoolPlayerChoice = true;
                                game.EnablePlayerChoice();
                                yield return StartCoroutine(PlayerWaiting());
                                if (_boolWait == false)
                                {
                                    yield return StartCoroutine(PlayerAttack(indexTurn));
                                }
                                else
                                {
                                    _boolWait = false;
                                }
                            }
                            else
                            {
                                yield return StartCoroutine(AIAttack(charactersController.CharacterRight, indexTurn));
                            }
                        }
                        else
                        {
                            yield return StartCoroutine(AIAttack(charactersController.CharacterLeft, indexTurn));
                        }
                    }
                }
                foreach (var item in turn)
                {
                    if (item.IsDead == false)
                    {
                        newTurn.Add(item);
                    }
                }
                turn = newTurn;
            }
        }
        public IEnumerator PlayerWaiting()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.01f);
                if (_game.BoolPlayerChoice == false)
                {
                    _boolFight = false;
                    yield break;
                }
            }
        }

        private IEnumerator PlayerAttack(int turnIndex)
        {
            StartCoroutine(_turn[turnIndex].RunCenter(true));
            CancellationTokenSource cts = new CancellationTokenSource();
            StartCoroutine(_turn[turnIndex].Aim(_targetCharacter.AimTarget, cts.Token, _targetCharacter.transform));
            StartCoroutine(_targetCharacter.RunCenter(false));
            yield return new WaitForSeconds(_delayRun);
            StartCoroutine(_turn[turnIndex].Shoot(cts));
            yield return new WaitForSeconds(_delayShoot);
            _targetCharacter.GetHit();
            for (int i = 0; i < _turn.Count; i++)
            {
                Transform tempTransform = _targetCharacter.transform;
                if (_turn[i].transform == tempTransform)
                {
                    _turn[i] = _targetCharacter;
                    break;
                }
            }
            if (_targetCharacter.IsDead == true)
            {
                _targetCharacter.SetLayerBack();
                _charactersController.PoolCharacters.Add(_targetCharacter);
                _charactersController.CharacterRight.RemoveAt(_indexTarget);
            }
            else
            {
                StartCoroutine(_targetCharacter.Hit());
                StartCoroutine(_targetCharacter.RunBack());
                _charactersController.CharacterRight[_indexTarget] = _targetCharacter;
            }
            StartCoroutine(_turn[turnIndex].RunBack());
            yield return new WaitForSeconds(_delayRun);
        }

        private IEnumerator AIAttack(List<Characters.Character> characters, int turnIndex)
        {
            StartCoroutine(_turn[turnIndex].RunCenter(true));
            int index = Random.Range(0, characters.Count);
            Characters.Character targetCharacter = characters[index];
            CancellationTokenSource cts = new CancellationTokenSource();
            StartCoroutine(_turn[turnIndex].Aim(targetCharacter.AimTarget, cts.Token, targetCharacter.transform));
            StartCoroutine(targetCharacter.RunCenter(false));
            yield return new WaitForSeconds(_delayRun);
            StartCoroutine(_turn[turnIndex].Shoot(cts));
            yield return new WaitForSeconds(_delayShoot);
            targetCharacter.GetHit();
            int index2 = _turn.IndexOf(characters[index]);
            _turn[index2] = targetCharacter;
            if (targetCharacter.IsDead == true)
            {
                targetCharacter.SetLayerBack();
                _charactersController.PoolCharacters.Add(targetCharacter);
                characters.RemoveAt(index);
            }
            else
            {
                StartCoroutine(targetCharacter.Hit());
                StartCoroutine(targetCharacter.RunBack());
                characters[index] = targetCharacter;
            }
            StartCoroutine(_turn[turnIndex].RunBack());
            yield return new WaitForSeconds(_delayRun);
        }
        public void SetIndexTarget(int index)
        {
            _indexTarget = index;
        }
        public void SetTargetCharacter(Characters.Character targetCharacter)
        {
            this._targetCharacter = targetCharacter;
        }
        public bool GetboolFight()
        {
            return _boolFight;
        }
        public void SetBoolFight(bool bool1)
        {
            _boolFight = bool1;
        }
        public void SetBoolWait(bool bool1)
        {
            _boolWait = bool1;
        }
        public bool GetBoolAutoFight()
        {
            return _boolAutoFight;
        }
        public void SetBoolAutoFight(bool bool1)
        {
            _boolAutoFight = bool1;
        }
    }
}