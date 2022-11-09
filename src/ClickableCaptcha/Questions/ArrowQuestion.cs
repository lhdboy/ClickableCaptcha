using SkiaSharp;

namespace ClickableCaptcha.Questions
{
    /// <summary>
    /// 箭头问题，通过不同方向的箭头让用户根据文字判断正确方向的箭头
    /// </summary>
    public class ArrowQuestion : AbsQuestion
    {
        public ArrowQuestion((string, SKColor)[] colorDict)
            : base(colorDict)
        {

        }

        public override (string, string)[] CandidateList => new (string, string)[]
            {
                ("向上箭头","↑"),
                ("向下箭头","↓"),
                ("向左箭头","←"),
                ("向右箭头","→"),
            };

        public override bool CheckAnswerValue => true;
    }
}
