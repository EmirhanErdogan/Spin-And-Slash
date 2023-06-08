using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Emir
{
    public class SoundManager : Singleton<SoundManager>
    {
        #region Serializable Fields

        [Header("General")] [SerializeField] private List<Sound> m_sounds;
        [SerializeField] private AudioSource m_background;

        #endregion

        #region Private Fields

        private bool state;
        private bool IsMute = true;

        #endregion

        /// <summary>
        /// This function called when before first frame.
        /// </summary>
        protected override void Awake()
        {
            foreach (Sound sound in m_sounds)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();

                if (sound.Clips.Length == 0)
                    continue;

                AudioClip audioClip = sound.Clips[Random.Range(0, sound.Clips.Length)];

                source.clip = audioClip;
                source.pitch = sound.Pitch;
                source.volume = sound.Volume;
                source.loop = sound.IsLoop;

                sound.Source = source;
            }

            state = PlayerPrefs.GetInt(CommonTypes.SOUND_STATE_KEY) == 0;

            base.Awake();
        }
        
        
        /// <summary>
        /// This function helper for play sound with tag.
        /// </summary>
        /// <param name="tag"></param>
        public void Play(string tag)
        {
            if (!IsMute) return;

            Sound targetSound = m_sounds.SingleOrDefault(x => x.Tag == tag);
            AudioClip targetClip = null;

            if (targetSound == null)
                return;

            if (targetSound.Clips.Length == 0)
                return;

            targetClip = targetSound.Clips[Random.Range(0, targetSound.Clips.Length)];

            if (targetClip == null)
            {
                return;
            }

            targetSound.Source.PlayOneShot(targetClip);
        }


        /// <summary>
        /// This function returns related state.
        /// </summary>
        /// <returns></returns>
        public bool GetState()
        {
            return state;
        }

        public void SetMuteState(bool value)
        {
            IsMute = value;
        }
    }
}