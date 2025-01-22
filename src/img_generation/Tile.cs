namespace DreamScape.ImgGeneration;

using Godot;
using System;
using System.Collections.Generic;
using System.IO;

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

/// <summary>
/// used to check the tiles state
/// </summary>
public enum TileState
{
    Undefined = 0,
    Idle = 1,
    FadingIn = 2,
    FadeToNextColor = 3,
    FadingOut = 4,
}


public partial class Tile : Sprite2D
{
    private Vector2I globalPosition;
    private Dictionary<Vector2, Tile> neighbors;

    private ImgGenerator imgGenerator;

    private Vector2I graphicSize;

    private Color fadedTileColor;
    private Color currentTileColor;
    private Color newTileColor;

    public Tile(ImgGenerator imgGenerator, Vector2 position, Color color)
    {
        Name = $"Tile {position}";
        State = TileState.Idle;

        graphicSize = new Vector2I(ImgConstants.CellSize, ImgConstants.CellSize);

        globalPosition = new Vector2I(Mathf.FloorToInt(position.X / graphicSize.X), Mathf.FloorToInt(position.Y / graphicSize.Y));
        Position = position;
        this.imgGenerator = imgGenerator;

        TileColor = color;
        fadedTileColor = Colors.Black;

        neighbors = new Dictionary<Vector2, Tile>();
    }

    /// <summary>
    /// the state of the tile
    /// </summary>
    public TileState State { get; set; }

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
        }
    }

    /// <summary>
    /// used to track wether or not tiles colors needs to be blended
    /// with it's neighbors
    /// </summary>
    public bool ColorNeedingBlended { get; private set; }

    /// <summary>
    /// used to track if the tile fade was started by a neighbor
    /// </summary>
    public bool FadeStartedByNeighbor { get; private set; }

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
    }

    /// <summary>
    /// Blends the color of the tile with it's neighbors
    /// </summary>
    public void BlendColorWithNeighbors()
    {
        ColorNeedingBlended = false;

        Color blendedColor = TileColor;
        blendedColor.A = 0.5f;

        foreach (var neighbor in neighbors.Values)
        {
            Color colorToBlend = neighbor.TileColor;
            colorToBlend.A = 0.5f;

            if (blendedColor != colorToBlend)
            {
                blendedColor = blendedColor.Blend(colorToBlend);
            }

            if (neighbor.ColorNeedingBlended)
            {
                neighbor.BlendColorWithNeighbors();
            }
        }

        blendedColor.A = 0;
        TileColor = blendedColor;
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
    public void FadeTileOld(TileState tileState)
    {
        if (!TileDelayTimer.IsStopped())
        {
            TileDelayTimer.Stop();
        }

        if (ColorNeedingBlended)
        {
            BlendColorWithNeighbors();
        }

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

    /// <summary>
    /// fades the tile in
    /// </summary>
    private void FadeIn()
    {
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
    }

    /// <summary>
    /// fades the tile out
    /// </summary>
    private void FadeOut()
    {
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
    }

}

