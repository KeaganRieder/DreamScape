namespace DreamScape.Img;

using Godot;
using System;
using System.Collections.Generic;
using System.IO;

<<<<<<< Updated upstream:src/img_generation/Tile.cs
/// <summary>
/// used to check the tiles state
/// </summary>
// public enum TileStateOld
// {
//     Undefined = 0,
//     Idle = 1,
//     FadingIn = 2,
//     FadingToNextColor = 3,
//     FadingOut = 4,
//     FadingDone = 5,
//     FadeInByNeighbors = 6,
//     FadeOutByNeighbors = 7,
// }

=======
>>>>>>> Stashed changes:src/img/Tile.cs
/// <summary>
/// used to check the tiles state
/// </summary>
public enum TileState
{
    Undefined = 0,
    Idle = 1,
<<<<<<< Updated upstream:src/img_generation/Tile.cs
    FadingIn = 2,
    FadeToNextColor = 3,
    FadingOut = 4,
}


=======
    Fading = 2,
}

/// <summary>
/// a tile is a pixel on the img
/// </summary>
>>>>>>> Stashed changes:src/img/Tile.cs
public partial class Tile : Sprite2D
{
    private Vector2I globalPosition;
    private Dictionary<Vector2, Tile> neighbors;

    private ImgGenerator imgGenerator;

    private Vector2I graphicSize;
<<<<<<< Updated upstream:src/img_generation/Tile.cs

    private Color fadedTileColor;
    private Color currentTileColor;
    private Color newTileColor;
=======
    private Color currentColor;
    private Color newColor;
    private Color tileHiddenColor;

    private DreamScapeStates imgStates;
>>>>>>> Stashed changes:src/img/Tile.cs

    public Tile(ImgGenerator imgGenerator, Vector2 position, Color color)
    {
        Name = $"Tile {position}";
<<<<<<< Updated upstream:src/img_generation/Tile.cs
        State = TileState.Idle;

=======
        this.imgGenerator = imgGenerator;
        imgStates = DreamScapeStates.Instance;
>>>>>>> Stashed changes:src/img/Tile.cs
        graphicSize = new Vector2I(ImgConstants.CellSize, ImgConstants.CellSize);
        State = TileState.Idle;

        globalPosition = new Vector2I(Mathf.FloorToInt(position.X / graphicSize.X), Mathf.FloorToInt(position.Y / graphicSize.Y));
        Position = position;
<<<<<<< Updated upstream:src/img_generation/Tile.cs
        this.imgGenerator = imgGenerator;

        TileColor = color;
        fadedTileColor = Colors.Black;
=======
>>>>>>> Stashed changes:src/img/Tile.cs

        neighbors = new Dictionary<Vector2, Tile>();

        tileHiddenColor = Colors.Black;
        color.A = 0;
        currentColor = color;
        newColor = color;
        Modulate = currentColor;
    }

    /// <summary>
    /// the state of the tile
    /// </summary>
    public TileState State { get; set; }

<<<<<<< Updated upstream:src/img_generation/Tile.cs
    /// <summary>
    /// the color of the tile
    /// </summary>
    public Color TileColor
    {
        get => currentTileColor;
        private set
        {
            ColorNeedingBlended = true;
            currentTileColor = value;
=======
    public Color TileColor
    {
        get => currentColor;

        set
        {
            newColor = value;
>>>>>>> Stashed changes:src/img/Tile.cs
        }
    }

    /// <summary>
<<<<<<< Updated upstream:src/img_generation/Tile.cs
    /// used to track wether or not tiles colors needs to be blended
    /// with it's neighbors
    /// </summary>
    public bool ColorNeedingBlended { get; private set; }

    /// <summary>
    /// used to track if the tile fade was started by a neighbor
    /// </summary>
    public bool FadeStartedByNeighbor { get; private set; }

=======
    /// does the tile need to be blended with it's neighbors
    /// colors
    /// </summary>
    public bool ColorNeedingBlended { get; private set; }

>>>>>>> Stashed changes:src/img/Tile.cs
    /// <summary>
    /// the timer used to delay the fade in or out of the tile
    /// </summary>
    public Timer TileDelayTimer { get; private set; }

    public override void _Ready()
    {
        if (File.Exists(ImgConstants.IMG_PATH))
        {
            Image image = Image.LoadFromFile(ImgConstants.IMG_PATH);

            if (image.GetSize() != graphicSize)
            {
                image.Resize(graphicSize.X, graphicSize.Y, Image.Interpolation.Bilinear);
            }
            Texture2D texture = ImageTexture.CreateFromImage(image);

            Texture = texture;
        }

        else
        {
            GD.Print("File Doesn't exists");
        }

        TileDelayTimer = new Timer();
        AddChild(TileDelayTimer);
<<<<<<< Updated upstream:src/img_generation/Tile.cs
        TileDelayTimer.Timeout += () => FadeTileOld(State);

        State = TileStateOld.Idle;
        base._Ready();
    }

    /// <summary>
    /// sets the next color the tile will fade into
    /// </summary>
    public void SetNextColor(Color color)
    {
        if (newTileColor == color)
        {
            return;
        }
        newTileColor = color;
=======
        TileDelayTimer.Timeout += () => FadeTile();

        State = TileState.Idle;
        base._Ready();
>>>>>>> Stashed changes:src/img/Tile.cs
    }

    /// <summary>
    /// Blends the color of the tile with it's neighbors
    /// </summary>
    public void BlendColorWithNeighbors()
    {
<<<<<<< Updated upstream:src/img_generation/Tile.cs
        ColorNeedingBlended = false;
=======
        //todo
        // ColorNeedingBlended = false;
>>>>>>> Stashed changes:src/img/Tile.cs

        // Color blendedColor = TileColor;
        // blendedColor.A = 0.5f;

        // foreach (var neighbor in neighbors.Values)
        // {
        //     Color colorToBlend = neighbor.TileColor;
        //     colorToBlend.A = 0.5f;

        //     if (blendedColor != colorToBlend)
        //     {
        //         blendedColor = blendedColor.Blend(colorToBlend);
        //     }

<<<<<<< Updated upstream:src/img_generation/Tile.cs
            if (neighbor.ColorNeedingBlended)
            {
                neighbor.BlendColorWithNeighbors();
            }
        }

        blendedColor.A = 0;
        TileColor = blendedColor;
=======
        //     if (neighbor.State == TileState.NeedingColorBlended)
        //     {
        //         neighbor.BlendColorWithNeighbors();
        //     }
        // }

        // blendedColor.A = 0;
        // TileColor = blendedColor;
        // State = TileState.Idle; // ensure it's marked as idle
>>>>>>> Stashed changes:src/img/Tile.cs
    }

    /// <summary>
    /// runs through provided tile map and updates tiles neighbors
    /// </summary>
    public void FindNeighbors(Dictionary<Vector2, Tile> tileMap)
    {
        if (tileMap == null)
        {
            throw new ArgumentNullException(nameof(tileMap));
        }

        Vector2[] neighborCoords = new Vector2[]
        {
            new (globalPosition.X - 1, globalPosition.Y),
            new (globalPosition.X + 1, globalPosition.Y),
            new (globalPosition.X, globalPosition.Y - 1),
            new (globalPosition.X, globalPosition.Y + 1),
        };

        foreach (var coord in neighborCoords)
        {
            if (tileMap.TryGetValue(coord, out Tile tile))
            {
                SetNeighbor(coord, tile);
                tile.SetNeighbor(globalPosition, this);
            }
        }
    }

    /// <summary>
    /// sets neighbor of the tile at the given coords
    /// </summary>
    public void SetNeighbor(Vector2 coords, Tile tile)
    {
        if (!neighbors.ContainsKey(coords))
        {
            neighbors.Add(coords, tile);
        }
    }

    /// <summary>
    /// begins the tiles and it's neighbors fade in or out effect
    /// </summary>
<<<<<<< Updated upstream:src/img_generation/Tile.cs
    public void FadeTileOld(TileState tileState)
=======
    public void FadeTile()
>>>>>>> Stashed changes:src/img/Tile.cs
    {
        imgStates.ImgState = ImgState.Fading;

        if (!TileDelayTimer.IsStopped())
        {
            TileDelayTimer.Stop();
        }

<<<<<<< Updated upstream:src/img_generation/Tile.cs
        if (ColorNeedingBlended)
=======
        if (State == TileState.Idle)
>>>>>>> Stashed changes:src/img/Tile.cs
        {
            State = TileState.Fading;
            StartNeighbors();
        }

<<<<<<< Updated upstream:src/img_generation/Tile.cs
        switch (tileState)
        {
            case TileState.FadingIn:
                FadeIn();
                break;

            case TileState.FadeToNextColor:
                FadeToNextColor();
                break;

            case TileState.FadingOut:
                FadeOut();
                break;
            case TileState.Idle:
                TileDelayTimer.Stop();
                State = TileState.Idle;
                imgGenerator.State = ImgGeneratorState.Idle;
                break;

            default:
                GD.PrintErr($"Invalid State: {tileState} for tile {globalPosition}");
                break;
=======
        if (State == TileState.Fading)
        {
            switch (imgStates.FadeState)
            {
                case FadeState.FadeIn:
                    FadeIn();
                    break;

                case FadeState.FadeToNextColor:
                    FadeToNextColor();
                    break;

                case FadeState.FadeOut:
                    FadeOut();
                    break;

                default:
                    GD.PrintErr($"Invalid State: {imgStates.FadeState} for tile {globalPosition}");
                    break;
            }
>>>>>>> Stashed changes:src/img/Tile.cs
        }

        // switch (state)
        // {
        //     case TileState.FadingIn:
        //     case TileState.FadeInByNeighbors:
        //         if (State == TileState.Idle || State == TileState.FadeInByNeighbors)
        //         {
        //             State = TileState.FadingIn;
        //             StartNeighbors();
        //         }
        //         imgGenerator.State = ImgGeneratorState.Fading;
        //         FadeIn();
        //         break;

        //     case TileState.FadingOut:
        //     case TileState.FadeOutByNeighbors:
        //         if (State == TileState.Idle || State == TileState.FadeOutByNeighbors)
        //         {
        //             State = TileState.FadingOut;
        //             StartNeighbors();
        //         }
        //         imgGenerator.State = ImgGeneratorState.Fading;
        //         FadeOut();
        //         break;
        //     case TileState.Idle:
        //     case TileState.FadingDone:
        //         TileDelayTimer.Stop();
        //         State = TileState.Idle;
        //         imgGenerator.State = ImgGeneratorState.Idle;
        //         break;

        //     
        // }
    }

    /// <summary>
    /// begins the tiles and it's neighbors fade in or out effect
    /// </summary>
    public void FadeTile()
    {
        // if (!TileDelayTimer.IsStopped())
        // {
        //     //make sure when called the timer is stopped
        //     TileDelayTimer.Stop();
        // }

        if (ColorNeedingBlended)
        {
            BlendColorWithNeighbors();
        }
    }

    /// <summary>
    /// starts the fade effect on the neighbors of the tile after a delay
    /// </summary>
    private void StartNeighbors()
    {
        foreach (var neighbor in neighbors.Values)
        {
            if (neighbor.State == TileState.Idle)
            {
                if (neighbor.TileDelayTimer.IsStopped())
                {
                    neighbor.TileDelayTimer.Start(ImgConstants.NeighborDelay);
                }
            }

            // if (neighbor.State == TileStateOld.Idle)
            // {
            //     if (State == TileStateOld.FadingIn)
            //     {
            //         neighbor.State = TileStateOld.FadeInByNeighbors;
            //     }
            //     else if (State == TileStateOld.FadingOut)
            //     {
            //         neighbor.State = TileStateOld.FadeOutByNeighbors;
            //     }

            //     if (neighbor.TileDelayTimer.IsStopped())
            //     {
            //         neighbor.TileDelayTimer.Start(ImgConstants.NeighborDelay);
            //     }
            // }
        }
    }

    private void FadeIn()
    {
<<<<<<< Updated upstream:src/img_generation/Tile.cs
        // currentTileColor.A += ImgConstants.FadeInAmt;

        // if (currentTileColor.A >= 1)
        // {
        //     currentTileColor.A = 1;

        //     State = TileState.FadingOut;
        //     TileDelayTimer.Stop();
        //     TileDelayTimer.Start(ImgConstants.VisibleTime);
        // }

        // else
        // {
        //     if (TileDelayTimer.IsStopped())
        //     {
        //         TileDelayTimer.Start(ImgConstants.FadeInDelay);
        //     }
        // }
        // Modulate = currentTileColor;
    }

    /// <summary>
    /// fades the tile to the next color
    /// </summary>
    private void FadeToNextColor()
    {
        // currentTileColor.A -= ImgConstants.FadeOutAmt;

        // if (currentTileColor.A <= 0)
        // {
        //     // GD.Print("Now Idle");

        //     currentTileColor.A = 0;
        //     TileDelayTimer.Stop();

        //     FadeTile(TileState.FadingDone);
        // }
        // else
        // {
        //     if (TileDelayTimer.IsStopped())
        //     {
        //         TileDelayTimer.Start(ImgConstants.FadeOutDelay);
        //     }
        // }
        // Modulate = currentTileColor;
=======
        currentColor.A += ImgConstants.FadeInAmt;

        if (currentColor.A >= 1)
        {
            currentColor.A = 1;

            State = TileState.Idle;
            imgStates.ImgState = ImgState.Idle;

            // not fading in so make it fade to next color
            imgStates.FadeState = FadeState.FadeToNextColor;

            TileDelayTimer.Stop();
        }

        else
        {
            // if still fading in track that
            imgStates.FadeState = FadeState.FadeIn;

            if (TileDelayTimer.IsStopped())
            {
                TileDelayTimer.Start(ImgConstants.FadeDelay);
            }
        }
        Modulate = currentColor;
    }

    private void FadeToNextColor()
    {
        currentColor.Lerp(newColor, ImgConstants.FadeInAmt);

        if (currentColor == newColor)
        {
            State = TileState.Idle;
            imgStates.ImgState = ImgState.Idle;



            TileDelayTimer.Stop();
        }
        else
        {
            if (TileDelayTimer.IsStopped())
            {
                TileDelayTimer.Start(ImgConstants.FadeDelay);
            }
        }

        Modulate = currentColor;
>>>>>>> Stashed changes:src/img/Tile.cs
    }

    private void FadeOut()
    {
<<<<<<< Updated upstream:src/img_generation/Tile.cs
        // currentTileColor.A -= ImgConstants.FadeOutAmt;

        // if (currentTileColor.A <= 0)
        // {
        //     currentTileColor.A = 0;
        //     TileDelayTimer.Stop();

        //     FadeTile(TileState.FadingDone);
        // }
        // else
        // {
        //     if (TileDelayTimer.IsStopped())
        //     {
        //         TileDelayTimer.Start(ImgConstants.FadeOutDelay);
        //     }
        // }
        // Modulate = currentTileColor;
=======
        currentColor.A -= ImgConstants.FadeOutAmt;

        if (currentColor.A <= 0)
        {
            currentColor.A = 0;

            State = TileState.Idle;
            imgStates.ImgState = ImgState.Idle;

            // done fading in so make fade in
            imgStates.FadeState = FadeState.FadeIn;

            TileDelayTimer.Stop();
        }

        else
        {
            imgStates.FadeState = FadeState.FadeOut;

            if (TileDelayTimer.IsStopped())
            {
                TileDelayTimer.Start(ImgConstants.FadeOutDelay);
            }
        }
        Modulate = currentColor;
>>>>>>> Stashed changes:src/img/Tile.cs
    }
}