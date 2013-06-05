using System.Windows;
using GTCommons.Commands;
using System;
using System.Collections.Generic;

namespace GTCommons
{
    public class GTCommands : Window
    {
        #region Variables 

        private static GTCommands instance;

        private readonly AutotuneCommands autotuneCommands;
        private readonly CalibrationCommands calibrationCommands;
        private readonly CameraCommands cameraCommands;
        private readonly SettingsCommands settingsCommands;
        private readonly TrackerViewerCommands videoViewerCommands;

        #endregion

        #region Events

        public static readonly RoutedEvent NetworkClientEvent = EventManager.RegisterRoutedEvent("NetworkClientEvent",
                                                                                                 RoutingStrategy.Bubble,
                                                                                                 typeof (RoutedEventHandler),
                                                                                                 typeof (GTCommands));

        public static readonly RoutedEvent TrackQualityEvent = EventManager.RegisterRoutedEvent("TrackStatsEvent",
                                                                                                RoutingStrategy.Bubble,
                                                                                                typeof (RoutedEventHandler),
                                                                                                typeof (GTCommands));

        #endregion

        #region Constructor

        private GTCommands()
        {
            settingsCommands = new SettingsCommands();
            calibrationCommands = new CalibrationCommands();
            autotuneCommands = new AutotuneCommands();
            videoViewerCommands = new TrackerViewerCommands();
            cameraCommands = new CameraCommands();
        }

        #endregion

        #region EventHandlers

        public event RoutedEventHandler OnNetworkClient
        {
            add { base.AddHandler(NetworkClientEvent, value); }
            remove { base.RemoveHandler(NetworkClientEvent, value); }
        }

        public event RoutedEventHandler OnTrackingQuality
        {
            add { base.AddHandler(TrackQualityEvent, value); }
            remove { base.RemoveHandler(TrackQualityEvent, value); }
        }

        #endregion

        #region Raise events

        public void NetworkClient()
        {
            var args1 = new RoutedEventArgs();
            args1 = new RoutedEventArgs(NetworkClientEvent, this);
            RaiseEvent(args1);
        }

        public void TrackQuality()
        {
            var args1 = new RoutedEventArgs();
            args1 = new RoutedEventArgs(TrackQualityEvent, this);
            RaiseEvent(args1);
        }

        #endregion

        #region Get/Set

        public SettingsCommands Settings
        {
            get { return settingsCommands; }
        }

        public CalibrationCommands Calibration
        {
            get { return calibrationCommands; }
        }

        public AutotuneCommands Autotune
        {
            get { return autotuneCommands; }
        }

        public TrackerViewerCommands TrackerViewer
        {
            get { return videoViewerCommands; }
        }

        public CameraCommands Camera
        {
            get { return cameraCommands; }
        }

        public static GTCommands Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GTCommands();
                }

                return instance;
            }
        }

        #endregion

		#region Public methods (parse and execute)

		public void ParseAndExecuteCommand(string command)
		{
			if (command == null) return;

			char[] seperator = { ' ' };
			string[] cmd = command.Split(seperator, 50);

			string cmdStr = cmd[0];
			string cmdParam1 = "";
            string cmdParam2 = "";

            if (cmd.Length == 2)
                cmdParam1 = cmd[1];

            else if (cmd.Length == 3)
            {
                cmdParam1 = cmd[1];
                cmdParam2 = cmd[2];
            }

			switch (cmdStr)
			{
				#region Calibration

				case Protocol.CalibrationStart:
					Calibration.Start();
					break;

				case Protocol.CalibrationAbort:
					Calibration.Abort();
					break;

                case Protocol.CalibrationParameters:
                    Calibration.Parameters(cmdParam1);
                    break;

                case Protocol.CalibrationAreaSize:
                    Calibration.AreaSize(int.Parse(cmdParam1), int.Parse(cmdParam2));
                    break;

                case Protocol.CalibrationValidate:
                    Calibration.Accept();
                    break;

				//    //case CalibrationCheckLevel:
				//    //    if (OnCalibrationCheckLevel != null)
				//    //        OnCalibrationCheckLevel(Int32.Parse(cmd[1]));
				//    //    break;

                case Protocol.StreamStart:
                    break;

                case Protocol.StreamStop:
                    break;

			    #endregion
			}
		}

		#endregion
    }
}