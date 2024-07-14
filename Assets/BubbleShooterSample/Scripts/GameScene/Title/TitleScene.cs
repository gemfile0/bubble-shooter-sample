using BubbleShooterSample.System.SceneTransition;
using System;
using UnityEngine;

namespace BubbleShooterSample.Title
{
    public class TitleScene : MonoBehaviour, ISpecificSceneTrigger
    {
        public event Action<SceneName> requestSpecificScene;

        public void OnPlayButtonClick()
        {
            requestSpecificScene?.Invoke(SceneName.LevelPlayerScene);
        }
    }
}
