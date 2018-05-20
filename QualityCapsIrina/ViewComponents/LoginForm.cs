using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityCapsIrina.Models.ViewModels;

namespace QualityCapsIrina.ViewComponents
{
    public class LoginForm : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(new LoginViewModel());
        }
    }
}
