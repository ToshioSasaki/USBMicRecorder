

namespace USBMicRecorder
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class ThickGroupBox : GroupBox
    {
        public float BorderThickness { get; set; } = 1f;  // お好みの太さ

        protected override void OnPaint(PaintEventArgs e)
        {
            // 背景・テキストは通常どおり
            base.OnPaint(e);

            SizeF stringSize = e.Graphics.MeasureString(this.Text, this.Font);
            float textPadding = 8f;
            float y = stringSize.Height / 2;

            // テキスト左右位置
            float textLeft = textPadding;
            float textRight = textLeft + stringSize.Width;

            // 枠線の太さに応じたオフセット
            float offset = BorderThickness / 2f;

            using (Pen lightPen = new Pen(Color.White, BorderThickness))       // ハイライト
            using (Pen darkPen = new Pen(Color.Gray, BorderThickness))         // シャドウ
            {
                lightPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                darkPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

                // 上辺（テキストを避けて2分割） - 明るく
                e.Graphics.DrawLine(lightPen, 0f, y, textLeft - 2f, y);
                e.Graphics.DrawLine(lightPen, textRight + 2f, y, this.Width - 1f, y);

                // 左辺（明るく）
                e.Graphics.DrawLine(lightPen, 0f, y, 0f, this.Height - 1f);

                // 下辺（暗く）
                e.Graphics.DrawLine(darkPen, 0f, this.Height - 1f, this.Width - 1f, this.Height - 1f);

                // 右辺（暗く）
                e.Graphics.DrawLine(darkPen, this.Width - 1f, y, this.Width - 1f, this.Height - 1f);
            }
        }
    }
}

