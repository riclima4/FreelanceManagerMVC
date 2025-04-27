using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Blazor.Notifications;

namespace FreelanceManager.Services.Utils
{
    public interface IUtilsService
    {
        Task ShowSuccessToast(SfToast ToastObj, string message);
        Task ShowErrorToast(SfToast ToastObj, string message);

    }
}