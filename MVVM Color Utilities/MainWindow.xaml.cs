using System.Windows;

namespace MVVM_Color_Utilities
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DragWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        bool maximized = false;
        private void MinimizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ChangeWindowState_Click(object sender, RoutedEventArgs e)
        {
            if (maximized)
            {
                this.WindowState = WindowState.Normal;
                maximized = false;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                maximized = true;
            }
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
