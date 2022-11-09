using SkiaSharp;

namespace ClickableCaptcha.Questions
{
    public abstract class AbsQuestion : ICaptchaQuestion
    {
        readonly Random _random;
        readonly (string, SKColor)[] _colorDict;

        string questionName;

        protected AbsQuestion((string, SKColor)[] colorDict)
        {
            _random = new Random();
            _colorDict = colorDict;
        }

        public abstract (string, string)[] CandidateList { get; }

        public abstract bool CheckAnswerValue { get; }

        public CapthcaPoint[] DrawAnswerCandidate(
            SKCanvas canvas, CapthcaPoint[] candidatePositions, int width, int height, string fontFamily, int fontSize = 21, bool dysopsia = false)
        {
            using (SKPaint drawStyle = new SKPaint())
            {
                Random random = new Random();
                drawStyle.IsAntialias = true;
                drawStyle.Typeface = SKTypeface.FromFamilyName(fontFamily, SKFontStyleWeight.Bold, SKFontStyleWidth.ExtraCondensed, SKFontStyleSlant.Upright);
                drawStyle.TextSize = fontSize;

                List<CapthcaPoint> anwserResult = new List<CapthcaPoint>();
                ((string colorName, SKColor color) colorInfo, (string questionName, string answerValue) questionInfo)[] candidateAnswer =
                    GenerateAnswerCandidate(candidatePositions.Length).ToArray();

                for (int i = 0; i < candidatePositions.Length; i++)
                {
                    if (CheckAnswerValue)
                    {
                        if (candidateAnswer[i].questionInfo.questionName == candidateAnswer[0].questionInfo.questionName &&
                            candidateAnswer[i].colorInfo.colorName == candidateAnswer[0].colorInfo.colorName &&
                            candidateAnswer[i].questionInfo.answerValue == candidateAnswer[0].questionInfo.answerValue)
                        {
                            anwserResult.Add(candidatePositions[i]);
                        }
                    }
                    else
                    {
                        if (candidateAnswer[i].questionInfo.questionName == candidateAnswer[0].questionInfo.questionName &&
                            candidateAnswer[i].colorInfo.colorName == candidateAnswer[0].colorInfo.colorName)
                        {
                            anwserResult.Add(candidatePositions[i]);
                        }
                    }

                    drawStyle.Color = candidateAnswer[i].colorInfo.color;

                    if (dysopsia)
                    {
                        //视觉障碍模式
                        canvas.DrawText(
                            candidateAnswer[i].questionInfo.answerValue,
                            candidatePositions[i].X + Convert.ToInt32(width * 0.25),
                            candidatePositions[i].Y - Convert.ToInt32(height * 0.5),//百分比越小就越下，越大就越上
                            drawStyle);

                        drawStyle.TextSize = Convert.ToInt32(fontSize * 0.5);

                        //直接打印颜色方便视觉障碍人士直接辨识
                        canvas.DrawText(
                            candidateAnswer[i].colorInfo.colorName,
                            candidatePositions[i].X + Convert.ToInt32(width * 0.25),
                            candidatePositions[i].Y - Convert.ToInt32(height * 0.1),
                            drawStyle);

                        drawStyle.TextSize = fontSize;
                    }
                    else
                        canvas.DrawText(
                            candidateAnswer[i].questionInfo.answerValue,
                            candidatePositions[i].X + Convert.ToInt32(width * 0.25),
                            candidatePositions[i].Y - Convert.ToInt32(height * 0.25),
                            drawStyle);
                }

                questionName = $"请点击网格内 {anwserResult.Count} 个 {candidateAnswer[0].colorInfo.colorName} 的 {candidateAnswer[0].questionInfo.questionName}";

                return anwserResult.ToArray();
            }
        }

        private IEnumerable<((string, SKColor), (string, string))> GenerateAnswerCandidate(int count)
        {
            for (int i = 0; i < count; i++)
            {
                (string, SKColor) color = _colorDict[_random.Next(0, _colorDict.Length - 1)];
                (string, string) shape = CandidateList[_random.Next(0, CandidateList.Length - 1)];

                yield return (color, shape);
            }
        }

        public string GetQuestionName()
        {
            return questionName;
        }
    }
}
