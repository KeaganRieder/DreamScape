namespace DreamScape.ImgGeneration;

using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// an enum that is used to track the img generators state
/// </summary>
public enum ImgGeneratorState
{
    Undefine = 0,
    New = 1, // set when img generator is just made
    Fading = 2, // set when img is fading
    Idle = 3, // set when img is not doing anything
}
/// <summary>
/// uses the FastNoiseLite library to generate noise that is the n used to
/// create an abstract img
/// </summary>
public partial class ImgGenerator : Node2D
{
    private static ImgGenerator instance;
    public static ImgGenerator Instance
    {
        get
        {
            instance ??= new ImgGenerator(new Vector2(ImgConstants.ImgWidth, ImgConstants.imgHeight));
            return instance;
        }
    }

    private FastNoiseLite noiseGenerator;
    private Color[] colorPalette;
    private readonly Dictionary<Vector2, Tile> tiles;

    public ImgGenerator(Vector2 ImgSize)
    {
        Name = "ImgGenerator";
        noiseGenerator = new FastNoiseLite();
        tiles = new Dictionary<Vector2, Tile>();

        this.ImgSize = ImgSize;
        colorPalette = new Color[8];
        RandomizePalette();

        //center img based on size
        Position = new Vector2(
            (-ImgSize.X * ImgConstants.CellSize / 2) + (ImgConstants.CellSize * .5f),
            (-ImgSize.Y * ImgConstants.CellSize / 2) + (ImgConstants.CellSize * .5f)
        );

        State = ImgGeneratorState.New;
    }

    /// <summary>
    /// the state of the img generator, meant to be used to 
    /// see what the img generator is currently doing
    /// </summary>
    public ImgGeneratorState State { get; set; }

    /// <summary>
    /// the noise generator used to create the img
    /// </summary>
    public FastNoiseLite NoiseGenerator { get => noiseGenerator; set => noiseGenerator = value; }

    /// <summary>
    /// determines how many tile will be used to make the width a
    /// nd height of  the dream scape img
    /// </summary>
    public Vector2 ImgSize { get; set; }


    /// <summary>
    /// generates a dream scape img form the noise map and other settings
    /// </summary>
    public void Generate()
    {
        if (State == ImgGeneratorState.New)
        {
            for (int x = 0; x < ImgSize.X; x++)
            {
                for (int y = 0; y < ImgSize.Y; y++)
                {
                    Vector2 cords = new Vector2(x, y);
                    Vector2 tileCords = cords * ImgConstants.CellSize;

                    Color tileColor = GetColorTileColor(noiseGenerator.GetNoise2D(x, y));
                    Tile tile = new Tile(this, tileCords, tileColor);

                    tile.FindNeighbors(tiles);
                    tiles.Add(cords, tile);

                    AddChild(tile);

                }
            }
            tiles[new Vector2(0, 0)].BlendColorWithNeighbors();
            State = ImgGeneratorState.Idle;
        }

        else if (State == ImgGeneratorState.Idle)
        {
            GD.Print("Regenerating");
            for (int x = 0; x < ImgSize.X; x++)
            {
                for (int y = 0; y < ImgSize.Y; y++)
                {
                    Vector2 cords = new Vector2(x, y);
                    Color tileColor = GetColorTileColor(noiseGenerator.GetNoise2D(x, y));
                    tiles[cords].TileColor = tileColor;
                }
            }
            tiles[new Vector2(0, 0)].BlendColorWithNeighbors();
            State = ImgGeneratorState.Idle;
            StartFadeEffect();
        }
    }


    /// <summary>
    /// starts the fade effect on a random tile
    /// </summary>
    public void StartFadeEffect()
    {
        if (State == ImgGeneratorState.Idle)
        {
            State = ImgGeneratorState.Fading;
            GD.Print("starting Fade");

            RandomNumberGenerator rng = new RandomNumberGenerator();
            int xCoord = rng.RandiRange(0, (int)ImgSize.X - 1);
            int yCoord = rng.RandiRange(0, (int)ImgSize.Y - 1);
            Vector2 cord = new Vector2(xCoord, yCoord);

            if (tiles.ContainsKey(cord))
            {
                tiles[cord].FadeTile(TileState.FadingIn);
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
        return height switch
        {
            float h when h < -0.75f => colorPalette[0],
            float h when h < -0.5f => colorPalette[1],
            float h when h < -0.25f => colorPalette[2],
            float h when h < 0.0f => colorPalette[3],
            float h when h < 0.25f => colorPalette[4],
            float h when h < 0.5f => colorPalette[5],
            float h when h < 0.75f => colorPalette[6],
            float h when h <= 1f => colorPalette[7],
            _ => new Color(height, height, height, 0),
        };
    }

    /// <summary>
    /// randomizes the color palate of the img
    /// </summary>
    public void RandomizePalette()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        colorPalette = new Color[8];

        for (int i = 0; i < colorPalette.Length; i++)
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

        colorPalette = new Color[8];

        for (int i = 0; i < colorPalette.Length; i++)
        {
            colorPalette[i] = oldPalette[colorPalette.Length - 1 - i];
        }
    }


}
