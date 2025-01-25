namespace DreamScape.img;

using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public enum TileState
{
    Undefine = 0,
    Idle = 1,
    Fading = 2,
    StartedByNeighbors = 3,
}


public enum TileFadeType
{
    Undefine = 0,
    FadeIn = 1,
    FadeOut = 2,
    FadeToNextColor = 3,
}

/// <summary>
/// a tile is a pixel on the img
/// </summary>
public partial class Tile : Sprite2D
{
    private Vector2I globalPosition;
    private Dictionary<Vector2, Tile> neighbors;

    private Vector2I graphicSize;

    private Color currentColor;
    private Color nextColor;
    private float nextColorFadeAmount;

    public Tile(Vector2 position, Color color)
    {
        Name = $"Tile {position}";
        // this.img = img;
        graphicSize = new Vector2I(ImgConstants.CellSize, ImgConstants.CellSize);

        State = TileState.Idle;

        globalPosition = new Vector2I(Mathf.FloorToInt(position.X / graphicSize.X), Mathf.FloorToInt(position.Y / graphicSize.Y));
        Position = position;

        neighbors = new Dictionary<Vector2, Tile>();

        ColorNeedingBlended = true;
        IsNewTile = true;

        color.A = 0;
        currentColor = color;
        nextColor = color;
        nextColorFadeAmount = 1;
        Modulate = currentColor;
    }

    /// <summary>
    /// the state of the tile
    /// </summary>
    public TileState State { get; set; }

    public TileFadeType TileFadeType { get; private set; }

    /// <summary>
    /// the color of the tile
    /// </summary>
    public Color TileColor
    {
        get => currentColor;

        set
        {
            if (States.Instance.FadeMode == FadeType.CycleFade)
            {
                ColorNeedingBlended = false;
                value.A = .5f;
                nextColor = value;
                nextColorFadeAmount = 0;
            }
            else
            {
                currentColor = value;
                // Modulate = currentColor;
            }
        }
    }

    /// <summary>
    /// used to track wether or not tiles colors needs to be blended
    /// with it's neighbors
    /// </summary>
    public bool ColorNeedingBlended { get; private set; }

    /// <summary>
    /// used to see which color the of the tile gets blended 
    /// </summary>
    public bool IsNewTile { get; private set; }

    /// <summary>
    /// used to track if the tile fade was started by a neighbor
    /// </summary>
    public bool FadeStartedByNeighbor { get; private set; }

    /// <summary>
    /// the timer used to delay the fade in or out of the tile
    /// </summary>
    public Timer TileDelayTimer { get; private set; }

    /// <summary>
    /// the timer used to delay the fade in or out of the tile
    /// </summary>
    // public Timer NeighborDelayTimer { get; private set; }
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
        TileDelayTimer.Timeout += () => FadeTile();

        base._Ready();
    }

    /// <summary>
    /// Blends the color of the tile with it's neighbors
    /// </summary>
    public void BlendColorWithNeighbors()
    {
        // if (ColorNeedingBlended)
        // {
        //     ColorNeedingBlended = false;

        //     if (IsNewTile)
        //     {
        //         Color blendedColor = currentColor;
        //         blendedColor.A = 0.5f;
        //         foreach (var neighbor in neighbors.Values)
        //         {
        //             if (neighbor.ColorNeedingBlended)
        //             {
        //                 neighbor.BlendColorWithNeighbors();
        //             }

        //             Color colorToBlend = neighbor.currentColor;
        //             colorToBlend.A = 0.5f;

        //             if (blendedColor != colorToBlend)
        //             {
        //                 blendedColor = blendedColor.Blend(colorToBlend);
        //             }
        //         }
        //         currentColor = blendedColor;
        //         IsNewTile = false;
        //     }

        //     else
        //     {
        //         // Color blendedColor = newColor;
        //         // blendedColor.A = 0.5f;
        //         // foreach (var neighbor in neighbors.Values)
        //         // {
        //         //     if (neighbor.ColorNeedingBlended)
        //         //     {
        //         //         neighbor.BlendColorWithNeighbors();
        //         //     }

        //         //     Color colorToBlend = neighbor.newColor;
        //         //     colorToBlend.A = 0.5f;

        //         //     if (blendedColor != colorToBlend)
        //         //     {
        //         //         blendedColor = blendedColor.Blend(colorToBlend);
        //         //     }
        //         // }
        //         // newColor = blendedColor;
        //     }
        // }
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
    /// starts the neighbors fade effect
    /// </summary>
    private void StartNeighbors()
    {
        foreach (var neighbor in neighbors.Values)
        {
            if (neighbor.State == TileState.Idle && neighbor.TileDelayTimer.IsStopped())
            {
                neighbor.State = TileState.StartedByNeighbors;
                neighbor.TileFadeType = TileFadeType;
                neighbor.TileDelayTimer.Start(ImgConstants.NeighborDelay);
            }
        }
    }

    /// <summary>
    /// begins the tiles and it's neighbors fade in or out effect
    /// </summary>
    public void StartFadeEffect(TileFadeType fadeType)
    {
        if (State == TileState.Idle)
        {
            TileFadeType = fadeType;
            State = TileState.Fading;
            StartNeighbors();
            FadeTile();
        }
    }

    /// <summary>
    /// decides the fade effect of the tile
    /// </summary>
    private void FadeTile()
    {
        if (State == TileState.Idle)
        {
            if (!TileDelayTimer.IsStopped())
            {
                TileDelayTimer.Stop();
            }
            States.Instance.TilesDoneFading++;
        }

        else if (State == TileState.StartedByNeighbors)
        {
            State = TileState.Fading;
            StartNeighbors();
        }

        else if (State == TileState.Fading)
        {
            if (TileFadeType == TileFadeType.FadeIn)
            {
                FadeIn();
            }

            else if (TileFadeType == TileFadeType.FadeOut)
            {
                FadeOut();
            }

            else if (TileFadeType == TileFadeType.FadeToNextColor)
            {
                FadeToNextColor();
            }
        }
    }

    private void FadeIn()
    {
        currentColor.A += ImgConstants.FadeInAmt;
        if (currentColor.A >= 1)
        {
            currentColor.A = 1;
            if (States.Instance.FadeMode == FadeType.DefaultFade)
            {
                TileFadeType = TileFadeType.FadeOut;
                State = TileState.StartedByNeighbors;
            }
            else
            {
                State = TileState.Idle;
            }
        }
        else
        {
            if (TileDelayTimer.IsStopped())
            {
                TileDelayTimer.Start(ImgConstants.FadeDelay);
            }
        }
        Modulate = currentColor;
    }

    private void FadeOut()
    {
        currentColor.A -= ImgConstants.FadeOutAmt;

        if (currentColor.A <= 0)
        {
            currentColor.A = 0;

            State = TileState.Idle;
        }
        else
        {
            if (TileDelayTimer.IsStopped())
            {
                TileDelayTimer.Start(ImgConstants.FadeOutDelay);
            }
        }
        Modulate = currentColor;

    }

    private void FadeToNextColor()
    {
        // currentColor.Blend(newColor);
        Modulate = currentColor.Blend(nextColor);
        State = TileState.Idle;

        //         nextColorFadeAmount += ImgConstants.FadeInAmt;

        //  nextColorFadeAmount = 1;
        // if (nextColorFadeAmount >= 1)
        // {

        // }
        // else
        // {
        //     if (TileDelayTimer.IsStopped())
        //     {
        //         TileDelayTimer.Start(ImgConstants.FadeDelay);
        //     }
        // }

    }

}
