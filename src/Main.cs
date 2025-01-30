namespace DreamScape;

using DreamScape.sound;
using DreamScape.img;
using Godot;
using System.Collections.Generic;

/// <summary>
/// this is the main class of the work, 
/// it handles connecting different classes and things together
/// in order to create the experience
/// </summary>
public partial class Main : Node
{
	public static readonly Dictionary<string, InputEvent> KeyBindings = new Dictionary<string, InputEvent>
	{
		{"PlayAmbientMusic",new InputEventMouseButton { ButtonIndex = MouseButton.Left}}, // mouse click
		{"NewPerlinDream", new InputEventKey{PhysicalKeycode = Key.Down}}, // makes new random img with perlin noise
		{"NewDream",  new InputEventKey{PhysicalKeycode = Key.Up}}, // randomizes frequency used by generator noise maybe make it cellular noise

		{"MostRecentDream", new InputEventKey{PhysicalKeycode = Key.Space}}, // uses last generated img
		{"NewPalette",  new InputEventKey{PhysicalKeycode = Key.Left}}, // swaps to a Light palette WIP
		{"InvertedDream", new InputEventKey{PhysicalKeycode = Key.Right}}, // swaps to a dark palette WIP
		{"Quit", new InputEventKey{PhysicalKeycode = Key.Escape}}, // swaps to a dark palette WIP
	};


	private FadeType fadeType;
	private RandomNumberGenerator rng;
	private DreamScapeImg image;
	private AudioPlayer audioPlayer;

	public Main()
	{
		Name = "DreamScapeMain";
		audioPlayer = new AudioPlayer();
		image = DreamScapeImg.Instance;
		rng = new RandomNumberGenerator();

		Input.SetMouseMode(Input.MouseModeEnum.Hidden);

		//setting initial values for noise generator
		var NoiseGenerator = image.NoiseGenerator;
		NoiseGenerator.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
		NoiseGenerator.Seed = rng.RandiRange(0, 10000);
		NoiseGenerator.Frequency = 0.05f;
		NoiseGenerator.FractalLacunarity = 2f;
		NoiseGenerator.FractalOctaves = 6;
		NoiseGenerator.FractalGain = 0.4f;

		image.Generate();
		AddChild(image);
		AddChild(audioPlayer);
	}

	public override void _Ready()
	{
		foreach (var binding in KeyBindings)
		{
			InputMap.AddAction(binding.Key);
			InputMap.ActionAddEvent(binding.Key, binding.Value);
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("PlayAmbientMusic"))
		{
			if (States.Instance.Debugging)
			{
				GD.Print("\nImg Back to Cycling\n");
			}

			fadeType = FadeType.CycleFade;
			States.Instance.UpdatedFadeMode = true;

			audioPlayer.LoadAudio(0);
		}

		if (Input.IsActionPressed("PlayAmbientMusic"))
		{
			if (States.Instance.Debugging)
			{
				GD.Print("Playing Ambient Music WIP");
			}
			audioPlayer.PlayAudio();
		}

		if (Input.IsActionJustReleased("PlayAmbientMusic"))
		{
			if (States.Instance.Debugging)
			{
				GD.Print("\nImg Back to default\n");
			}
			fadeType = FadeType.DefaultFade;
			States.Instance.UpdatedFadeMode = true;

		}

		if (States.Instance.UpdatedFadeMode)
		{
			if (States.Instance.ImgState == ImgState.Idle)
			{
				States.Instance.FadeMode = fadeType;

				if (States.Instance.Debugging)
				{
					GD.Print("Swapping Fade modes");
				}
				States.Instance.UpdatedFadeMode = false;

				if (fadeType == FadeType.DefaultFade)
				{
					audioPlayer.StopAudio();
				}

				image.StartFadeEffect();

			}
		}

		States.Instance.CheckIfDoneFading();
		base._Process(delta);
	}

	public override void _UnhandledInput(InputEvent inputEvent)
	{
		if (States.Instance.ImgState == ImgState.Idle)
		{
			if (Input.IsActionJustPressed("NewPerlinDream"))
			{
				if (States.Instance.Debugging)
				{
					GD.Print("New Perlin Dream");
				}

				if (States.Instance.FadeMode == FadeType.DefaultFade)
				{
					audioPlayer.LoadAudio(1);
					audioPlayer.PlayAudio();
					audioPlayer.StopAudio();
				}

				rng = new RandomNumberGenerator();
				image.NoiseGenerator.Seed = rng.RandiRange(0, 10000);
				image.RandomizePalette();
				image.Generate();
				image.StartFadeEffect();
			}

			else if (Input.IsActionJustPressed("NewDream"))
			{
				if (States.Instance.Debugging)
				{
					GD.Print("New Dream");
				}

				rng = new RandomNumberGenerator();
				image.NoiseGenerator.Seed = rng.RandiRange(0, 10000);
				image.RandomizePalette();
				image.Generate();
				image.StartFadeEffect();

				if (States.Instance.FadeMode == FadeType.DefaultFade)
				{
					audioPlayer.LoadAudio(2);
					audioPlayer.PlayAudio();
					audioPlayer.StopAudio();
				}
			}

			else if (Input.IsActionJustPressed("MostRecentDream"))
			{
				if (States.Instance.Debugging)
				{
					GD.Print("Using most recent dream");
				}

				if (States.Instance.FadeMode == FadeType.CycleFade)
				{
					rng = new RandomNumberGenerator();
					image.NoiseGenerator.Seed = rng.RandiRange(0, 10000);
					image.Generate();
					image.StartFadeEffect();
				}

				else
				{
					image.StartFadeEffect();
					audioPlayer.LoadAudio(3);
					audioPlayer.PlayAudio();
					audioPlayer.StopAudio();
				}
			}

			else if (Input.IsActionJustPressed("NewPalette"))
			{
				if (States.Instance.Debugging)
				{
					GD.Print("different toned Dream");
				}

				image.Generate();
				image.StartFadeEffect();

				if (States.Instance.FadeMode == FadeType.DefaultFade)
				{
					audioPlayer.LoadAudio(4);
					audioPlayer.PlayAudio();
					audioPlayer.StopAudio();
				}
			}

			else if (Input.IsActionJustPressed("InvertedDream"))
			{
				if (States.Instance.Debugging)
				{
					GD.Print("Inverting Dream");
				}

				image.InvertPalette();
				image.Generate();
				image.StartFadeEffect();

				if (States.Instance.FadeMode == FadeType.DefaultFade)
				{
					audioPlayer.LoadAudio(5);
					audioPlayer.PlayAudio();
					audioPlayer.StopAudio();
				}
			}

		}
		if (Input.IsActionJustPressed("Quit"))
		{
			GetTree().Quit();
		}

		base._UnhandledInput(inputEvent);
	}
}
