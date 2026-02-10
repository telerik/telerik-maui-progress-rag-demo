namespace MauiProgressRagDemo.Services;

public partial class ToastMessageService : IToastMessageService
{
#if WINDOWS || MACCATALYST
    public void ShortAlert(string message)
    {
    }
#endif
}