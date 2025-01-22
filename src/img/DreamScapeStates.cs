namespace DreamScape.Img;

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
public enum FadeState
{
    Undefined = 0,
    FadeOut = 1,
    FadeIn = 2,
    FadeToNextColor = 3,
}

/// <summary>
/// holds and manges the various states of the img
/// </summary>
public class DreamScapeStates
{
    private static DreamScapeStates instance;
    
    /// <summary>
    /// instance of the object
    /// </summary>
    public static DreamScapeStates Instance
    {
        get
        {
            instance ??= new DreamScapeStates();
            return instance;
        }
    }

    private DreamScapeStates()
    {
        ImgState = ImgState.New;
        FadeState = FadeState.FadeOut;
    }

    /// <summary>
    /// the current state of the dreamScape img
    /// </summary>
    public ImgState ImgState { get; set; }

    /// <summary>
    /// the current fade status of the img
    /// </summary>
    public FadeState FadeState { get; set; }

}
