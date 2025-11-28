using SkiaSharp;

namespace WaveSent.ClickableCaptcha.Questions
{
    /// <summary>
    /// 形状问题，通过四种毕竟明显的形状让用户找到对应的形状图案
    /// </summary>
    public class ShapeQuestion : AbsQuestion
    {
        public ShapeQuestion((string, SKColor)[] colorDict)
            : base(colorDict)
        {

        }

        public override (string, string)[] CandidateList => new (string, string)[]
            {
                ("实心三角形","▲"),
                ("空心三角形","△"),
                ("实心正方形","■"),
                ("空心正方形","□"),
                ("实心圆形","●"),
                ("空心圆形","○"),
                ("实心菱形","◆"),
                ("空心菱形","◇"),
            };

        public override bool CheckAnswerValue => true;
    }
}
