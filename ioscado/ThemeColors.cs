using Microsoft.Maui.Graphics;

namespace ioscado
{
    public static class ThemeColors
    {
        // Nền tối cho "iOS 26"
        public static Color CardBackground = Color.FromArgb("#1A1A1A"); // Xám đen
        public static Color PageBackground = Color.FromArgb("#000000"); // Đen tuyền

        // Màu Neon tương lai
        public static Color PrimaryNeon = Color.FromArgb("#0A84FF");    // Xanh Neon (iOS Blue)
        public static Color SuccessNeon = Color.FromArgb("#30D158");    // Xanh lá Neon
        public static Color HotNeon = Color.FromArgb("#FFD60A");        // Vàng rực

        // Logic màu nền dòng (Chuyển sang dạng tint nhẹ trên nền đen)
        public static Color BlvHigh = Color.FromArgb("#330A84FF");     // Xanh trong suốt
        public static Color BlvMedium = Color.FromArgb("#33FFD60A");   // Vàng trong suốt

        public static Color TextWhite = Colors.White;
        public static Color TextGray = Color.FromArgb("#98989E");
    }
}