using SkiaSharp;

using WaveSent.ClickableCaptcha.Questions;

namespace WaveSent.ClickableCaptcha
{
    public class CaptchaGenerate
    {
        public static (string colorName, SKColor color)[] ColorDict = new (string colorName, SKColor color)[]
        {
            ("红色",SKColors.Red),//红色
            ("绿色",SKColors.Green),//绿色
            ("蓝色",SKColors.Blue),//蓝色
            ("红色",SKColors.Red),//红色
            ("绿色",SKColors.Green),//绿色
            ("蓝色",SKColors.Blue),//蓝色
            ("红色",SKColors.Red),//红色
            ("绿色",SKColors.Green),//绿色
            ("蓝色",SKColors.Blue),//蓝色
            ("红色",SKColors.Red),//红色
            ("绿色",SKColors.Green),//绿色
            ("蓝色",SKColors.Blue),//蓝色
            //("白色",SKColors.Yellow),//白色
            //("黑色",SKColors.Black),//黑色
            //("粉色",SKColors.Pink),//粉色
            //("紫色",SKColors.Purple),//紫色
            //("灰色",SKColors.Gray),//灰色
        };

        readonly ICaptchaQuestion[] _questions;
        readonly Random _random;

        readonly int _gridCount;
        readonly int _gridMargin;
        readonly int _gridSize;
        readonly int _gridStrokeWidth;
        readonly int _textFontSize;
        readonly string _fontFileName;

        /// <summary>
        /// 图形验证码生成器（修改此构造函数的数值需要变更前端硬编码的宽高参数）
        /// </summary>
        /// <param name="questions">问题列表，可自定义和扩展增加，建议不要太多</param>
        /// <param name="gridSize">网格单个格子的大小（像素）</param>
        /// <param name="gridMargin">网格外边距大小（像素）</param>
        /// <param name="gridCount">网格数量（gridCount * gridCount）</param>
        /// <param name="gridStrokeWidth">网格变宽宽度</param>
        /// <param name="textFontSize">默认字体大小</param>
        /// <param name="fontFileName">字体名称，建议不要使用微软雅黑，如果绘制出来的验证码都是“口口口”那就要换成你机子里支持的字体名称</param>
        public CaptchaGenerate(ICaptchaQuestion[] questions,
            int gridSize = 30,
            int gridMargin = 20,
            int gridCount = 6,
            int gridStrokeWidth = 1,
            int textFontSize = 16,
            string fontFileName = "Data/NotoSansCJKsc-Bold.otf")
        {
            _questions = questions;
            _random = new Random();

            _gridSize = gridSize;
            _gridMargin = gridMargin;
            _gridCount = gridCount;
            _gridStrokeWidth = gridStrokeWidth;
            _textFontSize = textFontSize;
            _fontFileName = fontFileName;
        }

        public static ICaptchaQuestion[] GetDefaultQuestions()
        {
            return new ICaptchaQuestion[]
            {
                new MathQuestion(ColorDict),
                new ShapeQuestion(ColorDict),
                new SearchQuestion(ColorDict),
                new ArrowQuestion(ColorDict),
            };
        }

        /// <summary>
        /// 获取图形验证码
        /// </summary>
        /// <param name="questionIndex">采纳的问题索引</param>
        /// <param name="dysopsia">视觉障碍模式</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public (byte[], string) GetCaptcha(int questionIndex, bool dysopsia)
        {
            if (questionIndex >= _questions.Length)
                throw new IndexOutOfRangeException();

            //保存答案坐标
            CapthcaPoint[]? answerList = null;

            int width = (_gridSize * _gridCount) + (_gridMargin * 2);//宽 = （网格大小 * 网格数量） + （边距 * 左边两边）
            int height = width + (_textFontSize * 2);//高 = 宽 + （底部说明文本字体大小 * 2）

            //创建bitmap位图
            using SKBitmap image2d = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            //创建画笔
            using SKCanvas canvas = new SKCanvas(image2d);
            //填充背景颜色为白色
            canvas.DrawColor(SKColors.White);

            CapthcaPoint[] positions;

            //绘制边框和网格
            using (SKPaint borderPaint = new SKPaint())
            {
                borderPaint.Color = SKColors.Black;
                borderPaint.StrokeWidth = _gridStrokeWidth;

                //绘制正方体边框
                positions = DrawRectGrid(canvas, borderPaint);
            }

            //打乱位置
            Queue<CapthcaPoint> positionQueues = DisturbPosition(positions);

            for (int i = 0; i < _questions.Length; i++)
            {
                //绘制候选答案
                var answer = _questions[i].DrawAnswerCandidate(
                    canvas,
                    AssignPosition(positionQueues, positions.Length, i == _questions.Length - 1).ToArray(),
                    _gridSize,
                    _gridSize,
                    _fontFileName,
                    Convert.ToInt32(_gridSize * 0.5),
                    dysopsia);
                if (i == questionIndex)
                    answerList = answer;
            }

            //绘制问题标题
            DrawQuesitonTitle(
                canvas,
                width / 2f,
                (_gridSize * _gridCount) + (_gridMargin * 2) + _textFontSize,
                _questions[questionIndex].GetQuestionName());

            //TODO: 还可以增加噪点和随机线条以增强机器识别难度

            //返回图片byte
            using SKImage img = SKImage.FromBitmap(image2d);
            using SKData p = img.Encode(SKEncodedImageFormat.Png, 100);

            return (p.ToArray(), SerializeAnswerList(answerList));
        }

        private string SerializeAnswerList(CapthcaPoint[] points)
        {
            return string.Join(';', points.Select(p => $"{p.X},{p.Y}"));
        }

        /// <summary>
        /// 打乱位置以队列方式返回
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        private Queue<CapthcaPoint> DisturbPosition(CapthcaPoint[] positions)
        {
            (int x, int y, int rand)[] rands = positions.Select(p => (p.X, p.Y, _random.Next(1000, 9999))).ToArray();

            return new Queue<CapthcaPoint>(rands.OrderByDescending(p => p.rand).Select(p => new CapthcaPoint() { X = p.x, Y = p.y }));
        }

        /// <summary>
        /// 分配坐标
        /// </summary>
        /// <param name="positionQueues"></param>
        /// <param name="totalPosition"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        private IEnumerable<CapthcaPoint> AssignPosition(Queue<CapthcaPoint> positionQueues, int totalPosition, bool last)
        {
            if (last)
            {
                while (positionQueues.TryDequeue(out CapthcaPoint pos))
                {
                    yield return pos;
                }
            }
            else
            {
                int min = totalPosition / _questions.Length / 2;
                int max = (totalPosition / _questions.Length) + 3;

                int dequeueCount = _random.Next(min, max);
                for (int i = 0; i < dequeueCount; i++)
                {
                    if (positionQueues.TryDequeue(out CapthcaPoint pos))
                    {
                        yield return pos;
                    }
                }
            }
        }

        /// <summary>
        /// 绘制问题标题
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="text"></param>
        private void DrawQuesitonTitle(SKCanvas canvas, float x, float y, string text)
        {
            using SKFont font = new SKFont(SKTypeface.FromFile(_fontFileName), _textFontSize);
            using SKPaint paint = new SKPaint();
            paint.Color = ColorDict[_random.Next(0, ColorDict.Length - 1)].color;

            canvas.DrawText(text, x, y, SKTextAlign.Center, font, paint);
        }

        /// <summary>
        /// 绘制正方形网格
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="borderPaint"></param>
        /// <returns></returns>
        private CapthcaPoint[] DrawRectGrid(SKCanvas canvas, SKPaint borderPaint)
        {
            List<CapthcaPoint> positions = new List<CapthcaPoint>(_gridCount * _gridCount);

            //绘制网格（横线）
            for (int i = 0; i <= _gridCount; i++)
            {
                canvas.DrawLine(
                    new SKPoint(_gridMargin, (i * _gridSize) + _gridMargin),
                    new SKPoint((_gridSize * _gridCount) + _gridMargin, (i * _gridSize) + _gridMargin),
                    borderPaint);
            }

            //绘制网格（竖线）
            for (int i = 0; i <= _gridCount; i++)
            {
                canvas.DrawLine(
                    new SKPoint((i * _gridSize) + _gridMargin, _gridMargin),
                    new SKPoint((i * _gridSize) + _gridMargin, (_gridSize * _gridCount) + _gridMargin + _gridStrokeWidth),
                    borderPaint);
            }

            for (int x = 0; x < _gridCount; x++)
            {
                for (int y = 0; y < _gridCount; y++)
                {
                    positions.Add(new CapthcaPoint()
                    {
                        X = (x * _gridSize) + _gridMargin,
                        Y = (y * _gridSize) + _gridMargin + _gridSize
                    });
                }
            }

            return [.. positions];
        }
    }
}
