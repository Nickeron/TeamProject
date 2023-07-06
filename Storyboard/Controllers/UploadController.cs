using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeamProject.Data.Entities;

namespace TeamProject.Controllers
{
    [Authorize]
    public class UploadController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly UserManager<User> _userManager;

        public UploadController(IHostingEnvironment env,
            UserManager<User> userManager)
        {
            _env = env;
            _userManager = userManager;
        }
        
        [HttpPost]
        public async Task UserAvatar(IFormFile image)
        {
            // full path to file in temp location
            if (image is { Length: > 0 })
            {
                if (image.ContentType.Contains("png") || image.ContentType.Contains("jpg") || image.ContentType.Contains("jpeg"))
                {
                    var userId = _userManager.GetUserId(HttpContext.User);
                    var filePath = _env.WebRootPath + "\\userAvatars\\" + userId + '.' + image.ContentType.Split('/')[1];

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    var user = await _userManager.GetUserAsync(User);
                    user.UserAvatar = "/userAvatars/" + userId + '.' + image.ContentType.Split('/')[1];
                    var updateProfileResult = await _userManager.UpdateAsync(user);
                }
            }
            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            Response.Redirect("/Identity/Account/Manage");
        }

    }
}