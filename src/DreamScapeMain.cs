namespace DreamScape;

using DreamScape.ImgGeneration;
using Godot;
using System.Collections.Generic;

/// <summary>
/// this is the main class of the work, 
/// it handles connecting different classes and things together
/// in order to create the experience
/// </summary>
public partial class DreamScapeMain : Node
{
	public static readonly Dictionary<string, InputEvent> KeyBindings = new Dictionary<string, InputEvent>
	{
		{"PlayAmbientMusic",new InputEventMouseButton { ButtonIndex = MouseButton.Left}}, // mouse click
		{"NewPerlinDream", new InputEventKey{PhysicalKeycode = Key.Down}}, // makes new random img with perlin noise
		{"NewDream",  new InputEventKey{PhysicalKeycode = Key.Up}}, // randomizes frequency used by generator noise maybe make it cellular noise

		{"MostRecentDream", new InputEventKey{PhysicalKeycode = Key.Space}}, // uses last generated img
		{"NewPalette",  new InputEventKey{PhysicalKeycode = Key.Left}}, // swaps to a Light palette WIP
		{"InvertedDream", new InputEventKey{PhysicalKeycode = Key.Right}}, // swaps to a dark palette WIP
	};

	private ImgGenerator imgGenerator;
	private RandomNumberGenerator rng;

	public DreamScapeMain()
	{
		Name = "DreamScapeMain";
		imgGenerator = ImgGenerator.Instance;
		rng = new RandomNumberGenerator();

		//setting initial values for noise generator
		var NoiseGenerator = imgGenerator.NoiseGenerator;
		NoiseGenerator.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
		NoiseGenerator.Seed = rng.RandiRange(0, 10000);
		NoiseGenerator.Frequency = 0.05f;
		NoiseGenerator.FractalLacunarity = 2f;
		NoiseGenerator.FractalOctaves = 6;
		NoiseGenerator.FractalGain = 0.4f;

		imgGenerator.Generate();
		AddChild(imgGenerator);
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
		if (Input.IsActionPressed("PlayAmbientMusic"))
		{
			GD.Print("Playing Ambient Music WIP"); //maybe make this also have the img no fade in or out
		}
	}

	public override void _UnhandledInput(InputEvent inputEvent)
	{
		if (imgGenerator.State == ImgGeneratorState.Idle)
		{
			if (Input.IsActionJustPressed("NewPerlinDream"))
			{
				GD.Print("New Perlin Dream");
				rng = new RandomNumberGenerator();
				imgGenerator.NoiseGenerator.Seed = rng.RandiRange(0, 10000);
				imgGenerator.Generate();
			}

			else if (Input.IsActionJustPressed("NewDream"))
			{
				GD.Print("New Dream");
				rng = new RandomNumberGenerator();
				imgGenerator.NoiseGenerator.Seed = rng.RandiRange(0, 10000);
				imgGenerator.RandomizePalette();
				imgGenerator.Generate();
			}

			else if (Input.IsActionJustPressed("MostRecentDream"))
			{
				GD.Print("Using most recent dream");
				imgGenerator.StartFadeEffect();
			}

			else if (Input.IsActionJustPressed("NewPalette"))
			{
				GD.Print("different toned Dream");
				imgGenerator.RandomizePalette();
				imgGenerator.Generate();
			}

			else if (Input.IsActionJustPressed("InvertedDream"))
			{
				GD.Print("Inverting Dream");
				imgGenerator.InvertPalette();
				imgGenerator.Generate();
			}
			
			else{
				GD.Print(inputEvent);
			}
		}

		base._UnhandledInput(inputEvent);
	}


}
