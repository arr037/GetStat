using System;

namespace GetStat.Services
{
    public class ModalService
    {
        public event Action<string, string> OnModalWindowChanged;

        public void ShowModalWindow(string title, string text)
        {
            OnModalWindowChanged?.Invoke(title, text);
        }

        public void HideModalWindow()
        {
            OnModalWindowChanged?.Invoke("", "");
        }
    }
}