﻿using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;

using Library;
using Units;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Button = UnityEngine.UI.Button;
using Event = Define.Event;

namespace UI
{
    public class UIManager : MonoSingleton<UIManager>, IParentable
    {
        #region -- VARIABLES --
        [SerializeField]
        private SkillButton m_SkillButtonPrefab;
        [SerializeField]
        private List<SkillButton> m_SkillButtons;
        [SerializeField]
        private Text m_WaveCounter;
        [SerializeField]
        private RectTransform m_HUD;
        [SerializeField]
        private RectTransform m_QuitMenu;
        [SerializeField]
        private RectTransform m_InstructionMenu;
        [SerializeField]
        private RectTransform m_GameOverMenu;
        [SerializeField]
        private Button m_NewGame;
        [SerializeField]
        private Button m_LoadGame;
        [SerializeField]
        private Button m_Instructions;
        [SerializeField]
        private Button m_QuitGame;
        #endregion

        #region -- UNITY FUNCTIONS --
        protected override void Awake()
        {
            base.Awake();

            m_SkillButtons = new List<SkillButton>();

            GetComponents();

            Publisher.self.Subscribe(Event.Instructions, OnInstructions);
            Publisher.self.Subscribe(Event.ToggleQuitMenu, OnToggleQuitMenu);
            Publisher.self.Subscribe(Event.SpawnWave, OnSpawnWave);
            Publisher.self.Subscribe(Event.MainMenu, OnMainMenu);
            Publisher.self.Subscribe(Event.GameOver, OnGameOver);
            if (m_SkillButtonPrefab != null)
                Publisher.self.Subscribe(Event.UnitInitialized, OnUnitInitialized);
            else
                Debug.LogWarning("UIManager needs a 'Skill Button Prefab' in order to function properly");

        }

        // Use this for initialization
        private void Start()
        {
            if (m_SkillButtonPrefab.GetComponent<RectTransform>() == null)
                return;

            List<IUsesSkills> skillUsers =
                FindObjectsOfType<GameObject>().
                    Where(
                        x => x.GetComponent<IControllable>() != null &&
                        x.GetComponent<IControllable>().controllerType == ControllerType.User &&
                        x.GetComponent<IUsesSkills>() != null).
                    Select(x => x.GetComponent<IUsesSkills>()).
                    ToList();

            skillUsers.Sort((a, b) => a.unitName.CompareTo(b.unitName));

            int numOfSkills = 0;
            foreach (IUsesSkills skillUser in skillUsers)
                numOfSkills += skillUser.skills.Count - 1;

            int k = 0;
            for (int i = 0; i < skillUsers.Count; i++)
            {
                for (int j = 0; j < skillUsers[i].skills.Count; j++)
                {
                    SkillButton skillButton = InstantiateRectTransform(
                        m_SkillButtonPrefab,
                        new Vector3(
                            k * 70 + i * 100 - (numOfSkills * 70 + (skillUsers.Count - 1) * 100) / 2,
                            0,
                            0));

                    skillButton.parent = skillUsers[i];
                    skillButton.skillIndex = j;
                    skillButton.sprite = skillUsers[i].skills[j].sprite;

                    m_SkillButtons.Add(skillButton);

                    k++;
                }
                k--;
            }
        }

        //LateUpdate is called once per frame
        private void LateUpdate()
        {

        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            Publisher.self.UnSubscribe(Event.Instructions, OnInstructions);
            Publisher.self.UnSubscribe(Event.ToggleQuitMenu, OnToggleQuitMenu);
            Publisher.self.UnSubscribe(Event.SpawnWave, OnSpawnWave);
            Publisher.self.UnSubscribe(Event.MainMenu, OnMainMenu);
            Publisher.self.UnSubscribe(Event.GameOver, OnGameOver);
            if (m_SkillButtonPrefab != null)
                Publisher.self.UnSubscribe(Event.UnitInitialized, OnUnitInitialized);
        }
        #endregion

        #region -- PRIVATE VOID FUNCTIONS --
        private void GetComponents()
        {
            foreach (Transform child in transform)
            {
                switch (child.tag)
                {
                    case "HUD":
                        {
                            //if (m_HUD == null)
                                m_HUD = child.GetComponent<RectTransform>();
                            if (m_HUD == null)
                            {
                                Debug.LogWarning("UIManager is missing an object with the 'HUD' tag parented to it");
                                continue;
                            }

                            Button spawnWaveButton = m_HUD.GetComponentsInChildren<Button>()[0];
                            Button instructionsButton = m_HUD.GetComponentsInChildren<Button>()[1];

                            spawnWaveButton.onClick.AddListener(OnSpawnWaveClick);
                            instructionsButton.onClick.AddListener(OnInstructionsClick);
                            m_WaveCounter = m_HUD.GetComponentInChildren<Text>();
                        }
                        break;
                    case "Quit Menu":
                        {
                            //if (m_QuitMenu == null)
                                m_QuitMenu = child.GetComponent<RectTransform>();
                            if (m_QuitMenu == null)
                            {
                                Debug.LogWarning("UIManager is missing an object with the 'Quit Menu' tag parented to it");
                                continue;
                            }

                            Button quitButton = m_QuitMenu.GetComponentsInChildren<Button>()[0];
                            Button resumeButton = m_QuitMenu.GetComponentsInChildren<Button>()[1];

                            quitButton.onClick.AddListener(OnQuitGameClick);
                            resumeButton.onClick.AddListener(OnResumeClick);

                            m_QuitMenu.gameObject.SetActive(false);
                        }
                        break;
                    case "Instructions Menu":
                        {
                            //if (m_InstructionMenu == null)
                                m_InstructionMenu = child.GetComponent<RectTransform>();
                            if (m_InstructionMenu == null)
                            {
                                Debug.LogWarning("UIManager is missing an object with the 'Instructions Menu' tag parented to it");
                                continue;
                            }

                            Button closeButton = m_InstructionMenu.GetComponentInChildren<Button>();

                            closeButton.onClick.AddListener(OnInstructionsCloseClick);

                            m_InstructionMenu.gameObject.SetActive(false);
                        }
                        break;
                    case "Game Over Menu":
                        {
                            //if (m_GameOverMenu == null)
                                m_GameOverMenu = child.GetComponent<RectTransform>();
                            if (m_GameOverMenu == null)
                            {
                                Debug.LogWarning("UIManager is missing an object with the 'Game Over Menu' tag parented to it");
                                continue;
                            }

                            Button mainMenuButton = m_GameOverMenu.GetComponentsInChildren<Button>()[0];
                            Button quitButton = m_GameOverMenu.GetComponentsInChildren<Button>()[1];

                            mainMenuButton.onClick.AddListener(OnMainMenuClick);
                            quitButton.onClick.AddListener(OnQuitGameClick);

                            m_GameOverMenu.gameObject.SetActive(false);
                        }
                        break;
                    case "New Game":
                        {
                            //if (m_NewGame == null)
                                m_NewGame = child.GetComponent<Button>();
                            if (m_NewGame == null)
                            {
                                Debug.LogWarning("UIManager is missing an object with the 'New Game' tag parented to it");
                                continue;
                            }

                            m_NewGame.onClick.AddListener(delegate {SceneManager.LoadScene("Andrew"); });
                        }
                        break;
                    case "Load Game":
                        {
                            //if (m_LoadGame == null)
                                m_LoadGame = child.GetComponent<Button>();
                            if (m_LoadGame == null)
                            {
                                Debug.LogWarning("UIManager is missing an object with the 'Load Game' tag parented to it");
                                continue;
                            }

                            m_LoadGame.onClick.AddListener(OnLoadGameClick);
                        }
                        break;
                    case "Instructions":
                        {
                            //if (m_Instructions == null)
                                m_Instructions = child.GetComponent<Button>();
                            if (m_Instructions == null)
                            {
                                Debug.LogWarning("UIManager is missing an object with the 'Instructions' tag parented to it");
                                continue;

                            }

                            m_Instructions.onClick.AddListener(OnInstructionsClick);
                        }
                        break;
                    case "Quit Game":
                        {
                            //if (m_QuitGame == null)
                                m_QuitGame = child.GetComponent<Button>();
                            if (m_QuitGame == null)
                            {
                                Debug.LogWarning("UIManager is missing an object with the 'Quit Game' tag parented to it");
                                continue;

                            }

                            m_QuitGame.onClick.AddListener(delegate { Application.Quit(); });
                        }
                        break;
                }


            }
        }
        #endregion

        #region -- EVENT FUNCTIONS --

        private void OnUnitInitialized(Event a_Event, params object[] a_Params)
        {

        }

        private void OnMainMenu(Event a_Event, params object[] a_Params)
        {
            SceneManager.LoadScene(0);
        }

        private void OnToggleQuitMenu(Event a_Event, params object[] a_Params)
        {
            m_QuitMenu.gameObject.SetActive(!m_QuitMenu.gameObject.activeInHierarchy);
            Publisher.self.Broadcast(m_QuitMenu.gameObject.activeInHierarchy ? Event.PauseGame : Event.UnPauseGame);
        }

        private void OnInstructions(Event a_Event, params object[] a_Params)
        {
            // Do stuff...
            m_InstructionMenu.gameObject.SetActive(true);
        }

        public void OnInstructionsClick()
        {
            Publisher.self.Broadcast(Event.Instructions);
        }

        public void OnInstructionsCloseClick()
        {
            m_InstructionMenu.gameObject.SetActive(false);
        }

        public void OnResumeClick()
        {
            m_QuitMenu.gameObject.SetActive(false);
            Publisher.self.Broadcast(Event.UnPauseGame);
        }

        public void OnSpawnWaveClick()
        {
            Publisher.self.Broadcast(Event.SpawnWaveClicked);
        }

        public void OnMainMenuClick()
        {
            Publisher.self.Broadcast(Event.MainMenu);
        }

        //Function for LoadGame button
        public void OnLoadGameClick()
        {
            //Load Game Function
            //Publisher Subscriber for LoadGame/ Broadcast
            Publisher.self.Broadcast(Event.LoadGame);
        }

        //Function for QuitGame button
        public void OnQuitGameClick()
        {
            //Publisher Subscriber or QuitGame / Broadcast 
            Publisher.self.Broadcast(Event.QuitGame);
        }

        private void OnSpawnWave(Event a_Event, params object[] a_Params)
        {
            int waveCounter = (int)a_Params[0];
            m_WaveCounter.text = " Wave: " + waveCounter;
        }
        #endregion

        #region -- PRIVATE FUNCTIONS --
        private SkillButton InstantiateRectTransform(SkillButton a_RectTransform, Vector3 a_Position)
        {
            SkillButton skillButton = Instantiate(a_RectTransform);
            skillButton.GetComponent<RectTransform>().SetParent(transform, false);

            skillButton.transform.localPosition += a_Position;

            return skillButton;
        }

        private void OnGameOver(Event a_Event, params object[] a_Params)
        {
            if (this == null)
                return;
            m_GameOverMenu.gameObject.SetActive(true);
        }
#endregion
    }
}