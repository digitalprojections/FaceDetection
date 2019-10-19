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
using FaceDetection;

namespace GitHub.secile.Video
{
     public class UsbCamcorder
    {
        string sourcePath = @"D:\TEMP";
        string targetPath = String.Empty;
        BackgroundWorker backgroundWorker;
        private string dstFile = String.Empty;
        private string movFile = String.Empty;
        private List<string> fileNames = new List<string>();

        //public DirectShow.IVideoWindow videoWindow = null;

        /// <summary>Usb camera image size.</summary>
        public Size Size { get; private set; }

        /// <summary>Start using.</summary>
        public Action Start { get; private set; }

        /// <summary>Stop using.</summary>
        public Action Stop { get; private set; }

        /// <summary>Release resource.</summary>
        public Action Release { get; private set; }

        /// <summary>Get image.</summary>
        /// <remarks>Immediately after starting, images may not be acquired.</remarks>
        public Func<Bitmap> GetBitmap { get; private set; }

        private DirectShow.IBaseFilter renderer;


        /// <summary>
        /// Get available USB camera list.
        /// </summary>
        /// <returns>Array of camera name, or if no device found, zero length array/</returns>
        public static string[] FindDevices()
        {
            return DirectShow.GetFilters(DirectShow.DsGuid.CLSID_VideoInputDeviceCategory).ToArray();
        }

        /// <summary>
        /// Get video formats.
        /// </summary>
        public static VideoFormat[] GetVideoFormat(int cameraIndex)
        {
            var filter = DirectShow.CreateFilter(DirectShow.DsGuid.CLSID_VideoInputDeviceCategory, cameraIndex);
            var pin = DirectShow.FindPin(filter, 0, DirectShow.PIN_DIRECTION.PINDIR_OUTPUT);
            return GetVideoOutputFormat(pin);
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
            targetPath = Path.Combine(FaceDetection.Properties.Settings.Default.video_file_location, cameraIndex+dstFileName);

            if (backgroundWorker == null)
            {
                backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += BackgroundWorker_DoWork;
                backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
                backgroundWorker.Dispose();
            }
            var camera_list = FindDevices();
            if (cameraIndex >= camera_list.Length) throw new ArgumentException("USB camera is not available.", "index");
            Init(cameraIndex, size, fps, pbx, targetPath);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MoveFiles();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void MoveFiles()
        {
            string sourceFile = String.Empty;
            string destFile = String.Empty;

            Directory.CreateDirectory(sourcePath);
            Directory.CreateDirectory(targetPath);

            //Think about how to join files here

            foreach (string s in fileNames)
            {
                sourceFile = Path.Combine(sourcePath, s);
                destFile = Path.Combine(targetPath, s);
                File.Move(sourceFile, destFile);
            }
            fileNames.Clear();

        }
        static void checkHR(int hr, string msg)
        {
            if (hr < 0)
            {
                Console.WriteLine(msg);
                DirectShow.DsError.ThrowExceptionForHR(hr);
            }
        }
        private void Init(int index, Size size, double fps, IntPtr pbx, string dstFileName)
        {
            //----------------------------------
            // Create Filter Graph with FileWriter
            //----------------------------------
            // +--------------------+  +----------------+  +---------------+  +-----------------+
            //                                             SmarTee-Capture |→| FileWriter       |
            // +--------------------+  +----------------+  +---------------+  +-----------------+
            // |Video Capture Source|→| Sample Grabber |→| SmarTee-Preview |→| VMR9 Renderer attached to your form Control.Handle |
            // +--------------------+  +----------------+  +---------------+  +-----------------+
            //                                 ↓GetBitmap()

            var graph = DirectShow.CreateGraph();
            //----------------------------------
            // VideoCaptureSource
            //----------------------------------
            var vcap_source = CreateVideoCaptureSource(index, size, fps);
            graph.AddFilter(vcap_source, "VideoCapture");

            //------------------------------
            // Smart Tee
            //------------------------------

            var smartee = CreateSmartee();

            //------------------------------
            // SampleGrabber
            //------------------------------
            var grabber = CreateSampleGrabber();
            graph.AddFilter(grabber, "SampleGrabber");
            var i_grabber = (DirectShow.ISampleGrabber)grabber;
            i_grabber.SetBufferSamples(true); //サンプルグラバでのサンプリングを開始

            //---------------------------------------------------
            // Null Renderer
            //---------------------------------------------------
            renderer = DirectShow.CoCreateInstance(DirectShow.DsGuid.CLSID_VideoMixingRenderer9) as DirectShow.IBaseFilter;
            graph.AddFilter(renderer, "VMR9");

            //---------------------------------------------------
            // Create Filter Graph
            //---------------------------------------------------

            DirectShow.IVMRAspectRatioControl9 ratioControl9 = (DirectShow.IVMRAspectRatioControl9)renderer;
            int hr = ratioControl9.SetAspectRatioMode(DirectShow.VMRAspectRatioMode.LetterBox);
            checkHR(hr, "can not set aspect ratio");

            IVMRFilterConfig9 config9 = (IVMRFilterConfig9)renderer;
            hr = config9.SetRenderingMode(VMR9Mode.Windowless);
            checkHR(hr, "Can't set windowless mode");

            IVMRWindowlessControl9 control9 = (IVMRWindowlessControl9)renderer;
            hr = control9.SetVideoClippingWindow(pbx);
            checkHR(hr, "Can't set video clipping window");

            hr = control9.SetVideoPosition(null, new DsRect(0, 0, size.Width, size.Height));
            checkHR(hr, "Can't set rectangles of the video position");

            var builder = DirectShow.CoCreateInstance(DirectShow.DsGuid.CLSID_CaptureGraphBuilder2) as DirectShow.ICaptureGraphBuilder2;
            builder.SetFiltergraph(graph);
            var pinCategory = DirectShow.DsGuid.PIN_CATEGORY_PREVIEW;
            var mediaType = DirectShow.DsGuid.MEDIATYPE_Video;
            builder.RenderStream(ref pinCategory, ref mediaType, vcap_source, grabber, renderer);
            
            // SampleGrabber Format.
            {
                var mt = new DirectShow.AM_MEDIA_TYPE();
                i_grabber.GetConnectedMediaType(mt);
                var header = (DirectShow.VIDEOINFOHEADER)Marshal.PtrToStructure(mt.pbFormat, typeof(DirectShow.VIDEOINFOHEADER));
                var width = header.bmiHeader.biWidth;
                var height = header.bmiHeader.biHeight;
                var stride = width * (header.bmiHeader.biBitCount / 8);
                DirectShow.DeleteMediaType(ref mt);

                Size = new Size(width, height);
                GetBitmap = () => GetBitmapMain(i_grabber, width, height, stride);
            }

            // Assign Delegates
            Start = () => DirectShow.PlayGraph(graph, DirectShow.FILTER_STATE.Running);
            Stop = () => DirectShow.PlayGraph(graph, DirectShow.FILTER_STATE.Stopped);
            Release = () =>
            {
                Stop();
/*                GitHub.secile.Video.DirectShow.IEnumFilters enumFilters = null;
                GitHub.secile.Video.DirectShow.IBaseFilter baseFilters = { null};
                IntPtr fetched = IntPtr.Zero;
                hr = graph.EnumFilters(ref enumFilters);


                int r = 0;
                while (r == 0)
                {
                    try
                    {
                        hr = enumFilters.Next(1, ref baseFilters, ref fetched);
                        DsError.ThrowExceptionForHR(hr);
                        baseFilters[0].QueryFilterInfo(out FilterInfo filterInfo);
                        
                    }
                    catch
                    {
                        r = 1;
                        continue;
                    }

                }*/

                DirectShow.ReleaseInstance(ref grabber);
                DirectShow.ReleaseInstance(ref control9);
                DirectShow.ReleaseInstance(ref config9);
                DirectShow.ReleaseInstance(ref ratioControl9);
                DirectShow.ReleaseInstance(ref builder);
                DirectShow.ReleaseInstance(ref renderer);
                DirectShow.ReleaseInstance(ref i_grabber);
                DirectShow.ReleaseInstance(ref builder);
                DirectShow.ReleaseInstance(ref graph);
            };
            /*
            videoWindow = (DirectShow.IVideoWindow)graph;
            //need to pass the handle for the picture box
            videoWindow.put_Owner(pbx);
            videoWindow.put_WindowState(DirectShow.WindowState.Normal);
            /*This method is a thin wrapper over the SetWindowLong function 
             * and must be treated with care. In particular, you should retrieve 
             * the current styles and then add or remove flags
            videoWindow.put_WindowStyle(DirectShow.WindowStyle.Child | DirectShow.WindowStyle.ClipChildren);
            //videoWindow.put_FullScreenMode(DirectShow.OABool.True);
            videoWindow.SetWindowPosition(0, 0, 1280, 720);
            */
        }

        public void SetWindowPosition(Size size)
        {
            int hr = 0;
            IVMRWindowlessControl9 control9 = (IVMRWindowlessControl9)renderer;
            hr = control9.SetVideoPosition(null, new DsRect(0, 0, size.Width, size.Height));
            checkHR(hr, "Can't set rectangles of the video position");
        }

        /// <summary>Get Bitmap from Sample Grabber</summary>
        private Bitmap GetBitmapMain(DirectShow.ISampleGrabber i_grabber, int width, int height, int stride)
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
        private Bitmap GetBitmapMainMain(DirectShow.ISampleGrabber i_grabber, int width, int height, int stride)
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

        private DirectShow.IBaseFilter CreateSmartee()
        {
            var filter = DirectShow.CreateFilter(DirectShow.DsGuid.CLSID_SmartTee);
            var ismp = filter as DirectShowLib.SmartTee;
            return filter;
        }
        /// <summary>
        /// サンプルグラバを作成する
        /// </summary>
        private DirectShow.IBaseFilter CreateSampleGrabber()
        {
            var filter = DirectShow.CreateFilter(DirectShow.DsGuid.CLSID_SampleGrabber);
            var ismp = filter as DirectShow.ISampleGrabber;
            var mt = new DirectShow.AM_MEDIA_TYPE();
            mt.MajorType = DirectShow.DsGuid.MEDIATYPE_Video;
            mt.SubType = DirectShow.DsGuid.MEDIASUBTYPE_RGB24;
            ismp.SetMediaType(mt);
            DirectShow.DeleteMediaType(ref mt);
            return filter;
        }

        /// <summary>
        /// Video Capture Sourceフィルタを作成する
        /// </summary>
        private DirectShow.IBaseFilter CreateVideoCaptureSource(int index, Size size, double fps)
        {
            var filter = DirectShow.CreateFilter(DirectShow.DsGuid.CLSID_VideoInputDeviceCategory, index);
            var pin = DirectShow.FindPin(filter, 0, DirectShow.PIN_DIRECTION.PINDIR_OUTPUT);
            SetVideoOutputFormat(pin, size, fps);            
            return filter;
        }

        /// <summary>
        /// ビデオキャプチャデバイスの出力形式を選択する。
        /// フォーマットの列挙で同サイズがあれば、そのフォーマットを設定する。
        /// 存在しなかった場合は出力形式を変更しない。
        /// </summary>
        /// <param name="pin">ビデオキャプチャデバイスの出力ピン</param>
        /// <param name="size">指定のサイズ</param>
        /// <param name="fps">フレームレートを指定する。0のとき変更しない(デフォルト)。</param>
        private static bool SetVideoOutputFormat(DirectShow.IPin pin, Size size, double fps)
        {
            var vformat = GetVideoOutputFormat(pin);

            for (int i = 0; i < vformat.Length; i++)
            {
                if (vformat[i].MajorType == DirectShow.DsGuid.GetNickname(DirectShow.DsGuid.MEDIATYPE_Video))
                {
                    // MajorTypeがVideoの場合、SubTypeは色空間を表す。
                    // BuffaloのWebカメラは[YUY2]と[MPEG]だった。
                    // マイクロビジョンのUSBカメラは[YUY2]と[YUVY]だった。
                    // 固定できないためコメントアウト。最初に見つかったフォーマットを利用する。
                    // if (vformat[i].SubType == DSUtility.GetMediaTypeName(DSConst.MediaTypeGUID.MEDIASUBTYPE_YUY2))

                    // FORMAT_VideoInfoのみ対応する。(FORMAT_VideoInfo2はSampleGrabber未対応のためエラー。)
                    // https://msdn.microsoft.com/ja-jp/library/cc370616.aspx

                    if (vformat[i].Caps.Guid == DirectShow.DsGuid.FORMAT_VideoInfo)
                    {
                        if (vformat[i].Size.Width == size.Width && vformat[i].Size.Height == size.Height && vformat[i].TimePerFrame==10000000/fps)
                        {
                            SetVideoOutputFormat(pin, i, size, fps);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// ビデオキャプチャデバイスがサポートするメディアタイプ・サイズを取得する。
        /// </summary>
        private static VideoFormat[] GetVideoOutputFormat(DirectShow.IPin pin)
        {
            // IAMStreamConfigインタフェース取得
            var config = pin as DirectShow.IAMStreamConfig;
            if (config == null)
            {
                throw new Exception("IAMStreamConfigインタフェースを取得できません。");
            }

            // フォーマット個数取得
            int cap_count = 0, cap_size = 0;
            config.GetNumberOfCapabilities(ref cap_count, ref cap_size);
            if (cap_size != Marshal.SizeOf(typeof(DirectShow.VIDEO_STREAM_CONFIG_CAPS)))
            {
                throw new Exception("IAMStreamConfigインタフェースを取得できません。");
            }

            // 返却値の確保
            var result = new VideoFormat[cap_count];

            // データ用領域確保
            var cap_data = Marshal.AllocHGlobal(cap_size);

            // 列挙
            for (int i = 0; i < cap_count; i++)
            {
                var entry = new VideoFormat();

                // x番目のフォーマット情報取得
                DirectShow.AM_MEDIA_TYPE mt = null;
                config.GetStreamCaps(i, ref mt, cap_data);
                entry.Caps = PtrToStructure<DirectShow.VIDEO_STREAM_CONFIG_CAPS>(cap_data);

                // フォーマット情報の読み取り
                entry.MajorType = DirectShow.DsGuid.GetNickname(mt.MajorType);
                entry.SubType = DirectShow.DsGuid.GetNickname(mt.SubType);

                if (mt.FormatType == DirectShow.DsGuid.FORMAT_VideoInfo)
                {
                    var vinfo = PtrToStructure<DirectShow.VIDEOINFOHEADER>(mt.pbFormat);
                    entry.Size = new Size(vinfo.bmiHeader.biWidth, vinfo.bmiHeader.biHeight);
                    entry.TimePerFrame = vinfo.AvgTimePerFrame;
                }
                else if (mt.FormatType == DirectShow.DsGuid.FORMAT_VideoInfo2)
                {
                    var vinfo = PtrToStructure<DirectShow.VIDEOINFOHEADER2>(mt.pbFormat);
                    entry.Size = new Size(vinfo.bmiHeader.biWidth, vinfo.bmiHeader.biHeight);
                    entry.TimePerFrame = vinfo.AvgTimePerFrame;
                }

                // 解放
                DirectShow.DeleteMediaType(ref mt);

                result[i] = entry;
            }

            // 解放
            Marshal.FreeHGlobal(cap_data);

            return result;
        }

        /// <summary>
        /// ビデオキャプチャデバイスの出力形式を選択する。
        /// 事前にGetVideoOutputFormatでメディアタイプ・サイズを得ておき、その中から希望のindexを指定する。
        /// 同時に出力サイズとフレームレートを変更することができる。
        /// </summary>
        /// <param name="index">希望のindexを指定する</param>
        /// <param name="size">Empty以外を指定すると出力サイズを変更する。事前にVIDEO_STREAM_CONFIG_CAPSで取得した可能範囲内を指定すること。</param>
        /// <param name="fps">0以上を指定するとフレームレートを変更する。事前にVIDEO_STREAM_CONFIG_CAPSで取得した可能範囲内を指定すること。</param>
        private static void SetVideoOutputFormat(DirectShow.IPin pin, int index, Size size, double fps)
        {
            // IAMStreamConfigインタフェース取得
            var config = pin as DirectShow.IAMStreamConfig;
            if (config == null)
            {
                throw new Exception("ピンはIAMStreamConfigインタフェースを公開しません。");
            }

            // フォーマット個数取得
            int cap_count = 0, cap_size = 0;
            config.GetNumberOfCapabilities(ref cap_count, ref cap_size);
            if (cap_size != Marshal.SizeOf(typeof(DirectShow.VIDEO_STREAM_CONFIG_CAPS)))
            {
                throw new Exception("VIDEO_STREAM_CONFIG_CAPSを取得できません。");
            }

            // データ用領域確保
            var cap_data = Marshal.AllocHGlobal(cap_size);

            // idx番目のフォーマット情報取得
            DirectShow.AM_MEDIA_TYPE mt = null;
            config.GetStreamCaps(index, ref mt, cap_data);
            var cap = PtrToStructure<DirectShow.VIDEO_STREAM_CONFIG_CAPS>(cap_data);
            // 仕様ではVideoCaptureDeviceはメディア タイプごとに一定範囲の出力フォーマットをサポートできる。例えば以下のように。
            // [0]:YUY2 最小:160x120, 最大:320x240, X軸4STEP, Y軸2STEPごと
            // [1]:RGB8 最小:640x480, 最大:640x480, X軸0STEP, Y軸0STEPごと
            // SetFormatで出力サイズとフレームレートをこの範囲内で設定可能。
            // ただし試した限り、ほとんどのUSBカメラはサイズ固定(最大・最小が同じ)で返してきた。
            // https://msdn.microsoft.com/ja-jp/library/cc353344.aspx
            // https://msdn.microsoft.com/ja-jp/library/cc371290.aspx

            if (mt.FormatType == DirectShow.DsGuid.FORMAT_VideoInfo)
            {
                var vinfo = PtrToStructure<DirectShow.VIDEOINFOHEADER>(mt.pbFormat);
                if (!size.IsEmpty) { vinfo.bmiHeader.biWidth = size.Width; vinfo.bmiHeader.biHeight = size.Height; }
                if (fps > 0) { vinfo.AvgTimePerFrame = (long)(10000000 / fps); }
                Marshal.StructureToPtr(vinfo, mt.pbFormat, true);
            }
            else if (mt.FormatType == DirectShow.DsGuid.FORMAT_VideoInfo2)
            {
                var vinfo = PtrToStructure<DirectShow.VIDEOINFOHEADER2>(mt.pbFormat);
                if (!size.IsEmpty) { vinfo.bmiHeader.biWidth = size.Width; vinfo.bmiHeader.biHeight = size.Height; }
                if (fps > 0) { vinfo.AvgTimePerFrame = (long)(10000000 / fps); }
                Marshal.StructureToPtr(vinfo, mt.pbFormat, true);
            }

            // フォーマットを選択
            config.SetFormat(mt);

            // 解放
            if (cap_data != System.IntPtr.Zero) Marshal.FreeHGlobal(cap_data);
            if (mt != null) DirectShow.DeleteMediaType(ref mt);
        }

        private static T PtrToStructure<T>(IntPtr ptr)
        {
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }

        public class VideoFormat
        {
            private DirectShow.VIDEO_STREAM_CONFIG_CAPS caps;

            public string MajorType { get; set; }  // [Video]など
            public string SubType { get; set; }    // [YUY2], [MJPG]など
            public Size Size { get; set; }         // ビデオサイズ
            public long TimePerFrame { get; set; } // ビデオフレームの平均表示時間を100ナノ秒単位で。30fpsのとき「333333」
            public DirectShow.VIDEO_STREAM_CONFIG_CAPS Caps
            {
                get
                {
                    return caps;
                }
                set
                {
                    caps = value;
                }
            }
            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}, {3}, {4}", MajorType, SubType, Size, TimePerFrame, CapsString());
            }

            public string CapsString()
            {
                var sb = new StringBuilder();
                foreach (var info in Caps.GetType().GetFields())
                {
                    sb.AppendFormat("{0}={1}, ", info.Name, info.GetValue(Caps));
                }
                return sb.ToString();
            }
        }
    }
}

