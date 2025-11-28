using Microsoft.Extensions.Caching.Distributed;

using WaveSent.ClickableCaptcha;

namespace WebDemo.Services
{
    public class ClickableCaptchaService : ICaptchaService
    {
        const string CaptchaCacheKeyFormat = "CAPTCHA_{0}";
        const int GridSize = 40;

        readonly IDistributedCache _cache;

        public ClickableCaptchaService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<CaptchaPayload> GenerateCaptchaAsync(bool dysopsia)
        {
            string cid = Guid.NewGuid().ToString("N");

            var questions = CaptchaGenerate
                .GetDefaultQuestions();

            (byte[] imageBytes, string answer) = new CaptchaGenerate(questions, GridSize)
                .GetCaptcha(new Random().Next(questions.Length), dysopsia);

            await _cache.SetStringAsync(
                string.Format(CaptchaCacheKeyFormat, cid), $"{answer}|0");

            return new CaptchaPayload(imageBytes, cid);
        }

        public async Task<bool> VerifyCaptchaAsync(string captchaId, string answer, int retryMax = 5)
        {
            string cacheKey = string.Format(
                CaptchaCacheKeyFormat, captchaId);

            string? realAnswer = await _cache
                .GetStringAsync(cacheKey);
            if (realAnswer == null) return false;

            string[] realAnswerSplited = realAnswer.Split('|');

            string realAnswerPair = realAnswerSplited[0];
            int retryCount = Convert.ToInt32(realAnswerSplited[1]);

            bool match = new CaptchaValidator(realAnswerPair)
                .Checking(answer, GridSize);
            if (match)
            {
                //完全匹配，删除验证数据
                await _cache.RemoveAsync(cacheKey);
                return true;
            }
            else if (retryCount < retryMax)
            {
                //不匹配，但小于最多重试次数，更新错误重试次数
                await _cache.SetStringAsync(
                    cacheKey, $"{realAnswerPair}|{retryCount + 1}");
                return false;
            }
            else
            {
                //不匹配，且大于最多重试次数，删除验证数据
                await _cache.RemoveAsync(cacheKey);
                return false;
            }
        }
    }
}
