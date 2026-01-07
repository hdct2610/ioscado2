using Newtonsoft.Json;

namespace ioscado
{
    public class Match
    {
        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }

        [JsonProperty("subCateName")]
        public string LeagueName { get; set; }

        [JsonProperty("hostName")]
        public string HomeTeam { get; set; }

        [JsonProperty("guestName")]
        public string AwayTeam { get; set; }

        [JsonProperty("matchTime")]
        public long MatchTimestamp { get; set; }

        [JsonProperty("hot")]
        public string IsHot { get; set; }

        [JsonProperty("anchors")]
        public List<object> Anchors { get; set; }

        // --- CÁC THUỘC TÍNH HỖ TRỢ GIAO DIỆN (Logic chuyển từ Form1 cũ sang) ---

        // 1. Hiển thị giờ: Xuống dòng (HH:mm xuống dòng dd/MM)
        public string DisplayTime => DateTimeOffset.FromUnixTimeMilliseconds(MatchTimestamp).LocalDateTime.ToString("HH:mm\ndd/MM");

        public string MatchTitle => $"{HomeTeam} vs {AwayTeam}";

        public int BlvCount => Anchors?.Count ?? 0;

        // 2. Logic Màu Nền (>=5 xanh, >=3 vàng)
        // ... (Các phần trên giữ nguyên)

        // LOGIC MÀU SẮC MỚI (DARK MODE)
        public Color RowBackgroundColor
        {
            get
            {
                // Thay vì màu đặc, ta dùng màu trong suốt (Glass effect)
                if (BlvCount >= 5) return ThemeColors.BlvHigh;   // Xanh kính
                if (BlvCount >= 3) return ThemeColors.BlvMedium; // Vàng kính
                return Color.FromArgb("#1C1C1E");            // Màu đen xám mặc định của iOS
            }
        }

        // Logic màu chữ (Luôn sáng trên nền tối)
        public Color LeagueColor => IsHot == "1" ? ThemeColors.HotNeon : ThemeColors.TextGray;
        public FontAttributes LeagueFont => IsHot == "1" ? FontAttributes.Bold : FontAttributes.None;
    }

    public class ApiResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("data")]
        public List<Match> Data { get; set; }
    }
}