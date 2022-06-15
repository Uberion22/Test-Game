using UnityEngine;

public static class TriggersNames
{
    public const string AttackTrigger = "Attack";
    public const string HitTrigger = "Hurt";
    public const string DeathTrigger = "Death";
    public const string AnimationState = "AnimState";
}

public static class MyColors
{
    public static Color MyYellow = new Color(1, 0.92f, 0.016f, 0.4f);
    public static Color MyBlue = new Color(0, 0, 1, 0.45f);
    public static Color MyRed = new Color(1, 0, 0, 0.4f);
    public static Color MyWhite = new Color(1, 1, 1, 0.0f);
}

public static class CharacterTags
{
    public const string Dead = "Dead";
    public const string Enemy = "Enemy";
    public const string Player = "Player";
}

public static class EndGameText
{
    public const string WinText = "You Win!";
    public const string LooseText = "Game Over!";
}

public static class MessageColors
{
    public const string CurrentCharacter = "blue";
    public const string DeadCharacter = "red";
    public const string DefaultCharacter = "green";
}