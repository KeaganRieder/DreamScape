namespace DreamScape.sound;

using DreamScape.img;

public static class AudioConstants
{

    public const float FadeOutDelay = ImgConstants.FadeOutDelay + 0.1f;

    public const float FadeOutAmt = 1f;

    public static string[] Sounds = new string[]
    {
        "ambient_music", "new_perlin_dream", "new_dream",
        "most_recent_dream", "new_palette", "inverted_dream"
    };
}

