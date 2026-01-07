using ioscado;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace ioscado
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Match> Matches { get; set; } = new ObservableCollection<Match>();
        private readonly HttpClient client = new HttpClient();

        public MainPage()
        {
            InitializeComponent();
            MatchList.ItemsSource = Matches;
        }

        // --- 1. LẤY DỮ LIỆU ---
        private async void OnNhapDuLieuClicked(object sender, EventArgs e)
        {
            if (btnNhapDuLieu == null) return;

            btnNhapDuLieu.IsEnabled = false;
            btnNhapDuLieu.Text = "Đang tải...";
            Matches.Clear();

            try
            {
                var listToday = await GetMatchesForDateAsync(DateTime.Now);
                var listTomorrow = await GetMatchesForDateAsync(DateTime.Now.AddDays(1));

                // Lưu ý: Đảm bảo bạn đã tắt <Nullable>disable</Nullable> trong file project để tránh lỗi null ở đây
                var allMatches = listToday.Concat(listTomorrow)
                                          .Where(m => m != null && m.CategoryName == "Bóng đá")
                                          .OrderBy(m => m.MatchTimestamp)
                                          .ToList();

                if (allMatches.Count > 0)
                {
                    foreach (var match in allMatches) Matches.Add(match);
                }
                else
                {
                    await DisplayAlert("Thông báo", "Không tìm thấy trận bóng đá nào.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Lỗi", ex.Message, "OK");
            }
            finally
            {
                btnNhapDuLieu.IsEnabled = true;
            }
        }

        private async Task<List<Match>> GetMatchesForDateAsync(DateTime date)
        {
            string dateString = date.ToString("yyyyMMdd");
            string apiUrl = $"https://json.vnres.co/match/matches_{dateString}.json";

            try
            {
                string jsonpString = await client.GetStringAsync(apiUrl);
                int startIndex = jsonpString.IndexOf('(');
                int endIndex = jsonpString.LastIndexOf(')');

                if (startIndex == -1 || endIndex == -1) return new List<Match>();

                string jsonString = jsonpString.Substring(startIndex + 1, endIndex - startIndex - 1);
                var response = JsonConvert.DeserializeObject<ApiResponse>(jsonString);

                if (response != null && response.Code == 200 && response.Data != null)
                    return response.Data;
            }
            catch { }
            return new List<Match>();
        }

        // --- 2. SAO CHÉP PROMPT ---
        private async void OnSaoChepClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var match = button?.CommandParameter as Match;
            if (match == null) return;

            string tranDauText = $"{match.HomeTeam} vs {match.AwayTeam}";
            string giaiText = match.LeagueName ?? "";
            string finalPrompt = "";

            if (radPrompt1.IsChecked)
            {
                finalPrompt = $"Giải {giaiText}. {tranDauText}. Bối cảnh: Bạn là một nhà phân tích bóng đá, khoa học dữ liệu và chiến lược gia cá cược hàng đầu thế giới, với biệt tài \"đọc vị\" những trận đấu có dấu hiệu bất thường. Nhiệm vụ của bạn là soạn thảo một báo cáo tối mật, kết hợp giữa phân tích học thuật và nhận định thực chiến cho trận đấu đỉnh cao giữa [Đội 1] và [Đội 2].\r\nLưu ý quan trọng: Hãy sử dụng dữ liệu mới nhất bạn có thể truy cập và nêu rõ mốc thời gian (knowledge cut-off date) của dữ liệu bạn đang dùng để phân tích. Điều này đảm bảo tính minh bạch cho báo cáo. Trọng tâm của báo cáo này là phát hiện các yếu tố động lực bất thường, kịch bản \"thả lỏng\" hoặc các dấu hiệu phi bóng đá có thể ảnh hưởng đến kết quả.\r\nNhiệm vụ: Thực hiện một deep-research toàn diện, tích hợp mọi góc độ từ dữ liệu, chiến thuật, thị trường cho đến yếu tố tâm lý con người và các động cơ tài chính tiềm ẩn. Trình bày kết quả dưới dạng một báo cáo phân tích chiến lược có tính ứng dụng cao.\r\nYêu Cầu Phân Tích Chi Tiết:\r\nPhần 1: Phân Tích Dữ Liệu Định Lượng & Thị Trường\r\n * Phong độ gần đây (Last 8 matches): Phân tích kết quả (W-D-L), bàn thắng/bàn thua và đánh giá hiệu suất có tương xứng với kết quả không.\r\n * Thành tích đối đầu (Head-to-Head): Phân tích 6 lần đối đầu gần nhất, tìm ra các quy luật và ưu thế (nếu có).\r\n * Thống kê Hiệu suất Nâng cao: So sánh các chỉ số tấn công (xG, sút/trận) và phòng ngự (xGA, giữ sạch lưới) của hai đội trong mùa giải.\r\n * Thống kê Thị trường & Bối cảnh:\r\n   * Giá trị đội hình: So sánh tổng giá trị và giá trị các ngôi sao chủ chốt. Nhận định sự tương quan giữa giá trị và hiệu quả thi đấu.\r\n   * Tỉ lệ thắng kèo handicap gần đây: Phân tích tỉ lệ thắng kèo handicap của hai đội trong 10 trận gần nhất để đánh giá hiệu suất so với kỳ vọng của thị trường.\r\n * Yếu tố Sân nhà - Sân khách: Phân tích riêng biệt hiệu suất của [Đội 1] trên sân nhà và của [Đội 2] trên sân khách.\r\nPhần 2: Phân Tích Định Tính, Chiến Thuật & Động Lực\r\n * Lối chơi & Đối kháng Chiến thuật: Phân tích sơ đồ, triết lý và dự đoán sự khắc chế chiến thuật giữa hai đội.\r\n * Tình hình nhân sự: Liệt kê các cầu thủ chấn thương, treo giò và đánh giá mức độ ảnh hưởng. Xác định các cầu thủ chủ chốt đang có phong độ cao.\r\n * Tâm lý chung: Đánh giá tinh thần hiện tại của đội (hưng phấn, áp lực, mâu thuẫn nội bộ nếu có).\r\n * Phân tích Động lực Chuyên sâu & Các Yếu tố Ngầm:\r\n   * 4.1. Động lực Hiển nhiên (Surface Motivation):\r\n     * Phân tích bối cảnh mùa giải: Trận đấu này có ý nghĩa gì với mục tiêu của họ (trụ hạng, top 4, vô địch, danh dự)?\r\n     * Hậu quả trực tiếp nếu kết quả là Thua hoặc Hòa đối với từng đội là gì?\r\n   * 4.2. Phân tích Lịch thi đấu & Mức độ Ưu tiên (Schedule & Priority Analysis):\r\n     * Phân tích \"Look-ahead\": Sau trận này, họ sẽ gặp đối thủ nào? Trận đấu tiếp theo có phải là một trận chung kết cúp, một trận derby sinh tử, hay một trận đấu quan trọng hơn ở cúp châu Âu không?\r\n     * Đánh giá sự Mệt mỏi: Trận đấu trước đó của họ có tốn nhiều sức không (đá hiệp phụ, di chuyển xa, đối thủ khó chịu)?\r\n     * Chấm điểm Mức độ Ưu tiên: Dựa vào hai yếu tố trên, hãy chấm điểm mức độ ưu tiên của trận đấu này cho mỗi đội trên thang điểm 10 (1 = Trận thủ tục, 10 = Trận sống còn).\r\n   * 4.3. Kịch bản \"Hài lòng với 1 điểm\" hoặc \"Buông thả\" (The \"Complacency/Let-go\" Scenario):\r\n     * Vùng an toàn: Có đội nào đang ở vị trí \"lưng chừng\" trên BXH, không còn mục tiêu phấn đấu và cũng không có nguy cơ xuống hạng không?\r\n     * Kịch bản \"Bắt tay\": Liệu một kết quả hòa có làm hài lòng cả hai đội để cùng nhau đạt được mục tiêu ngắn hạn không (ví dụ: cùng dắt tay vào top 4, cùng chắc chắn trụ hạng)?\r\n     * Lịch sử \"Thân thiện\": Hai CLB có mối quan hệ tốt, có lịch sử tạo ra những kết quả \"thuận lợi\" cho nhau trong quá khứ không?\r\n   * 4.4. Cảnh báo \"Cờ đỏ\" & Yếu tố Phi bóng đá (Red Flags & Non-Football Factors):\r\n     * Đội hình ra sân: Phân tích đội hình dự kiến. Liệu có sự vắng mặt đáng ngờ của các trụ cột mà không phải vì chấn thương hay thẻ phạt không? Đây có thể là dấu hiệu cho thấy sự \"thả lỏng\".\r\n     * Thông tin Nội bộ: Dựa trên các nguồn tin uy tín, có tin đồn nào về mâu thuẫn nội bộ, nợ lương, hoặc các vấn đề ngoài sân cỏ có thể ảnh hưởng đến tinh thần thi đấu không?\r\n     * Tổng hợp Rủi ro \"Diễn xuất\": Dựa trên tất cả các phân tích từ 4.1 đến 4.4, hãy đưa ra đánh giá cuối cùng về nguy cơ xảy ra kịch bản đội mạnh thi đấu dưới sức, diễn xuất hoặc đá thả: Thấp, Trung bình, hay Cao? Lý giải tại sao.\r\n * Đánh giá Trọng số Yếu tố: Sau khi phân tích, hãy nhận định: Đối với trận đấu cụ thể này, yếu tố nào (ví dụ: phong độ, chiến thuật, động lực ngầm, chấn thương...) có trọng số ảnh hưởng lớn nhất đến kết quả cuối cùng? Lý giải.\r\nPhần 3: Tổng Hợp, Kịch Bản & Đề Xuất Chiến Lược\r\n * Những cuộc đối đầu then chốt (Key Battles): Xác định 2-3 cuộc đối đầu cá nhân hoặc khu vực có khả năng định đoạt trận đấu.\r\n * Dự đoán chuyên sâu:\r\n   * Dự đoán Tỉ số Hiệp 1: Đưa ra tỉ số và lý giải ngắn gọn dựa trên cách nhập cuộc dự kiến của hai đội.\r\n   * Dự đoán Tỉ số Cả trận (Full-time): Đưa ra tỉ số cuối cùng.\r\n   * Lý giải chi tiết cho dự đoán: Tổng hợp tất cả các phân tích (đặc biệt là yếu tố có trọng số cao nhất và đánh giá rủi ro \"diễn xuất\" đã xác định ở Phần 2) để bảo vệ cho các tỉ số dự đoán.\r\n   * Tỷ lệ xác suất: Ước tính % cho các kết quả (Thắng [Đội 1] - Hòa - Thắng [Đội 2]).\r\n   * Yếu tố Bất ngờ (Wildcard Factor): Xác định một kịch bản có xác suất thấp nhưng tác động cao có thể lật ngược mọi dự đoán (thẻ đỏ sớm, sai lầm cá nhân ngớ ngẩn...).\r\n * Đề Xuất Chiến Lược Cá Cược (Value Betting):\r\n   * Kèo Chính (Main Bet): Dựa trên tất cả phân tích, đâu là lựa chọn kèo chính mà bạn tự tin nhất (ví dụ: kèo chấp, tài xỉu, đội thắng)? Nêu rõ lý do tại sao bạn tin rằng thị trường có thể đã đánh giá sai (tìm kiếm \"value\").\r\n   * Kèo Phụ/Giá trị cao (Side/High-Value Bet): Đề xuất 1-2 kèo phụ có tỷ lệ cược hấp dẫn và có cơ sở từ phân tích (ví dụ: thẻ phạt, phạt góc, cầu thủ ghi bàn, cả hai đội ghi bàn...).\r\n   * Quản lý Rủi ro: Đề cập ngắn gọn đến rủi ro lớn nhất có thể làm hỏng các nhận định trên (thường liên quan đến đánh giá rủi ro \"diễn xuất\" hoặc Wildcard Factor).\r\nĐịnh dạng đầu ra: Trình bày báo cáo một cách chuyên nghiệp, sử dụng Markdown với các tiêu đề rõ ràng cho từng phần và gạch đầu dòng để dễ theo dõi.";
            }
            else if (radPrompt2.IsChecked)
            {
                finalPrompt = $"Giải {giaiText}. {tranDauText}, hãy dự đoán tỉ số trong hiệp 1 và hiệp 2. Nhớ để ý: Giá trị đội hình, phong độ gần đây, tỉ lệ thắng kèo handicap, các cầu thủ chấn thương treo giò và quan trọng nhất là tìm rõ động lực, so sánh động lực, ảnh hưởng khi thua/hoà, đánh giá. Bên nào muốn thắng hơn.";
            }
            else if (radPrompt3.IsChecked)
            {
                finalPrompt = $"Giải {giaiText}. {tranDauText}# VAI TRÒ\r\nBạn là một \"Thợ Săn Kèo Bóng Cỏ\" (Underground Betting Scout) chuyên nghiệp. Bạn không quan tâm đến chiến thuật hoa mỹ, bạn chỉ quan tâm đến DÒNG TIỀN, ĐỘNG LỰC SINH TỒN và NHỮNG CON SỐ BIẾT NÓI.\r\nTư duy cốt lõi: \"Ở giải cỏ, Kèo (Odds) lạ thường quan trọng hơn Phong độ\".\r\n\r\n# NHIỆM VỤ\r\nHãy thực hiện quy trình Deep Research Tự Động (Auto-Scan) và đưa ra phán quyết cho trận đấu sau:\r\n- Trận đấu: [ĐIỀN TÊN ĐỘI A] vs [ĐIỀN TÊN ĐỘI B]\r\n- Giải đấu: [ĐIỀN TÊN GIẢI - Ví dụ: AZE D1, Hạng 2 Nhật...]\r\n\r\n# BƯỚC 1: TỰ ĐỘNG THU THẬP DỮ LIỆU (SEARCH & EXTRACT)\r\nHãy sử dụng Google Search để tìm kiếm dữ liệu mới nhất (Real-time) về:\r\n1. 💰 Tỷ Lệ Kèo (Odds): Tìm trên OddsPortal, Flashscore hoặc các trang uy tín.\r\n   - Tìm Kèo Chấp (Asian Handicap) và Tài/Xỉu (Over/Under) hiện tại.\r\n   - Xu hướng: Odds đang tăng hay giảm?\r\n2. 📊 Vị Trí & Phong Độ:\r\n   - BXH hiện tại: Đội nào đang cần điểm (Đua vô địch/Trụ hạng)? Đội nào hết mục tiêu?\r\n   - Phong độ \"Home/Away Split\": Đội chủ nhà đá sân nhà có tốt không? Đội khách đá sân khách có tệ không?\r\n\r\n# BƯỚC 2: PHÂN TÍCH \"SĂN MA\" (ANOMALY DETECTION)\r\nHãy đối chiếu dữ liệu vừa tìm được để trả lời 3 câu hỏi tử thần:\r\n1. Kèo có \"Lừa\" không? (The Trap Check):\r\n   - Nếu Đội A đứng Top đầu, đá sân nhà gặp Đội B bét bảng mà Kèo Chấp rất thấp (ví dụ chỉ chấp 0.25 hoặc 0.5) -> BÁO ĐỘNG ĐỎ (Có mùi buông/bán).\r\n2. Động lực có chênh lệch không? (Survival Check):\r\n   - Có phải một đội đang khát điểm trụ hạng gặp một đội đã \"no bụng\" (giữa BXH) không? (Đây là mỏ vàng của bóng cỏ).\r\n3. Quy luật giải đấu:\r\n   - Giải này thường có xu hướng gì (Nổ tài, Sân nhà thắng thế...)?\r\n\r\n# ĐẦU RA YÊU CẦU (BÁO CÁO NGẮN GỌN & DỨT KHOÁT)\r\nHãy trình bày kết quả theo format sau:\r\n\r\n🔎 **1. DỮ LIỆU TRINH SÁT (SCOUT REPORT):**\r\n   - **Odds tìm được:** (Ví dụ: Chủ chấp 0.5, Tài xỉu 2.5).\r\n   - **Động lực:** (Ví dụ: Chủ nhà cần thắng để vô địch, Khách đã hết mục tiêu).\r\n   - **Phong độ dị:** (Ví dụ: Khách thua 5 trận sân khách liên tiếp).\r\n\r\n⚠️ **2. PHÁT HIỆN BẤT THƯỜNG (ANOMALY):**\r\n   - (Chỉ ra sự vô lý giữa Kèo và Thực tế. Nếu Kèo hợp lý thì ghi \"Kèo đúng thực lực\").\r\n\r\n🎯 **3. PHÁN QUYẾT CUỐI CÙNG (THE VERDICT):**\r\n   - **KÈO SÁNG NHẤT (BEST BET):** (Chọn Đội hoặc Tài/Xỉu).\r\n   - **ĐỘ TIN CẬY (1-10):** (Lưu ý: Bóng cỏ tối đa là 8/10).\r\n   - **LỜI KHUYÊN RUNG (IN-PLAY):** (Ví dụ: Nếu H1 hòa 0-0, nhồi Tài H2).";
            }

            await Clipboard.Default.SetTextAsync(finalPrompt);
            await DisplayAlert("Đã chép", $"Đã copy: {match.HomeTeam}", "OK");
        }

        // --- 3. PROMPT TỔNG HỢP ---
        private async void OnPromptKetHopClicked(object sender, EventArgs e)
        {
            string prompt = @"# VAI TRÒ
Bạn là Trưởng ban Tình báo Chiến lược Bóng đá (Chief Football Analyst). Bạn đang nắm trong tay 6 báo cáo mật từ các điệp viên khác nhau về trận đấu [Đội Nhà] vs [Đội Khách].
Nhiệm vụ: Tổng hợp, đối chiếu và đưa ra phán quyết cuối cùng (Final Verdict).

# DỮ LIỆU ĐẦU VÀO
Tôi cung cấp 6 file báo cáo (Nguồn 1 đến Nguồn 6). Lưu ý:
- Nguồn 1, 2: Thiên về thống kê (Data-driven).
- Nguồn 3, 4: Thiên về tin tức nội bộ và chấn thương.
- Nguồn 5, 6: Thiên về phân tích tâm lý và thuyết âm mưu.

# QUY TRÌNH XỬ LÝ (THE VERDICT ENGINE) - PHIÊN BẢN NÂNG CAO

## BƯỚC 1: LỌC NHIỄU VÀ GOM NHÓM (QUAN TRỌNG)
Vì có quá nhiều nguồn, đừng liệt kê từng nguồn. Hãy tìm ra ""Mẫu số chung"" và ""Sự khác biệt tử thần"":
- Điểm Đồng Thuận (Consensus): Tất cả 6 nguồn đều đồng ý về điều gì? (Ví dụ: Tất cả đều xác nhận Cầu thủ X chấn thương).
- Điểm Xung Đột (Conflict): Nguồn nào đi ngược lại đám đông? Tại sao? (Ví dụ: 5 nguồn nói Đội A thắng, nhưng Nguồn 6 nói Đội A sẽ buông).
-> Hãy tin vào nguồn có lý giải về ""Động lực"" và ""Bối cảnh"" tốt hơn là nguồn chỉ dựa vào thống kê quá khứ.

## BƯỚC 2: MA TRẬN CHẨN ĐOÁN (DIAGNOSTIC MATRIX)
Tạo bảng phân tích 3 yếu tố then chốt nhất quyết định trận này:
1. Động lực thực sự (Ai cần thắng hơn? Ai sợ thua hơn?).
2. Nhân sự & Giá trị thực chiến (Field Value) sau khi trừ đi chấn thương.
3. Yếu tố môi trường (Sân bãi, thời tiết, trọng tài, VAR).

## BƯỚC 3: KỊCH BẢN ""BLACK SWAN"" (THIÊN NGA ĐEN)
Dựa trên các báo cáo ""Deep Research"", hãy chỉ ra một kịch bản ít ai ngờ tới nhưng có xác suất xảy ra cao (Ví dụ: Đội mạnh cố tình thua, thẻ đỏ, vỡ trận sớm).

# ĐẦU RA YÊU CẦU: BÁO CÁO QUYẾT ĐỊNH CUỐI CÙNG
Hãy viết báo cáo theo cấu trúc sau, văn phong sắc sảo, dứt khoát, dùng từ ngữ chuyên ngành phân tích đầu tư:
1. Tóm tắt dành cho lãnh đạo (Executive Summary).
2. Cơ sở bác bỏ (Tại sao đám đông lại sai? Bác bỏ các luận điểm hời hợt).
3. Phân tích chi tiết (Điền vào bảng các yêu cầu: Giá trị đội hình, Phong độ, Động lực, Chấn thương, Tác động Thua/Hoà).
4. Lập luận thống nhất (Câu chuyện của trận đấu sẽ diễn ra thế nào?).
5. Dự đoán cuối cùng (Tỉ số, Kèo chính, Kèo phụ, Lời khuyên hành động).";

            await Clipboard.Default.SetTextAsync(prompt);
            await DisplayAlert("Thành công", "Đã chép Prompt Tổng Hợp!", "OK");
        }
    }
}