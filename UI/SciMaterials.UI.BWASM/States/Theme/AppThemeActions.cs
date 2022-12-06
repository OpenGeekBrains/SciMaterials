namespace SciMaterials.UI.BWASM.States.Theme;

public static class AppThemeActions
{
    public record struct SwitchThemeColorScheme(string ColorScheme);
    public record struct SwitchThemeToDarkMode;
    public record struct SwitchThemeToLightMode;

    public static SwitchThemeToDarkMode SwitchToDarkMode() => new();
    public static SwitchThemeToLightMode SwitchToLightMode() => new();
    public static SwitchThemeColorScheme ChangeColorScheme(string ColorScheme) => new(ColorScheme);
}
