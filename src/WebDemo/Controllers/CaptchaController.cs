using Microsoft.AspNetCore.Mvc;

using WebDemo.Services;

namespace WebDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CaptchaController : ControllerBase
    {
        readonly ICaptchaService _captchaService;

        public CaptchaController(ICaptchaService captchaService)
        {
            _captchaService = captchaService;
        }

        /// <summary>
        /// 获取图形验证码
        /// </summary>
        /// <param name="dysopsia">视觉障碍模式是否打开</param>
        /// <returns>图形验证码</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool dysopsia = false)
        {
            CaptchaPayload captcha = await _captchaService
                .GenerateCaptchaAsync(dysopsia);

            HttpContext.Session.SetString("cid", captcha.CaptchaId);
            HttpContext.Response.Headers
                .Add("X-Content-Type-Options", "nosniff");

            return File(captcha.ImageBytes, "image/png");
        }

        /// <summary>
        /// 获取图形验证码
        /// </summary>
        /// <param name="dysopsia">视觉障碍模式是否打开</param>
        /// <returns>图形验证码</returns>
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> Check([FromQuery] string answer)
        {
            string? captchaId = HttpContext.Session.GetString("cid");
            if (captchaId == null)
                return BadRequest();

            bool result = await _captchaService
                .VerifyCaptchaAsync(captchaId, answer);

            return result ? "成功，完全匹配" : "错了~~~~";
        }
    }
}
