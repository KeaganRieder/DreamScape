namespace DreamScape;

using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// uses the FastNoiseLite library to generate noise that is the n used to
/// create an abstract img
/// </summary>
public partial class DreamScapeImg : Node2D
{
    public const int ColorPaletteSize = 7;
    private static DreamScapeImg instance;

    /// <summary>
    /// instance of the object
    /// </summary>
    public static DreamScapeImg Instance
    {
        get
        {
            instance ??= new DreamScapeImg(new Vector2(ImgConstants.ImgWidth, ImgConstants.imgHeight));
            return instance;
        }
    }

    private FastNoiseLite noiseGenerator;

    private Color[] colorPalette;
    private readonly Dictionary<Vector2, Tile> tiles;
    private Vector2 imgFadeOrigins;

    private DreamScapeImg(Vector2 ImgSize)
    {
        Name = "DreamScapeImg";
        this.ImgSize = ImgSize;

        tiles = new Dictionary<Vector2, Tile>();
        colorPalette = new Color[ColorPaletteSize];
        noiseGenerator = new FastNoiseLite();

        //centering the img
        Position = new Vector2(
            (-ImgSize.X * ImgConstants.CellSize / 2) + (ImgConstants.CellSize * .5f),
            (-ImgSize.Y * ImgConstants.CellSize / 2) + (ImgConstants.CellSize * .5f)
        );
        State = States.Instance;
    }

    /// <summary>
    /// The State The img generator is in
    /// </summary>
    public States State { get; private set; }

    /// <summary>
    /// the noise generator used to create the img
    /// </summary>
    public FastNoiseLite NoiseGenerator { get => noiseGenerator; set => noiseGenerator = value; }

    /// <summary>
    /// determines how many tile will be used to make the width 
    /// and height of  the dream scape img
    /// </summary>
    public Vector2 ImgSize { get; set; }

    /// <summary>
    /// generates a dream scape img form the noise map and other settings
    /// </summary>
    public void Generate()
    {
        if (State.ImgState == ImgState.New)
        {
            GD.Print("Generating");
            RandomizePalette();
            for (int x = 0; x < ImgSize.X; x++)
            {
                for (int y = 0; y < ImgSize.Y; y++)
                {
                    Vector2 cords = new Vector2(x, y);
                    Vector2 tileCords = cords * ImgConstants.CellSize;

                    Tile tile = new Tile(tileCords, GetColorTileColor(noiseGenerator.GetNoise2D(x, y)));

                    tile.FindNeighbors(tiles);
                    tiles.Add(cords, tile);

                    AddChild(tile);
                }
            }
            tiles[new Vector2(0, 0)].BlendColorWithNeighbors();
            State.ImgState = ImgState.Idle;
        }

        else if (State.ImgState == ImgState.Idle)
        {
            GD.Print("Regenerating");
            for (int x = 0; x < ImgSize.X; x++)
            {
                for (int y = 0; y < ImgSize.Y; y++)
                {
                    Vector2 cords = new Vector2(x, y);
                    tiles[cords].TileColor = GetColorTileColor(noiseGenerator.GetNoise2D(x, y));
                }
            }
            tiles[new Vector2(0, 0)].BlendColorWithNeighbors();
            State.ImgState = ImgState.Idle;
        }
    }

    /// <summary>
    /// starts the fade effect on a random tile
    /// </summary>
    public void StartFadeEffect()
    {
        if (State.ImgState == ImgState.Idle)
        {
            GD.Print("Starting Fade Effect");
            State.ImgState = ImgState.Fading;
            RandomNumberGenerator rng = new RandomNumberGenerator();
            int xCoord = rng.RandiRange(0, (int)ImgSize.X - 1);
            int yCoord = rng.RandiRange(0, (int)ImgSize.Y - 1);
            Vector2 cord = new Vector2(xCoord, yCoord);

            if (tiles.ContainsKey(cord))
            {
                if (tiles[cord].TileColor.A == 1 && State.FadeMode == FadeType.DefaultFade)
                {
                    tiles[cord].StartFadeEffect(TileFadeType.FadeOut);
                }
                if (tiles[cord].TileColor.A == 1 && State.FadeMode == FadeType.CycleFade)
                {
                    tiles[cord].StartFadeEffect(TileFadeType.FadeToNextColor);
                }
                else
                {
                    tiles[cord].StartFadeEffect(TileFadeType.FadeIn);
                }

                State.TilesDoneFading = 1;
                State.TilesFading = tiles.Count;
            }
            else
            {
                GD.PrintErr($"Tile at {cord} not found");
            }
        }
    }

    /// <summary>
    /// gets tile color from current value in noise generator
    /// </summary>
    private Color GetColorTileColor(float height)
    {
        if (height < -0.5)
        {
            return colorPalette[0];
        }

        else if (height < -0.25)
        {
            return colorPalette[1];
        }

        else if (height < 0)
        {
            return colorPalette[2];
        }

        else if (height < 0.25)
        {
            return colorPalette[3];
        }

        else if (height < 0.5)
        {
            return colorPalette[4];
        }
        else if (height < 0.75)
        {
            return colorPalette[5];
        }
        else if (height < 1)
        {
            return colorPalette[6];
        }
        else
        {
            GD.Print(height);
            return new Color(height, height, height, 0);
        }
    }

    /// <summary>
    /// randomizes the color palate of the img
    /// </summary>
    public void RandomizePalette()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        colorPalette = new Color[ColorPaletteSize];

        for (int i = 0; i < ColorPaletteSize; i++)
        {
            int colorIndex = rng.RandiRange(0, ImgConstants.Colors.Length - 1);
            Color color = new(ImgConstants.Colors[colorIndex]);
            color.A = 0;
            colorPalette[i] = color;
        }
    }

    /// <summary>
    /// flips the color palette (invert it)
    /// </summary>
    public void InvertPalette()
    {
        Color[] oldPalette = colorPalette;

        colorPalette = new Color[ColorPaletteSize];

        for (int i = 0; i < ColorPaletteSize; i++)
        {
            colorPalette[i] = oldPalette[ColorPaletteSize - 1 - i];
        }
    }
}