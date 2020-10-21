using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
