using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class Fight : MonoBehaviour
{
    private bool boolFight = false;
    private bool boolWait = false;
    private bool boolAutoFight = false;
    private float delayRun = 2.5f;
    private float delayShoot = 0.5f;
    private int indexTarget;
    private Game game;
    Generation generation;
    List<Character> turn;
    Character targetCharacter;
    public IEnumerator StartFight(Generation generation, List<Character> turn, Game game)
    {
        this.game = game;
        this.generation = generation;
        this.turn = turn;
        while (true)
        {
            List<Character> newTurn = new List<Character>();
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
                    bool leftSide = turn[indexTurn].getSide();
                    if (leftSide == true)
                    {
                        if (boolAutoFight == false)
                        {
                            game.SetStatusPlayerChoice(true);
                            game.EnablePlayerChoice();
                            Fight test = new Fight();
                            yield return StartCoroutine(PlayerWaiting());
                            if (boolWait == false)
                            {
                                yield return StartCoroutine(PlayerAttack(indexTurn));
                            }
                            else
                            {
                                boolWait = false;
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
            if (game.GetStatusPlayerChoice() == false)
            {
                boolFight = false;
                yield break;
            }
        }
    }
    IEnumerator PlayerAttack(int turnIndex)
    {
        bool attack = true;
        StartCoroutine(turn[turnIndex].RunCenter(attack));
        CancellationTokenSource cts = new CancellationTokenSource();
        StartCoroutine(turn[turnIndex].Aim(targetCharacter.GetBoneHead(), cts.Token, targetCharacter.GetBody()));
        attack = false;
        StartCoroutine(targetCharacter.RunCenter(attack));
        yield return new WaitForSeconds(delayRun);
        StartCoroutine(turn[turnIndex].Shoot(cts));
        yield return new WaitForSeconds(delayShoot);
        targetCharacter.TakeAwayHealth();
        for (int i = 0; i < turn.Count; i++)
        {
            Transform tempTransform = targetCharacter.GetBody();
            if (turn[i].GetBody() == tempTransform)
            {
                turn[i] = targetCharacter;
                break;
            }
        }
        if (targetCharacter.GetDeath() == true)
        {
            targetCharacter.SetLayerBack();
            generation.AddCharacterPool(targetCharacter);
            generation.RemoveCharacterRight(indexTarget);
        }
        else
        {
            StartCoroutine(targetCharacter.Hit());
            StartCoroutine(targetCharacter.RunBack());
            generation.ChangeCharacterRight(indexTarget, targetCharacter);
        }
        StartCoroutine(turn[turnIndex].RunBack());
        yield return new WaitForSeconds(delayRun);
    }
    IEnumerator AIAttack(List<Character> Characters, int turnIndex)
    {
        bool attack = true;
        StartCoroutine(turn[turnIndex].RunCenter(attack));
        int index = Random.Range(0, Characters.Count);
        Character targetCharacter = Characters[index];
        CancellationTokenSource cts = new CancellationTokenSource();
        StartCoroutine(turn[turnIndex].Aim(targetCharacter.GetBoneHead(), cts.Token, targetCharacter.GetBody()));
        attack = false;
        StartCoroutine(targetCharacter.RunCenter(attack));
        yield return new WaitForSeconds(delayRun);
        StartCoroutine(turn[turnIndex].Shoot(cts));
        yield return new WaitForSeconds(delayShoot);
        targetCharacter.TakeAwayHealth();
        int index2 = turn.IndexOf(Characters[index]);
        turn[index2] = targetCharacter;
        if (targetCharacter.GetDeath() == true)
        {
            targetCharacter.SetLayerBack();
            generation.AddCharacterPool(targetCharacter);
            Characters.RemoveAt(index);
        }
        else
        {
            StartCoroutine(targetCharacter.Hit());
            StartCoroutine(targetCharacter.RunBack());
            Characters[index] = targetCharacter;
        }
        StartCoroutine(turn[turnIndex].RunBack());
        yield return new WaitForSeconds(delayRun);
    }
    public void SetIndexTarget(int index)
    {
        indexTarget = index;
    }
    public void SetTargetCharacter(Character targetCharacter)
    {
        this.targetCharacter = targetCharacter;
    }
    public bool GetboolFight()
    {
        return boolFight;
    }
    public void SetBoolFight(bool bool1)
    {
        boolFight = bool1;
    }
    public void SetBoolWait(bool bool1)
    {
        boolWait = bool1;
    }
    public bool GetBoolAutoFight()
    {
        return boolAutoFight;
    }
    public void SetBoolAutoFight(bool bool1)
    {
        boolAutoFight = bool1;
    }
}