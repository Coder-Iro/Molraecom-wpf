using System;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using System.Windows.Input;

namespace Molraecom_wpf
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private KeyboardHook _hook = new KeyboardHook();
        public MainWindow()
        {
            InitializeComponent();
            Cursor = Cursors.None;
        }

        private void OnHookKeyDown(object sender, HookEventArgs e)
        {
            if (HookEventArgs.Control && HookEventArgs.Alt && HookEventArgs.Key == Keys.N)
            {
                if (Visibility == Visibility.Hidden)
                {
                    Visibility = Visibility.Visible;
                    Focus();
                }
                else
                {
                    Visibility = Visibility.Hidden;
                }
            }
        }
        private void OnKeyDownHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.R)))
            {
                if (Visibility == Visibility.Hidden)
                {
                    Visibility = Visibility.Visible;
                    Focus();
                }
                else
                {
                    Visibility = Visibility.Hidden;
                }
            }
        }
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Focus();
            _hook.KeyDown += OnHookKeyDown;
        }
        private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            e.Cancel = true;
            //Do whatever you want here..
        }

        
    }
}
