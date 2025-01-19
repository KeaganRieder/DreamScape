namespace DreamScape.ImgGeneration;

using Godot;
using System;
using System.Collections.Generic;
using System.IO;


/// <summary>
/// used to check the tiles state
/// </summary>
public enum TileState
{
    Undefined = 0,
    Idle = 1,
    NeedingColorBlended = 2,
    FadingIn = 3,
    FadingOut = 4,
    FadingDone = 5,
    FadeInByNeighbors = 6,
    FadeOutByNeighbors = 7,

}

public partial class Tile : Sprite2D
{
    private Vector2I globalPosition;
    private Dictionary<Vector2, Tile> neighbors;

    private ImgGenerator imgGenerator;

    private Vector2I graphicSize;
    private Color tileColor;

    public Tile(ImgGenerator imgGenerator, Vector2 position, Color color)
    {
        Name = $"Tile {position}";
        State = TileState.Undefined;

        graphicSize = new Vector2I(ImgConstants.CellSize, ImgConstants.CellSize);

        globalPosition = new Vector2I(Mathf.FloorToInt(position.X / graphicSize.X), Mathf.FloorToInt(position.Y / graphicSize.Y));
        Position = position;
        this.imgGenerator = imgGenerator;
        TileColor = color;

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
        get => tileColor; set
        {
            State = TileState.NeedingColorBlended;
            tileColor = value;
            tileColor.A = 0;
            Modulate = tileColor;
        }
    }
    /// <summary>
    /// the timer used to delay the fade in or out of the tile
    /// </summary>
    public Timer TileDelayTimer { get; private set; }

    /// <summary>
    /// runs when node is added to the scene
    /// </summary>
    public override void _Ready()
    {
        ReadTexture();

        TileDelayTimer = new Timer();
        AddChild(TileDelayTimer);
        TileDelayTimer.Timeout += () => FadeTile(State);

        State = TileState.Idle;
        base._Ready();
    }

    /// <summary>
    /// reads and sets texture of the tile form the file location
    /// </summary>
    public void ReadTexture()
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
    }

    /// <summary>
    /// Blends the color of the tile with it's neighbors
    /// </summary>
    public void BlendColorWithNeighbors()
    {
        State = TileState.Idle; // make sure it doesn't get ran again

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

            if (neighbor.State == TileState.NeedingColorBlended)
            {
                neighbor.BlendColorWithNeighbors();
            }
        }

        blendedColor.A = 0;
        TileColor = blendedColor;
        State = TileState.Idle; // ensure it's marked as idle
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
    /// returns if their is a neighbor of the tile at the given coords
    /// </summary>
    public bool HasNeighbor(Vector2 coords)
    {
        return neighbors.ContainsKey(coords);
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
    public void FadeTile(TileState state)
    {
        if (!TileDelayTimer.IsStopped())
        {
            TileDelayTimer.Stop();
        }

        if (State == TileState.NeedingColorBlended)
        {
            BlendColorWithNeighbors();
        }

        switch (state)
        {
            case TileState.FadingIn:
            case TileState.FadeInByNeighbors:
                if (State == TileState.Idle || State == TileState.FadeInByNeighbors)
                {
                    State = TileState.FadingIn;
                    StartNeighbors();
                }
                imgGenerator.State = ImgGeneratorState.Fading;
                FadeIn();
                break;

            case TileState.FadingOut:
            case TileState.FadeOutByNeighbors:
                if (State == TileState.Idle || State == TileState.FadeOutByNeighbors)
                {
                    State = TileState.FadingOut;
                    StartNeighbors();
                }
                imgGenerator.State = ImgGeneratorState.Fading;
                FadeOut();
                break;

            case TileState.FadingDone:
                TileDelayTimer.Stop();
                State = TileState.Idle;
                imgGenerator.State = ImgGeneratorState.Idle;
                break;

            default:
                GD.PrintErr($"Invalid State: {state} for tile {globalPosition}");
                break;
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
                if (State == TileState.FadingIn)
                {
                    neighbor.State = TileState.FadeInByNeighbors;
                }
                else if (State == TileState.FadingOut)
                {
                    neighbor.State = TileState.FadeOutByNeighbors;
                }
                // neighbor.State = State;
                if (neighbor.TileDelayTimer.IsStopped())
                {
                    neighbor.TileDelayTimer.Start(ImgConstants.NeighborDelay);
                }
            }
        }
    }

    /// <summary>
    /// fades the tile in
    /// </summary>
    private void FadeIn()
    {
        tileColor.A += ImgConstants.FadeInAmt;

        if (tileColor.A >= 1)
        {
            tileColor.A = 1;

            State = TileState.FadingOut;
            TileDelayTimer.Stop();
            TileDelayTimer.Start(ImgConstants.VisibleTime);
        }

        else
        {
            if (TileDelayTimer.IsStopped())
            {
                TileDelayTimer.Start(ImgConstants.FadeInDelay);
            }
        }
        Modulate = tileColor;
    }

    /// <summary>
    /// fades the tile out
    /// </summary>
    private void FadeOut()
    {
        tileColor.A -= ImgConstants.FadeOutAmt;

        if (tileColor.A <= 0)
        {
            // GD.Print("Now Idle");

            tileColor.A = 0;
            TileDelayTimer.Stop();

            FadeTile(TileState.FadingDone);
        }
        else
        {
            if (TileDelayTimer.IsStopped())
            {
                TileDelayTimer.Start(ImgConstants.FadeOutDelay);
            }
        }
        Modulate = tileColor;
    }

}

