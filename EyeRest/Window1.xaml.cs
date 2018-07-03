/*
 * Created by SharpDevelop.
 * User: SriHarsha
 * Date: 07-06-2018
 * Time: 08:36 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace EyeRest
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		bool isLongBreak = false;
		int noOfShortBreak = 0;
		int noOfbreakEscaped = 0;
		
		int shortBreakAfter;
	    int shortBreakDuration;
		int longBreakAfter;
		int longBreakDuration;
		int noSBtoLB;
		int maxEscapes;
		
		int minuteText = 0;
		int secondText = 0;
		
		int shortBreakTimer = 0;
		int longBreakTimer = 0;
		Timer myTimer = new Timer();
		OverlayItem overlay;
		
		bool isRunning = false;

		public Window1()
		{
			InitializeComponent();
			myInit();
			overlay = new OverlayItem(this);
		}
		
		void myInit()
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\EyeRest");  
              
			//if it does exist, retrieve the stored values  
			if (key != null)  
			{  
				// All these are always in seconds
				shortBreakAfter = (int) key.GetValue("ShortBreakAfter");
			    shortBreakDuration = (int) key.GetValue("ShortBreakDuration");
				longBreakAfter = (int) key.GetValue("LongBreakAfter");
				longBreakDuration = (int) key.GetValue("LongBreakDuration");
				noSBtoLB = (int) key.GetValue("noSBtoLB");
				maxEscapes = (int) key.GetValue("maxEscapes");
				
			    key.Close();  
			}
			else{
				key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EyeRest"); 

				shortBreakAfter = 20*60;
	    		shortBreakDuration = 20;
	    		longBreakAfter = (int) (4000);
				longBreakDuration = 5*60;
				noSBtoLB = 3;
				maxEscapes = 2;
				
				//storing the values in seconds
				key.SetValue("ShortBreakAfter", shortBreakAfter); // 20 mins
				key.SetValue("ShortBreakDuration", shortBreakDuration);  // 30 seconds
				key.SetValue("LongBreakAfter", longBreakAfter); // 1 hour
				key.SetValue("LongBreakDuration", longBreakDuration); // 5 mins
				key.SetValue("noSBtoLB", noSBtoLB); // 3 short reaks to 1 long break
				key.SetValue("maxEscapes", maxEscapes); // max can escape two breaks

				key.Close();  
			}

	        myTimer.Interval = 1000;
	        myTimer.Tick += myTimer_Tick;
	        
	        minuteText = (shortBreakAfter/60);
	        secondText = 0;
	        
	        updateTimer(minuteText,secondText);
		}

		void myTimer_Tick(object sender, EventArgs e)
		{
			shortBreakTimer += 1;
			longBreakTimer += 1;
			
			updateTimer();
			if(isLongBreak && longBreakTimer == longBreakAfter){
				// This is a long break
				// reset everything and take a break
				longBreakTimer = 0;
				shortBreakTimer = 0;
				noOfShortBreak = 0;
				myTimer.Stop();
				System.Diagnostics.Debug.WriteLine("Taking a long break");
				overlay.takeBreak(longBreakDuration, true, noOfbreakEscaped <= maxEscapes);
			}
			else if(!isLongBreak && shortBreakTimer == shortBreakAfter){
				// This is a short break
				shortBreakTimer = 0;
				noOfShortBreak++;
				myTimer.Stop();
				System.Diagnostics.Debug.WriteLine("Taking a short break");
				overlay.takeBreak(shortBreakDuration, false, noOfbreakEscaped <= maxEscapes);
				
			}
		}
		
		void updateTimer(){
			if(secondText == 0){
				secondText = 59;
				minuteText--;
			}else{
				secondText--;
			}
			updateTimer(minuteText,secondText);
		}
		
		void updateTimer(int min, int sec){
			string textToDisplay = timeRemText.Text = (min<10?"0"+min:min+"")+":"+(sec<10?"0"+sec:sec+"");
			timeRemTextAlt.Text = "in "+textToDisplay;
		}
		
		public void nextBreak(bool wasLongBreak, bool breakEscaped)
		{
			int brkDur = 0;
			int brkAfer = 0;
			if(!wasLongBreak){
				// check whether next is a long break
				int timeRem = longBreakAfter - longBreakTimer;
				System.Diagnostics.Debug.WriteLine("Time rem:"+ timeRem+" no of short breaks:"+noOfShortBreak);
				if(timeRem < shortBreakAfter || noOfShortBreak >= noSBtoLB){
					// yes next is long break
					isLongBreak = true;
					brkDur = longBreakDuration;
					brkAfer = timeRem;
					breakTypeText.Text = "(Long)";
					System.Diagnostics.Debug.WriteLine("Next is a long break");
				}
				else{
					isLongBreak = false;
					brkDur = shortBreakDuration;
					brkAfer = shortBreakAfter;
					shortBreakTimer = 0;
					breakTypeText.Text = "(Short)";
					System.Diagnostics.Debug.WriteLine("Next is a short break");
				}
			}else{
				isLongBreak = false;
				brkDur = shortBreakDuration;
				brkAfer = shortBreakAfter;
				shortBreakTimer = 0;
				breakTypeText.Text = "(Short)";
				System.Diagnostics.Debug.WriteLine("Next is a short break");
			}
			
			if(brkAfer >= 60){
				minuteText = brkAfer / 60;
				secondText = brkAfer % 60;
			}else{
				minuteText = 0;
				secondText = brkAfer;
			}
			if(breakEscaped) noOfbreakEscaped++;
			myTimer.Start();
			
		}
				
		void Window_Loaded(object sender, RoutedEventArgs e)
		{
		    var desktopWorkingArea = SystemParameters.WorkArea;
		    Left = desktopWorkingArea.Right - Width - 5;
		    Top = desktopWorkingArea.Bottom - Height - 5;
		}
		
		void StartPause_Click(object sender, RoutedEventArgs e)
		{
			
			if(!isRunning){
				myTimer.Start();
				isRunning = true;
				btn1Img.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/pause-bars.png"));
				System.Diagnostics.Debug.WriteLine("Started");
			}else{
				myTimer.Stop();
				isRunning = false;
				btn1Img.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/play-button.png"));
				System.Diagnostics.Debug.WriteLine("paused");
			}
		}
		
		void Reset_Click(object sender, RoutedEventArgs e)
		{
			myTimer.Stop();
			longBreakTimer = 0;
			shortBreakTimer = 0;
			
			int brkAfter = 0;
			
			brkAfter = isLongBreak ? longBreakAfter : shortBreakAfter;
			
			if(brkAfter >= 60){
				minuteText = brkAfter / 60;
				secondText = brkAfter % 60;
			}else{
				minuteText = 0;
				secondText = brkAfter;
			}
			btn1Img.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/play-button.png"));
			updateTimer(minuteText,secondText);
		}
		
		void Settings_Click(object sender, RoutedEventArgs e)
		{
			mainPanel.Visibility = Visibility.Collapsed;
			settingPanel.Visibility = Visibility.Visible;
			timeRemTextAlt.Visibility = Visibility.Visible;
			
			value1.Text = shortBreakAfter.ToString();
			value2.Text = shortBreakDuration.ToString();
			value3.Text = longBreakAfter.ToString();
			value4.Text = longBreakDuration.ToString();
			value5.Text = noSBtoLB.ToString();
			value6.Text = maxEscapes.ToString();
			
			var desktopWorkingArea = SystemParameters.WorkArea;
		    Left = desktopWorkingArea.Right - Width - 5;
		    Top = desktopWorkingArea.Bottom - Height - 5;
		    
		    System.Diagnostics.Debug.WriteLine("Height is: "+ Height);
		}
		
		void Save_Click(object sender, RoutedEventArgs e)
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\EyeRest",true); 

			shortBreakAfter = int.Parse(value1.Text);
    		shortBreakDuration = int.Parse(value2.Text);
    		longBreakAfter = int.Parse(value3.Text);
			longBreakDuration = int.Parse(value4.Text);
			noSBtoLB = int.Parse(value5.Text);
			maxEscapes = int.Parse(value6.Text);
			
			//storing the values in seconds
			key.SetValue("ShortBreakAfter", shortBreakAfter); // 20 mins
			key.SetValue("ShortBreakDuration", shortBreakDuration);  // 30 seconds
			key.SetValue("LongBreakAfter", longBreakAfter); // 1 hour
			key.SetValue("LongBreakDuration", longBreakDuration); // 5 mins
			key.SetValue("noSBtoLB", noSBtoLB); // 3 short reaks to 1 long break
			key.SetValue("maxEscapes", maxEscapes);

			key.Close();  
			
			mainPanel.Visibility = Visibility.Visible;
			settingPanel.Visibility = Visibility.Collapsed;
			timeRemTextAlt.Visibility = Visibility.Collapsed;
			
			var desktopWorkingArea = SystemParameters.WorkArea;
		    Left = desktopWorkingArea.Right - Width - 5;
		    Top = desktopWorkingArea.Bottom - Height - 5;
		    System.Diagnostics.Debug.WriteLine("Height is: "+ Height);
		}
		
		void Close_Setting_Click(object sender, RoutedEventArgs e)
		{
			mainPanel.Visibility = Visibility.Visible;
			settingPanel.Visibility = Visibility.Collapsed;
			timeRemTextAlt.Visibility = Visibility.Collapsed;
			
			var desktopWorkingArea = SystemParameters.WorkArea;
		    Left = desktopWorkingArea.Right - Width - 5;
		    Top = desktopWorkingArea.Bottom - Height - 5;
		    System.Diagnostics.Debug.WriteLine("Height is: "+ Height);
		}

		bool isMinimized;
		
		void Close_Click(object sender, RoutedEventArgs e)
		{
			overlay.closeAll();
			Close();
		}
		
		void TimePanel_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if(isMinimized){
				isMinimized = false;
				topBtns.Visibility = Visibility.Visible;
				btnArray.Visibility = Visibility.Visible;
				timeRemText.FontSize = 50;
				timeRemText.Padding = new Thickness(10);
				nxtBrkTxt.Visibility = Visibility.Collapsed;
			}
			else{
				isMinimized = true;
				topBtns.Visibility = Visibility.Collapsed;
				btnArray.Visibility = Visibility.Collapsed;
				timeRemText.FontSize = 30;
				timeRemText.Padding = new Thickness(5);
				nxtBrkTxt.Visibility = Visibility.Visible;
			}
		}
		void TimePanel_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
        		DragMove();
		}
	}
}