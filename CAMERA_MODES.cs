namespace FaceDetection
{
    /// <summary>
    ///All cameras have the same modes,
    ///but only the Main camera supports operator capture.         
    ///(各カメラには異なるモードがあります メインカメラのみがオペレータキャプチャをサポートしています)
    /// Camera modes to assist identify current status 
    /// of the application during operation.
    /// Manage it properly, carefully, 
    /// so there are no confusions or contradictions.
    /// Available choices
    /// </summary>
    public enum CAMERA_MODES
    {
        PREVIEW, HIDDEN, EVENT, OPERATOR, MANUAL, PREEVENT
    }
}
