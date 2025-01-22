namespace DreamScape.Img;

using Godot;
using System;
using System.Collections.Generic;

/// <summary>
<<<<<<< Updated upstream:src/img_generation/ImgGenerator.cs
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
=======
>>>>>>> Stashed changes:src/img/DreamScapeImg.cs
/// uses the FastNoiseLite library to generate noise that is the n used to
/// create an abstract img
/// </summary>
public partial class ImgGenerator : Node2D
{
    public const int ColorPaletteSize = 5;
    private static ImgGenerator instance;

    /// <summary>
    /// instance of the object
    /// </summary>
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
    private Vector2 imgFadeOrigins;

    private ImgGenerator(Vector2 ImgSize)
    {
        Name = "DreamScapeImg";
        this.ImgSize = ImgSize;

        tiles = new Dictionary<Vector2, Tile>();
        colorPalette = new Color[ColorPaletteSize];
        noiseGenerator = new FastNoiseLite();

        //centering the img
        Position = new Vector2(
<<<<<<< Updated upstream:src/img_generation/ImgGenerator.cs
            (-ImgSize.X * ImgConstants.CellSize / 2) + (ImgConstants.CellSize * .5f),
            (-ImgSize.Y * ImgConstants.CellSize / 2) + (ImgConstants.CellSize * .5f)
        );
        imgFadeOrigins = new Vector2(-1, -1);
        State = ImgGeneratorState.New;
=======
                    (-ImgSize.X * ImgConstants.CellSize / 2) + (ImgConstants.CellSize * .5f),
                    (-ImgSize.Y * ImgConstants.CellSize / 2) + (ImgConstants.CellSize * .5f)
                );

        DreamScapeState = DreamScapeStates.Instance;
>>>>>>> Stashed changes:src/img/DreamScapeImg.cs
    }

    /// <summary>
    /// The State The img generator is in
    /// </summary>
    public DreamScapeStates DreamScapeState { get; private set; }

    /// <summary>
    /// the noise generator used to create the img
    /// </summary>
    public FastNoiseLite NoiseGenerator { get => noiseGenerator; set => noiseGenerator = value; }

    /// <summary>
    /// determines how many tile will be used to make the width 
    /// and height of  the dream scape img
    /// </summary>
<<<<<<< Updated upstream:src/img_generation/ImgGenerator.cs
    public Vector2 ImgSize { get; set; }

 
=======
    public Vector2 ImgSize { get; private set; }
>>>>>>> Stashed changes:src/img/DreamScapeImg.cs

    /// <summary>
    /// generates a dream scape img form the noise map and other settings
    /// </summary>
    public void Generate()
    {
        if (DreamScapeState.ImgState == ImgState.New)
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
            DreamScapeState.ImgState = ImgState.Idle;
        }

        else if (DreamScapeState.ImgState == ImgState.Idle)
        {
            GD.Print("Regenerating");
            for (int x = 0; x < ImgSize.X; x++)
            {
                for (int y = 0; y < ImgSize.Y; y++)
                {
                    Vector2 cords = new Vector2(x, y);
                    Color tileColor = GetColorTileColor(noiseGenerator.GetNoise2D(x, y));
                    tiles[cords].SetNextColor(tileColor);
                }
            }
            tiles[new Vector2(0, 0)].BlendColorWithNeighbors();
<<<<<<< Updated upstream:src/img_generation/ImgGenerator.cs
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
                // tiles[cord].FadeTile(TileStateOld.FadingIn);
            }
            else
            {
                GD.PrintErr($"Tile at {cord} not found");
            }
=======
            DreamScapeState.ImgState = ImgState.Idle;
>>>>>>> Stashed changes:src/img/DreamScapeImg.cs
        }
    }

    public void FadeImg()
    {
        if (imgFadeOrigins == new Vector2(-1, -1))
        {
            RandomNumberGenerator rng = new RandomNumberGenerator();
            int xCoord = rng.RandiRange(0, (int)ImgSize.X - 1);
            int yCoord = rng.RandiRange(0, (int)ImgSize.Y - 1);
            imgFadeOrigins = new Vector2(xCoord, yCoord);
        }
    }

    /// <summary>
    /// fades out the img
    /// </summary>
    public void FadeOutImg()
    {
        if (imgFadeOrigins == new Vector2(-1, -1))
        {
            RandomNumberGenerator rng = new RandomNumberGenerator();
            int xCoord = rng.RandiRange(0, (int)ImgSize.X - 1);
            int yCoord = rng.RandiRange(0, (int)ImgSize.Y - 1);
            imgFadeOrigins = new Vector2(xCoord, yCoord);
        }
    }

    /// <summary>
    /// gets tile color from current value in noise generator
    /// </summary>
    private Color GetColorTileColor(float height)
    {
        // if (height < -0.75)
        // {
        //     return colorPalette[0];
        // }
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

        else
        {
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