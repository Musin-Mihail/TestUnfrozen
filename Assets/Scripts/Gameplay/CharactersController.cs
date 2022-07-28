using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace Gameplay
{
    public class CharactersController : MonoBehaviour
    {
        [SerializeField] private float _firstCharacterSpace;
        [SerializeField] private float _spaceBetweenCharacters;
        [SerializeField] private Transform _attackPositionLeft;
        [SerializeField] private Transform _attackPositionRight;
        public List<Character> PoolCharacters { get; } = new List<Character>();
        public List<Character> CharacterRight { get; } = new List<Character>();
        public List<Character> CharacterLeft { get; } = new List<Character>();

        private readonly List<Vector3> _characterLocationsLeft = new List<Vector3>();
        private readonly List<Vector3> _characterLocationsRight = new List<Vector3>();

        private void Awake()
        {
            float pos = _firstCharacterSpace;
            for (int i = 0; i < 4; i++, pos += _spaceBetweenCharacters)
                _characterLocationsLeft.Add(new Vector3(pos, 0, 0) * -1);

            pos = _firstCharacterSpace;
            for (int i = 0; i < 4; i++, pos += _spaceBetweenCharacters)
                _characterLocationsRight.Add(new Vector3(pos, 0, 0));
        }

        public List<Character> StartGeneration()
        {
            List<Character> turn = new List<Character>();
            foreach (var item in _characterLocationsLeft)
            {
                PoolAddCharacter(true,
                    item,
                    _attackPositionLeft.position,
                    CharacterLeft,
                    turn);
            }

            foreach (var item in _characterLocationsRight)
            {
                PoolAddCharacter(false,
                    item,
                    _attackPositionRight.position,
                    CharacterRight,
                    turn);
            }

            for (int i = 0; i < turn.Count; i++)
            {
                Character tempCharacter = turn[i];
                int randomIndex = Random.Range(i, turn.Count);
                turn[i] = turn[randomIndex];
                turn[randomIndex] = tempCharacter;
            }

            return turn;
        }

        private void PoolAddCharacter(bool leftSide, Vector3 position, Vector3 center,
                                      List<Character> characters,
                                      List<Character> turn)
        {
            if (PoolCharacters.Count > 0)
            {
                Character tempCharacter = PoolCharacters[0];
                PoolCharacters.RemoveAt(0);
                tempCharacter.Reset();
                turn.Add(tempCharacter);
                characters.Add(tempCharacter);
            }
            else
            {
                GameObject prefab = Resources.Load<GameObject>("Character");
                Quaternion rotation = leftSide ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, -180, 0);

                var character = Instantiate(prefab, position, rotation)
                                .AddComponent<Character>()
                                .Setup(leftSide, center);
                turn.Add(character);
                characters.Add(character);
            }
        }
    }
}