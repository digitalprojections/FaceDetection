using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    public class MOUSE_KEYBOARD:IDisposable
    {
        bool listen = false;
        //LL Muse and keyboard
        private readonly KeyboardListener keyboardListener = new KeyboardListener();
        private readonly MouseListener mouseListener = new MouseListener();
        private static readonly MouseListener mouseListenerClick = new MouseListener();
        /// <summary>
        /// MAIN CAMERA
        /// </summary>
        public int CAMERA_INDEX = 0;
        public bool Listen { get => listen; set => listen = value; }

        public MOUSE_KEYBOARD()
        {
            CAMERA_INDEX = Properties.Settings.Default.main_camera_index;
            PROPERTY_FUNCTIONS.GetCaptureOperatorSwitch(CAMERA_INDEX, out bool captureOperatorEnabled);
            PROPERTY_FUNCTIONS.GetOnOperationStartSwitch(CAMERA_INDEX, out bool recordWhenOperation);
            START_CLICK_LISTENER();
            //if ((Properties.Settings.Default.C1_enable_capture_operator || Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation)
            //        || (Properties.Settings.Default.C2_enable_capture_operator || Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation)
            //        || (Properties.Settings.Default.C3_enable_capture_operator || Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation)
            //        || (Properties.Settings.Default.C4_enable_capture_operator || Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation))
            if(captureOperatorEnabled && recordWhenOperation)
            {                
                Listen = true;
            }
        }

        public void START_CLICK_LISTENER()
        {
            //CAMERA_INDEX = Properties.Settings.Default.main_camera_index;
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

        public void RemoveMouseAndKeyboard()
        {
            Listen = false;
        }

        private void MouseListener_MouseMove(object sender, MouseEventArgs e)
        {
            MouseKeyEventInit();
        }

        private void MouseKeyEventInit()
        {
            CAMERA_INDEX = Properties.Settings.Default.main_camera_index;
            string captureMethod = "";
            int timeBeforeEvent = 0, timeAfterEvent = 0;
            bool captureOperatorEnabled = false, recordWhenOperation = false, preeventRecording = false;

            PROPERTY_FUNCTIONS.GetCaptureOperatorSwitch(CAMERA_INDEX, out captureOperatorEnabled);
            PROPERTY_FUNCTIONS.GetOnOperationStartSwitch(CAMERA_INDEX, out recordWhenOperation);

            try
            {
                if(MULTI_WINDOW.formList[CAMERA_INDEX].crossbar!=null)
                {
                    if (captureOperatorEnabled && recordWhenOperation && Listen && !MULTI_WINDOW.formList[CAMERA_INDEX].crossbar.OPER_BAN)
                    {
                        PROPERTY_FUNCTIONS.GetCaptureMethod(CAMERA_INDEX, out captureMethod);
                        PROPERTY_FUNCTIONS.GetSecondsBeforeEvent(CAMERA_INDEX, out timeBeforeEvent);
                        PROPERTY_FUNCTIONS.GetSecondsAfterEvent(CAMERA_INDEX, out timeAfterEvent);
                        preeventRecording = MULTI_WINDOW.formList[CAMERA_INDEX].crossbar.PREEVENT_RECORDING;

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
                
            }
            catch (Exception ex)
            {
                // listening closed form 
                //Main camera down?
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    keyboardListener.KeyUpAll -= KeyboardListener_KeyUpAll;
                    mouseListener.MouseLeftDown -= MouseListener_MouseLeftDown;
                    mouseListenerClick.MouseMove -= MouseListener_MouseMove;
                    // TODO: dispose managed state (managed objects).
                }
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MOUSE_KEYBOARD()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
