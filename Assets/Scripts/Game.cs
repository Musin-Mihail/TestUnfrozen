using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class Game : MonoBehaviour
{
    [SerializeField] List<Transform> characterLocationsLeft;
    [SerializeField] List<Transform> characterLocationsRight;
    [SerializeField] Transform centerLeft;
    [SerializeField] Transform centerRight;
    List<Character> turn = new List<Character>();
    GameObject character;
    List<Character> characterLeft = new List<Character>();
    List<Character> characterRight = new List<Character>();
    List<Character> DeceasedCharacters = new List<Character>();
    float delayRun = 2.5f;
    float delayShoot = 0.5f;
    void Start()
    {
        // Атака или пропустить ход. Выбор цели атаки.
        character = Resources.Load<GameObject>("Character");
        RestartGame();
    }
    void Generation()
    {
        bool leftSide;
        turn.Clear();
        foreach (var item in characterLocationsLeft)
        {
            leftSide = true;
            if (DeceasedCharacters.Count > 0)
            {
                Character temp = DeceasedCharacters[0];
                DeceasedCharacters.RemoveAt(0);
                temp.Reset(item.position, leftSide, centerLeft);
                turn.Add(temp);
                characterLeft.Add(temp);
            }
            else
            {
                GameObject GO = Instantiate(this.character, item.position, Quaternion.identity);
                Character character = new Character(GO, leftSide, centerLeft);
                turn.Add(character);
                characterLeft.Add(character);
            }
        }
        foreach (var item in characterLocationsRight)
        {
            leftSide = false;
            if (DeceasedCharacters.Count > 0)
            {
                Character temp = DeceasedCharacters[0];
                DeceasedCharacters.RemoveAt(0);
                temp.Reset(item.position, leftSide, centerRight);
                turn.Add(temp);
                characterRight.Add(temp);
            }
            else
            {
                GameObject GO = Instantiate(this.character, item.position, Quaternion.Euler(0, 180, 0));
                Character character = new Character(GO, leftSide, centerRight);
                turn.Add(character);
                characterRight.Add(character);
            }
        }
        for (int i = 0; i < turn.Count; i++)
        {
            Character temp = turn[i];
            int randomIndex = Random.Range(i, turn.Count);
            turn[i] = turn[randomIndex];
            turn[randomIndex] = temp;
        }
    }
    void RestartGame()
    {
        Generation();
        StartCoroutine(AutoFight());
    }
    IEnumerator AutoFight()
    {
        for (int i = 0; i < 100; i++)
        {
            Debug.Log("Начало очереди");
            List<Character> newTurn = new List<Character>();
            for (int y = 0; y < turn.Count; y++)
            {
                if (characterRight.Count == 0 || characterLeft.Count == 0)
                {
                    int count = characterRight.Count;
                    for (int z = 0; z < count; z++)
                    {
                        DeceasedCharacters.Add(characterRight[0]);
                        characterRight.RemoveAt(0);
                    }
                    count = characterLeft.Count;
                    for (int x = 0; x < count; x++)
                    {
                        DeceasedCharacters.Add(characterLeft[0]);
                        characterLeft.RemoveAt(0);
                    }
                    Debug.Log("Конец игры");
                    yield return new WaitForSeconds(2.0f);
                    RestartGame();
                    yield break;
                }
                if (turn[y].GetDeath() == false)
                {
                    bool attack = true;
                    StartCoroutine(turn[y].RunCenter(attack));
                    bool leftSide = turn[y].getSide();
                    if (leftSide == true)
                    {
                        yield return StartCoroutine(Attack(characterRight, y));
                    }
                    else
                    {
                        yield return StartCoroutine(Attack(characterLeft, y));
                    }
                    StartCoroutine(turn[y].RunBack());
                    yield return new WaitForSeconds(delayRun);
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
            Debug.Log("Конец очереди");
        }
    }
    IEnumerator Attack(List<Character> Characters, int turnIndex)
    {
        Debug.Log("Атака");
        int index = Random.Range(0, Characters.Count);
        Character temp = Characters[index];
        bool attack = false;
        Debug.Log("В центр");
        CancellationTokenSource cts = new CancellationTokenSource();
        StartCoroutine(turn[turnIndex].Aim(temp.GetBoneHead(), cts.Token, temp.GetBody()));
        StartCoroutine(temp.RunCenter(attack));
        yield return new WaitForSeconds(delayRun);
        Debug.Log("Стрельба");
        StartCoroutine(turn[turnIndex].Shoot(cts));
        yield return new WaitForSeconds(delayShoot);
        temp.TakeAwayHealth();
        int index2 = turn.IndexOf(Characters[index]);
        turn[index2] = temp;
        if (temp.GetDeath() == true)
        {
            temp.SetLayerBack();
            DeceasedCharacters.Add(temp);
            Characters.RemoveAt(index);
        }
        else
        {
            StartCoroutine(temp.Hit());
            StartCoroutine(temp.RunBack());
            Characters[index] = temp;
        }
    }
}