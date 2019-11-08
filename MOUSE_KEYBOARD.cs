using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    public static class MOUSE_KEYBOARD
    {
        //LL Muse and keyboard
        private static readonly KeyboardListener keyboardListener = new KeyboardListener();
        private static readonly MouseListener mouseListener = new MouseListener();
        //private static readonly MouseListener mouseListenerClick = new MouseListener();

        public static void INIT()
        {
            if (Properties.Settings.Default.capture_operator || Properties.Settings.Default.Recording_when_at_the_start_of_operation)
            {
                keyboardListener.KeyDownAll += new KeyEventHandler(KeyDownAllEventHandler);
                mouseListener.MouseMove += MouseListener_MouseMove;                
            }
        }
        public static void START_CLICK_LISTENER()
        {
            keyboardListener.KeyUpAll += KeyboardListener_KeyUpAll;
            mouseListener.MouseLeftDown += MouseListener_MouseLeftDown;
        }

        private static void KeyboardListener_KeyUpAll(object sender, KeyEventArgs e)
        {
            if (MainForm.GetMainForm != null)
            {
                MainForm.GetMainForm.BackLight.Restart();
            }
        }

        private static void MouseListener_MouseLeftDown(object sender, MouseEventArgs e)
        {
            if(MainForm.GetMainForm!=null)
            {
                MainForm.GetMainForm.BackLight.Restart();
            }

            
            
        }

        internal static void AddMouseAndKeyboardBack()
        {            
            Logger.Add("AddMouseAndKeyboardBackAddMouseAndKeyboardBackAddMouseAndKeyboardBack");
            keyboardListener.KeyDownAll -= new KeyEventHandler(KeyDownAllEventHandler);
            mouseListener.MouseMove -= MouseListener_MouseMove;
            keyboardListener.KeyDownAll += new KeyEventHandler(KeyDownAllEventHandler);
            mouseListener.MouseMove += MouseListener_MouseMove;
        }
        private static void KeyDownAllEventHandler(object sender, KeyEventArgs e)
        {
            MouseKeyEventInit();
        }
        private static void MouseListener_MouseMove(object sender, MouseEventArgs e)
        {
            MouseKeyEventInit();
        }
        private static void MouseKeyEventInit()
        {
            //↓20191107 Nagayama deleted↓
            //MainForm.Or_pb_recording.Visible = Properties.Settings.Default.show_recording_icon;
            //↑20191107 Nagayama deleted↑
            //MainForm.RSensor.Stop_IR_Timer();
            //MainForm.FaceDetector.Stop_Face_Timer();
            keyboardListener.KeyDownAll -= new KeyEventHandler(KeyDownAllEventHandler);
            mouseListener.MouseMove -= MouseListener_MouseMove;
            Logger.Add("TODO: " + CAMERA_MODES.EVENT);
            if (Properties.Settings.Default.capture_operator)
            {
                //↓20191107 Nagayama added↓
                if (Properties.Settings.Default.capture_method == 0)
                {
                //↑20191107 Nagayama added↑
                if (MainForm.GetMainForm.crossbar.PREEVENT_RECORDING)
                {
                    if(Properties.Settings.Default.seconds_after_event>0)
                    {
                        TaskManager.EventAppeared(RECORD_PATH.EVENT, 1,
                        decimal.ToInt32(Properties.Settings.Default.seconds_before_event),
                        decimal.ToInt32(Properties.Settings.Default.seconds_after_event));
                        MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.seconds_after_event));
                            MainForm.GetMainForm.crossbar.SET_ICON_TIMER();
                        }                    

                    }
                    else
                    {
                        MainForm.GetMainForm.crossbar.Start(0, CAMERA_MODES.OPERATOR);
                    }
                //↓20191107 Nagayama added↓
                }
                else
                {
                    SNAPSHOT_SAVER.TakeSnapShot();
                    MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(0);
                }
                //↑20191107 Nagayama added↑
            }

            MainForm.GetMainForm.BackLight.Restart();
        }
    }
}
