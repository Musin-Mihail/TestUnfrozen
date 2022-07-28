using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Characters;
using UnityEngine;

namespace Gameplay
{
    public class Fight : MonoBehaviour
    {
        [SerializeField] private float _delayRun = 2.5f;
        [SerializeField] private float _delayShoot = 0.5f;
    
        public bool BoolFight { get; set; }
        public bool BoolWait { get; set; }
        public bool BoolAutoFight { get; set; }
        public int IndexTarget { get; set; }
        public Character TargetCharacter { get; set; }
        
        private Game _game;
        private CharactersController _charactersController;
        private List<Character> _turn;

        public IEnumerator StartFight(CharactersController charactersController, List<Character> turn, Game game)
        {
            _game = game;
            _charactersController = charactersController;
            _turn = turn;
            while (true)
            {
                List<Character> newTurn = new List<Character>();
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
                            if (BoolAutoFight == false)
                            {
                                game.BoolPlayerChoice = true;
                                game.EnablePlayerChoice();
                                yield return StartCoroutine(PlayerWait());
                                if (BoolWait == false)
                                {
                                    yield return StartCoroutine(PlayerAttack(indexTurn));
                                }
                                else
                                {
                                    BoolWait = false;
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

        private IEnumerator PlayerWait()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.01f);
                if (_game.BoolPlayerChoice == false)
                {
                    BoolFight = false;
                    yield break;
                }
            }
        }

        private IEnumerator PlayerAttack(int turnIndex)
        {
            StartCoroutine(_turn[turnIndex].RunCenter(true));
            CancellationTokenSource cts = new CancellationTokenSource();
            StartCoroutine(_turn[turnIndex].Aim(TargetCharacter.AimTarget, cts.Token, TargetCharacter.transform));
            StartCoroutine(TargetCharacter.RunCenter(false));
            yield return new WaitForSeconds(_delayRun);
            StartCoroutine(_turn[turnIndex].Shoot(cts));
            yield return new WaitForSeconds(_delayShoot);
            TargetCharacter.GetHit();
            for (int i = 0; i < _turn.Count; i++)
            {
                Transform tempTransform = TargetCharacter.transform;
                if (_turn[i].transform == tempTransform)
                {
                    _turn[i] = TargetCharacter;
                    break;
                }
            }
            if (TargetCharacter.IsDead == true)
            {
                TargetCharacter.SetLayerBack();
                _charactersController.PoolCharacters.Add(TargetCharacter);
                _charactersController.CharacterRight.RemoveAt(IndexTarget);
            }
            else
            {
                StartCoroutine(TargetCharacter.Hit());
                StartCoroutine(TargetCharacter.RunBack());
                _charactersController.CharacterRight[IndexTarget] = TargetCharacter;
            }
            StartCoroutine(_turn[turnIndex].RunBack());
            yield return new WaitForSeconds(_delayRun);
        }

        private IEnumerator AIAttack(List<Character> characters, int turnIndex)
        {
            StartCoroutine(_turn[turnIndex].RunCenter(true));
            int index = Random.Range(0, characters.Count);
            Character targetCharacter = characters[index];
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
    }
}