﻿#pragma checksum "..\..\PathConfigBox.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "970B535560A4B8737A8551487F02F7DE1873BA45"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Hyperspec_VideoProcessing;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Hyperspec_VideoProcessing {
    
    
    /// <summary>
    /// PathConfigBox
    /// </summary>
    public partial class PathConfigBox : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\PathConfigBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SearchDirectory;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\PathConfigBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PathText;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\PathConfigBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SetPath;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Hyperspec_VideoProcessing;component/pathconfigbox.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\PathConfigBox.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 9 "..\..\PathConfigBox.xaml"
            ((Hyperspec_VideoProcessing.PathConfigBox)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.SearchDirectory = ((System.Windows.Controls.Button)(target));
            
            #line 19 "..\..\PathConfigBox.xaml"
            this.SearchDirectory.Click += new System.Windows.RoutedEventHandler(this.SearchDirectory_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.PathText = ((System.Windows.Controls.TextBox)(target));
            
            #line 20 "..\..\PathConfigBox.xaml"
            this.PathText.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.PathText_TextChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.SetPath = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\PathConfigBox.xaml"
            this.SetPath.Click += new System.Windows.RoutedEventHandler(this.SetPath_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
