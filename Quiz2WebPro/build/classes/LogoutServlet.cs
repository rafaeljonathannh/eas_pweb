using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace WebEAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Menghapus session pengguna jika ada
            HttpContext.Session.Clear();

            // Mengarahkan pengguna ke halaman login
            return RedirectToAction("Login", "Account"); // Menyesuaikan dengan controller dan action login Anda
        }
    }
}
