﻿using Contents;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Basic
{
    class BasicSceneManager : MonoBehaviour
    {
        private void Start()
        {
            DialogText = GameObject.Find("DialogText").GetComponent<Text>();
            PortraitBodyImage = GameObject.Find("Speaker").GetComponent<Image>();
            PortraitFaceImage = GameObject.Find("Face").GetComponent<Image>();
            PortraitDressImage = GameObject.Find("Dress").GetComponent<Image>();
            BackgroudImage = GameObject.Find("Background").GetComponent<Image>();
            SkipButton = GameObject.Find("Skip").GetComponent<Button>();
            NextButton = GameObject.Find("DialogArea").GetComponent<Button>();
            YesButton = GameObject.Find("YesButton").GetComponent<Button>();
            NoButton = GameObject.Find("NoButton").GetComponent<Button>();
        }

        public Text DialogText { get; set; }
        public Image PortraitFaceImage { get; set; }
        public Image PortraitDressImage { get; set; }
        public Image PortraitBodyImage { get; set; }
        public Image BackgroudImage { get; set; }
        public Button SkipButton { get; set; }
        public Button NextButton { get; set; }
        public Button YesButton { get; set; }
        public Button NoButton { get; set; }

        private void Update()
        {
            if (RootState.PlayState == null)
            {
                SceneManager.LoadScene("Loading");
                return;
            }

            var e = RootState.PlayState.ActiveEntity;

            DialogText.text = e?.DialogText ?? "";
            if (RootState.FlagsState.Development)
            {
                DialogText.text = "개발 플래그가 설정되어 있습니다. 게임을 바로 시작합니다.";
            }
            KaraResources.LoadSprite(PortraitFaceImage, e, n => n.PortraitFaceImage);
            KaraResources.LoadSprite(PortraitBodyImage, e, n => n.PortraitBodyImage);
            KaraResources.LoadSprite(PortraitDressImage, e, n => n.PortraitDressImage);
            KaraResources.LoadSprite(BackgroudImage, RootState.PlayState, p => p.BackgroundImage);

            SkipButton.gameObject.SetActive(e.SkipKey != null);

            var yesNo = e.ClickedYesKey != null;
            NextButton.interactable = !yesNo;
            YesButton.gameObject.SetActive(yesNo);
            NoButton.gameObject.SetActive(yesNo);
        }

        public void DialogSkip()
        {
            RootState.PlayState.ActiveEntity =
                GameConfiguration.Root.FindByKey(RootState.PlayState.ActiveEntity.SkipKey);
        }

        public void DialogNext()
        {
            var e = RootState.PlayState.ActiveEntity;
            if (RootState.FlagsState.Development)
            {
                FastTrack();
            }
            if (e.IsGameOver)
            {
                RootState.PlayState.ActiveEntity = null;
                SceneManager.LoadScene("Loading");
                return;
            }
            if (e.NextScene != null)
            {
                SceneManager.LoadScene(e.NextScene);
                return;
            }
            if (e.NextKey != null)
            {
                RootState.PlayState.ActiveEntity = GameConfiguration.Root.FindByKey(e.NextKey);
            }
        }

        private void FastTrack()
        {
            var e = RootState.PlayState.ActiveEntity;
            if (e.NextScene != null)
            {
                SceneManager.LoadScene(e.NextScene);
                return;
            }
            if (e.NextKey != null)
            {
                RootState.PlayState.ActiveEntity = GameConfiguration.Root.FindByKey(e.NextKey);
                FastTrack();
            }
            if (e.ClickedYesKey != null)
            {
                RootState.PlayState.ActiveEntity = GameConfiguration.Root.FindByKey(e.ClickedYesKey);
                FastTrack();
            }
        }

        public void DialogYes()
        {
            var e = RootState.PlayState.ActiveEntity;
            RootState.PlayState.ActiveEntity = GameConfiguration.Root.FindByKey(e.ClickedYesKey);
        }

        public void DialogNo()
        {
            var e = RootState.PlayState.ActiveEntity;
            RootState.PlayState.ActiveEntity = GameConfiguration.Root.FindByKey(e.ClickedNoKey);
        }
    }
}
