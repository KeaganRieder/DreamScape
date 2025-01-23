namespace DreamScape;

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
	};

    private FadeType fadeType;
    private DreamScapeImg image;
    private RandomNumberGenerator rng;

    public Main()
    {
        Name = "DreamScapeMain";
        image = DreamScapeImg.Instance;
        rng = new RandomNumberGenerator();

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
            GD.Print("\nImg Back to Cycling\n");

            fadeType = FadeType.CycleFade;
            States.Instance.UpdatedFadeMode = true;
        }

        if (Input.IsActionPressed("PlayAmbientMusic"))
        {
            // GD.Print("Playing Ambient Music WIP");
        }

        if (Input.IsActionJustReleased("PlayAmbientMusic"))
        {
            GD.Print("\nImg Back to default\n");
            fadeType = FadeType.DefaultFade;
            States.Instance.UpdatedFadeMode = true;
        }

        if (States.Instance.UpdatedFadeMode)
        {
            if (States.Instance.ImgState == ImgState.Idle)
            {
                States.Instance.FadeMode = fadeType;

                GD.Print("Swapping Fade modes");
                States.Instance.UpdatedFadeMode = false;
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
                GD.Print("New Perlin Dream");
                rng = new RandomNumberGenerator();
                image.NoiseGenerator.Seed = rng.RandiRange(0, 10000);
                image.Generate();
                image.StartFadeEffect();
            }

            else if (Input.IsActionJustPressed("NewDream"))
            {
                GD.Print("New Dream");
                rng = new RandomNumberGenerator();
                image.NoiseGenerator.Seed = rng.RandiRange(0, 10000);
                image.RandomizePalette();
                image.Generate();
                image.StartFadeEffect();
            }

            else if (Input.IsActionJustPressed("MostRecentDream"))
            {
                if (States.Instance.FadeMode == FadeType.CycleFade)
                {
                    GD.Print("New Perlin Dream");
                    rng = new RandomNumberGenerator();
                    image.NoiseGenerator.Seed = rng.RandiRange(0, 10000);
                    image.RandomizePalette();
                    image.Generate();
                    image.StartFadeEffect();
                }
                else
                {
                    GD.Print("Using most recent dream");
                    image.StartFadeEffect();
                }
            }

            else if (Input.IsActionJustPressed("NewPalette"))
            {
                GD.Print("different toned Dream");
                image.RandomizePalette();
                image.StartFadeEffect();
            }

            else if (Input.IsActionJustPressed("InvertedDream"))
            {
                GD.Print("Inverting Dream - WIP");
                image.InvertPalette();
                image.StartFadeEffect();
            }
        }

        base._UnhandledInput(inputEvent);
    }
}
