namespace DreamScape;

using DreamScape.ImgGeneration;
using Godot;
using System;
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
        {"PlayAmbientMusic",new InputEventMouseButton { ButtonIndex = MouseButton.Left}},
        {"RandomizeSeed", new InputEventKey{PhysicalKeycode = Key.Down}},
        {"RandomFrequency",  new InputEventKey{PhysicalKeycode = Key.Up}},
        {"MostRecentDream", new InputEventKey{PhysicalKeycode = Key.Space}},
        {"DarkPalette", new InputEventKey{PhysicalKeycode = Key.Right}},
        {"LightPalette",  new InputEventKey{PhysicalKeycode = Key.Left}},
    };

    private ImgGenerator imgGenerator;
    private RandomNumberGenerator rng;

    public DreamScapeMain()
    {
        imgGenerator = new ImgGenerator(new Vector2(64 * 2, 64));
        rng = new RandomNumberGenerator();

        //setting initial values for noise generator
        var NoiseGenerator = imgGenerator.NoiseGenerator;
        NoiseGenerator.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
        NoiseGenerator.Seed = rng.RandiRange(0, 10000);
        NoiseGenerator.Frequency = 0.05f;
        NoiseGenerator.FractalLacunarity = 2.5f;
        NoiseGenerator.FractalOctaves = 5;
        NoiseGenerator.FractalGain = 0.5f;

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
            GD.Print("Playing Ambient Music");
        }
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if (!imgGenerator.Fading && !imgGenerator.Generating)
        {
            if (Input.IsActionJustPressed("RandomizeSeed"))
            {
                imgGenerator.NoiseGenerator.Seed = rng.RandiRange(0, 10000);
                GD.Print("New Seed");
                imgGenerator.Clear();
                imgGenerator.Generate();
                imgGenerator.StartFadeEffect();
            }

            if (Input.IsActionJustPressed("RandomFrequency"))
            {
                GD.Print("Random Frequency");
                imgGenerator.Clear();
                imgGenerator.Generate();
                imgGenerator.StartFadeEffect();
            }

            if (Input.IsActionJustPressed("MostRecentDream"))
            {
                imgGenerator.Generate();//if generated will just fade
                imgGenerator.StartFadeEffect();
            }

            if (Input.IsActionJustPressed("DarkPalette"))
            {
                GD.Print("Swapping To Dark Palette");
                imgGenerator.Clear();
                imgGenerator.Generate();
                imgGenerator.StartFadeEffect();
            }

            if (Input.IsActionJustPressed("LightPalette"))
            {
                GD.Print("Swapping To Light Palette");
                imgGenerator.Clear();
                imgGenerator.Generate();
                imgGenerator.StartFadeEffect();
            }
        }

        base._UnhandledInput(inputEvent);
    }


}