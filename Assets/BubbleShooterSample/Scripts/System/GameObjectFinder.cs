using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BubbleShooterSample.System
{
    public interface IGameObjectFinder
    {
        IEnumerable<T> FindGameObjectOfType<T>();
        T FindRootGameObject<T>(string name);
    }

    public interface IGameObjectFinderSetter
    {
        void OnGameObjectFinderAwake(IGameObjectFinder finder);
    }

    public class GameObjectFinder : MonoBehaviour, IGameObjectFinder
    {
        private GameObject[] _rootObjectArray;

        private void Awake()
        {
            _rootObjectArray = SceneManager.GetActiveScene().GetRootGameObjects();

            IEnumerable<IGameObjectFinderSetter> _setters = FindGameObjectOfType<IGameObjectFinderSetter>();
            foreach (IGameObjectFinderSetter setter in _setters)
            {
                setter.OnGameObjectFinderAwake(this);
            }
        }

        public T FindRootGameObject<T>(string name)
        {
            T result = default;
            foreach (GameObject rootObject in _rootObjectArray)
            {
                if (rootObject.name == name)
                {
                    result = rootObject.GetComponent<T>();
                    break;
                }
            }

            return result;
        }

        public IEnumerable<T> FindGameObjectOfType<T>()
        {
            List<T> result = new List<T>();

            foreach (GameObject rootObject in _rootObjectArray)
            {
                foreach (T component in rootObject.GetComponentsInChildren<T>())
                {
                    result.Add(component);
                }
            }

            return result;
        }
    }
}
