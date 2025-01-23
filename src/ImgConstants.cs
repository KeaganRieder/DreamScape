namespace DreamScape;

/// <summary>
/// class contains constants for the img generation
/// </summary>
public static class ImgConstants
{
    //tile values
    public const string IMG_PATH = "assets/textures/tile.png";

    public const float FadeInAmt = 0.2f;
    public const float FadeOutAmt = 0.2f;

    public const float FadeOutDelay = 0.1f;
    public const float FadeDelay = 0.5f;
    public const float VisibleTime = 12f;
    public const float NeighborDelay = 0.2f;

    //generation values
    public const int ImgWidth = 24;
    public const int imgHeight = 12;
    //   public const int ImgWidth = 5;
    // public const int imgHeight = 5;


    public const int CellSize = 42;

    /// <summary>
    /// make tile fade to to new palette from old
    /// </summary>
    public static string[] Colors = new string[] {
        "#FFE8BD", "EEF9FA", "#C8F1E8", "#DEFDEF",
        "#BFC8C5", "#FADECF", "#FAEDCB", "#C9E4DE", 
        "#C6DEF1", "#DBCDF0", "#F2C6DE", "#f7D9C4",
        "#FFD7D7", "#FFF1F0", "#FFA7A6", "#FEDCDB",
        "#D4E0EE", "#FAE7EB", "#F2D7E8", "#ECF8F7",
        "#B58EBC", "#D1EDEA", "#D8BBD3",
    };

}