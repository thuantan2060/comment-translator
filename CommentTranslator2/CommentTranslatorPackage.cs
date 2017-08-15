//------------------------------------------------------------------------------
// <copyright file="TranslateCommandPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using CommentTranlsator.Client;
using CommentTranslator.Common;
using CommentTranslator.Page;
using Microsoft.VisualStudio.Shell;

namespace CommentTranslator.Command
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
    [Guid(CommentTranslatorPackage.PackageGuidString)]
    [ProvideOptionPage(typeof(OptionPageGrid), "Comment Translator", "General", 0, 0, true)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class CommentTranslatorPackage : Package
    {
        /// <summary>
        /// TranslateCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "307a3d04-8453-4fc7-a06f-48b7a504196a";
        public static Settings Settings = new Settings();
        public static TranslateClient TranslateClient;

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

            //Load setting and create if not exist
            Settings.ReloadSetting((OptionPageGrid)GetDialogPage(typeof(OptionPageGrid)));

            //Create translate client
            TranslateClient = new TranslateClient(Settings);
        }

        #endregion

        #region Functions

        //private void LoadOrCreateSetting()
        //{
        //    var serviceProvider = GetService(typeof(ServiceProvider)) as ServiceProvider;
        //    if (serviceProvider != null)
        //    {
        //        var settingsManager = new ShellSettingsManager(serviceProvider);

        //        if (!SettingExist(settingsManager))
        //        {
        //            CreateSetting(settingsManager);
        //        }

        //        LoadSettinng(settingsManager);
        //    }
        //}

        //private bool SettingExist(SettingsManager settingsManager)
        //{
        //    var configurationSettingsStore = settingsManager.GetReadOnlySettingsStore(SettingsScope.Configuration);
        //    return configurationSettingsStore.CollectionExists(Settings.SettingCollection);
        //}

        //private void LoadSettinng(SettingsManager settingsManager)
        //{
        //    var configurationSettingsStore = settingsManager.GetReadOnlySettingsStore(SettingsScope.Configuration);

        //    Settings.TranslateUrl = configurationSettingsStore.GetString(Settings.SettingCollection, Settings.TranslateUrlProperty);
        //    Settings.TranslateFrom = configurationSettingsStore.GetString(Settings.SettingCollection, Settings.TranslateFromProperty);
        //    Settings.TranslateTo = configurationSettingsStore.GetString(Settings.SettingCollection, Settings.TranslateToProperty);
        //    Settings.AutoDetect = configurationSettingsStore.GetBoolean(Settings.SettingCollection, Settings.AutoDetectProperty);
        //}

        //private void CreateSetting(SettingsManager settingsManager)
        //{
        //    var configurationSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.Configuration);

        //    configurationSettingsStore.SetString(Settings.SettingCollection, Settings.TranslateUrlProperty, "http://mtigoogletranslateapi20170814014836.azurewebsites.net");
        //    configurationSettingsStore.SetString(Settings.SettingCollection, Settings.TranslateFromProperty, "ja");
        //    configurationSettingsStore.SetString(Settings.SettingCollection, Settings.TranslateToProperty, "en");
        //    configurationSettingsStore.SetBoolean(Settings.SettingCollection, Settings.AutoDetectProperty, true);
        //}

        #endregion
    }
}
