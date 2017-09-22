//------------------------------------------------------------------------------
// <copyright file="CommentTranslatorPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using CommentTranlsator.Client;
using CommentTranslator.Option;
using Microsoft.VisualStudio.Shell;
using CommentTranslator.Util;
using EnvDTE80;
using EnvDTE;
using System.Diagnostics;

namespace CommentTranslator
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [ProvideOptionPage(typeof(OptionPageGrid), "Comment Translator", "General", 0, 0, true)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]
    public sealed class CommentTranslatorPackage : Package
    {
        /// <summary>
        /// CommentTranslatorPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "2e2206c4-ab10-44d9-a016-aedfe6a8975f";
        public static Settings Settings = new Settings();
        public static TranslateClient TranslateClient;

        public DTE2 DTE { get; set; }
        public Events Events { get; set; }
        public DocumentEvents DocumentEvents { get; set; }
        public WindowEvents WindowEvents { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateCommand"/> class.
        /// </summary>
        public CommentTranslatorPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            TranslateCommand.Initialize(this);
            base.Initialize();

            //Load settting
            Settings.ReloadSetting((OptionPageGrid)GetDialogPage(typeof(OptionPageGrid)));

            //Create client
            TranslateClient = new TranslateClient(Settings);

            DTE = (DTE2)GetService(typeof(DTE));
            Events = DTE.Events;
            DocumentEvents = Events.DocumentEvents;
            WindowEvents = Events.WindowEvents;

            DocumentEvents.DocumentOpened += DocumentEvents_DocumentOpened;
            DocumentEvents.DocumentSaved += DocumentEvents_DocumentSaved;
            WindowEvents.WindowActivated += WindowEvents_WindowActivated;
        }

        private void WindowEvents_WindowActivated(Window GotFocus, Window LostFocus)
        {
            Debug.WriteLine("Focus: " + GotFocus.Caption);
        }

        private void DocumentEvents_DocumentSaved(Document Document)
        {
            Debug.WriteLine("Save: " + Document.Name);

        }

        private void DocumentEvents_DocumentOpened(Document Document)
        {
            Debug.WriteLine("Open: " + Document.Name);
        }

        #endregion
    }
}
