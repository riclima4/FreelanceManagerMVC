using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Blazor.Notifications;

namespace FreelanceManager.Services.Utils
{
    public class UtilsService : IUtilsService
    {
        public async Task ShowSuccessToast(SfToast ToastObj, string message)
        {
            await ToastObj.ShowAsync(new ToastModel
            {
                Content = message,
                CssClass = "e-toast-success",
                Timeout = 3000,
                Title = "Success",
            });
        }
    }
}