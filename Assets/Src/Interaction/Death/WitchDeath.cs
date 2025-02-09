using System;
using Cysharp.Threading.Tasks;
using Src.Audio;
using Src.Common;
using Src.Control;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Src.Interaction.Death
{
    [RequireComponent(typeof(TopDownController))]
    public class WitchDeath : MonoBehaviour
    {
        private static readonly int Death = Animator.StringToHash("Death");

        [SerializeField] private Animator _characterAnimator;

        private TopDownController _characterController;
        private AudioSource _audioSource;
        private Blackout _blackout;
        private MusicManager _musicManager;

        private void Awake()
        {
            _characterController = GetComponent<TopDownController>();
            _audioSource = GetComponent<AudioSource>();
            _blackout = FindFirstObjectByType<Blackout>();
            _musicManager = FindFirstObjectByType<MusicManager>();
        }

        public Action OnDeath;

        public async UniTaskVoid Kill()
        {
            _characterController.CanControl = false;
            _characterAnimator.SetTrigger(Death);
            _audioSource.Play();
            _blackout.SetWitchOnBlack();
            _musicManager.ChangeMusic(MusicState.Death, false);
            OnDeath?.Invoke();
            await UniTask.Delay(10000);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}