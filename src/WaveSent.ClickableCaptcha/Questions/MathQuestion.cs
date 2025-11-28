using SkiaSharp;

namespace WaveSent.ClickableCaptcha.Questions
{
    /// <summary>
    /// 数据问题，通过简单的数学问题让用户思考正确的答案（此类还可以通过随机方式扩展）
    /// </summary>
    public class MathQuestion : AbsQuestion
    {
        public MathQuestion((string, SKColor)[] colorDict)
                    : base(colorDict)
        {

        }

        public override (string, string)[] CandidateList => new (string, string)[]
            {
                ("3的倍数","3"),
                ("3的倍数","6"),
                ("3的倍数","9"),
                ("3的倍数","12"),
                ("3的倍数","18"),
                ("3的倍数","21"),
                ("3的倍数","24"),
                ("3的倍数","27"),
                ("3的倍数","42"),

                ("5的倍数","5"),
                ("5的倍数","10"),
                ("5的倍数","20"),
                ("5的倍数","25"),
                ("5的倍数","35"),
                ("5的倍数","40"),
                ("5的倍数","45"),
            };

        public override bool CheckAnswerValue => false;
    }
}
