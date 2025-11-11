namespace FinanceApp.Shared.Design;

/// <summary>
/// Apple-inspired design tokens for consistent UI/UX
/// </summary>
public static class DesignTokens
{
    public static class Spacing
    {
        public const string XS = "0.25rem";    // 4px
        public const string SM = "0.5rem";     // 8px
        public const string MD = "1rem";       // 16px
        public const string LG = "1.5rem";     // 24px
        public const string XL = "2rem";       // 32px
        public const string XXL = "3rem";      // 48px
    }

    public static class BorderRadius
    {
        public const string SM = "0.375rem";   // 6px
        public const string MD = "0.5rem";     // 8px
        public const string LG = "0.75rem";    // 12px
        public const string XL = "1rem";       // 16px
        public const string FULL = "9999px";   // Pill shape
    }

    public static class Typography
    {
        public const string FontFamily = "-apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif";

        public static class FontSize
        {
            public const string XS = "0.75rem";      // 12px
            public const string SM = "0.875rem";     // 14px
            public const string BASE = "1rem";       // 16px
            public const string LG = "1.125rem";     // 18px
            public const string XL = "1.25rem";      // 20px
            public const string XXL = "1.5rem";      // 24px
            public const string XXXL = "2rem";       // 32px
        }

        public static class FontWeight
        {
            public const string Regular = "400";
            public const string Medium = "500";
            public const string Semibold = "600";
            public const string Bold = "700";
        }

        public static class LineHeight
        {
            public const string Tight = "1.25";
            public const string Normal = "1.5";
            public const string Relaxed = "1.75";
        }
    }

    public static class Colors
    {
        // Light mode
        public static class Light
        {
            public const string Primary = "#007AFF";
            public const string Secondary = "#5856D6";
            public const string Success = "#34C759";
            public const string Warning = "#FF9500";
            public const string Danger = "#FF3B30";

            public const string Background = "#FFFFFF";
            public const string Surface = "#F2F2F7";
            public const string Border = "#C6C6C8";

            public const string TextPrimary = "#000000";
            public const string TextSecondary = "#3C3C43";
            public const string TextTertiary = "#8E8E93";
        }

        // Dark mode
        public static class Dark
        {
            public const string Primary = "#0A84FF";
            public const string Secondary = "#5E5CE6";
            public const string Success = "#32D74B";
            public const string Warning = "#FF9F0A";
            public const string Danger = "#FF453A";

            public const string Background = "#000000";
            public const string Surface = "#1C1C1E";
            public const string Border = "#38383A";

            public const string TextPrimary = "#FFFFFF";
            public const string TextSecondary = "#EBEBF5";
            public const string TextTertiary = "#8E8E93";
        }
    }

    public static class Shadows
    {
        public const string SM = "0 1px 2px 0 rgba(0, 0, 0, 0.05)";
        public const string MD = "0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)";
        public const string LG = "0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)";
        public const string XL = "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)";
    }

    public static class Transitions
    {
        public const string Fast = "150ms cubic-bezier(0.4, 0, 0.2, 1)";
        public const string Base = "300ms cubic-bezier(0.4, 0, 0.2, 1)";
        public const string Slow = "500ms cubic-bezier(0.4, 0, 0.2, 1)";
    }

    public static class ZIndex
    {
        public const int Dropdown = 1000;
        public const int Sticky = 1020;
        public const int Fixed = 1030;
        public const int ModalBackdrop = 1040;
        public const int Modal = 1050;
        public const int Popover = 1060;
        public const int Tooltip = 1070;
    }
}
