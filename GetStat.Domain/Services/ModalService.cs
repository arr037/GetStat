using System;

namespace GetStat.Services
{
    public class ModalService
    {
        public event Action<string, string,ModalButton> OnModalWindowChanged;

        public void ShowModalWindow(string title, string text)
        {
            OnModalWindowChanged?.Invoke(title, text,ModalButton.Ok);
        }

        public void ShowModalWindow(string title, string text, ModalButton button)
        {
            OnModalWindowChanged?.Invoke(title, text, button);
        }

        public void HideModalWindow()
        {
            OnModalWindowChanged?.Invoke("", "",ModalButton.Ok);
        }
    }

    public enum ModalButton
    {
        None,
        Ok,
        OkNo,
        Cancel
    }
}