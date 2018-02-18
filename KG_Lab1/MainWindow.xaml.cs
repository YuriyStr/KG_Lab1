using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KG_Lab1.ColorModels;
using Xceed.Wpf.Toolkit;

namespace KG_Lab1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ColorModel colorModel;
        public MainWindow()
        {
            InitializeComponent();
            colorModel = (ColorModel) Resources["ColorModel"];
        }

        private void sliderRgb_R_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider) sender).Value = Math.Round(e.NewValue, 0);
            CheckForErrors();
        }

        private void sliderRgb_G_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(e.NewValue, 0);
            CheckForErrors();
        }

        private void sliderRgb_B_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(e.NewValue, 0);
            CheckForErrors();
        }

        private void CheckForErrors()
        {
            if (colorModel.Rgb_R > 255 || colorModel.Rgb_R < 0)
                labelError.Content = "The colour R component has been trimmed.";
            else if (colorModel.Rgb_G > 255 || colorModel.Rgb_G < 0)
                labelError.Content = "The colour G component has been trimmed.";
            else if (colorModel.Rgb_B > 255 || colorModel.Rgb_B < 0)
                labelError.Content = "The colour B component has been trimmed.";
            else
                labelError.Content = "";
        }

        private void sliderCmyk_C_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(e.NewValue, 2);
        }

        private void sliderCmyk_M_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(e.NewValue, 2);
        }

        private void sliderCmyk_Y_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(e.NewValue, 2);
        }

        private void sliderCmyk_K_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(e.NewValue, 2);
        }

        private void sliderHsl_H_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(e.NewValue, 0);
        }

        private void sliderHsl_S_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(e.NewValue, 1);
        }

        private void sliderHsl_L_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(e.NewValue, 1);
        }

        private void colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPicker.SelectedColor != null)
                mainCanvas.Background = new SolidColorBrush(colorPicker.SelectedColor.Value);
        }

        private void sliderLab_L_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(e.NewValue, 4);
        }

        private void sliderLab_A_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(e.NewValue, 4);
        }

        private void sliderLab_B_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(e.NewValue, 4);
        }
    }
}
