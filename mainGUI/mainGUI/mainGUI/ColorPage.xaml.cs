﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mainGUI
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ColorPage : Rg.Plugins.Popup.Pages.PopupPage
	{
		public ColorPage ()
		{
			InitializeComponent ();
		}
	}
}