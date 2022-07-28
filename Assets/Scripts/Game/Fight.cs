using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class Fight : MonoBehaviour
{
    private float _delayRun = 2.5f;
    private float _delayShoot = 0.5f;
    
    private bool _boolFight = false;
    private bool _boolWait = false;
    private bool _boolAutoFight = false;
    private int _indexTarget;
    private Game _game;
    private Generation _generation;
    private List<Character.Character> _turn;
    private Character.Character _targetCharacter;
    public IEnumerator StartFight(Generation generation, List<Character.Character> turn, Game game)
    {
        _game = game;
        _generation = generation;
        _turn = turn;
        while (true)
        {
            List<Character.Character> newTurn = new List<Character.Character>();
            for (int indexTurn = 0; indexTurn < turn.Count; indexTurn++)
            {
                if (generation.GetCountCharactersRight() == 0 || generation.GetCountCharactersLeft() == 0)
                {
                    int count = generation.GetCountCharactersLeft();
                    for (int i = 0; i < count; i++)
                    {
                        generation.AddCharacterPool(generation.GetCharacterLeft(0));
                        generation.RemoveCharacterLeft(0);
                    }
                    count = generation.GetCountCharactersRight();
                    for (int i = 0; i < count; i++)
                    {
                        generation.AddCharacterPool(generation.GetCharacterRight(0));
                        generation.RemoveCharacterRight(0);
                    }
                    yield return new WaitForSeconds(2.0f);
                    game.RestartGame();
                    yield break;
                }
                if (turn[indexTurn].GetDeath() == false)
                {
                    bool leftSide = turn[indexTurn].GetSide();
                    if (leftSide == true)
                    {
                        if (_boolAutoFight == false)
                        {
                            game.SetStatusPlayerChoice(true);
                            game.EnablePlayerChoice();
                            Fight test = new Fight();
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
                            yield return StartCoroutine(AIAttack(generation.GetListCharacterRight(), indexTurn));
                        }
                    }
                    else
                    {
                        yield return StartCoroutine(AIAttack(generation.GetListCharacterLeft(), indexTurn));
                    }
                }
            }
            foreach (var item in turn)
            {
                if (item.GetDeath() == false)
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
            if (_game.GetStatusPlayerChoice() == false)
            {
                _boolFight = false;
                yield break;
            }
        }
    }
    IEnumerator PlayerAttack(int turnIndex)
    {
        bool attack = true;
        StartCoroutine(_turn[turnIndex].RunCenter(attack));
        CancellationTokenSource cts = new CancellationTokenSource();
        StartCoroutine(_turn[turnIndex].Aim(_targetCharacter.GetBoneHead(), cts.Token, _targetCharacter.GetBody()));
        attack = false;
        StartCoroutine(_targetCharacter.RunCenter(attack));
        yield return new WaitForSeconds(_delayRun);
        StartCoroutine(_turn[turnIndex].Shoot(cts));
        yield return new WaitForSeconds(_delayShoot);
        _targetCharacter.TakeAwayHealth();
        for (int i = 0; i < _turn.Count; i++)
        {
            Transform tempTransform = _targetCharacter.GetBody();
            if (_turn[i].GetBody() == tempTransform)
            {
                _turn[i] = _targetCharacter;
                break;
            }
        }
        if (_targetCharacter.GetDeath() == true)
        {
            _targetCharacter.SetLayerBack();
            _generation.AddCharacterPool(_targetCharacter);
            _generation.RemoveCharacterRight(_indexTarget);
        }
        else
        {
            StartCoroutine(_targetCharacter.Hit());
            StartCoroutine(_targetCharacter.RunBack());
            _generation.ChangeCharacterRight(_indexTarget, _targetCharacter);
        }
        StartCoroutine(_turn[turnIndex].RunBack());
        yield return new WaitForSeconds(_delayRun);
    }
    IEnumerator AIAttack(List<Character.Character> Characters, int turnIndex)
    {
        bool attack = true;
        StartCoroutine(_turn[turnIndex].RunCenter(attack));
        int index = Random.Range(0, Characters.Count);
        Character.Character targetCharacter = Characters[index];
        CancellationTokenSource cts = new CancellationTokenSource();
        StartCoroutine(_turn[turnIndex].Aim(targetCharacter.GetBoneHead(), cts.Token, targetCharacter.GetBody()));
        attack = false;
        StartCoroutine(targetCharacter.RunCenter(attack));
        yield return new WaitForSeconds(_delayRun);
        StartCoroutine(_turn[turnIndex].Shoot(cts));
        yield return new WaitForSeconds(_delayShoot);
        targetCharacter.TakeAwayHealth();
        int index2 = _turn.IndexOf(Characters[index]);
        _turn[index2] = targetCharacter;
        if (targetCharacter.GetDeath() == true)
        {
            targetCharacter.SetLayerBack();
            _generation.AddCharacterPool(targetCharacter);
            Characters.RemoveAt(index);
        }
        else
        {
            StartCoroutine(targetCharacter.Hit());
            StartCoroutine(targetCharacter.RunBack());
            Characters[index] = targetCharacter;
        }
        StartCoroutine(_turn[turnIndex].RunBack());
        yield return new WaitForSeconds(_delayRun);
    }
    public void SetIndexTarget(int index)
    {
        _indexTarget = index;
    }
    public void SetTargetCharacter(Character.Character targetCharacter)
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