﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace shadowsocks_csharp.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("shadowsocks_csharp.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] polipo {
            get {
                object obj = ResourceManager.GetObject("polipo", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # Sample configuration file for Polipo. -*-sh-*-
        ///
        ///# You should not need to use a configuration file; all configuration
        ///# variables have reasonable defaults.  If you want to use one, you
        ///# can copy this to /etc/polipo/config or to ~/.polipo and modify.
        ///
        ///# This file only contains some of the configuration variables; see the
        ///# list given by ``polipo -v&apos;&apos; and the manual for more.
        ///
        ///
        ///### Basic configuration
        ///### *******************
        ///
        ///# Uncomment one of these if you want to allow remote clients to
        ///# connect:
        ///
        ///# prox [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string polipo_config {
            get {
                return ResourceManager.GetString("polipo_config", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // Generated by gfwlist2pac
        ///// https://github.com/clowwindy/gfwlist2pac
        ///
        ///var domains = {
        ///  &quot;gimpshop.com&quot;: 1, 
        ///  &quot;directcreative.com&quot;: 1, 
        ///  &quot;speedpluss.org&quot;: 1, 
        ///  &quot;mingpaovan.com&quot;: 1, 
        ///  &quot;wikinews.org&quot;: 1, 
        ///  &quot;joachims.org&quot;: 1, 
        ///  &quot;maiio.net&quot;: 1, 
        ///  &quot;idv.tw&quot;: 1, 
        ///  &quot;mail-archive.com&quot;: 1, 
        ///  &quot;surfeasy.com.au&quot;: 1, 
        ///  &quot;hihistory.net&quot;: 1, 
        ///  &quot;alexlur.org&quot;: 1, 
        ///  &quot;finalion.jp&quot;: 1, 
        ///  &quot;nrk.no&quot;: 1, 
        ///  &quot;nyt.com&quot;: 1, 
        ///  &quot;cmule.com&quot;: 1, 
        ///  &quot;gappp.org&quot;: 1, 
        ///  &quot;givemesomethingtoread.com&quot;: 1, 
        ///   [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string proxy_pac {
            get {
                return ResourceManager.GetString("proxy_pac", resourceCulture);
            }
        }
    }
}
