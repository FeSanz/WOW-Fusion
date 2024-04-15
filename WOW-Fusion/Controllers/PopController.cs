using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;

namespace WOW_Fusion
{
    internal class PopController
    {
        static frmLoading FormLoading;
        Thread threadLoading;

        public void Show()
        {
            threadLoading = new Thread(new ThreadStart(LoadingProcess));
            threadLoading.Start();
        }
        public void Show(Form parent)
        {
            threadLoading = new Thread(new ParameterizedThreadStart(LoadingProcess));
            threadLoading.Start(parent);
        }

        public void Close()
        {
            if(FormLoading != null)
            {
                FormLoading.BeginInvoke(new ThreadStart(FormLoading.LoadingClose));
                FormLoading = null;
                threadLoading = null;
                Constants.pop = "Procesando...";
            }
        }

        private void LoadingProcess()
        {
            FormLoading = new frmLoading();
            FormLoading.ShowDialog();
        }

        private void LoadingProcess(object parent)
        {
            Form parent1 = parent as Form;
            FormLoading = new frmLoading(parent1);
            FormLoading.ShowDialog();
        }
    }
}
