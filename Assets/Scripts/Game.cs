using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
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
            Character character = new Character(GO, leftSide);
            turn.Add(character);
            characterLeft.Add(character);
        }
        foreach (var item in characterLocationsRight)
        {
            GameObject GO = Instantiate(this.character, item.position, Quaternion.Euler(0, 180, 0));
            leftSide = false;
            Character character = new Character(GO, leftSide);
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
            List<Character> newTurn = new List<Character>();
            for (int y = 0; y < turn.Count; y++)
            {
                if (characterRight.Count == 0 || characterLeft.Count == 0)
                {
                    yield break;
                }
                if (turn[y].GetDeath() == false)
                {
                    yield return new WaitForSeconds(0.5f);
                    turn[y].Shoot();
                    if (turn[y].getSide() == true)
                    {
                        int index = Random.Range(0, characterRight.Count);
                        Character temp = characterRight[index];
                        temp.TakeAwayHealth();
                        int index2 = turn.IndexOf(characterRight[index]);
                        turn[index2] = temp;
                        if (temp.GetDeath() == true)
                        {
                            DeceasedCharacters.Add(temp);
                            characterRight.RemoveAt(index);
                        }
                        else
                        {
                            characterRight[index] = temp;
                        }
                    }
                    else
                    {
                        int index = Random.Range(0, characterLeft.Count);
                        Character temp = characterLeft[index];
                        temp.TakeAwayHealth();
                        int index2 = turn.IndexOf(characterLeft[index]);
                        turn[index2] = temp;

                        if (temp.GetDeath() == true)
                        {
                            DeceasedCharacters.Add(temp);
                            characterLeft.RemoveAt(index);
                        }
                        else
                        {
                            characterLeft[index] = temp;
                        }
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
    struct Character
    {
        int health;
        bool leftSide;
        SkeletonAnimation skeletonAnimation;
        bool death;
        public Character(GameObject GO, bool leftSide)
        {
            health = 100;
            this.leftSide = leftSide;
            skeletonAnimation = GO.GetComponent<SkeletonAnimation>();
            skeletonAnimation.state.SetAnimation(0, "idle", true);
            death = false;
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
            health -= Random.Range(0, 100);
            if (health <= 0)
            {
                death = true;
                skeletonAnimation.state.SetAnimation(0, "death", false);
                Debug.Log("Death");
            }
            else
            {
                Debug.Log(health);
            }
            return health;
        }
        public void Shoot()
        {
            skeletonAnimation.state.SetAnimation(1, "shoot", false);
        }
        public bool GetDeath()
        {
            return death;
        }
    }
}