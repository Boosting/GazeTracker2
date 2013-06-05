// WSComUI.MainWindow.xaml.cs
// ------------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation; either version 3 of the License, 
// or (at your option) any later version.
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
// General Public License for more details.
// You should have received a copy of the GNU General Public License 
// along with this program; if not, see http://www.gnu.org/licenses/.
// **************************************************************
// </copyright>
// <author>Nicola Riparelli</author>
// <email>nicola.riparelli@gmail.com</email>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using WSCom;
using GTCommons.Events;

namespace WSComUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml; User Interface of WSClient.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region VARIABLES

        WSClient client;
        private bool windowOpen;
        private bool slideIsOpen = false;

        #endregion

        #region CONSTRUCTOR

        public MainWindow()
        {
            InitializeComponent();
            this.SetOnScreen();
            client = new WSClient();
            windowOpen = true;

            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            client.SetScreenResolution((int)screenWidth, (int)screenHeight);
            client.Error += new WSClient.ErrorEventHandler(OnError);
            client.ConnectionChange += new WSClient.ConnectionChangedHandler(OnConnectionChanged);
        }

        #endregion

        #region PRIVATE METHODS

        private void OnError(object sender, ErrorEventArgs e)
        {
            UiInvoke(() => TextBlockConsole.Text += "Errore di connessione \n");
            client.WSDisconnect();
            MessageBox.Show(e.GetException().Message);
        }

        private void OnConnectionChanged(object sender, StringEventArgs e)
        {
            if ((e.Param).Equals("True"))
            {
                UiInvoke(() => TextBlockConsole.Text += "Eye Tracker connesso \n");
                UiInvoke(() => ButtonConnect.Content = "Disconnetti");
                UiInvoke(() => ButtonSetup.IsEnabled = false);
            }
            else
            {
                UiInvoke(() => TextBlockConsole.Text += "Eye Tracker disconnesso \n");
                UiInvoke(() => ButtonConnect.Content = "Connetti");
                UiInvoke(() => ButtonSetup.IsEnabled = true);
            }
        }

        public static void UiInvoke(Action a)
        {
            Application.Current.Dispatcher.Invoke(a);
        }

        private void SetOnScreen()
        {
            double height = SystemParameters.WorkArea.Height;
            double width = SystemParameters.WorkArea.Width;
            this.Top = (height - this.Height) / 2;
            this.Left = ((width - this.Width) / 2) - 390;
        }

        ///<summary>
        /// Button Connect: connects WSClient if not connected; else disconnects it.
        /// </summary>
        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!client.WSIsConnected())
            {
                TextBlockConsole.Text += "Connessione in corso... \n";
                client.WSConnect();
            }
            else
            {
                client.WSDisconnect();
                UiInvoke(() => TextBlockConsole.Text += "Eye Tracker disconnesso \n");
                UiInvoke(() => ButtonConnect.Content = "Connetti");
                UiInvoke(() => ButtonSetup.IsEnabled = true);
            }
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            client.WSDisconnect();
            windowOpen = false;
            Application.Current.MainWindow.Close();
            Environment.Exit(Environment.ExitCode);
        }

        ///<summary>
        /// Button Setup: opens the setup menu.
        /// </summary>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!slideIsOpen)
            {
                for (int i = 0; i <= 70; i++)
                {
                    SlideSetup.Height += 1;
                    GridControls.Height += 1;
                    UpGrid.Height += 1;
                    GazeClientWindow.Height += 1;
                }
                LabelSetupAddress.Visibility = Visibility.Visible;
                TextBoxSetupAddress.Visibility = Visibility.Visible;
                ButtonOKAddress.Visibility = Visibility.Visible;
                LabelOkAddress.Visibility = Visibility.Visible;

                slideIsOpen = true;
            }
            else
            {
                LabelSetupAddress.Visibility = Visibility.Hidden;
                TextBoxSetupAddress.Visibility = Visibility.Hidden;
                ButtonOKAddress.Visibility = Visibility.Hidden;
                LabelOkAddress.Visibility = Visibility.Hidden;
                for (int i = 0; i <= 70; i++)
                {
                    SlideSetup.Height -= 1;
                    GridControls.Height -= 1;
                    UpGrid.Height -= 1;
                    GazeClientWindow.Height -= 1;
                }

                slideIsOpen = false;
            }
        }

        ///<summary>
        /// Button OK: validates the new WebSocket address.
        /// </summary>
        private void Rectangle_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var r = new Regex("^(wss?)://(.*)\\:([0-9]*)/(.*)$");
                var matches = r.Match(TextBoxSetupAddress.Text);

                var host = matches.Groups[2].Value;
                var port = Int32.Parse(matches.Groups[3].Value);
                var path = matches.Groups[4].Value;
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Formato indirizzo errato");
                return;
            }
            client = new WSClient(TextBoxSetupAddress.Text);
            client.Error += new WSClient.ErrorEventHandler(OnError);

            LabelSetupAddress.Visibility = Visibility.Hidden;
            TextBoxSetupAddress.Visibility = Visibility.Hidden;
            ButtonOKAddress.Visibility = Visibility.Hidden;
            for (int i = 0; i <= 70; i++)
            {
                SlideSetup.Height -= 1;
                GridControls.Height -= 1;
                UpGrid.Height -= 1;
                GazeClientWindow.Height -= 1;
            }
            slideIsOpen = false;
        }

        private void AppClose(object sender, MouseButtonEventArgs e)
        {
            if (client.WSIsConnected())
                client.WSDisconnect();
            this.Close();
            windowOpen = false;
        }

        private void Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        #endregion

        #region PUBLIC METHODS

        private bool IsOpen()
        {
            return windowOpen;
        }

        #endregion
    }
}
