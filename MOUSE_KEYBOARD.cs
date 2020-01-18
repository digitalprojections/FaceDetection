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
        private int CAMERA_INDEX = 0;
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

            CAMERA_INDEX = Properties.Settings.Default.main_camera_index;
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
            string captureMethod = "";
            int timeBeforeEvent = 0, timeAfterEvent = 0;
            bool captureOperatorEnabled = false, recordWhenOperation = false, preeventRecording = false;

            PROPERTY_FUNCTIONS.GetSecondsBeforeEvent(CAMERA_INDEX, out timeBeforeEvent);
            PROPERTY_FUNCTIONS.GetSecondsAfterEvent(CAMERA_INDEX, out timeAfterEvent);
            PROPERTY_FUNCTIONS.GetCaptureMethod(CAMERA_INDEX, out captureMethod);
            PROPERTY_FUNCTIONS.GetCaptureOperatorSwitch(CAMERA_INDEX, out captureOperatorEnabled);
            PROPERTY_FUNCTIONS.GetCaptureOnOperationStartSwitch(CAMERA_INDEX, out recordWhenOperation);
            try
            {                
                preeventRecording = MULTI_WINDOW.formList[CAMERA_INDEX].crossbar.PREEVENT_RECORDING;
                if (captureOperatorEnabled && recordWhenOperation && Listen && !MULTI_WINDOW.formList[CAMERA_INDEX].crossbar.OPER_BAN)
                {
                    Listen = false;
                    if (captureMethod != "Snapshot") // Video
                    {
                        if (preeventRecording && timeAfterEvent > 0)
                        {
                            TaskManager.EventAppeared(RECORD_PATH.EVENT, CAMERA_INDEX + 1, timeBeforeEvent, timeAfterEvent, DateTime.Now);
                        }
                        else
                        {
                            MULTI_WINDOW.formList[CAMERA_INDEX].crossbar.Start(CAMERA_INDEX, CAMERA_MODES.OPERATOR);
                        }
                        MULTI_WINDOW.formList[CAMERA_INDEX].crossbar.NoCapTimerON(timeAfterEvent);
                        MULTI_WINDOW.formList[CAMERA_INDEX].SetRecordIcon(CAMERA_INDEX, timeAfterEvent);
                    }
                    else // Snapshot
                    {
                        SNAPSHOT_SAVER.TakeSnapShot(CAMERA_INDEX, "event");

                        MULTI_WINDOW.formList[CAMERA_INDEX].crossbar.NoCapTimerON(0);
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
                //Main camera down?
            }
        }
    }
}
