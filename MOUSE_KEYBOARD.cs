using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    public class MOUSE_KEYBOARD
    {
        //LL Muse and keyboard
        private readonly KeyboardListener keyboardListener = new KeyboardListener();
        private readonly MouseListener mouseListener = new MouseListener();
        //private static readonly MouseListener mouseListenerClick = new MouseListener();

        public MOUSE_KEYBOARD()
        {
            if (Properties.Settings.Default.capture_operator || Properties.Settings.Default.Recording_when_at_the_start_of_operation)
            {
                START_CLICK_LISTENER();
            }
        }
        public void START_CLICK_LISTENER()
        {
            keyboardListener.KeyUpAll += KeyboardListener_KeyUpAll;
            mouseListener.MouseLeftDown += MouseListener_MouseLeftDown;
        }

        private void KeyboardListener_KeyUpAll(object sender, KeyEventArgs e)
        {
            if (MainForm.GetMainForm != null)
            {
                MainForm.GetMainForm.BackLight.Restart();
            }
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
            Logger.Add("AddMouseAndKeyboardBackAddMouseAndKeyboardBackAddMouseAndKeyboardBack");
            keyboardListener.KeyDownAll -= new KeyEventHandler(KeyDownAllEventHandler);
            mouseListener.MouseMove -= MouseListener_MouseMove;
            keyboardListener.KeyDownAll += new KeyEventHandler(KeyDownAllEventHandler);
            mouseListener.MouseMove += MouseListener_MouseMove;
        }
        private void KeyDownAllEventHandler(object sender, KeyEventArgs e)
        {
            MouseKeyEventInit();
        }
        private void MouseListener_MouseMove(object sender, MouseEventArgs e)
        {
            MouseKeyEventInit();
        }
        private void MouseKeyEventInit()
        {
            //↓20191107 Nagayama deleted↓
            //MainForm.Or_pb_recording.Visible = Properties.Settings.Default.show_recording_icon;
            //↑20191107 Nagayama deleted↑
            //MainForm.RSensor.Stop_IR_Timer();
            //MainForm.FaceDetector.Stop_Face_Timer();
            keyboardListener.KeyDownAll -= new KeyEventHandler(KeyDownAllEventHandler);
            mouseListener.MouseMove -= MouseListener_MouseMove;
            Logger.Add("TODO: " + CAMERA_MODES.EVENT);
            if (Properties.Settings.Default.capture_operator && Properties.Settings.Default.Recording_when_at_the_start_of_operation)
            {
                //↓20191107 Nagayama added↓
                if (Properties.Settings.Default.capture_method <= 0)
                {
                    //↑20191107 Nagayama added↑
                    if (MainForm.GetMainForm.crossbar.PREEVENT_RECORDING && Properties.Settings.Default.seconds_after_event > 0)
                    {
                        TaskManager.EventAppeared(RECORD_PATH.EVENT, 1,
                            decimal.ToInt32(Properties.Settings.Default.seconds_before_event),
                            decimal.ToInt32(Properties.Settings.Default.seconds_after_event));                                                
                        //↓20191107 Nagayama added↓
                    }
                    else
                    {
                        MainForm.GetMainForm.crossbar.Start(0, CAMERA_MODES.OPERATOR);                                                                        
                    }
                    MainForm.GetMainForm.crossbar.SET_ICON_TIMER();
                    //↑20191107 Nagayama added↑
                }
                else
                {
                    SNAPSHOT_SAVER.TakeSnapShot(0, "event");
                    MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(0);
                }

                MainForm.GetMainForm.BackLight.Restart();
                MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.seconds_after_event));
            }
            else
            {
                keyboardListener.KeyDownAll -= new KeyEventHandler(KeyDownAllEventHandler);
                mouseListener.MouseMove -= MouseListener_MouseMove;
            }
        }
    }
}
