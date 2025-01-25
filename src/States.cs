namespace DreamScape;

using Godot;

/// <summary>
/// tracks teh state of the image
/// </summary>
public enum ImgState
{
    Undefined = 0,
    New = 1,
    Idle = 2,
    Fading = 3,
}

/// <summary>
/// state that tracks how the image should be fading
/// </summary>
public enum FadeType
{
    Undefined = 0,
    DefaultFade = 1, // fades in, then out
    CycleFade = 2, // Cycles Colors
}

/// <summary>
/// holds and manges the various states of the img
/// </summary>
public class States
{
    private static States instance;

    /// <summary>
    /// instance of the object
    /// </summary>
    public static States Instance
    {
        get
        {
            instance ??= new States();
            return instance;
        }
    }

    private States()
    {
        ImgState = ImgState.New;
        FadeMode = FadeType.DefaultFade;
    }

    /// <summary>
    /// used to activate debugging
    /// </summary>
    public bool Debugging { get; set; } = false;

    /// <summary>
    /// the current state of the dreamScape img
    /// </summary>
    public ImgState ImgState { get; set; }

    /// <summary>
    /// the current fade status of the img
    /// </summary>
    public FadeType FadeMode { get; set; }

    /// <summary>
    /// used to track if the fade mode has been updated or not
    /// </summary>
    public bool UpdatedFadeMode { get; set; }

    public int TilesFading;
    public int TilesDoneFading;

    /// <summary>
    /// checks if all the tiles are done fading 
    /// and updates the state of the image
    /// </summary>
    public void CheckIfDoneFading()
    {
        if (ImgState == ImgState.Idle)
        {
            return;
        }
        else if (ImgState == ImgState.Fading)
        {
            if (TilesFading <= TilesDoneFading)
            {

                if (Debugging)
                {
                    GD.Print("Tiles Fading: " + TilesFading + " Tiles Done Fading: " + TilesDoneFading);
                    GD.Print("All Tiles Done Fading");
                }
                ImgState = ImgState.Idle;
            }
            else
            {
                // GD.Print("Tiles Fading: " + TilesFading + " Tiles Done Fading: " + TilesDoneFading);
            }
        }

    }

}
