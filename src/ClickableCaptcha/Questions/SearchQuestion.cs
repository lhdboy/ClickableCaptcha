using SkiaSharp;

namespace ClickableCaptcha.Questions
{
    /// <summary>
    /// 字母大小写搜索问题，让用户根据提示找到对应的大小写字母
    /// </summary>
    public class SearchQuestion : AbsQuestion
    {
        public SearchQuestion((string, SKColor)[] colorDict)
            : base(colorDict)
        {

        }

        public override (string, string)[] CandidateList => new (string, string)[]
            {
                ("小写字母A","a"),
                ("小写字母B","b"),
                ("小写字母D","d"),
                ("小写字母N","n"),
                ("小写字母E","e"),
                ("小写字母F","f"),
                ("小写字母G","g"),
                ("小写字母Q","q"),
                ("小写字母R","r"),
                ("小写字母T","t"),
                ("小写字母Y","y"),
                ("小写字母H","h"),
                ("大写字母a","A"),
                ("大写字母b","B"),
                ("大写字母d","D"),
                ("大写字母n","N"),
                ("大写字母e","E"),
                ("大写字母f","F"),
                ("大写字母g","G"),
                ("大写字母q","Q"),
                ("大写字母r","R"),
                ("大写字母t","T"),
                ("大写字母y","Y"),
                ("大写字母h","H"),
            };

        public override bool CheckAnswerValue => true;
    }
}
