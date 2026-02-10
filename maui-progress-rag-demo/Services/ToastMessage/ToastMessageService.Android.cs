#if ANDROID
using Android.App;
using Android.Widget;

namespace MauiProgressRagDemo.Services;

partial class ToastMessageService
{
    public void ShortAlert(string message)
    {
        var toast = Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short);
        toast.Show();
    }
}
#endif