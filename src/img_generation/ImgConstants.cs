namespace DreamScape.ImgGeneration;

/// <summary>
/// class contains constants for the img generation
/// </summary>
public static class ImgConstants
{
	//tile values
	public const string IMG_PATH = "assets/textures/tile.png";

	public const float FadeInAmt = 0.2f;
	public const float FadeOutAmt = 0.2f;

	public const float FadeOutDelay = 0.0f;
	public const float FadeInDelay = 0.1f;
	public const float VisibleTime = 2;
	public const float NeighborDelay = 0.2f;

	//generation values
	public const int ImgWidth = 64;
	public const int imgHeight = 124;

	public const int CellSize = 32;
	
	public static string[] TestColors = new string[] {
		"#000000", "#000000", "#FFFFFF", "#FFFFFF"
	};
	
	public static string[] Colors = new string[] {
		"#FFD700",         "#FFA500",         "#FF8C00",
		"#FF7F50",         "#FF6347",         "#FF4500",
		"#FF0000",         "#000000",         "#2F4F4F",
		"#708090",         "#696969",         "#A9A9A9",
		"#808080",         "#C0C0C0"
	};	
}
