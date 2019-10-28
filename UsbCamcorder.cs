using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Diagnostics;
using DirectShowLib;
using System.ComponentModel;
using System.IO;

namespace FaceDetection
{
     public class UsbCamcorder
    {
        string sourcePath = @"D:\TEMP";
        string targetPath = String.Empty;
        //BackgroundWorker backgroundWorker;
        
        private List<string> fileNames = new List<string>();

        IBaseFilter pVideoMixingRenderer9 = null;
        IBaseFilter pSmartTee;
        IBaseFilter pSampleGrabber;
        IFileSinkFilter pFilewriter_sink;
        IBaseFilter pFilewriter;
        IBaseFilter pAVIMux;
        IBaseFilter pUSB;
        ICaptureGraphBuilder2 pBuilder;
        IMediaControl mediaControl;
        IMediaEvent mediaEvent;
        IGraphBuilder graph;

        private Guid CLSID_VideoCaptureSources = new Guid("{860BB310-5D01-11D0-BD3B-00A0C911CE86}"); //
        private Guid CLSID_SampleGrabber = new Guid("{C1F400A0-3F08-11D3-9F0B-006008039E37}"); //qedit.dll
        private Guid CLSID_VideoMixingRenderer9 = new Guid("{51B4ABF3-748F-4E3B-A276-C828330E926A}"); //quartz.dll

        /// <summary>
        /// Get available USB camera list.
        /// </summary>
        /// <returns>Array of camera name, or if no device found, zero length array/</returns>
        public static IBaseFilter FindDevices()
        {
            List<object> devs = new List<object>();
            IEnumMoniker classEnum = null;
            IMoniker[] moniker = { null };

            object source = null;
            ICreateDevEnum devEnum = (ICreateDevEnum)(new CreateDevEnum());

            int hr = devEnum.CreateClassEnumerator(FilterCategory.VideoInputDevice, out classEnum, CDef.None);
            DsError.ThrowExceptionForHR(hr);

            if (classEnum == null)
            {
                throw new ApplicationException("No video capture device was detected.\\r\\n\\r\\n" + "This sample requires a video capture device, such as a USB WebCam,\\r\\nto be installed and working properly.  The sample will now close.");
            }

            IntPtr none = IntPtr.Zero;
            while (classEnum.Next(1, moniker, none) == 0)
            {

                Guid iid = typeof(IBaseFilter).GUID;
                moniker[0].BindToObject(null, null, ref iid, out source);
                Marshal.ReleaseComObject(moniker[0]);
                devs.Add(source);
            }

            Debug.WriteLine(devs.Count + " === cameras found");

            Marshal.ReleaseComObject(devEnum);
            Marshal.ReleaseComObject(classEnum);

            //we can now select cameras
            return (IBaseFilter)devs[0];
        }
                
        /// <summary>
        /// Create USB Camera with recording functionality.
        /// Everything is the same as the regular camera, but file writing            
        /// </summary>
        /// <param name="cameraIndex">Camera index in FindDevices() result.</param>
        /// <param name="size">
        /// Size you want to create. If camera does not support the size, created with default size.
        /// Check Size property to know actual created size.
        /// </param>
        public UsbCamcorder(int cameraIndex, Size size, double fps, IntPtr pbx, string dstFileName)
        {
            string str = Path.Combine(FaceDetection.Properties.Settings.Default.video_file_location, (cameraIndex + 1).ToString());
            Console.WriteLine(str);
            Directory.CreateDirectory(str);
            targetPath = Path.Combine(str, dstFileName);
            Console.WriteLine(targetPath);
            
            //var camera_list = FindDevices();
            //if (cameraIndex >= camera_list) throw new ArgumentException("USB camera is not available.", "index");
            Init(cameraIndex, size, fps, pbx, targetPath);
        }

        
        private void Init(int index, Size size, double fps, IntPtr pbx, string dstFileName)
        {

            pVideoMixingRenderer9 = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_VideoMixingRenderer9));
            
        }
        void BuildGraph(IGraphBuilder pGraph, string dstFile1)
        {
            int hr = 0;

            //graph builder
            pBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = pBuilder.SetFiltergraph(pGraph);
            checkHR(hr, "Can't SetFiltergraph");

            //add USB ビデオ デバイス
            pUSB = CreateFilterByName(@"USB ビデオ デバイス", CLSID_VideoCaptureSources);
            hr = pGraph.AddFilter(pUSB, "USB ビデオ デバイス");
            checkHR(hr, "Can't add USB ビデオ デバイス to graph");

            //add AVI Mux
            pAVIMux = (IBaseFilter)new AviDest();
            hr = pGraph.AddFilter(pAVIMux, "AVI Mux");
            checkHR(hr, "Can't add AVI Mux to graph");

            //add File writer
            pFilewriter = (IBaseFilter)new FileWriter();
            hr = pGraph.AddFilter(pFilewriter, "File writer");
            checkHR(hr, "Can't add File writer to graph");
            //set destination filename
            pFilewriter_sink = pFilewriter as IFileSinkFilter;
            if (pFilewriter_sink == null)
                checkHR(unchecked((int)0x80004002), "Can't get IFileSinkFilter");
            hr = pFilewriter_sink.SetFileName(dstFile1, null);
            checkHR(hr, "Can't set filename");

            //add SampleGrabber
            pSampleGrabber = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_SampleGrabber));
            hr = pGraph.AddFilter(pSampleGrabber, "SampleGrabber");
            checkHR(hr, "Can't add SampleGrabber to graph");
            


            //add Smart Tee
            pSmartTee = (IBaseFilter)new SmartTee();
            hr = pGraph.AddFilter(pSmartTee, "Smart Tee");
            checkHR(hr, "Can't add Smart Tee to graph");

            //add Video Mixing Renderer 9
            
            hr = pGraph.AddFilter(pVideoMixingRenderer9, "Video Mixing Renderer 9");
            checkHR(hr, "Can't add Video Mixing Renderer 9 to graph");

            //connect USB ビデオ デバイス and SampleGrabber
            hr = pGraph.ConnectDirect(GetPin(pUSB, "キャプチャ"), GetPin(pSampleGrabber, "Input"), null);
            checkHR(hr, "Can't connect USB ビデオ デバイス and SampleGrabber");

            //connect SampleGrabber and Smart Tee
            hr = pGraph.ConnectDirect(GetPin(pSampleGrabber, "Output"), GetPin(pSmartTee, "Input"), null);
            checkHR(hr, "Can't connect SampleGrabber and Smart Tee");

            //connect Smart Tee and AVI Mux
            hr = pGraph.ConnectDirect(GetPin(pSmartTee, "Capture"), GetPin(pAVIMux, "Input 01"), null);
            checkHR(hr, "Can't connect Smart Tee and AVI Mux");

            //connect AVI Mux and File writer
            hr = pGraph.ConnectDirect(GetPin(pAVIMux, "AVI Out"), GetPin(pFilewriter, "in"), null);
            checkHR(hr, "Can't connect AVI Mux and File writer");
                        
            //connect Smart Tee and MJPEG Decompressor
            hr = pBuilder.RenderStream(null, MediaType.Video, pSmartTee, null, pVideoMixingRenderer9);
            checkHR(hr, "Can't connect Smart Tee and MJPEG Decompressor");


        }
        private static void SafeReleaseComObject(object obj)
        {
            if (obj != null)
            {
                Marshal.ReleaseComObject(obj);
            }
        }

        public void SetWindowPosition(Size size)
        {
            int hr = 0;
            DirectShowLib.IVMRWindowlessControl9 control9 = (DirectShowLib.IVMRWindowlessControl9)pVideoMixingRenderer9;
            hr = control9.SetVideoPosition(null, new DirectShowLib.DsRect(0, 0, size.Width, size.Height));
            checkHR(hr, "Can't set rectangles of the video position");
        }

        /// <summary>Get Bitmap from Sample Grabber</summary>
        private Bitmap GetBitmapMain(ISampleGrabber i_grabber, int width, int height, int stride)
        {
            try
            {
                return GetBitmapMainMain(i_grabber, width, height, stride);
            }
            catch (COMException ex)
            {
                const uint VFW_E_WRONG_STATE = 0x80040227;
                if ((uint)ex.ErrorCode == VFW_E_WRONG_STATE)
                {
                    // image data is not ready yet. return empty bitmap.
                    return new Bitmap(width, height);
                }

                throw;
            }
        }

        /// <summary>Get Bitmap from Sample Grabber</summary>
        private Bitmap GetBitmapMainMain(ISampleGrabber i_grabber, int width, int height, int stride)
        {
            // サンプルグラバから画像を取得するためには
            // まずサイズ0でGetCurrentBufferを呼び出しバッファサイズを取得し
            // バッファ確保して再度GetCurrentBufferを呼び出す。
            // 取得した画像は逆になっているので反転させる必要がある。
            int sz = 0;
            i_grabber.GetCurrentBuffer(ref sz, IntPtr.Zero); // IntPtr.Zeroで呼び出してバッファサイズ取得
            if (sz == 0) return null;

            // メモリ確保し画像データ取得
            var ptr = Marshal.AllocCoTaskMem(sz);
            i_grabber.GetCurrentBuffer(ref sz, ptr);

            // 画像データをbyte配列に入れなおす
            var data = new byte[sz];
            Marshal.Copy(ptr, data, 0, sz);

            Bitmap result = null;

            try
            {
                // 画像を作成
                result = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var bmp_data = result.LockBits(new Rectangle(Point.Empty, result.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                //Debug.WriteLine(i_grabber + " igrabber " + ptr + " pointer, " + data.Length + " data  length " + sz);

                // 上下反転させながら1行ごとコピー
                for (int y = 0; y < height; y++)
                {
                    var src_idx = sz - (stride * (y + 1)); // 最終行から
                    var dst = new IntPtr(bmp_data.Scan0.ToInt32() + (stride * y));
                    Marshal.Copy(data, src_idx, dst, stride);
                }
                result.UnlockBits(bmp_data);
                Marshal.FreeCoTaskMem(ptr);
            }
            catch(ArgumentException ax)
            {
                //SnapShot.Form1.CleanBuffer();
                System.Windows.Forms.MessageBox.Show(ax.Message);

            }

            return result;
        }

        internal void Release()
        {
            if (mediaControl != null)
                mediaControl.StopWhenReady();

            //CurrentState = PlayState.Stopped;

 
            // Release DirectShow interfaces
            if (mediaControl != null)
            {
                Marshal.ReleaseComObject(mediaControl);
                mediaControl = null;
            }


            if (mediaEvent != null)
            {
                Marshal.ReleaseComObject(mediaEvent);
                mediaEvent = null;
            }

            if (pVideoMixingRenderer9 != null)
            {
                Marshal.ReleaseComObject(pVideoMixingRenderer9);
                pVideoMixingRenderer9 = null;
            }

            if (graph != null)
            {
                Marshal.ReleaseComObject(graph);
                graph = null;
            }

            if (pBuilder != null)
            {
                Marshal.ReleaseComObject(pBuilder);
                pBuilder = null;
            }
            if (pSampleGrabber != null)
            {
                Marshal.ReleaseComObject(pSampleGrabber);
                pSampleGrabber = null;
            }
            if (pSmartTee != null)
            {
                Marshal.ReleaseComObject(pSmartTee);
                pSmartTee = null;
            }
            if (pAVIMux != null)
            {
                Marshal.ReleaseComObject(pAVIMux);
                pAVIMux = null;
            }
            if (pFilewriter != null)
            {
                Marshal.ReleaseComObject(pFilewriter);
                pFilewriter = null;
            }
        }

        internal void Start()
        {
            try
            {
                graph = (IGraphBuilder)new FilterGraph();
                Console.WriteLine("Building graph...");
                BuildGraph(graph, targetPath);
                Console.WriteLine("Running...");
                mediaControl = (IMediaControl)graph;
                mediaEvent = (IMediaEvent)graph;
                int hr = mediaControl.Run();
                checkHR(hr, "Can't run the graph");
                bool stop = false;
                while (!stop)
                {
                    System.Threading.Thread.Sleep(500);
                    Console.Write(".");
                    EventCode ev;
                    IntPtr p1, p2;
                    System.Windows.Forms.Application.DoEvents();
                    while (mediaEvent.GetEvent(out ev, out p1, out p2, 0) == 0)
                    {
                        if (ev == EventCode.Complete || ev == EventCode.UserAbort)
                        {
                            Console.WriteLine("Done!");
                            stop = true;
                        }
                        else
                        if (ev == EventCode.ErrorAbort)
                        {
                            Console.WriteLine("An error occured: HRESULT={0:X}", p1);
                            mediaControl.Stop();
                            stop = true;
                        }
                        mediaEvent.FreeEventParams(ev, p1, p2);
                    }
                }
            }
            catch (COMException ex)
            {
                Console.WriteLine("COM error: " + ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
        }

        public static IBaseFilter CreateFilterByName(string filterName, Guid category)
        {
            int hr = 0;
            DsDevice[] devices = DsDevice.GetDevicesOfCat(category);
            foreach (DsDevice dev in devices)
                if (dev.Name == filterName)
                {
                    IBaseFilter filter = null;
                    IBindCtx bindCtx = null;
                    try
                    {
                        hr = CreateBindCtx(0, out bindCtx);
                        DsError.ThrowExceptionForHR(hr);
                        Guid guid = typeof(IBaseFilter).GUID;
                        object obj;
                        dev.Mon.BindToObject(bindCtx, null, ref guid, out obj);
                        filter = (IBaseFilter)obj;
                    }
                    finally
                    {
                        if (bindCtx != null) Marshal.ReleaseComObject(bindCtx);
                    }
                    return filter;
                }
            return null;
        }

        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        static IPin GetPin(IBaseFilter filter, string pinname)
        {
            IEnumPins epins;
            int hr = filter.EnumPins(out epins);
            checkHR(hr, "Can't enumerate pins");
            IntPtr fetched = Marshal.AllocCoTaskMem(4);
            IPin[] pins = new IPin[1];
            while (epins.Next(1, pins, fetched) == 0)
            {
                PinInfo pinfo;
                pins[0].QueryPinInfo(out pinfo);
                bool found = (pinfo.name == pinname);
                DsUtils.FreePinInfo(pinfo);
                if (found)
                    return pins[0];
            }
            checkHR(-1, "Pin not found");
            return null;
        }
        static void checkHR(int hr, string msg)
        {
            if (hr < 0)
            {
                Console.WriteLine(msg);
                DsError.ThrowExceptionForHR(hr);
            }
        }
    }    
}

