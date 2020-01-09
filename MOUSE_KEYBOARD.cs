using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    public class MOUSE_KEYBOARD
    {
        bool listen = false;
        //LL Muse and keyboard
        private readonly KeyboardListener keyboardListener = new KeyboardListener();
        private readonly MouseListener mouseListener = new MouseListener();
        private static readonly MouseListener mouseListenerClick = new MouseListener();

        public bool Listen { get => listen; set => listen = value; }

        public MOUSE_KEYBOARD()
        {
            START_CLICK_LISTENER();
            if ((Properties.Settings.Default.C1_enable_capture_operator || Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation)
                    || (Properties.Settings.Default.C2_enable_capture_operator || Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation)
                    || (Properties.Settings.Default.C3_enable_capture_operator || Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation)
                    || (Properties.Settings.Default.C4_enable_capture_operator || Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation))
            {                
                Listen = true;
            }
            
        }
        public void START_CLICK_LISTENER()
        {
            Task.Run(() => {
                Thread.Sleep(7000);
                keyboardListener.KeyUpAll += KeyboardListener_KeyUpAll;
                mouseListener.MouseLeftDown += MouseListener_MouseLeftDown;
                mouseListenerClick.MouseMove += MouseListener_MouseMove;
            });
            }
        private void KeyboardListener_KeyUpAll(object sender, KeyEventArgs e)
        {
            if (MainForm.GetMainForm != null)
            {
                MainForm.GetMainForm.BackLight.Restart();
            }
            MouseKeyEventInit();
        }
        private void MouseListener_MouseLeftDown(object sender, MouseEventArgs e)
        {
            if (MainForm.GetMainForm != null)
            {
                MainForm.GetMainForm.BackLight.Restart();
            }
        }
        public void AddMouseAndKeyboardBack()
        {
            Listen = true;
        }
        private void MouseListener_MouseMove(object sender, MouseEventArgs e)
        {
            MouseKeyEventInit();
        }
        private void MouseKeyEventInit()
        {
            int camindex = Properties.Settings.Default.main_camera_index;
            string captureMethod = "";
            int timeBeforeEvent = 0, timeAfterEvent = 0;
            bool captureOperatorEnabled = false, recordWhenOperation = false, preeventRecording = false;

            try
            {
                switch (camindex)
                {
                    case 0:
                        timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C1_seconds_before_event);
                        timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C1_seconds_after_event);
                        captureOperatorEnabled = Properties.Settings.Default.C1_enable_capture_operator;
                        recordWhenOperation = Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation;
                        preeventRecording = MainForm.GetMainForm.crossbar.PREEVENT_RECORDING;
                        captureMethod = Properties.Settings.Default.C1_capture_type;
                        break;
                    case 1:
                        timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C2_seconds_before_event);
                        timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C2_seconds_after_event);
                        captureOperatorEnabled = Properties.Settings.Default.C2_enable_capture_operator;
                        recordWhenOperation = Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation;
                        preeventRecording = CameraForm.crossbarList[0].PREEVENT_RECORDING;
                        captureMethod = Properties.Settings.Default.C2_capture_type;
                        break;
                    case 2:
                        timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C3_seconds_before_event);
                        timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C3_seconds_after_event);
                        captureOperatorEnabled = Properties.Settings.Default.C3_enable_capture_operator;
                        recordWhenOperation = Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation;
                        preeventRecording = CameraForm.crossbarList[1].PREEVENT_RECORDING;
                        captureMethod = Properties.Settings.Default.C3_capture_type;
                        break;
                    case 3:
                        timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C4_seconds_before_event);
                        timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C4_seconds_after_event);
                        captureOperatorEnabled = Properties.Settings.Default.C4_enable_capture_operator;
                        recordWhenOperation = Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation;
                        preeventRecording = CameraForm.crossbarList[2].PREEVENT_RECORDING;
                        captureMethod = Properties.Settings.Default.C4_capture_type;
                        break;
                }

                if (captureOperatorEnabled && recordWhenOperation && Listen && !MainForm.GetMainForm.crossbar.OPER_BAN)
                {
                    Listen = false;
                    if (captureMethod != "Snapshot") // Video
                    {
                        if (preeventRecording && timeAfterEvent > 0)
                        {
                            TaskManager.EventAppeared(RECORD_PATH.EVENT, camindex + 1, timeBeforeEvent, timeAfterEvent, DateTime.Now);

                            if (camindex == 0)
                            {
                                MainForm.GetMainForm.crossbar.SetIconTimer(timeAfterEvent);
                                MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(timeAfterEvent);
                            }
                            else
                            {
                                MULTI_WINDOW.formList[camindex - 1].SetRecordIcon(camindex, timeAfterEvent);
                            }
                        }
                        else
                        {
                            if (camindex == 0)
                            {
                                MainForm.GetMainForm.crossbar.Start(camindex, CAMERA_MODES.OPERATOR);
                            }
                            else
                            {
                                CameraForm.crossbarList[camindex - 1].Start(camindex, CAMERA_MODES.OPERATOR);
                            }
                        }
                    }
                    else // Snapshot
                    {
                        SNAPSHOT_SAVER.TakeSnapShot(camindex, "event");

                        if (camindex == 0)
                        {
                            MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(0);
                        }
                        else
                        {
                            CameraForm.crossbarList[camindex - 1].No_Cap_Timer_ON(0);
                        }
                    }

                    MainForm.GetMainForm.BackLight.Restart();
                }
                else
                {
                    Listen = false;
                }
                if (MainForm.GetMainForm != null)
                {
                    MainForm.GetMainForm.BackLight.Restart();
                }
            }
            catch (Exception ex)
            {
                // listening closed form 
            }
        }
    }
}
