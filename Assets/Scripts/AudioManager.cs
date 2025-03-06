using UnityEngine;

namespace FrogStar
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Main;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource soundSource;

        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private AudioClip shiverSound;
        [SerializeField] private AudioClip burningSound;
        [SerializeField] private AudioClip comfortSound;
        [SerializeField] private AudioClip destroySound;
        [SerializeField] private AudioClip winSound;

        private void Awake()
        {
            if (Main == null)
                Main = this;
            else
                Destroy(this);

            DontDestroyOnLoad(this.gameObject);
            musicSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void PlayerShiver()
        {
            soundSource.pitch = 1 + Random.Range(-0.1f, 0.1f);
            soundSource.PlayOneShot(shiverSound);
        }

        public void PlayBurning()
        {
            soundSource.pitch = 1 + Random.Range(-0.1f, 0.1f);
            soundSource.PlayOneShot(burningSound);
        }

        public void PlayComfort()
        {
            soundSource.pitch = 1 + Random.Range(-0.1f, 0.1f);
            soundSource.PlayOneShot(comfortSound);
        }

        public void PlayWin()
        {
            soundSource.pitch = 1 + Random.Range(-0.1f, 0.1f);
            soundSource.PlayOneShot(winSound);
        }

        public void PlayDestroy()
        {
            soundSource.pitch = 1 + Random.Range(-0.1f, 0.1f);
            soundSource.PlayOneShot(destroySound);
        }
    }
}
