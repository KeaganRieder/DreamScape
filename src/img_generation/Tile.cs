namespace DreamScape.ImgGeneration;

using Godot;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// a tile is a cell of the img, it contains various aspect that relate to it
/// </summary>
public partial class Tile : Sprite2D
{
    public const string IMG_PATH = "assets/textures/tile.png";

    private Vector2I globalPosition;

    private Dictionary<Vector2, Tile> neighbors;
    private ImgGenerator imgGenerator;

    private Vector2I graphicSize;
    private Color color;

    private Timer fadeTileTimer;
    private Timer visibilityTimer;
    private Timer neighborDelayTimer;
    private bool fadeIn;

    private float neighborDelay;
    private float alphaChangeAmt;
    private float alphaChangeDelay;
    private float visibleFor;

    public Tile(ImgGenerator imgGenerator, Vector2 position, Vector2I graphicSize, Color color,
        float alphaChangeAmt, float alphaChangeDelay, float neighborDelay, float visibleFor)
    {
        Name = $"Tile {position}";
        globalPosition = new Vector2I(Mathf.FloorToInt(position.X / graphicSize.X), Mathf.FloorToInt(position.Y / graphicSize.Y));
        Position = position;
        this.imgGenerator = imgGenerator;
        this.graphicSize = graphicSize;
        this.color = color;
        this.alphaChangeAmt = alphaChangeAmt;
        this.alphaChangeDelay = alphaChangeDelay;
        this.neighborDelay = neighborDelay;
        this.visibleFor = visibleFor;

        neighbors = new Dictionary<Vector2, Tile>();
    }

    public bool IsFading { get; private set; }
    public bool CheckedIfDone { get; set; }

    public override void _Ready()
    {
        ReadTexture();

        fadeTileTimer = new Timer();
        AddChild(fadeTileTimer);
        fadeTileTimer.Timeout += () => FadeTile();

        visibilityTimer = new Timer();
        AddChild(visibilityTimer);
        visibilityTimer.Timeout += () => StartFadeEffect(false);

        neighborDelayTimer = new Timer();
        AddChild(neighborDelayTimer);
        neighborDelayTimer.Timeout += () => StartNeighborsFade(fadeIn);

        base._Ready();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    /// <summary>
    /// reads and sets texture of the tile form the file location
    /// </summary>
    public void ReadTexture()
    {
        if (File.Exists(IMG_PATH))
        {
            Image image = Image.LoadFromFile(IMG_PATH);

            if (image.GetSize() != graphicSize)
            {
                image.Resize(graphicSize.X, graphicSize.Y, Image.Interpolation.Bilinear);
            }
            Texture2D texture = ImageTexture.CreateFromImage(image);

            Texture = texture;
            Modulate = color;
        }

        else
        {
            GD.Print("File Doesn't exists");
        }
    }

    /// <summary>
    /// updates the color of the tile
    /// </summary>
    public void UpdateColor(Color color)
    {
        this.color = color;
        Modulate = color;
    }

    /// <summary>
    /// begins the tiles and it's neighbors fade in or out effect
    /// </summary>
    public void StartFadeEffect(bool fadeIn)
    {
        if (!IsFading)
        {
            CheckedIfDone = false;
            IsFading = true;
            neighborDelayTimer.Start(neighborDelay);
        }

        this.fadeIn = fadeIn;
        FadeTile();
    }

    private void StartNeighborsFade(bool fadeIn)
    {
        neighborDelayTimer.Stop();

        foreach (var neighbor in neighbors.Values)
        {
            if (neighbor.IsFading == false)
            {
                neighbor.StartFadeEffect(fadeIn);
            }
        }
    }

    /// <summary>
    /// determines if the tile is fading in or out 
    /// and calls the appropriate method
    /// </summary>
    private void FadeTile()
    {
        if (fadeIn)
        {
            FadeIn();
        }
        else
        {
            FadeOut();
        }
    }
    /// <summary>
    /// fades the tile in
    /// </summary>
    private void FadeIn()
    {
        color.A += alphaChangeAmt;

        if (color.A >= 1)
        {
            color.A = 1;
            Modulate = color;

            if (visibilityTimer.IsStopped())
            {
                fadeTileTimer.Stop();
                visibilityTimer.Start(visibleFor);
            }
        }
        else
        {
            Modulate = color;
            // not finished fading in so continue
            if (fadeTileTimer.IsStopped())
            {
                fadeTileTimer.Start(alphaChangeDelay);
            }
        }
    }

    /// <summary>
    /// fades the tile out
    /// </summary>
    private void FadeOut()
    {
        color.A -= alphaChangeAmt;
        if (color.A <= 0)
        {
            color.A = 0;
            Modulate = color;
            fadeTileTimer.Stop();
            visibilityTimer.Stop();
            IsFading = false;

            imgGenerator.RemoveTileFromFadingList(globalPosition);

            if (imgGenerator.IsFadeEffectDone())
            {
                GD.Print("Done Fading");
                imgGenerator.Fading = false;
            }
        }

        else
        {
            Modulate = color;

            // not finished fading in so continue
            if (fadeTileTimer.IsStopped())
            {
                fadeTileTimer.Start(alphaChangeDelay);
            }
        }
    }

    /// <summary>
    /// returns if their is a neighbor of the tile at the given coords
    /// </summary>
    public bool hasNeighbor(Vector2 coords)
    {
        return neighbors.ContainsKey(coords);
    }

    /// <summary>
    /// sets neighbor of the tile at the given coords
    /// </summary>
    private void SetNeighbor(Vector2 coords, Tile tile)
    {
        if (!neighbors.ContainsKey(coords))
        {
            neighbors.Add(coords, tile);
        }
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

  


}