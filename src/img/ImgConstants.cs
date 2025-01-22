namespace DreamScape.Img;

/// <summary>
/// class contains constants for the img generation
/// </summary>
public static class ImgConstants
{
    //tile values
    public const string IMG_PATH = "assets/textures/tile.png";

    public const float FadeInAmt = 0.1f;
    public const float FadeOutAmt = 0.1f;

<<<<<<< Updated upstream:src/img_generation/ImgConstants.cs
    public const float FadeOutDelay = 0.01f;
    public const float FadeInDelay = 0.1f;
    public const float VisibleTime = 8f;
=======
    public const float FadeOutDelay = 0.0f;
    public const float FadeDelay = 0.3f;
    public const float VisibleTime = 12f;
>>>>>>> Stashed changes:src/img/ImgConstants.cs
    public const float NeighborDelay = 0.2f;


    //generation values
<<<<<<< Updated upstream:src/img_generation/ImgConstants.cs
    public const int ImgWidth = 28;
    public const int imgHeight = 16;


    public const int CellSize = 36;

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




=======
    public const int ImgWidth = 16;
    public const int imgHeight = 16;

    public const int CellSize = 34;
    
    public static string[] Colors = new string[] {
        "#FFD700",         "#FFA500",         "#FF8C00",
        "#FF7F50",         "#FF6347",         "#FF4500",
        "#FF0000",         "#000000",         "#2F4F4F",
        "#708090",         "#696969",         "#A9A9A9",
        "#808080",         "#C0C0C0"
    };    
>>>>>>> Stashed changes:src/img/ImgConstants.cs
}