namespace WebDemo.Services
{
    public interface ICaptchaService
    {
        /// <summary>
        /// 生成图形验证码
        /// </summary>
        /// <param name="identify"></param>
        /// <param name="dysopsia"></param>
        /// <returns></returns>
        Task<CaptchaPayload> GenerateCaptchaAsync(bool dysopsia);

        /// <summary>
        /// 验证图形验证码
        /// </summary>
        /// <param name="captchaId"></param>
        /// <param name="answer"></param>
        /// <param name="retryMax"></param>
        /// <returns></returns>
        Task<bool> VerifyCaptchaAsync(string captchaId, string answer, int retryMax = 5);
    }

    public record class CaptchaPayload(
        byte[] ImageBytes, string CaptchaId);
}
