using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DirectShowLib;

namespace FaceDetection
{
    public static class Camera
    {
        public static Action Release { get; private set; }       

        public static DsDevice[] GetCameraCount()
        {
            var capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
           
            return capDevices;
        }

        /// <summary>
        /// Creates a camera just for the sake of checking a camera at an index, if it is ON or OFF
        /// as per
        /// <paramref name="index"/>
        /// </summary>
        public static bool CheckCamera(int index)
        {
            
            bool retval = false;            
            try
            {
                IGraphBuilder graph = (IGraphBuilder)new FilterGraph();
                
                BuildGraph(graph);
                
                IMediaControl mediaControl = (IMediaControl)graph;
                IMediaEvent mediaEvent = (IMediaEvent)graph;
                try
                {
                    int hr = mediaControl.Pause();
                    retval = true;
                    Release();
                }
                catch(COMException comx)
                {
                    Logger.Add(comx);
                    retval = false;
                    Release();
                }
                bool stop = false;
                while (!stop)
                {
                    System.Threading.Thread.Sleep(500);
                    Console.Write(".");
                    EventCode ev;
                    IntPtr p1, p2;
                    System.Windows.Forms.Application.DoEvents();
                    try
                       {
                    while (mediaEvent.GetEvent(out ev, out p1, out p2, 0) == 0)
                    {
                        if (ev == EventCode.Complete || ev == EventCode.UserAbort)
                        {
                            Logger.Add("Done!");                            
                            mediaControl.Stop();
                            SafeReleaseComObject(mediaControl);
                            Release();
                            stop = true;
                        }
                        else
                        if (ev == EventCode.ErrorAbort)
                        {
                            Logger.Add("An error occured: HRESULT={0:X}"+ p1);
                            mediaControl.Stop();
                            SafeReleaseComObject(mediaControl);
                            stop = true;
                        }
                        mediaEvent.FreeEventParams(ev, p1, p2);
                    }

                    }catch(Exception x)
                    {
                        retval = false;
                        Logger.Add("An error occured:" + x.InnerException);                        
                        SafeReleaseComObject(mediaControl);
                        stop = true;                        
                    }
                }
            }
            catch (COMException ex)
            {
                Logger.Add("COM error: " + ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Add("Error: " + ex.ToString());
            }
            Release();
            return retval;
        }

        public static void CountCamera ()
        {
            var capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            if (Properties.Settings.Default.main_camera_index >= capDevices.Length)
            {
                Properties.Settings.Default.main_camera_index = 0;
                Properties.Settings.Default.camera_count = capDevices.Length;
            }
            else if (Properties.Settings.Default.camera_count > capDevices.Length)
            {
                Properties.Settings.Default.camera_count = capDevices.Length;
            }
        }


        public static void SetNumberOfCameras()
        {
            // Get the collection of video devices
            var capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            if (capDevices.Length > 0)
            {                
                if (Properties.Settings.Default.camera_count == 0 || Properties.Settings.Default.camera_count > capDevices.Length)
                {
                    Properties.Settings.Default.camera_count = capDevices.Length;                 
                    //if (capDevices.Length > Properties.Settings.Default.camera_count && capDevices.Length<5)
                    //{
                    //    //MessageBox.Show("The settings do not allow more than " + numericUpDownCamCount.Value + " cameras");
                    //    MainForm.Setting_ui.ArrangeCameraNames(capDevices.Length);
                    //}
                    //else
                    //{
                    //    //settings are missing
                    //    MainForm.Setting_ui.ArrangeCameraNames(4);
                    //    //Properties.Settings.Default.camera_count = 4;
                    //    //Logger.Add("There are more than 4 cameras");
                    //}
                }
                MainForm.Settingui.ArrangeCameraNames(Decimal.ToInt32(Properties.Settings.Default.camera_count));
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("No cameras have been detected! \n\r カメラが検出されません");
            }
            
        }

        static void BuildGraph(IGraphBuilder pGraph)
        {
            int hr = 0;

            //graph builder
            ICaptureGraphBuilder2 pBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = pBuilder.SetFiltergraph(pGraph);
            checkHR(hr, "Can't SetFiltergraph");

            Guid CLSID_VideoCaptureSources = new Guid("{860BB310-5D01-11D0-BD3B-00A0C911CE86}"); //
            Guid CLSID_NullRenderer = new Guid("{C1F400A4-3F08-11D3-9F0B-006008039E37}"); //qedit.dll

            //add FHD Camera
            IBaseFilter captureSource = CreateVideoCaptureSource(0);
            hr = pGraph.AddFilter(captureSource, "FHD Camera");
            checkHR(hr, "Can't add FHD Camera to graph");

            //add Null Renderer
            IBaseFilter pNullRenderer = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_NullRenderer));
            hr = pGraph.AddFilter(pNullRenderer, "Null Renderer");
            checkHR(hr, "Can't add Null Renderer to graph");

            //connect FHD Camera and Null Renderer
            hr = pBuilder.RenderStream(null, MediaType.Video, captureSource, null, pNullRenderer);
            checkHR(hr, "Can't connect FHD Camera and Null Renderer");

            Release = () =>
            {
                
                SafeReleaseComObject(pBuilder);
                SafeReleaseComObject(captureSource);
                SafeReleaseComObject(pNullRenderer);
                SafeReleaseComObject(pGraph);
                

            };
        }
        private static void SafeReleaseComObject(object obj)
        {
            if (obj != null)
            {
                Marshal.ReleaseComObject(obj);
            }
        }
        private static IBaseFilter CreateVideoCaptureSource(int index)
        {
            var filter = DirectShow.CreateFilter(DirectShow.DsGuid.CLSID_VideoInputDeviceCategory, index);            
            return (IBaseFilter)filter;
        }
        static void checkHR(int hr, string msg)
        {
            if (hr < 0)
            {
                Logger.Add(msg);
                DsError.ThrowExceptionForHR(hr);
            }
        }

        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);
    }
}
