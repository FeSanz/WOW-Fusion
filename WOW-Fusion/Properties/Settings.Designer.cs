﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WOW_Fusion.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.5.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("127.0.0.1")]
        public string PrinterIP {
            get {
                return ((string)(this["PrinterIP"]));
            }
            set {
                this["PrinterIP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("9100")]
        public int PrinterPort {
            get {
                return ((int)(this["PrinterPort"]));
            }
            set {
                this["PrinterPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4001")]
        public int WeighingPort {
            get {
                return ((int)(this["WeighingPort"]));
            }
            set {
                this["WeighingPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int Aditional {
            get {
                return ((int)(this["Aditional"]));
            }
            set {
                this["Aditional"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://iapxqy-test.fa.ocs.oraclecloud.com/fscmRestApi/resources/11.13.18.05")]
        public string FusionUrl {
            get {
                return ((string)(this["FusionUrl"]));
            }
            set {
                this["FusionUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://129.146.124.5:8080/ords/wow/wo")]
        public string ApexUrl {
            get {
                return ((string)(this["ApexUrl"]));
            }
            set {
                this["ApexUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ZmVsaXBlLmFudG9uaW9AaS1jb25kb3IuY29tOkNvbmRvclhSMTEy")]
        public string Credentials {
            get {
                return ((string)(this["Credentials"]));
            }
            set {
                this["Credentials"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("192.168.12.1")]
        public string WeighingIP {
            get {
                return ((string)(this["WeighingIP"]));
            }
            set {
                this["WeighingIP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("300000003523428")]
        public string WorkCenterP2 {
            get {
                return ((string)(this["WorkCenterP2"]));
            }
            set {
                this["WorkCenterP2"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public int RollToPrint {
            get {
                return ((int)(this["RollToPrint"]));
            }
            set {
                this["RollToPrint"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public int PalletToPrint {
            get {
                return ((int)(this["PalletToPrint"]));
            }
            set {
                this["PalletToPrint"] = value;
            }
        }
    }
}
