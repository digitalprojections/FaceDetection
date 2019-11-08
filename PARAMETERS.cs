using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    class PARAMETERS
    {
        public static List<string> PARAM;
        static string param;
        public static void HandleParameters(IReadOnlyCollection<string> parameters)
        {
            
            PARAM = parameters.ToList<string>();
           
            Logger.Add(param + "sdfsdfdsdgf");

            param = String.Concat(parameters).ToLower();
            /*
             Handle the initial start up CL parameters, if exist
             */
            if (param.Contains("uvccameraviewer"))
            {
                if (parameters.Count > 0)
                {
                    //Only 2 parameter elements
                    if (parameters.Count == 2)
                    {
                        if (CheckCameraIndex(parameters))
                        {
                            if(Int32.Parse(parameters.ElementAt(1))!=9)
                            {


                            }else
                            {
                                //the value is 9. Start all cameras
                            }
                        }
                    }
                    switch (parameters.ElementAt(1))
                    {
                        case "-c":
                            try
                            {
                                if (parameters.ElementAt(2) == "1")
                                {


                                    if (MainForm.Setting_ui != null && MainForm.Setting_ui.Visible == false)
                                    {
                                        //settingUI.TopMost = true;
                                        MainForm.GetMainForm.TopMost = false;
                                        MainForm.Setting_ui.ShowDialog();
                                    }

                                }
                                else
                                {
                                    MainForm.Setting_ui.Hide();
                                    MainForm.AllChangesApply();

                                }
                            }

                            catch (ArgumentOutOfRangeException e)
                            {
                                //MessageBox.Show("Incorrect or missing parameters");
                                Trace.WriteLine(e.ToString() + " in line 271");
                            }
                            break;
                        case "-s":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {
                                    //Properties.Settings.Default.main_camera_index = Int32.Parse(parameters.ElementAt(2));
                                    SNAPSHOT_SAVER.TakeSnapShot();
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.Message + " in line 286");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;
                        case "-b":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {
                                    Logger.Add("テスト" + parameters.ElementAt(0) + "×" + parameters.ElementAt(1) + "×" + parameters.ElementAt(2) + "×" + parameters.ElementAt(3));

                                    if (parameters.ElementAt(3) == "1")
                                    {
                                        /*
                                     SHOW CONTROL BUTTONS    
                                     */
                                        MainForm.Or_controlBut.Visible = true;
                                    }
                                    else if (parameters.ElementAt(3) == "0")
                                    {
                                        /*
                                     HIDE CONTROL BUTTONS    
                                     */
                                        MainForm.Or_controlBut.Visible = false;
                                    }

                                }

                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;
                        case "-d":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {
                                    if (parameters.ElementAt(3) == "1")
                                    {
                                        MainForm.FaceDetector.Start_Face_Timer();
                                    }
                                    if (parameters.ElementAt(3) == "0")
                                    {
                                        if(MainForm.GetMainForm.crossbar.Recording_is_on)
                                        {
                                            MainForm.GetMainForm.crossbar.PreviewMode();
                                            MainForm.FaceDetector.Stop_Face_Timer();
                                        }
                                        
                                    }

                                    CycleTime(parameters);
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 387");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;
                        //kameyama beginning 20191018  
                        case "-h":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {
                                    if (parameters.ElementAt(2) == "1")
                                    {
                                        MainForm.RSensor.Start_IR_Timer();
                                    }
                                    else
                                    {
                                        MainForm.RSensor.Stop_IR_Timer();
                                    }

                                    CycleTime(parameters);
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Logger.Add(e.Message + " paramameters 399");
                            }
                            break;

                        case "-v":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {

                                    if (parameters.ElementAt(3) == "1")
                                    {
                                        MainForm.GetMainForm.TopMost = true;
                                        //settingUI.TopMost = false;
                                        MainForm.GetMainForm.Show();
                                    }
                                    else if (parameters.ElementAt(3) == "0")
                                    {
                                        MainForm.GetMainForm.Hide();
                                        MainForm.AllChangesApply();
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;

                        case "-l":
                            try
                            {

                                if (parameters.ElementAt(3) == "1")
                                {
                                    MainForm.GetMainForm.BackLight.ON();

                                }
                                else if (parameters.ElementAt(3) == "0")
                                {

                                    MainForm.GetMainForm.BackLight.OFF();
                                }

                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                //MessageBox.Show("Incorrect or missing parameters");
                                Debug.WriteLine(e.ToString() + " in line 271");
                            }
                            break;
                        case "-n":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {
                                    if (parameters.ElementAt(3) == "1")
                                    {

                                    }
                                    else if (parameters.ElementAt(3) == "0")
                                    {

                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Logger.Add(e.Message + " paramameters 466");
                            }
                            break;
                        case "-w":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {
                                    if (parameters.ElementAt(2) == "1")
                                    {

                                        //kameyama comennt 20191020
                                        //Properties.Settings.Default.show_window_pane = true;
                                        //FormChangesApply();
                                        MainForm.GetMainForm.FormBorderStyle = FormBorderStyle.Sizable;
                                        Properties.Settings.Default.show_window_pane = true;
                                        Properties.Settings.Default.Save();

                                        if (MainForm.Setting_ui != null && MainForm.Setting_ui.Visible == false)
                                        {
                                            //settingUI.TopMost = true;
                                            MainForm.GetMainForm.TopMost = false;
                                            //MainForm.Setting_ui.ShowDialog();
                                            Logger.Add("Properties.Settings.Default.show_window_pane " + Properties.Settings.Default.show_window_pane);
                                        }

                                    }
                                    else
                                    {
                                        Properties.Settings.Default.show_window_pane = false;
                                        Properties.Settings.Default.Save();
                                        MainForm.AllChangesApply();
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters"); } 
                            }
                            break;
                        case "-q":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {
                                    ManualRecordingOn(parameters.ElementAt(1));
                                }
                                else if (parameters.ElementAt(2) == "0")
                                {
                                    ManualRecordingOff(parameters.ElementAt(1));
                                }



                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters"); } 
                            }
                            break;
                        case "-e":
                            try
                            {
                                /////////////////////////////////////
                                ///    dont CheckCameraIndex()    ///
                                /////////////////////////////////////
                                    if (parameters.ElementAt(2) == "1")
                                    {
                                        MainForm.GetMainForm.EventRecorderOn();
                                    }
                                    else if (parameters.ElementAt(2) == "0")
                                    {
                                        MainForm.GetMainForm.EventRecorderOff();
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters"); } 
                            }
                            break;

                    }
                }

            }
        }
        /// <summary>
        /// Camera number is correct if one of 1,2,3,4 or 9
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static bool CheckCameraIndex(IReadOnlyCollection<string> parameters)
        {
            bool Retval = false;

            switch (Int32.Parse(parameters.ElementAt(2)))
            {
                case 1:
                    Retval = true;
                    Logger.Add("カメラ番号まで来た");
                    //Camera.CheckCamera(parameters.ElementAt());
                    break;
                case 2:
                    Retval = true;
                    break;
                case 3:
                    Retval = true;
                    break;
                case 4:
                    Retval = true;
                    break;
                case 9:
                    Retval = true;
                    break;
            }
            return Retval;
        }
        private static void SetWindowPane(bool value)
        {
            MainForm.GetMainForm.ControlBox = value;
        }
        private static void ManualRecordingOn(string camnum)
        {
            //StartVideoRecording();
        }

        private static void ManualRecordingOff(string camnum)
        {
            //StopVideoRecording();
        }
        /// <summary>
        /// No parameters
        /// </summary>
        public static void HandleParameters()
        {

        }
        
        //kameyama comment 20191019 end

        private static void CycleTime(IReadOnlyCollection<string> parameters)
        {
            if (Int32.Parse(parameters.ElementAt(4)) >= 0 && Int32.Parse(parameters.ElementAt(4)) <= 1000)
            {
                Logger.Add("できた");
            }
        }
        #region DLL IMPORTS

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point p);
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(
            IntPtr hWnd,
            uint Msg,
            UIntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags fuFlags,
            uint uTimeout,
            out UIntPtr lpdwResult);

        [Flags]
        enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }
        #endregion DLL IMPORTS
    }
}
