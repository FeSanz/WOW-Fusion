using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Tulpep.NotificationWindow;

namespace WOW_Fusion.Controllers
{
    internal class NotifierController
    {
        private static PopupNotifier pop = new PopupNotifier();
        public static void Success(string content)
        {
            pop.TitleText = "";
            pop.ContentText = content;
            pop.ContentColor = Color.Black;
            pop.ContentFont = new Font("Arial", 12);
            pop.ContentPadding = new Padding(10, 15, 10, 5);
            pop.ShowGrip = false;
            pop.HeaderHeight = 1;
            pop.BorderColor = Color.DarkGray; //Color.FromArgb(35, 35, 35);
            pop.Image = Properties.Resources.success;
            pop.ImageSize = new Size(70, 70);
            pop.ImagePadding = new Padding(10);
            pop.Size = new Size(350, 90);
            pop.IsRightToLeft = false;
            pop.Popup();
        }
        public static void Error(string content)
        {
            pop.TitleText = "";
            pop.ContentText = content;
            pop.ContentColor = Color.Black;
            pop.ContentFont = new Font("Arial", 12);
            pop.ContentPadding = new Padding(10, 15, 10, 5);
            pop.ShowGrip = false;
            pop.HeaderHeight = 1;
            pop.BorderColor = Color.DarkGray; //Color.FromArgb(35, 35, 35);
            pop.Image = Properties.Resources.error;
            pop.ImageSize = new Size(70, 70);
            pop.ImagePadding = new Padding(10);
            pop.Size = new Size(350, 90);
            pop.IsRightToLeft = false;
            pop.Popup();
        }
        public static void Warning(string content)
        {
            pop.TitleText = "";
            pop.ContentText = content;
            pop.ContentColor = Color.Black;
            pop.ContentFont = new Font("Arial", 12);
            pop.ContentPadding = new Padding(10, 15, 10, 5);
            pop.ShowGrip = false;
            pop.HeaderHeight = 1;
            pop.BorderColor = Color.DarkGray; //Color.FromArgb(35, 35, 35);
            pop.Image = Properties.Resources.warning;
            pop.ImageSize = new Size(70, 70);
            pop.ImagePadding = new Padding(10);
            pop.Size = new Size(350, 90);
            pop.IsRightToLeft = false;
            pop.Popup();
        }

        public static void DetailError(string title, string content)
        {
            pop.TitleText = title;
            pop.TitlePadding = new Padding(0, 10, 5, 0);
            pop.ContentFont = new Font("Tahoma", 9);
            pop.ContentText = content;
            pop.ContentFont = new Font("Tahoma", 9);
            pop.ContentPadding = new Padding(0, 5, 5, 5);
            pop.ShowCloseButton = true;
            pop.ShowOptionsButton = false;
            pop.ShowGrip = false;
            pop.HeaderHeight = 1;
            pop.Delay = 15000;
            pop.AnimationInterval = 10;
            pop.AnimationDuration = 500;
            pop.Image = Properties.Resources.error;
            pop.ImageSize = new Size(70, 70);
            pop.ImagePadding = new Padding(15);
            pop.Scroll = true;
            pop.IsRightToLeft = false;
            pop.Popup();
        }

        public static void DetailWarning(string title, string content)
        {
            pop.TitleText = title;
            pop.TitlePadding = new Padding(0, 10, 5, 0);
            pop.ContentFont = new Font("Tahoma", 9);
            pop.ContentText = content;
            pop.ContentFont = new Font("Tahoma", 9);
            pop.ContentPadding = new Padding(0, 5, 5, 5);
            pop.ShowCloseButton = true;
            pop.ShowOptionsButton = false;
            pop.ShowGrip = false;
            pop.HeaderHeight = 1;
            pop.Delay = 15000;
            pop.AnimationInterval = 10;
            pop.AnimationDuration = 500;
            pop.Image = Properties.Resources.warning;
            pop.ImageSize = new Size(70, 70);
            pop.ImagePadding = new Padding(10);
            pop.Scroll = true;
            pop.IsRightToLeft = false;
            pop.Popup();
        }
    }
}
