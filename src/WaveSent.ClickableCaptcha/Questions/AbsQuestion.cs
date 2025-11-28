using SkiaSharp;

namespace WaveSent.ClickableCaptcha.Questions
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
            SKCanvas canvas, CapthcaPoint[] candidatePositions, int width, int height, string fontFileName, int fontSize = 21, bool dysopsia = false)
        {
            Random random = new Random();

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

                if (dysopsia)
                {
                    //视觉障碍模式
                    DrawText(
                        canvas: canvas,
                        text: candidateAnswer[i].questionInfo.answerValue,
                        fontFilename: fontFileName,
                        size: fontSize,
                        x: candidatePositions[i].X + (width * 0.5f),
                        y: candidatePositions[i].Y - (height * 0.5f),//百分比越小就越下，越大就越上
                        textAlign: SKTextAlign.Center,
                        color: candidateAnswer[i].colorInfo.color);

                    //直接打印颜色方便视觉障碍人士直接辨识
                    DrawText(
                        canvas: canvas,
                        text: candidateAnswer[i].colorInfo.colorName,
                        fontFilename: fontFileName,
                        size: fontSize * 0.5f,
                        x: candidatePositions[i].X + (width * 0.5f),
                        y: candidatePositions[i].Y - (height * 0.1f),
                        textAlign: SKTextAlign.Center,
                        color: candidateAnswer[i].colorInfo.color);
                }
                else
                    DrawText(
                        canvas: canvas,
                        text: candidateAnswer[i].questionInfo.answerValue,
                        fontFilename: fontFileName,
                        size: fontSize,
                        x: candidatePositions[i].X + (width * 0.5f),
                        y: candidatePositions[i].Y - (height * 0.28f),
                        textAlign: SKTextAlign.Center,
                        color: candidateAnswer[i].colorInfo.color);
            }

            questionName = $"点击网格内 {anwserResult.Count} 个 {candidateAnswer[0].colorInfo.colorName} 的 {candidateAnswer[0].questionInfo.questionName}";

            return [.. anwserResult];
        }

        static void DrawText(
            SKCanvas canvas,
            string text,
            string fontFilename,
            float size,
            float x,
            float y,
            SKTextAlign textAlign,
            SKColor color)
        {
            using SKFont font = new SKFont(SKTypeface.FromFile(fontFilename), size);
            using SKPaint paint = new SKPaint();
            paint.Color = color;

            canvas.DrawText(text, x, y, textAlign, font, paint);
        }


        IEnumerable<((string, SKColor), (string, string))> GenerateAnswerCandidate(int count)
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
