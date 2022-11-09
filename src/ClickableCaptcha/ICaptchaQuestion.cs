using SkiaSharp;

namespace ClickableCaptcha
{
    /// <summary>
    /// 图形验证码提问接口，可通过继承此接口扩展更多问题
    /// </summary>
    public interface ICaptchaQuestion
    {
        string GetQuestionName();

        /// <summary>
        /// 绘制候选答案（就是画在格子里面的东西）
        /// </summary>
        /// <param name="canvas">画布</param>
        /// <param name="candidatePositions">分配给你的候选答案位置，把你的候选答案绘制在该位置上就行了</param>
        /// <param name="width">候选答案应该要绘制宽</param>
        /// <param name="height">候选答案应该要绘制高</param>
        /// <param name="fontFamily">候选答案需要使用的字体名称</param>
        /// <param name="fontSize">候选答案需要使用的字体大小</param>
        /// <param name="dysopsia">该候选答案是否开启了视觉障碍模式</param>
        /// <returns></returns>
        CapthcaPoint[] DrawAnswerCandidate(
            SKCanvas canvas,
            CapthcaPoint[] candidatePositions,
            int width,
            int height,
            string fontFamily,
            int fontSize = 21,
            bool dysopsia = false);
    }
}
