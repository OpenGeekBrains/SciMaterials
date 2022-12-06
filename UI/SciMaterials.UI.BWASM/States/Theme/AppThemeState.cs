using System.Collections.Immutable;

using Fluxor;

using MudBlazor;

namespace SciMaterials.UI.BWASM.States.Theme;

[FeatureState]
public record AppThemeState
{
    public MudTheme? CurrentTheme { get; init; }
    public bool IsDarkTheme { get; init; }

    public ImmutableDictionary<string, MudTheme> Themes { get; init; } = new[]
    {
        new KeyValuePair<string, MudTheme>("default", new MudTheme()
        {
            Palette = new Palette
            {
                White = AppColors.Gray0,
                Black = AppColors.Gray900,

                Primary             = AppColors.Primary_Main,
                PrimaryDarken       = AppColors.Primary_Dark,
                PrimaryLighten      = AppColors.Primary_Light,
                PrimaryContrastText = AppColors.Primary_Contrast_Text,

                Secondary             = AppColors.Secondary_Main,
                SecondaryDarken       = AppColors.Secondary_Dark,
                SecondaryLighten      = AppColors.Secondary_Light,
                SecondaryContrastText = AppColors.Secondary_Contrast_Text,

                Tertiary             = AppColors.Secondary_Main,
                TertiaryDarken       = AppColors.Secondary_Dark,
                TertiaryLighten      = AppColors.Secondary_Light,
                TertiaryContrastText = AppColors.Secondary_Contrast_Text,

                Info             = AppColors.Info_Main,
                InfoDarken       = AppColors.Info_Dark,
                InfoLighten      = AppColors.Info_Light,
                InfoContrastText = AppColors.Info_Contrast_Text,

                Success             = AppColors.Success_Main,
                SuccessDarken       = AppColors.Success_Dark,
                SuccessLighten      = AppColors.Success_Light,
                SuccessContrastText = AppColors.Success_Contrast_Text,

                Warning             = AppColors.Warning_Main,
                WarningDarken       = AppColors.Warning_Dark,
                WarningLighten      = AppColors.Warning_Light,
                WarningContrastText = AppColors.Warning_Contrast_Text,

                Error             = AppColors.Error_Main,
                ErrorDarken       = AppColors.Error_Dark,
                ErrorLighten      = AppColors.Error_Light,
                ErrorContrastText = AppColors.Error_Contrast_Text,

                Dark             = AppColors.Gray800,
                DarkDarken       = AppColors.Gray900,
                DarkLighten      = AppColors.Gray700,
                DarkContrastText = AppColors.Gray0,

                HoverOpacity = 0.16,

                GrayDefault  = AppColors.Gray300,
                GrayLight    = AppColors.Gray200,
                GrayLighter  = AppColors.Gray100,
                GrayDark     = AppColors.Gray600,
                GrayDarker   = AppColors.Gray700,
                OverlayDark  = AppColors.Gray500_24.ToString(),
                OverlayLight = AppColors.Gray500_12.ToString(),

                TextPrimary              = AppColors.Gray600,
                TextSecondary            = AppColors.Gray700,
                TextDisabled             = AppColors.Gray400,

                ActionDefault            = AppColors.Gray500,
                ActionDisabled           = AppColors.Gray300,
                ActionDisabledBackground = AppColors.Gray300,

                Background               = AppColors.Gray100,
                BackgroundGrey           = AppColors.Gray300,

                Surface                  = AppColors.Gray100,

                DrawerBackground = AppColors.Gray100,
                DrawerText       = AppColors.Gray600,
                DrawerIcon       = AppColors.Gray600,

                AppbarBackground = AppColors.Gray200,
                AppbarText       = AppColors.Gray900,

                LinesDefault = AppColors.Gray400,
                LinesInputs  = AppColors.Gray500,

                TableLines   = AppColors.Gray500,
                TableStriped = AppColors.Gray500_16,
                TableHover   = AppColors.Gray500_32,

                Divider      = AppColors.Gray300,
                DividerLight = AppColors.Gray700,
            }
        })
    }.ToImmutableDictionary(p => p.Key, p => p.Value);
}