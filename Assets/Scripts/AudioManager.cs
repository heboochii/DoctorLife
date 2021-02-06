using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PediatricSim
{

	public class AudioManager : MonoBehaviour
	{

		#region Singleton

		private static AudioManager m_Singleton;

		public static AudioManager Singleton {
			get {
				return m_Singleton;
			}
		}

		#endregion

		#region Variables

		[Header ("Audio Sources")]
		[Space]
		[SerializeField]
		protected AudioSource m_MusicAudioSource;
		[SerializeField]
		protected AudioSource m_UIAudioSource;

		[Header ("Music Clips")]
		[Space]
		[SerializeField]
		protected AudioClip m_MusicClip;

		[SerializeField]
		protected AudioClip m_ButtonClickSound;

		#endregion

		#region Monobehaviour

		void Awake ()
		{
			m_Singleton = this;
			PlayMusic ();
		}

		#endregion

		#region functions

		public void PlayMusic ()
		{
			m_MusicAudioSource.clip = m_MusicClip;
			m_MusicAudioSource.Play ();
		}

		public void PlaySoundAt (AudioClip clip, Vector3 position, float volume)
		{
			AudioSource.PlayClipAtPoint (clip, position, volume);
		}

		public void PlaySoundOn (AudioSource audio, AudioClip clip)
		{
			audio.Play ();
		}



		public void PlayClickSound ()
		{
			PlaySoundOn (m_UIAudioSource, m_ButtonClickSound);
		}

		#endregion

	}

}