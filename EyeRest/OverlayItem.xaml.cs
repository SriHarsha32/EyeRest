/*
 * Created by SharpDevelop.
 * User: srihas
 * Date: 08-06-2018
 * Time: 02:34 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace EyeRest
{
	/// <summary>
	/// Interaction logic for OverlayItem.xaml
	/// </summary>
	public partial class OverlayItem : Window
	{
		Window1 mainWindow;
		Overlay ovly;
		int breakSeconds;
		int currentBreakTimer;
		bool canEscape = false;
		bool isLongBreak = false;
		
		int minuteText = 0, secondText=0;
		Timer myTimer = new Timer();
		
		public OverlayItem(Window1 parent)
		{
			InitializeComponent();
			mainWindow = parent;
			myTimer.Interval = 1000;
			myTimer.Tick += myTimer_Tick;
			overlayCreate();
		}
		
		public void takeBreak(int breakSeconds, bool isLongBreak, bool canEscape)
		{
			mainWindow.Visibility = Visibility.Collapsed;
			ovly.Visibility = Visibility.Visible;
			ovly.Topmost = true;
			Visibility = Visibility.Visible;
			Topmost = true;
			currentBreakTimer = 0;
			this.breakSeconds = breakSeconds;
			this.canEscape = canEscape;
			this.isLongBreak = isLongBreak;
			
			if(breakSeconds>=60){
				minuteText = breakSeconds / 60;
				secondText = breakSeconds % 60;
			}else{
				minuteText = 0;
				secondText = breakSeconds;
			}
			timeRemText.Text = minuteText+":"+secondText;
			escapeBtn.IsEnabled = canEscape;
			
			string breakTime = (breakSeconds>60)?(breakSeconds/60)+" mins":(breakSeconds)+" secs";
			
			breakMessage.Text = "Having a "+(isLongBreak?"long break":"short break")+" for "+breakTime+". You "+(canEscape?"can":"can't")+" escape this break. Break Ends in..";
			myTimer.Start();
		}

		void myTimer_Tick(object sender, EventArgs e)
		{
			currentBreakTimer++;
			if(secondText == 0){
				secondText = 59;
				minuteText--;
			}else{
				secondText--;
			}
			
			timeRemText.Text = (minuteText<10?"0"+minuteText:""+minuteText)+":"+(secondText<10?"0"+secondText:""+secondText);
			if(currentBreakTimer == breakSeconds){
				// done with the timer
				myTimer.Stop();
				Visibility = Visibility.Collapsed;
				ovly.Visibility = Visibility.Collapsed;
				mainWindow.Visibility = Visibility.Visible;
				mainWindow.nextBreak(isLongBreak, false);
			}
		}
		
		void Close_Click(object sender, RoutedEventArgs e)
		{
			Visibility = Visibility.Collapsed;
			mainWindow.Visibility = Visibility.Visible;
			mainWindow.Close();
			ovly.Close();
		}
		
		void Escape_Click(object sender, RoutedEventArgs e)
		{
			myTimer.Stop();
			Visibility = Visibility.Collapsed;
			ovly.Visibility = Visibility.Collapsed;
			mainWindow.Visibility = Visibility.Visible;
			mainWindow.nextBreak(isLongBreak, true);
		}
		
		void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var desktopWorkingArea = SystemParameters.WorkArea;
			
			this.Left = (desktopWorkingArea.Width/2) - (Width/2);
		    this.Top = (desktopWorkingArea.Height/2) - (Height/2);
		}
		
		void overlayCreate(){
			Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);

			foreach(Screen screen in Screen.AllScreens)
			    rect = Rectangle.Union(rect, screen.Bounds);
			
			ovly = new Overlay();
			ovly.Left = 0;
			ovly.Top = 0;
			ovly.Height = rect.Height;
			ovly.Width = rect.Width;
		}
		
		public void closeAll(){
			ovly.Close();
			Close();
		}
	}
}