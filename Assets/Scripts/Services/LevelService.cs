using System;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Emir
{
    public static class LevelService
    {
        /// <summary>
        /// This function helper for initialize last level.
        /// </summary>
        public static void InitializeLevel()
        {
            GameSettings gameSettings = GameManager.Instance.GetGameSettings();
            
            int totalLevelCount = gameSettings.Levels.Length;
            int cachedLevelId = PlayerPrefs.GetInt(CommonTypes.LEVEL_ID_DATA_KEY);
            
            Level targetLevel = gameSettings.Levels.SingleOrDefault(x => x.Id == cachedLevelId % totalLevelCount);
            LoadLevel(targetLevel.SceneName);
        }

        /// <summary>
        /// This function helper for initialize next level.
        /// </summary>
        /// <param name="score"></param>
        public static void NextLevel(int score = 0)
        {
            GameSettings gameSettings = GameManager.Instance.GetGameSettings();
            
            int totalLevelCount = gameSettings.Levels.Length;
            int cachedLevelId = PlayerPrefs.GetInt(CommonTypes.LEVEL_ID_DATA_KEY);
            int nextLevelId = cachedLevelId + 1;
            
            PlayerPrefs.SetInt(CommonTypes.LEVEL_ID_DATA_KEY, nextLevelId);
            
            InitializeLevel();
        }

        /// <summary>
        /// This function helper for initialize current level.
        /// </summary>
        /// <param name="score"></param>
        public static void RetryLevel(int score = 0)
        {
            GameSettings gameSettings = GameManager.Instance.GetGameSettings();
            
            int totalLevelCount = gameSettings.Levels.Length;
            int cachedLevelId = GetCachedLevel();

            Level targetLevel = gameSettings.Levels.SingleOrDefault(x => x.Id == cachedLevelId % totalLevelCount);
            InitializeLevel();
        }

        /// <summary>
        /// This function helper for load level.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="loadSceneMode"></param>
        public static async void LoadLevel(string name, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            try
            {
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name, loadSceneMode);

                while (!asyncOperation.isDone)
                {
                    await Task.Delay(CommonTypes.DEFAULT_THREAD_SLEEP_MS);
                }

            }
            catch (Exception e)
            {
                throw;
            }


        }

        /// <summary>
        /// This function helper for unload level.
        /// </summary>
        /// <param name="name"></param>
        public static async void UnLoadLevel(string name)
        {
            try
            {
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);

                while (!asyncOperation.isDone)
                {
                    await Task.Delay(CommonTypes.DEFAULT_THREAD_SLEEP_MS);
                }
                
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// This function returns current level.
        /// </summary>
        /// <returns></returns>
        public static Level GetCurrentLevel()
        {
            GameSettings gameSettings = GameManager.Instance.GetGameSettings();
            
            int totalLevelCount = gameSettings.Levels.Length;
            int cachedLevelId = GetCachedLevel();

            Level targetLevel = gameSettings.Levels.SingleOrDefault(x => x.Id == cachedLevelId % totalLevelCount);

            return targetLevel;
        }

        /// <summary>
        /// This function returns true if target level is loaded.
        /// </summary>
        /// <returns></returns>
        public static bool IsLevelLoaded(string name)
        {
            bool isValid = false;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (scene.name == name)
                {
                    isValid = true;
                    break;
                }
            }
            
            return isValid;
        }
        
        /// <summary>
        /// This function returns cached level id.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int GetCachedLevel(int offset = 0)
        {
            return PlayerPrefs.GetInt(CommonTypes.LEVEL_ID_DATA_KEY) + offset;
        }
    }
}