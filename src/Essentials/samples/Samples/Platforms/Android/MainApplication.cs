using System;
using Android.App;
using Android.Runtime;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Samples.Droid
{
	[Application]
	public class MainApplication : MauiApplication
	{
		public MainApplication(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
		{
		}

		protected override MauiAppBuilder CreateAppBuilder() => MauiProgram.CreateAppBuilder();
	}
}