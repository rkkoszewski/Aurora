﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Aurora.Controls;
using Aurora.Settings;

namespace Aurora.Profiles.Magic_Duels_2012
{
    /// <summary>
    /// Interaction logic for Control_MagicDuels2012.xaml
    /// </summary>
    public partial class Control_MagicDuels2012 : UserControl
    {
        private ProfileManager profile_manager;

        public Control_MagicDuels2012()
        {
            InitializeComponent();

            profile_manager = Global.Configuration.ApplicationProfiles["MagicDuels2012"];

            SetSettings();

            //Apply LightFX Wrapper, if needed.
            if (!(profile_manager.Settings as MagicDuels2012Settings).first_time_installed)
            {
                InstallWrapper();
                (profile_manager.Settings as MagicDuels2012Settings).first_time_installed = true;
            }

            profile_manager.ProfileChanged += Profile_manager_ProfileChanged;
        }

        private void Profile_manager_ProfileChanged(object sender, EventArgs e)
        {
            SetSettings();
        }

        private void SetSettings()
        {
            this.profilemanager.ProfileManager = profile_manager;
            this.scriptmanager.ProfileManager = profile_manager;

            this.game_enabled.IsChecked = (profile_manager.Settings as MagicDuels2012Settings).isEnabled;
            this.cz.ColorZonesList = (profile_manager.Settings as MagicDuels2012Settings).lighting_areas;
        }

        private void patch_button_Click(object sender, RoutedEventArgs e)
        {
            if (InstallWrapper())
                MessageBox.Show("Aurora LightFX Wrapper installed successfully.");
            else
                MessageBox.Show("Aurora LightFX Wrapper could not be installed.\r\nGame is not installed.");

        }

        private void unpatch_button_Click(object sender, RoutedEventArgs e)
        {
            if (UninstallWrapper())
                MessageBox.Show("Aurora LightFX Wrapper uninstalled successfully.");
            else
                MessageBox.Show("Aurora LightFX Wrapper could not be uninstalled.\r\nGame is not installed.");
        }

        private void game_enabled_Checked(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                (profile_manager.Settings as MagicDuels2012Settings).isEnabled = (this.game_enabled.IsChecked.HasValue) ? this.game_enabled.IsChecked.Value : false;
                profile_manager.SaveProfiles();
            }
        }

        private void cz_ColorZonesListUpdated(object sender, EventArgs e)
        {
            if (IsLoaded)
            {
                (profile_manager.Settings as MagicDuels2012Settings).lighting_areas = (sender as ColorZones).ColorZonesList;
                profile_manager.SaveProfiles();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Global.geh.SetPreview(PreviewType.Predefined, profile_manager.ProcessNames[0]);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Global.geh.SetPreview(PreviewType.Desktop);
        }

        private bool InstallWrapper(string installpath = "")
        {
            if (String.IsNullOrWhiteSpace(installpath))
                installpath = Utils.SteamUtils.GetGamePath(49470);

            if (!String.IsNullOrWhiteSpace(installpath))
            {
                //86
                string path = System.IO.Path.Combine(installpath, "LightFX.dll");

                if (!File.Exists(path))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));

                using (BinaryWriter lightfx_wrapper_86 = new BinaryWriter(new FileStream(path, FileMode.Create)))
                {
                    lightfx_wrapper_86.Write(Properties.Resources.Aurora_LightFXWrapper86);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool UninstallWrapper(string installpath = "")
        {
            if (String.IsNullOrWhiteSpace(installpath))
                installpath = Utils.SteamUtils.GetGamePath(49470);

            if (!String.IsNullOrWhiteSpace(installpath))
            {
                //86
                string path = System.IO.Path.Combine(installpath, "LightFX.dll");

                if (File.Exists(path))
                    File.Delete(path);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
