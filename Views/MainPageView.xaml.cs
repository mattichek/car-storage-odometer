﻿using System;
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
using System.Windows.Shapes;

namespace car_storage_odometer.Views
{
    /// <summary>
    /// Interaction logic for MainPageView.xaml
    /// </summary>
    public partial class MainPageView : Window
    {
        public MainPageView()
        {
            InitializeComponent();
            this.Icon = new BitmapImage(new Uri("pack://application:,,,/Assets/logo_icon.ico"));
        }
    }
}
