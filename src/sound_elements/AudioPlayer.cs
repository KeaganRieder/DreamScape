namespace DreamScape.sound;

using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class AudioPlayer : AudioStreamPlayer
{
	private Timer audioTimer;
	private bool stopping;
	private float defaultVolume;
	public AudioPlayer()
	{
		Name = "AudioPlayer";
		stopping = false;

		audioTimer = new Timer();
		AddChild(audioTimer);
		audioTimer.Timeout += () => FadeAudio();
	}

	/// <summary>
	/// gets a audio path form a list of audio paths at the provided id
	/// and then loads the audio file
	/// </summary>
	public void LoadAudio(int soundID)
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		int soundType = rng.RandiRange(0, 1);
		string path = $"assets/sounds/{AudioConstants.Sounds[soundID]}_{soundType}.mp3";

		if (File.Exists(path))
		{
			var audioStream = GD.Load<AudioStream>(path);
			Stream = audioStream;
			if ((soundID == 5 && soundType == 0) || (soundID == 1 && soundType == 0))
			{
				defaultVolume = -10f;
				// GD.Print($"inverted_dream {defaultVolume}");
			}
			else
			{
				defaultVolume = 0;
			}

		}


		else
		{
			path = $"assets/sounds/{AudioConstants.Sounds[soundID]}_0.mp3";
			if (File.Exists(path))
			{
				var audioStream = GD.Load<AudioStream>(path);
				Stream = audioStream;
				defaultVolume = 0;
			}
			else
			{
				GD.PushError($"Audio file at path {path} not found");
			}
		}
	}

	/// <summary>
	/// plays the audio
	/// </summary>
	public void PlayAudio()
	{
		if (Stream == null)
		{
			GD.PushError("No audio loaded");
			return;
		}

		if (!Playing)
		{
			VolumeDb = defaultVolume;
			Play();
		}
	}

	/// <summary>
	/// stops the audio
	/// </summary>
	public void StopAudio()
	{
		if (!stopping)
		{
			stopping = true;
			audioTimer.Start(AudioConstants.FadeOutDelay);
		}
	}

	/// <summary>
	/// fades audio  out
	/// </summary>
	private void FadeAudio()
	{
		if (VolumeDb > -40)
		{
			VolumeDb -= AudioConstants.FadeOutAmt;

			//GD.Print($"fading out: {VolumeDb}");

			if (audioTimer.IsStopped())
			{
				audioTimer.Start(AudioConstants.FadeOutDelay);
			}
		}

		else
		{
			VolumeDb = 0;
			stopping = false;
			audioTimer.Stop();
			Stop();
		}
	}

}
