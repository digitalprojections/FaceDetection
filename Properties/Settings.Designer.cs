﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FaceDetection.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.1.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("English")]
        public string language {
            get {
                return ((string)(this["language"]));
            }
            set {
                this["language"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\UVCCAMERA")]
        public string video_file_location {
            get {
                return ((string)(this["video_file_location"]));
            }
            set {
                this["video_file_location"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool enable_delete_old_files {
            get {
                return ((bool)(this["enable_delete_old_files"]));
            }
            set {
                this["enable_delete_old_files"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30")]
        public decimal keep_old_files_days {
            get {
                return ((decimal)(this["keep_old_files_days"]));
            }
            set {
                this["keep_old_files_days"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public decimal backlight_offset_mins {
            get {
                return ((decimal)(this["backlight_offset_mins"]));
            }
            set {
                this["backlight_offset_mins"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool enable_backlight_off_when_idle {
            get {
                return ((bool)(this["enable_backlight_off_when_idle"]));
            }
            set {
                this["enable_backlight_off_when_idle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool enable_event_recorder {
            get {
                return ((bool)(this["enable_event_recorder"]));
            }
            set {
                this["enable_event_recorder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool recording_while_face_recognition {
            get {
                return ((bool)(this["recording_while_face_recognition"]));
            }
            set {
                this["recording_while_face_recognition"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool recording_on_start {
            get {
                return ((bool)(this["recording_on_start"]));
            }
            set {
                this["recording_on_start"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public decimal interval_before_reinitiating_recording {
            get {
                return ((decimal)(this["interval_before_reinitiating_recording"]));
            }
            set {
                this["interval_before_reinitiating_recording"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool show_all_cams_simulteneously {
            get {
                return ((bool)(this["show_all_cams_simulteneously"]));
            }
            set {
                this["show_all_cams_simulteneously"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int current_camera_index {
            get {
                return ((int)(this["current_camera_index"]));
            }
            set {
                this["current_camera_index"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool capture_operator {
            get {
                return ((bool)(this["capture_operator"]));
            }
            set {
                this["capture_operator"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10, 10")]
        public global::System.Drawing.Point window_location {
            get {
                return ((global::System.Drawing.Point)(this["window_location"]));
            }
            set {
                this["window_location"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool window_on_top {
            get {
                return ((bool)(this["window_on_top"]));
            }
            set {
                this["window_on_top"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool show_current_datetime {
            get {
                return ((bool)(this["show_current_datetime"]));
            }
            set {
                this["show_current_datetime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool show_recording_icon {
            get {
                return ((bool)(this["show_recording_icon"]));
            }
            set {
                this["show_recording_icon"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int capture_method {
            get {
                return ((int)(this["capture_method"]));
            }
            set {
                this["capture_method"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string culture {
            get {
                return ((string)(this["culture"]));
            }
            set {
                this["culture"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("16")]
        public decimal C1x {
            get {
                return ((decimal)(this["C1x"]));
            }
            set {
                this["C1x"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("16")]
        public decimal C1y {
            get {
                return ((decimal)(this["C1y"]));
            }
            set {
                this["C1y"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("15")]
        public string C1f {
            get {
                return ((string)(this["C1f"]));
            }
            set {
                this["C1f"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("16")]
        public decimal C2x {
            get {
                return ((decimal)(this["C2x"]));
            }
            set {
                this["C2x"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("16")]
        public decimal C2y {
            get {
                return ((decimal)(this["C2y"]));
            }
            set {
                this["C2y"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("640")]
        public decimal C2w {
            get {
                return ((decimal)(this["C2w"]));
            }
            set {
                this["C2w"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("480")]
        public decimal C2h {
            get {
                return ((decimal)(this["C2h"]));
            }
            set {
                this["C2h"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("15")]
        public string C2f {
            get {
                return ((string)(this["C2f"]));
            }
            set {
                this["C2f"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("16")]
        public decimal C3x {
            get {
                return ((decimal)(this["C3x"]));
            }
            set {
                this["C3x"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("16")]
        public decimal C3y {
            get {
                return ((decimal)(this["C3y"]));
            }
            set {
                this["C3y"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("640")]
        public decimal C3w {
            get {
                return ((decimal)(this["C3w"]));
            }
            set {
                this["C3w"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("480")]
        public decimal C3h {
            get {
                return ((decimal)(this["C3h"]));
            }
            set {
                this["C3h"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("15")]
        public string C3f {
            get {
                return ((string)(this["C3f"]));
            }
            set {
                this["C3f"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("16")]
        public decimal C4x {
            get {
                return ((decimal)(this["C4x"]));
            }
            set {
                this["C4x"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("16")]
        public decimal C4y {
            get {
                return ((decimal)(this["C4y"]));
            }
            set {
                this["C4y"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("15")]
        public string C4f {
            get {
                return ((string)(this["C4f"]));
            }
            set {
                this["C4f"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1280x720")]
        public string C1res {
            get {
                return ((string)(this["C1res"]));
            }
            set {
                this["C1res"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1280x720")]
        public string C2res {
            get {
                return ((string)(this["C2res"]));
            }
            set {
                this["C2res"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1280x720")]
        public string C3res {
            get {
                return ((string)(this["C3res"]));
            }
            set {
                this["C3res"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1280x720")]
        public string C4res {
            get {
                return ((string)(this["C4res"]));
            }
            set {
                this["C4res"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public string camera_index {
            get {
                return ((string)(this["camera_index"]));
            }
            set {
                this["camera_index"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Please, select the location to store videos")]
        public string store_location_description {
            get {
                return ((string)(this["store_location_description"]));
            }
            set {
                this["store_location_description"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool main_window_full_screen {
            get {
                return ((bool)(this["main_window_full_screen"]));
            }
            set {
                this["main_window_full_screen"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool use_ir_sensor {
            get {
                return ((bool)(this["use_ir_sensor"]));
            }
            set {
                this["use_ir_sensor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int main_camera_index {
            get {
                return ((int)(this["main_camera_index"]));
            }
            set {
                this["main_camera_index"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Recording_when_an_operator_senses {
            get {
                return ((bool)(this["Recording_when_an_operator_senses"]));
            }
            set {
                this["Recording_when_an_operator_senses"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Recording_when_at_the_start_of_operation {
            get {
                return ((bool)(this["Recording_when_at_the_start_of_operation"]));
            }
            set {
                this["Recording_when_at_the_start_of_operation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public decimal camera_count {
            get {
                return ((decimal)(this["camera_count"]));
            }
            set {
                this["camera_count"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool enable_face_recognition {
            get {
                return ((bool)(this["enable_face_recognition"]));
            }
            set {
                this["enable_face_recognition"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool backlight_on_upon_face_rec {
            get {
                return ((bool)(this["backlight_on_upon_face_rec"]));
            }
            set {
                this["backlight_on_upon_face_rec"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public decimal seconds_before_event {
            get {
                return ((decimal)(this["seconds_before_event"]));
            }
            set {
                this["seconds_before_event"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public decimal seconds_after_event {
            get {
                return ((decimal)(this["seconds_after_event"]));
            }
            set {
                this["seconds_after_event"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public decimal recording_length_seconds {
            get {
                return ((decimal)(this["recording_length_seconds"]));
            }
            set {
                this["recording_length_seconds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool show_window_pane {
            get {
                return ((bool)(this["show_window_pane"]));
            }
            set {
                this["show_window_pane"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool show_camera_no {
            get {
                return ((bool)(this["show_camera_no"]));
            }
            set {
                this["show_camera_no"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("640")]
        public decimal C1w {
            get {
                return ((decimal)(this["C1w"]));
            }
            set {
                this["C1w"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("480")]
        public decimal C1h {
            get {
                return ((decimal)(this["C1h"]));
            }
            set {
                this["C1h"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("300")]
        public decimal event_record_time_before_event {
            get {
                return ((decimal)(this["event_record_time_before_event"]));
            }
            set {
                this["event_record_time_before_event"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("300")]
        public decimal event_record_time_after_event {
            get {
                return ((decimal)(this["event_record_time_after_event"]));
            }
            set {
                this["event_record_time_after_event"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public decimal face_record_time_before {
            get {
                return ((decimal)(this["face_record_time_before"]));
            }
            set {
                this["face_record_time_before"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public decimal face_record_time_after {
            get {
                return ((decimal)(this["face_record_time_after"]));
            }
            set {
                this["face_record_time_after"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public string recording_length_seconds1 {
            get {
                return ((string)(this["recording_length_seconds1"]));
            }
            set {
                this["recording_length_seconds1"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\TEMP")]
        public string temp_folder {
            get {
                return ((string)(this["temp_folder"]));
            }
            set {
                this["temp_folder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public decimal face_rec_interval {
            get {
                return ((decimal)(this["face_rec_interval"]));
            }
            set {
                this["face_rec_interval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Video")]
        public string capture_type {
            get {
                return ((string)(this["capture_type"]));
            }
            set {
                this["capture_type"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1280")]
        public decimal C4w {
            get {
                return ((decimal)(this["C4w"]));
            }
            set {
                this["C4w"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("720")]
        public decimal C4h {
            get {
                return ((decimal)(this["C4h"]));
            }
            set {
                this["C4h"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool enable_Human_sensor {
            get {
                return ((bool)(this["enable_Human_sensor"]));
            }
            set {
                this["enable_Human_sensor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("600")]
        public decimal manual_record_time {
            get {
                return ((decimal)(this["manual_record_time"]));
            }
            set {
                this["manual_record_time"] = value;
            }
        }
    }
}
