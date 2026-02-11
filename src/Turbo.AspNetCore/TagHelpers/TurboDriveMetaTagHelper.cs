using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Turbo.AspNetCore.TagHelpers
{
    /// <summary>
    /// Turbo Drive の動作を制御するメタタグを生成
    /// </summary>
    [HtmlTargetElement("turbo-drive-meta")]
    public class TurboDriveMetaTagHelper : TagHelper
    {
        /// <summary>
        /// Turbo Drive を有効/無効にする (デフォルト: true)
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// ページ遷移時のアニメーション (デフォルト: "")
        /// 指定可能な値: "fade", "slide", "none"
        /// </summary>
        public string Transition { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null; // タグ自体は出力しない
            output.TagMode = TagMode.StartTagAndEndTag;

            var sb = new StringBuilder();

            // Turbo Drive の有効/無効
            // "advance": Turbo Drive を有効化（デフォルト）
            // "reload": 完全なページリロードを強制
            sb.AppendLine($"<meta name=\"turbo-visit-control\" content=\"{(Enabled ? "advance" : "reload")}\">");

            // Turbo 8 の新機能: Page Refresh Method
            sb.AppendLine("<meta name=\"turbo-refresh-method\" content=\"morph\">");
            sb.AppendLine("<meta name=\"turbo-refresh-scroll\" content=\"preserve\">");

            if (!string.IsNullOrEmpty(Transition))
            {
                sb.AppendLine($"<meta name=\"turbo-transition\" content=\"{Transition}\">");
            }

            output.Content.SetHtmlContent(sb.ToString());
        }
    }
}
