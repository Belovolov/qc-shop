using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityCapsIrina.Models.ViewModels;

namespace QualityCapsIrina.ViewComponents
{
    public class RegisterForm : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(new RegisterViewModel());
        }
    }
}
