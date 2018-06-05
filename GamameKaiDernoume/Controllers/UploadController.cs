using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IHostingEnvironment env;
        private readonly UserManager<User> userManager;

        public UploadController(IHostingEnvironment env,
            UserManager<User> userManager)
        {
            this.env = env;
            this.userManager = userManager;
        }
        
        [HttpPost]
        public async Task UserAvatar(IFormFile image)
        {
            // full path to file in temp location
            if (image != null && image.Length > 0)
            {
                if (image.ContentType.Contains("png") || image.ContentType.Contains("jpg") || image.ContentType.Contains("jpeg"))
                {
                    string userID = userManager.GetUserId(HttpContext.User);
                    var filePath = env.WebRootPath + "\\userAvatars\\" + userID + '.' + image.ContentType.Split('/')[1];

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    var user = await userManager.GetUserAsync(User);
                    user.UserAvatar = "/userAvatars/" + userID + '.' + image.ContentType.Split('/')[1];
                    var updateProfileResult = await userManager.UpdateAsync(user);
                }
            }
            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            Response.Redirect("/Identity/Account/Manage");
        }

    }
}