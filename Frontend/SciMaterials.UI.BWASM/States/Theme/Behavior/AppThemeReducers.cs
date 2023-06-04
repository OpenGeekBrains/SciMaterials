using Fluxor;

namespace SciMaterials.UI.BWASM.States.Theme.Behavior;

public static class AppThemeReducers
{
    [ReducerMethod(typeof(AppThemeActions.SwitchThemeToDarkMode))]
    public static AppThemeState SwitchThemeToDarkMode(AppThemeState state)
    {
        return state with { IsDarkTheme = true };
    }

    [ReducerMethod(typeof(AppThemeActions.SwitchThemeToLightMode))]
    public static AppThemeState SwitchThemeToLightMode(AppThemeState state)
    {
        return state with { IsDarkTheme = false };
    }

    [ReducerMethod]
    public static AppThemeState SwitchThemeColorScheme(AppThemeState state, AppThemeActions.SwitchThemeColorScheme action)
    {
        if (!state.Themes.TryGetValue(action.ColorScheme, out var scheme)) return state;
        return state with { CurrentTheme = scheme };
    }
}
