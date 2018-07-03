/*
 * Created by SharpDevelop.
 * User: SriHarsha
 * Date: 08-06-2018
 * Time: 10:07 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Windows;

namespace EyeRest{
	public static class BottomOnSizeChangeBehaviour
    {
        /// <summary>
        /// Moves the window to bottom in the screen, when its changing its size.
        /// </summary>
        public static readonly DependencyProperty BottomOnSizeChangeProperty =
            DependencyProperty.RegisterAttached
                (
                "BottomOnSizeChange",
                typeof(bool),
                typeof(BottomOnSizeChangeBehaviour),
                new UIPropertyMetadata(false, OnBottomOnSizeChangePropertyChanged)
                );

        public static bool GetBottomOnSizeChange(DependencyObject obj)
        {
            return (bool)obj.GetValue(BottomOnSizeChangeProperty);
        }
        public static void SetBottomOnSizeChange(DependencyObject obj, bool value)
        {
            obj.SetValue(BottomOnSizeChangeProperty, value);
        }

        static void OnBottomOnSizeChangePropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            Window window = dpo as Window;
            if (window != null)
            {
                if ((bool)args.NewValue)
                {
                    window.SizeChanged += OnWindowSizeChanged;
                }
                else
                {
                    window.SizeChanged -= OnWindowSizeChanged;
                }
            }
        }

        static void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Window window = (Window)sender;

            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Left = SystemParameters.WorkArea.Right - window.ActualWidth - 5;
            window.Top = SystemParameters.WorkArea.Bottom - window.ActualHeight - 5;
        }
    }
}