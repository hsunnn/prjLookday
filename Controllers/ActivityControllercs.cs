using Microsoft.AspNetCore.Mvc;
using prjLookday.Models;
using prjLookday.ViewModels;

namespace prjLookday.Controllers
{
    public class ActivityControllercs : SuperController
    {
        public IActionResult List(CKeywordViewModel vm)
        {
            lookdaysContext db = new lookdaysContext();

            IEnumerable<Activity> datas = null;

            if (string.IsNullOrEmpty(vm.txtKeyword))
                datas = from activity in db.Activities
                        select activity;
            else
                datas = db.Activities.Where(r => r.Name.Contains(vm.txtKeyword) ||
                r.Description.Contains(vm.txtKeyword));
            return View(datas);
        }

    }
}
 