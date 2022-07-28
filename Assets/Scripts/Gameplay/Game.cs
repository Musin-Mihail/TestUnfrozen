using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    [RequireComponent(typeof(Fight), typeof(CharactersController))]
    public class Game : MonoBehaviour
    {
        public bool BoolPlayerChoice { get; set; }
        
        [SerializeField] private Transform _selectTarget;
        [SerializeField] private Transform _playerFight;
        [SerializeField] private Transform _playerWait;
        
        private CharactersController _charactersController;
        private Fight _fight;
        private Camera _camera;

        private void Start()
        {
            _charactersController = GetComponent<CharactersController>();
            _fight = GetComponent<Fight>();
            
            _camera = Camera.main;
            RestartGame();
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && BoolPlayerChoice == true)
            {
                RaycastHit2D hit = Physics2D.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (!hit) return;
                if (_fight.BoolFight == true)
                {
                    for (int i = 0; i < _charactersController.CharacterRight.Count; i++)
                    {
                        if (_charactersController.CharacterRight[i].transform == hit.transform)
                        {
                            _fight.TargetCharacter = _charactersController.CharacterRight[i];
                            _fight.IndexTarget = i;
                            BoolPlayerChoice = false;
                            _selectTarget.gameObject.SetActive(false);
                            break;
                        }
                    }
                }
                else
                {
                    if (hit.transform == _playerFight)
                    {
                        _fight.BoolFight = true;
                        _playerFight.gameObject.SetActive(false);
                        _playerWait.gameObject.SetActive(false);
                        _selectTarget.gameObject.SetActive(true);
                    }
                }
                if (hit.transform == _playerWait)
                {
                    _fight.BoolWait = true;
                    ToggleChoiceUI(false);
                    BoolPlayerChoice = false;
                }
            }
        }

        public void ChangeAutoFight(Button button)
        {
            Text text = button.GetComponentInChildren<Text>();
            Image image = button.GetComponent<Image>();
            if (_fight.BoolAutoFight == false)
            {
                _fight.BoolAutoFight = true;
                text.text = "Автобой\nвключен";
                image.color = Color.green;
            }
            else
            {
                _fight.BoolAutoFight = false;
                text.text = "Автобой\nвыключен";
                image.color = Color.red;
            }
        }

        public void RestartGame()
        {
            List<Characters.Character> turn = _charactersController.StartGeneration();
            StartCoroutine(_fight.StartFight(_charactersController, turn, this));
        }

        public void EnablePlayerChoice() 
            => ToggleChoiceUI(true);

        private void ToggleChoiceUI(bool toggle)
        {
            _playerFight.gameObject.SetActive(toggle);
            _playerWait.gameObject.SetActive(toggle);
        }
    }
}