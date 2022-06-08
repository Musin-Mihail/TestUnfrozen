using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    void Start()
    {
        // Атака или пропустить ход. Выбор цели атаки. Движение к центру. При движении в центр другие персонажы сдвигаются вглубь. Перезапуск, пул персонажей.
        Generation();
        StartCoroutine(AutoFight());
    }
    void Generation()
    {
        character = Resources.Load<GameObject>("Character");
        bool leftSide;
        foreach (var item in characterLocationsLeft)
        {
            GameObject GO = Instantiate(this.character, item.position, Quaternion.identity);
            leftSide = true;
            Character character = new Character(GO, leftSide, centerLeft);
            turn.Add(character);
            characterLeft.Add(character);
        }
        foreach (var item in characterLocationsRight)
        {
            GameObject GO = Instantiate(this.character, item.position, Quaternion.Euler(0, 180, 0));
            leftSide = false;
            Character character = new Character(GO, leftSide, centerRight);
            turn.Add(character);
            characterRight.Add(character);
        }
        for (int i = 0; i < turn.Count; i++)
        {
            Character temp = turn[i];
            int randomIndex = Random.Range(i, turn.Count);
            turn[i] = turn[randomIndex];
            turn[randomIndex] = temp;
        }
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
                    Debug.Log("Конец игры");
                    yield break;
                }
                if (turn[y].GetDeath() == false)
                {
                    bool attack = true;
                    StartCoroutine(turn[y].RunCenter(attack));
                    bool leftSide = turn[y].getSide();
                    Debug.Log("Жду атаки");
                    if (leftSide == true)
                    {
                        yield return StartCoroutine(Attack(characterRight, y));
                    }
                    else
                    {
                        yield return StartCoroutine(Attack(characterLeft, y));
                    }
                    Debug.Log("Жду возврата");
                    StartCoroutine(turn[y].RunBack());
                    yield return new WaitForSeconds(3.0f);
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
        Debug.Log("Цель бежит в центр");
        StartCoroutine(temp.RunCenter(attack));
        yield return new WaitForSeconds(3.0f);
        Debug.Log("Стрельба");
        StartCoroutine(turn[turnIndex].Shoot(temp.GetPositionHead()));
        yield return new WaitForSeconds(0.5f);
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