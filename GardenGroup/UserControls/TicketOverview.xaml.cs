﻿using Logic;
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

namespace UI.UserControls
{
    /// <summary>
    /// Interaction logic for TicketOverview.xaml
    /// </summary>
    public partial class TicketOverview : UserControl
    {
        public TicketOverview()
        {
            InitializeComponent();
        }
        private void IncidentButton_Click(object sender, RoutedEventArgs e)
        {
            TicketLogic ticketLogic = new TicketLogic();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
