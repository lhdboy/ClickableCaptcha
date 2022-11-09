namespace ClickableCaptcha
{
    /// <summary>
    /// 图形验证码验证器
    /// </summary>
    public class CaptchaValidator
    {
        readonly CapthcaPoint[] _answerList;

        /// <summary>
        /// 构造图形验证码验证器
        /// </summary>
        /// <param name="realAnswer">真实答案</param>
        /// <exception cref="ArgumentNullException">真实答案不能为NULL</exception>
        public CaptchaValidator(string realAnswer)
        {
            if (string.IsNullOrEmpty(realAnswer))
                throw new ArgumentNullException(nameof(realAnswer));

            _answerList = ToCapthcaPointArray(realAnswer);
        }

        /// <summary>
        /// 根据网格大小和用户答案检查是否正确
        /// </summary>
        /// <param name="userAnswer"></param>
        /// <param name="GridSize"></param>
        /// <returns></returns>
        public bool Checking(string userAnswer, int GridSize)
        {
            if (_answerList == null)
                return false;

            if (string.IsNullOrEmpty(userAnswer))
                return false;

            CapthcaPoint[] inputs = ToCapthcaPointArray(userAnswer);
            if (inputs.Length != _answerList.Length)
                return false;

            return _answerList.All(
                answer => inputs.Any(
                    input => answer.X <= input.X && answer.X + GridSize >= input.X && answer.Y - GridSize <= input.Y && answer.Y >= input.Y));
        }

        private CapthcaPoint[] ToCapthcaPointArray(string code)
        {
            return code.Replace("\"", "")
                .Split(';')
                .Select(p =>
                {
                    string[] xy = p.Split(',');
                    if (xy.Length != 2)
                        return new CapthcaPoint();

                    if (int.TryParse(xy[0], out int inputX) && int.TryParse(xy[1], out int inputY))
                        return new CapthcaPoint() { X = inputX, Y = inputY };
                    else
                        return new CapthcaPoint();
                })
                .ToArray();
        }
    }
}
